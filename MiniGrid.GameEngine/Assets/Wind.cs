namespace MiniGrid.GameEngine.Assets;

public class Wind : EnergyAssets
{
    public string Name => "Wind";
    public int Price { get; set; } = 1000;
    public int MaxKiloWattHour { get; set; } = 1250;
    public int CarbonScore { get; set; } = 0;
    public int RampRateInHours { get; set; } = 1;
    public int FuelCostPerDay { get; set; } = 0;
    public decimal PercentageActivated { get; set; } = 1;
    public GameTime? ActivatedOn { get; set; }
    private const double cutIn = 3.0;
    private const double rated = 12.0;
    private const double cutOut = 25.0;

    public int CostPerHour()
    {
        return 0;
    }

    public int ProductionPerHour(DayInfo dayInfo)
    {
        double fraction =  TurbinePowerFraction(dayInfo.WindSpeed);
        return (int)Math.Round(MaxKiloWattHour * fraction);
    }

    /// <summary>
    /// A staggered cubic approximation of the power curve of a wind turbine. Returns a tuple with flow control and value.
    /// </summary>  
    private static double TurbinePowerFraction(double windSpeed)
    { 
        if (windSpeed < cutIn)
        {
            return 0;
        }
        if (windSpeed >= cutOut)
        {
            return 0;
        }
        if (windSpeed >= rated)
        {
            return 1;
        }

        // Cubic approximation
        double numerator =
            Math.Pow(windSpeed, 3) -
            Math.Pow(cutIn, 3);

        double denominator =
            Math.Pow(rated, 3) -
            Math.Pow(cutIn, 3);
        return numerator / denominator;
    }
}
