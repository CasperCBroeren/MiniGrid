namespace MiniGrid.Server.Dto;

public class JoinGameResponse
{
    public bool Success { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}
