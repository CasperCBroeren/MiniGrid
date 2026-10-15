namespace MiniGrid.GameEngine;

public interface EnergyAssets
{
    public string Id { get; }
    public string Name { get; }
    public int Price { get; set; }
    public int MaxKiloWattHour { get; set; }
    public int CarbonScore { get; set; }
    public int RampRateInHours { get; set; }
    public int FuelCostPerDay { get; set; }

    public decimal PercentageActivated { get; set; }

    public GameTime? ActivatedOn { get; set; }

    public string Description { get; }

    public string Image { get;  }

    /// <summary>
    /// The production of the asset in kWh
    /// </summary>
    /// <param name="dayInfo"></param>
    /// <returns>kWh</returns>
    public int ProductionPerHour(DayInfo dayInfo);


}
