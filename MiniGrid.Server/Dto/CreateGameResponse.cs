namespace MiniGrid.Server.Dto;

public class CreateGameResponse
{
    public string GameCode { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public bool IsGameLeader { get; set; }
}
