namespace MiniGrid.GameEngine.Assets;

public class GasPowerPlant : EnergyAssets
{
    public string Name => "Gas";
    public int Price { get; set; } = 200000;
    public int MaxKiloWattHour { get; set; } = 6;  
    public int CarbonScore { get; set; } = 1;
    public int RampRateInHours { get; set; } = 2;
    public int FuelCostPerDay { get; set; } = 0;
    public decimal PercentageActivated { get; set; } = 1;
    public GameTime? ActivatedOn { get; set; }

    public int CostPerHour()
    {
        return 2;
    }

    public int ProductionPerHour(DayInfo dayInfo)
    {
        return MaxKiloWattHour;
    }
}
