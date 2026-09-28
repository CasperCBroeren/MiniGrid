namespace MiniGrid.GameEngine;

public static class ConsumerModel
{
    public static int CalculateConsumptionKw(int clients, DayInfo dayInfo)
    {
        // Base consumption per client in watts
        const int baseConsumptionPerClientWatt = 5300;
        // Adjust consumption based on the time of day
        double timeOfDayMultiplier = dayInfo.Date.Hour switch
        {
            >= 6 and < 12 => 1.2, // Morning: 20% more consumption
            >= 12 and < 18 => 1.5, // Afternoon: 50% more consumption
            >= 18 and < 22 => 1.3, // Evening: 30% more consumption
            _ => 0.8 // Night: 20% less consumption
        };
        // Adjust consumption based on the season (month)
        double seasonalMultiplier = dayInfo.Date.Month switch
        {
            >= 6 and <= 8 => 1.4, // Summer: 40% more consumption
            >= 12 or <= 2 => 1.2, // Winter: 20% more consumption
            _ => 1.0 // Spring/Fall: normal consumption
        };
        // Total consumption calculation
        var kWh =  (baseConsumptionPerClientWatt * timeOfDayMultiplier * seasonalMultiplier) / 1000;
        return (int)(clients * kWh);
    }   
}

