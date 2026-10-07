using MiniGrid.GameEngine;
using MiniGrid.GameEngine.Assets;
using MiniGrid.Server.Hubs;
using MiniGrid.Server.Models;

namespace MiniGrid.Server.Services;

public static class GameManager
{
    private const int StartCash = 6000000;
    private static readonly Dictionary<string, ServerGame> _games = [];
    private static readonly object _lock = new();

    public static string CreateGame(string playerName, string connectionId)
    {
        lock (_lock)
        {
            var gameCode = GenerateGameCode();
            var serverGame = new ServerGame
            {
                GameCode = gameCode,
                GameEngineGame = new Game { GameCode = gameCode },
                Players = [],
                IsStarted = false,
                GameSpeed = GameSpeed.Normal,
                IsPaused = false
            };

            var playerId = Guid.NewGuid().ToString();
            var gamePlayer = new GamePlayer
            {
                ConnectionId = connectionId,
                PlayerId = playerId,
                Name = playerName,
                IsGameLeader = true,
                GameEnginePlayer = new Player { GameLeader = true, CashInEuro = StartCash, TakesPartIn = serverGame.GameEngineGame }
            };

            serverGame.Players.Add(gamePlayer);
            serverGame.GameEngineGame.Players.Add(gamePlayer.GameEnginePlayer);
            _games[gameCode] = serverGame;

            return gameCode;
        }
    }

    public static bool JoinGame(string gameCode, string playerName, string connectionId, out string playerId, out string error)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
            {
                playerId = string.Empty;
                error = "Game not found";
                return false;
            }

            if (serverGame.IsStarted)
            {
                playerId = string.Empty;
                error = "Game already started";
                return false;
            }

            if (serverGame.Players.Any(p => p.ConnectionId == connectionId))
            {
                playerId = string.Empty;
                error = "Already joined this game";
                return false;
            }

            playerId = Guid.NewGuid().ToString();
            var gamePlayer = new GamePlayer
            {
                ConnectionId = connectionId,
                PlayerId = playerId,
                Name = playerName,
                IsGameLeader = false,
                GameEnginePlayer = new Player { GameLeader = false, CashInEuro = StartCash, TakesPartIn = serverGame.GameEngineGame }
            };

            serverGame.Players.Add(gamePlayer);
            serverGame.GameEngineGame.Players.Add(gamePlayer.GameEnginePlayer);
            error = string.Empty;
            return true;
        }
    }

    public static bool StartGame(string gameCode, string playerId)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
                return false;

            var leader = serverGame.Players.FirstOrDefault(p => p.PlayerId == playerId);
            if (leader == null || !leader.IsGameLeader)
                return false;

            if (serverGame.IsStarted)
                return false;

            serverGame.IsStarted = true;
            serverGame.GameEngineGame.GameState = GameState.Playing;
            serverGame.GameEngineGame.GameSpeed = serverGame.GameSpeed;

            return true;
        }
    }

    public static bool PauseGame(string gameCode, string playerId, bool pause)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
                return false;

            var leader = serverGame.Players.FirstOrDefault(p => p.PlayerId == playerId);
            if (leader == null || !leader.IsGameLeader)
                return false;

            serverGame.IsPaused = pause;
            serverGame.GameEngineGame.GameState = pause ? GameState.Pauzed : GameState.Playing;

            return true;
        }
    }

    public static bool SetGameSpeed(string gameCode, string playerId, GameSpeed gameSpeed)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
                return false;

            var leader = serverGame.Players.FirstOrDefault(p => p.PlayerId == playerId);
            if (leader == null || !leader.IsGameLeader)
                return false;

            serverGame.GameSpeed = gameSpeed;
            serverGame.GameEngineGame.GameSpeed = gameSpeed;

            return true;
        }
    }

    public static ServerGame? GetGame(string gameCode)
    {
        lock (_lock)
        {
            _games.TryGetValue(gameCode, out var serverGame);
            return serverGame;
        }
    }

    public static IEnumerable<ServerGame> GetAllGames()
    {
        lock (_lock)
        {
            return _games.Values.ToList();
        }
    }

    public static void RemovePlayer(string gameCode, string connectionId)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
                return;

            var player = serverGame.Players.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (player != null)
            {
                serverGame.Players.Remove(player);
                serverGame.GameEngineGame.Players.Remove(player.GameEnginePlayer);

                // If the game leader leaves, transfer leadership to another player
                if (player.IsGameLeader && serverGame.Players.Any())
                {
                    serverGame.Players[0].IsGameLeader = true;
                    serverGame.Players[0].GameEnginePlayer.GameLeader = true;
                }

                // If no players left, remove the game
                if (!serverGame.Players.Any())
                {
                    _games.Remove(gameCode);
                }
            }
        }
    }
    
    // Method to update a player's connection ID (for reconnections)
    public static void UpdatePlayerConnection(string gameCode, string oldConnectionId, string newConnectionId)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
                return;

            var player = serverGame.Players.FirstOrDefault(p => p.ConnectionId == oldConnectionId);
            if (player != null)
            {
                player.ConnectionId = newConnectionId;
            }
        }
    }

    public static void TickAllGames(IGameHub gameHub)
    {
        lock (_lock)
        {
            foreach (var serverGame in _games.Values)
            {
                if (serverGame.IsStarted && !serverGame.IsPaused)
                {
                    serverGame.GameEngineGame.Tick();
                    gameHub.BroadcastGameState(serverGame.GameCode);
                }
            }
        }
    }

    private static string GenerateGameCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static GamePlayer? GetPlayer(string gameCode, string playerId)
    {
        lock (_lock)
        {
            if (!_games.TryGetValue(gameCode, out var serverGame))
                return null;
            return serverGame.Players.FirstOrDefault(p => p.PlayerId == playerId);
        }
    }

    public static bool BuyAsset(string gameCode, string playerId, string assetType)
    {
        lock (_lock)
        {
            var player = GetPlayer(gameCode, playerId);
            if (player == null )
                return false;

            if (assetType == "Wind")
            {   
                return player.GameEnginePlayer.BuyAsset(new Wind());
            }
            else if (assetType == "Solar")
            { 
                return player.GameEnginePlayer.BuyAsset(new Solar());
            }
            else
            {
                return false; // Invalid asset type
            } 
        }
    }
}
