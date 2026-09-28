using MiniGrid.GameEngine;

namespace MiniGrid.Server.Dto;

public class SetGameSpeedRequest
{
    public string GameCode { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public GameSpeed GameSpeed { get; set; }
}
