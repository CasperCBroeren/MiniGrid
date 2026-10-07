using MiniGrid.GameEngine;
using MiniGrid.Server.Models;

namespace MiniGrid.Server.Dto;

public class GameStateUpdate
{
    public string GameCode { get; set; } = string.Empty;
    public GameState GameState { get; set; }
    public int CurrentDay { get; set; }
    public int CurrentHour { get; set; }
    public List<PlayerState> Players { get; set; } = [];
    public bool IsPaused { get; set; }
    public GameSpeed GameSpeed { get; set; }
    public int CurrentMonth { get; internal set; }
    public double SolarRadiation { get; set; }
    public double WindSpeed { get; set; }
}

public class PlayerState
{
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsGameLeader { get; set; }
    public int CashInEuro { get; set; }
    public int Clients { get; set; }
    public int TotalProductionKWh { get; set; }
    public int TotalConsumptionKWh { get; set; }
    public int TotalCarbonEmitted { get; set; }
    public int Balance { get; set; }
    public List<string> Assets { get; set; } = [];
}
