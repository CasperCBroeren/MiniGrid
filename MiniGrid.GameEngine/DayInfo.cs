namespace MiniGrid.GameEngine;


public record GameTime(int Month, int Day, int Hour)
{
    public static GameTime StartOfGame => new GameTime(1, 1, 0);
    public bool IsNewYear => Month == 1 && Day == 1 && Hour == 0;

    public GameTime NextHour()
    {
        int nextHour = Hour + 1;
        int nextDay = Day;
        int nextMonth = Month;
        if (nextHour >= 24)
        {
            nextHour = 0;
            nextDay++;
            if (nextDay > DateTime.DaysInMonth(2026, Month))
            {
                nextDay = 1;
                nextMonth++;
                if (nextMonth > 12)
                {
                    nextMonth = 1; // Reset to January after Decemb er
                }
            }
        }
        return new GameTime(nextMonth, nextDay, nextHour);
    }

    internal GameTime AddHours(int hours)
    {
        var result = this;
        while (hours > 0)
        {
            hours--;
            result = result.NextHour();
        }
        return result;
    }



    public static bool operator >(GameTime a, GameTime b)
    {
        if (a.Month > b.Month) return true;
        if (a.Month == b.Month && a.Day > b.Day) return true;
        if (a.Month == b.Month && a.Day == b.Day && a.Hour > b.Hour) return true;
        return false;
    }

    public static bool operator <(GameTime a, GameTime b)
    {
        return b > a;
    }

    public static bool operator >=(GameTime a, GameTime b)
    {
        return a > b || a == b;
    }

    public static bool operator <=(GameTime a, GameTime b)
    {
        return a < b || a == b;
    }

}
public record DayInfo(GameTime Date, double WindSpeed, double SolarRadiation)
{
    public DayInfo(GameTime date)
        : this(
            date,
            WindModel.GetWindSpeed(date.Day, date.Month, date.Hour),
            SolarModel.GetSolarRadiation(date.Day, date.Month, date.Hour))
    {

    }

    public DayInfo Progress()
    {
        var radiation = SolarModel.GetSolarRadiation(Date.Day, Date.Month, Date.Hour);
        var windSpeed = WindModel.GetWindSpeed(Date.Day, Date.Month, Date.Hour);
        return this with
        {
            Date = Date.NextHour(),
            SolarRadiation = radiation,
            WindSpeed = windSpeed
        };
    }
}