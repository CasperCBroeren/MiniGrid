namespace MiniGrid.GameEngine.Assets;

public class Solar : EnergyAssets
{
    public string Name => "Solar";
    public int Price { get; set; } = 1000;
    public int MaxKiloWattHour { get; set; } = 800; // kind of imitates the size of the panels.
    public int CarbonScore { get; set; } = 0;
    public int RampRateInHours { get; set; } = 1;
    public int FuelCostPerDay { get; set; } = 0;
    public decimal PercentageActivated { get; set; } = 1;
    public GameTime? ActivatedOn { get; set; }

    public int CostPerHour()
    {
        return 0;
    }
    
    public int ProductionPerHour(DayInfo dayInfo)
    {
        return (int)Math.Round(dayInfo.SolarRadiation *  MaxKiloWattHour);
    }
}
