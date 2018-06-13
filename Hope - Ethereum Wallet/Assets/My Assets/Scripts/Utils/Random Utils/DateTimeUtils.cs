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
    /// Converts a unix time stamp to a DateTime object.
    /// </summary>
    /// <param name="timeStamp"> The unix time stamp to convert. </param>
    /// <returns> The DateTime of the unix time stamp. </returns>
    public static DateTime TimeStampToDateTime(double timeStamp) => UnixTimeStart.AddSeconds(timeStamp).ToLocalTime();

    /// <summary>
    /// Gets a formatted string in the form of month day, year.
    /// Example: January 1, 2018
    /// </summary>
    /// <param name="dateTime"> The DateTime object to get the string representation for. </param>
    /// <returns> The formatted string of this DateTime object. </returns>
    public static string GetStringFormattedDate(this DateTime dateTime) => GetMonthString(dateTime) + " " + dateTime.Day + ", " + dateTime.Year;

    /// <summary>
    /// Gets the text displaying '1 second ago' or anything similar.
    /// </summary>
    /// <param name="timeVal"> The value of how long ago the time was in the form of the timeValName. </param>
    /// <param name="timeValName"> The name of the time conversion type. Example: 'second', 'minute', 'hour', etc. </param>
    /// <returns> The text which displays how long ago something was in terms of x 'seconds' ago or anything similar. </returns>
    public static string GetTimeAgoText(int timeVal, string timeValName) => timeVal + " " + timeValName + "" + GetAddedCharacter(timeVal) + " ago";

    /// <summary>
    /// Gets an added 's' if the value is greater than one. 
    /// The original string should be plural.
    /// </summary>
    /// <param name="val"> The value to determine if there is an extra character. </param>
    /// <returns> The extra character to add at the end of the string if it is plural. </returns>
    private static string GetAddedCharacter(int val) => val != 1 ? "s" : "";

    /// <summary>
    /// Gets the string representation of the Month of a DateTime object.
    /// </summary>
    /// <param name="dateTime"> The DateTime object to get the month for. </param>
    /// <returns> The month in string format. </returns>
    private static string GetMonthString(DateTime dateTime)
    {
        switch(dateTime.Month)
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
