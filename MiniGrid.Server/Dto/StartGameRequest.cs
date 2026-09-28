namespace MiniGrid.Server.Dto;

public class StartGameRequest
{
    public string GameCode { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
}
