namespace MiniGrid.Server.Dto;

public class JoinGameRequest
{
    public string GameCode { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
}
