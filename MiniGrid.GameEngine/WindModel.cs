
using System;
namespace MiniGrid.GameEngine;

public static class WindModel
{
    private static readonly Random random = new Random(12345);

    // Average wind speed at 10 m.
    // Adjust this for your particular location.
    private const double MeanWindSpeed = 5.0;
 

    /// <summary>
    /// Generate wind speed for one hour.
    /// Returns m/s at approximately 10 m height.
    /// </summary>
    public static double GetWindSpeed(
        int dayOfYear,
        int hour,
        double previousWind)
    {
        // ---------------------------------------------------------
        // 1. Seasonal variation
        // ---------------------------------------------------------

        // Stronger winds during the Dutch autumn/winter.
        //
        // Maximum around January.
        // Minimum around July.
        double seasonal =
            1.0 +
            0.25 *
            Math.Cos(
                2.0 * Math.PI *
                (dayOfYear - 15) / 365.25
            );

        // ---------------------------------------------------------
        // 2. Diurnal variation
        // ---------------------------------------------------------

        // Land-based wind often has some daily variation due
        // to atmospheric mixing.
        double daily =
            1.0 +
            0.08 *
            Math.Sin(
                2.0 * Math.PI *
                (hour - 8) / 24.0
            );

        double target =
            MeanWindSpeed *
            seasonal *
            daily;

        // ---------------------------------------------------------
        // 3. Random weather disturbance
        // ---------------------------------------------------------

        // Gaussian-ish random number using Box-Muller.
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();

        double gaussian =
            Math.Sqrt(-2.0 * Math.Log(u1)) *
            Math.Cos(2.0 * Math.PI * u2);

        // Weather disturbance.
        double disturbance =
            gaussian * 1.5;

        // ---------------------------------------------------------
        // 4. Temporal correlation
        // ---------------------------------------------------------

        // 0.85 means wind changes relatively slowly.
        double wind =
            0.85 * previousWind +
            0.15 * target +
            disturbance * 0.25;

        // ---------------------------------------------------------
        // 5. Don't allow negative wind
        // ---------------------------------------------------------

        return Math.Max(0.0, wind);
    }
}
