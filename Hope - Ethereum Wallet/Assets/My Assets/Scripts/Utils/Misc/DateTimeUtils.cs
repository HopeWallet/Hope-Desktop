using System;

/// <summary>
/// Class which contains some utility methods and fields for date and time related stuff.
/// </summary>
public static class DateTimeUtils
{
    public static readonly DateTime UnixTimeStart = new DateTime(1970, 1, 1);

    public const int YEAR_IN_SECONDS = 31536000;
    public const int MONTH_IN_SECONDS = 2628000;
    public const int DAY_IN_SECONDS = 86400;
    public const int HOUR_IN_SECONDS = 3600;
    public const int MINUTE_IN_SECONDS = 60;

    /// <summary>
    /// Gets the current unix time.
    /// </summary>
    /// <returns> The current unix time. </returns>
    public static long GetCurrentUnixTime() => (long)(DateTime.UtcNow.Subtract(UnixTimeStart)).TotalSeconds;

    /// <summary>
    /// Converts a unix time stamp to a DateTime object.
    /// </summary>
    /// <param name="timeStamp"> The unix time stamp to convert. </param>
    /// <returns> The DateTime of the unix time stamp. </returns>
    public static DateTime TimeStampToDateTime(double timeStamp) => UnixTimeStart.AddSeconds(timeStamp).ToLocalTime();

    /// <summary>
    /// Gets the formatted date, and the proper 12 hour formatted time.
    /// </summary>
    /// <param name="dateTime"> The DateTime object to get the string representation for. </param>
    /// <returns></returns>
    public static string GetFormattedDateString(this DateTime dateTime) => GetMonthString(dateTime) + " " + dateTime.Day + ", " + dateTime.Year + " - " + dateTime.GetTimeString();

    /// <summary>
    /// Gets the formatted 12 hour time, not including the seconds
    /// </summary>
    /// <param name="dateTime"> The DateTime object to get the string representation for. </param>
    /// <returns></returns>
    public static string GetTimeString(this DateTime dateTime)
    {
        int hour = dateTime.Hour;

        string timeConvention = "<style=Symbol> " + (hour >= 12 && hour != 24 ? "PM" : "AM") + "</style>";
        string twelveHourTime = hour > 12 ? (hour - 12).ToString() : hour == 0 ? 12.ToString() : hour.ToString();

        return twelveHourTime + ":" + dateTime.Minute.ToString("00") + timeConvention;
    }

    /// <summary>
    /// Gets a formatted string in the form of month day, year.
    /// Example: January 1, 2018
    /// </summary>
    /// <param name="dateTime"> The DateTime object to get the string representation for. </param>
    /// <returns> The formatted string of this DateTime object. </returns>
    public static string GetStringFormattedDate(this DateTime dateTime) => GetMonthString(dateTime) + " " + dateTime.Day + ", " + dateTime.Year;

    /// <summary>
    /// Gets the maximum time interval in a unix time.
    /// For example, a value of 60 will produce '1 second'.
    /// A value of 3600 will produce '1 hour'.
    /// Etc.
    /// </summary>
    /// <param name="unixTime"> The unix time to convert to the maximum time interval. </param>
    /// <param name="extraText"> The extra text to add to the end of the time interval string. </param>
    /// <returns> The final time interval text. </returns>
    public static string GetMaxTimeInterval(long unixTime, string extraText = "")
    {
        string finalTimeText;

        long seconds = unixTime;
        int minutes = (int)(seconds / MINUTE_IN_SECONDS);
        int hours = (int)(seconds / HOUR_IN_SECONDS);
        int days = (int)(seconds / DAY_IN_SECONDS);
        int months = (int)(seconds / MONTH_IN_SECONDS);
        int years = (int)(seconds / YEAR_IN_SECONDS);

        if (years > 0) finalTimeText = GetCleanTimeText(years, "year");
        else if (months > 0) finalTimeText = GetCleanTimeText(months, "month");
        else if (days > 0) finalTimeText = GetCleanTimeText(days, "day");
        else if (hours > 0) finalTimeText = GetCleanTimeText(hours, "hour");
        else if (minutes > 0) finalTimeText = GetCleanTimeText(minutes, "minute");
        else finalTimeText = GetCleanTimeText(seconds, "second");

        return finalTimeText + extraText;
    }

    /// <summary>
    /// The clean version of the time text.
    /// </summary>
    /// <param name="timeVal"> The time value. </param>
    /// <param name="timeValName"> The name of the time conversion type. Example: 'second', 'minute', 'hour', etc. </param>
    /// <returns> The time text with the added s in case of plural time.. </returns>
    private static string GetCleanTimeText(dynamic timeVal, string timeValName) => timeVal + " " + timeValName + "" + GetAddedCharacter(timeVal);

    /// <summary>
    /// Gets an added 's' if the value is greater than one. 
    /// The original string should be plural.
    /// </summary>
    /// <param name="val"> The value to determine if there is an extra character. </param>
    /// <returns> The extra character to add at the end of the string if it is plural. </returns>
    private static string GetAddedCharacter(dynamic val) => val != 1 ? "s" : "";

    /// <summary>
    /// Gets the string representation of the Month of a DateTime object.
    /// </summary>
    /// <param name="dateTime"> The DateTime object to get the month for. </param>
    /// <returns> The month in string format. </returns>
    private static string GetMonthString(DateTime dateTime)
    {
        switch (dateTime.Month)
        {
            case 1: return "January";
            case 2: return "February";
            case 3: return "March";
            case 4: return "April";
            case 5: return "May";
            case 6: return "June";
            case 7: return "July";
            case 8: return "August";
            case 9: return "September";
            case 10: return "October";
            case 11: return "November";
            case 12: return "December";
            default: return null;
        }
    }

}
