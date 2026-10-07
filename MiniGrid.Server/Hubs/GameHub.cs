using Microsoft.AspNetCore.SignalR;
using MiniGrid.GameEngine;
using MiniGrid.Server.Dto;
using MiniGrid.Server.Services;

namespace MiniGrid.Server.Hubs;

public class GameHub : Hub, IGameHub
{ 
    private readonly ILogger<GameHub> _logger;
    private readonly IHubContext<GameHub> _hubContext;

    public GameHub(ILogger<GameHub> logger, IHubContext<GameHub> hubContext)
    { 
        _logger = logger;
        _hubContext = hubContext;
    }

    public CreateGameResponse CreateGame(string playerName)
    {
        var gameCode = GameManager.CreateGame(playerName, Context.ConnectionId);
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
        var success = GameManager.JoinGame(
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
        var result = GameManager.StartGame(request.GameCode, request.PlayerId);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        return result;
    }

    public bool PauseGame(PauseGameRequest request)
    {
        var result = GameManager.PauseGame(request.GameCode, request.PlayerId, request.Pause);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        return result;
    }


    public bool SetGameSpeed(SetGameSpeedRequest request)
    {
        var result = GameManager.SetGameSpeed(request.GameCode, request.PlayerId, request.GameSpeed);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        return result;
    }

    public bool BuyAsset(BuyAssetRequest request)
    {
        var result = GameManager.BuyAsset(request.GameCode, request.PlayerId, request.AssetType);
        if (result)
        {
            BroadcastGameState(request.GameCode);
        }
        else
        {
            var player = GameManager.GetPlayer(request.GameCode, request.PlayerId);
            if (player != null)
            { 
                _hubContext.Clients.Client(player.ConnectionId).SendAsync("FailedToBuyAsset", request.AssetType);
            }
        }
        return result;
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var game in GameManager.GetAllGames())
        {
            var gameCode = game.GameCode;
            GameManager.RemovePlayer(gameCode, Context.ConnectionId);
            BroadcastGameState(gameCode);
        }
        await base.OnDisconnectedAsync(exception);
    }

    private string GetPlayerIdFromConnection(string connectionId, string gameCode)
    {
        var game = GameManager.GetGame(gameCode);
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

    public void BroadcastGameState(string gameCode)
    {
        var game = GameManager.GetGame(gameCode);
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
            SolarRadiation = game.GameEngineGame.CurrentDay.SolarRadiation,
            WindSpeed = game.GameEngineGame.CurrentDay.WindSpeed,
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
                Balance = p.GameEnginePlayer.Balance,
                Assets = p.GameEnginePlayer.Assets.Select(a => a.Name).ToList()
            }).ToList()
        };

        // Send to all clients in the game
        foreach (var player in game.Players)
        {
            _hubContext.Clients.Client(player.ConnectionId).SendAsync("GameStateUpdated", gameStateUpdate);
        }
    }
}
