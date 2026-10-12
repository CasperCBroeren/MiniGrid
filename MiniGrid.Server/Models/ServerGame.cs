using MiniGrid.GameEngine;
using MiniGrid.Server.Models;

namespace MiniGrid.Server.Models;

public class ServerGame
{
    public string GameCode { get; set; } = string.Empty;
    public Game GameEngineGame { get; set; } = new Game();
    public List<GamePlayer> Players { get; set; } = [];
    public bool IsStarted => GameEngineGame.GameState == GameState.Playing || GameEngineGame.GameState == GameState.Pauzed;
    public bool IsEnded => GameEngineGame.GameState == GameState.Ended;
    public GameSpeed GameSpeed { get; set; } = GameSpeed.Normal;
    public bool IsPaused { get; set; } = false;
}
