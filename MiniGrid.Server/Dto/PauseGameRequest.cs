namespace MiniGrid.Server.Dto;

public class PauseGameRequest
{
    public string GameCode { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public bool Pause { get; set; }
}
