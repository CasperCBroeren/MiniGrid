using System;

namespace MiniGrid.GameEngine;

public static class SolarModel
{
    // Schiedam, Netherlands
    private const double Latitude = 51.92;
    private const double Longitude = 4.40;

    /// <summary>
    /// Gets normalized solar radiation for the given date and hour.
    /// Returns 0.0 to 1.0.
    /// </summary>
    public static double GetSolarRadiation(
        int day,
        int month,
        double hour)
    {
        var date = new DateTime(2026, month, day);

        // Day of year
        var dayOfYear = date.DayOfYear;

        // Solar declination in radians
        var declination =
            23.44 *
            Math.PI / 180.0 *
            Math.Sin(
                2.0 * Math.PI *
                (284 + dayOfYear) / 365.0);

        var latitude = Latitude * Math.PI / 180.0;

        // Approximate solar time correction
        var B = 2.0 * Math.PI * (dayOfYear - 81) / 364.0;

        var equationOfTime =
            9.87 * Math.Sin(2 * B)
            - 7.53 * Math.Cos(B)
            - 1.5 * Math.Sin(B);

        // Netherlands longitude ~4.4°E.
        // Solar noon is affected by longitude and equation of time.
        //
        // CET/CEST correction:
        var daylightSaving = IsEuropeanSummerTime(date);

        var utcOffset = daylightSaving ? 2.0 : 1.0;

        // Convert clock time to solar time
        var solarTime =
            hour
            + (4.0 * (Longitude - 15.0 * utcOffset)
            + equationOfTime) / 60.0;

        // Solar hour angle
        var hourAngle =
            15.0 * (solarTime - 12.0)
            * Math.PI / 180.0;

        // Sun elevation
        var sinElevation =
            Math.Sin(latitude) * Math.Sin(declination)
            + Math.Cos(latitude) *
              Math.Cos(declination) *
              Math.Cos(hourAngle);

        var elevation = Math.Asin(sinElevation);

        // Sun below horizon
        if (elevation <= 0)
        {
            return 0.0;
        }
        // ---------------------------------------------------------
        // Find maximum elevation for this particular day.
        // This occurs at solar noon.
        // ---------------------------------------------------------

        var maxSinElevation =
            Math.Sin(latitude) * Math.Sin(declination)
            + Math.Cos(latitude) * Math.Cos(declination);

        // Normalize to 0..1
        var result = sinElevation / maxSinElevation;

        var clearSkySolar = Math.Clamp(result, 0.0, 1.0);
        var cloudCover = GetCloudCover(dayOfYear, hour);

        double transmission = 1.0 - 0.85 * cloudCover;

        return clearSkySolar * transmission;


    }

    private static bool IsEuropeanSummerTime(DateTime date)
    {
        var start = new DateTime(2026, 3, 29);
        var end = new DateTime(2026, 10, 25);

        return date.Date >= start.Date &&
               date.Date < end.Date;
    }

    /// <summary>
    /// The amount of clouds per day 
    /// </summary>
    /// <param name="dayOfYear"></param>
    /// <param name="hour"></param>
    /// <returns></returns>
    private static double GetCloudCover(
    int dayOfYear,
    double hour)
    {
        // ---------------------------------------------------------
        // Seasonal cloudiness
        // ---------------------------------------------------------
        // More cloud in autumn/winter,
        // somewhat less in spring/summer.
        //
        // Result roughly varies between 0.45 and 0.80.
        // ---------------------------------------------------------

        double seasonal =
            0.63 +
            0.14 * Math.Cos(
                2.0 * Math.PI *
                (dayOfYear - 20) / 365.25);

        // ---------------------------------------------------------
        // Small daily variation
        // ---------------------------------------------------------
        // Clouds tend to vary somewhat during the day, but this
        // effect should be much smaller than the seasonal effect.
        // ---------------------------------------------------------

        double daily =
            0.04 * Math.Sin(
                2.0 * Math.PI *
                (hour - 14.0) / 24.0);

        double cloudCover =
            seasonal + daily;

        return Math.Clamp(cloudCover, 0.0, 1.0);
    }
}
