using MiniGrid.GameEngine;

namespace MiniGrid.Server.Models;

public class GamePlayer
{
    public string ConnectionId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsGameLeader { get; set; }
    public Player GameEnginePlayer { get; set; } = new Player();
}
