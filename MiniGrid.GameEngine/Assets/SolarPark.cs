namespace MiniGrid.GameEngine.Assets;

public class SolarPark : EnergyAssets
{
    public string Id => "Solar";
    public string Name => "Solar park";
    public int Price { get; set; } = 100000;
    public int MaxKiloWattHour { get; set; } = 11; // kind of imitates the size of the panels.
    public int CarbonScore { get; set; } = 0;
    public int RampRateInHours { get; set; } = 1;
    public int FuelCostPerDay { get; set; } = 0;
    public decimal PercentageActivated { get; set; } = 1;
    public string Image => "../assets/solar_park.png";
    public string Description => "Clean renewable energy from sunlight. Zero emissions and low maintenance.";
    public GameTime? ActivatedOn { get; set; }

    
    public int ProductionPerHour(DayInfo dayInfo)
    {
        return (int)Math.Round(dayInfo.SolarRadiation *  MaxKiloWattHour);
    }
}
