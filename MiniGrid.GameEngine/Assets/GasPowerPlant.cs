namespace MiniGrid.GameEngine.Assets;

public class GasPowerPlant : EnergyAssets
{
    public string Id => "Gas";
    public string Name => "Gas powerplant";
    public int Price { get; set; } = 200000;
    public int MaxKiloWattHour { get; set; } = 6;  
    public int CarbonScore { get; set; } = 1;
    public int RampRateInHours { get; set; } = 2;
    public int FuelCostPerDay { get; set; } = 2;
    public decimal PercentageActivated { get; set; } = 1;
    public GameTime? ActivatedOn { get; set; }
    public string Image => "../assets/gas_powerplant.png";
    public string Description => "Reliable and flexible power generation using natural gas. Suitable for meeting peak demand.";

   

    public int ProductionPerHour(DayInfo dayInfo)
    {
        return MaxKiloWattHour;
    }
}
