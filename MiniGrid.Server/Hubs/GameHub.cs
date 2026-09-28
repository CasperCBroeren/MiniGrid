using Microsoft.AspNetCore.SignalR;
using MiniGrid.Server.Dto;
using MiniGrid.Server.Services;

namespace MiniGrid.Server.Hubs;

public class GameHub : Hub
{
    private readonly GameManager _gameManager;
    private readonly ILogger<GameHub> _logger;
    private readonly IHubContext<GameHub> _hubContext;

    public GameHub(GameManager gameManager, ILogger<GameHub> logger, IHubContext<GameHub> hubContext)
    {
        _gameManager = gameManager;
        _logger = logger;
        _hubContext = hubContext;
    }

    public CreateGameResponse CreateGame(string playerName)
    {
        var gameCode = _gameManager.CreateGame(playerName, Context.ConnectionId);
        var playerId = GetPlayerIdFromConnection(Context.ConnectionId, gameCode);
        
        return new CreateGameResponse
        {
            GameCode = gameCode,
            PlayerId = playerId,
            IsGameLeader = true
        };
    }

    public JoinGameResponse JoinGame(JoinGameRequest request)
    {
        var success = _gameManager.JoinGame(
            request.GameCode,
            request.PlayerName,
            Context.ConnectionId,
            out var playerId,
            out var error);

        if (success)
        {
            // Notify all players in the game about the new player
            BroadcastGameState(request.GameCode);
            
            return new JoinGameResponse
            {
                Success = true,
                PlayerId = playerId
            };
        }

        return new JoinGameResponse
        {
            Success = false,
            Error = error
        };
    }

    public bool StartGame(StartGameRequest request)
    {
        var result = _gameManager.StartGame(request.GameCode, request.PlayerId);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        return result;
    }

    public bool PauseGame(PauseGameRequest request)
    {
        var result = _gameManager.PauseGame(request.GameCode, request.PlayerId, request.Pause);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        return result;
    }

    public bool SetGameSpeed(SetGameSpeedRequest request)
    {
        var result = _gameManager.SetGameSpeed(request.GameCode, request.PlayerId, request.GameSpeed);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        return result;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Find and remove the player from all games
        foreach (var game in _gameManager.GetAllGames())
        {
            var gameCode = game.GameCode;
            _gameManager.RemovePlayer(gameCode, Context.ConnectionId);
            BroadcastGameState(gameCode);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private string GetPlayerIdFromConnection(string connectionId, string gameCode)
    {
        var game = _gameManager.GetGame(gameCode);
        if (game != null)
        {
            var player = game.Players.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (player != null)
            {
                return player.PlayerId;
            }
        }
        return string.Empty;
    }

    private void BroadcastGameState(string gameCode)
    {
        var game = _gameManager.GetGame(gameCode);
        if (game == null) return;

        var gameStateUpdate = new GameStateUpdate
        {
            GameCode = gameCode,
            GameState = game.GameEngineGame.GameState,
            CurrentDay = game.GameEngineGame.CurrentDay.Date.Day,
            CurrentHour = game.GameEngineGame.CurrentDay.Date.Hour,
            CurrentMonth = game.GameEngineGame.CurrentDay.Date.Month,
            IsPaused = game.IsPaused,
            GameSpeed = game.GameSpeed,
            Players = game.Players.Select(p => new PlayerState
            {
                PlayerId = p.PlayerId,
                Name = p.Name,
                IsGameLeader = p.IsGameLeader,
                CashInEuro = p.GameEnginePlayer.CashInEuro,
                Clients = p.GameEnginePlayer.Clients,
                TotalProductionKWh = p.GameEnginePlayer.TotalProductionKWh,
                TotalConsumptionKWh = p.GameEnginePlayer.TotalConsumptionKWh,
                TotalCarbonEmitted = p.GameEnginePlayer.TotalCarbonEmitted,
                Balance = p.GameEnginePlayer.Balance
            }).ToList()
        };

        // Send to all clients in the game
        foreach (var player in game.Players)
        {
            _hubContext.Clients.Client(player.ConnectionId).SendAsync("GameStateUpdated", gameStateUpdate);
        }
    }
}
