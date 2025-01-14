using System;
using System.Diagnostics.Contracts;

namespace Soenneker.Extensions.Int;

/// <summary>
/// A collection of useful int (Int32) extension methods
/// </summary>
public static class IntExtension
{
    /// <summary>
    /// Formats integers so they have commas every three decimal places. Shorthand for "N0"
    /// </summary>
    [Pure]
    public static string ToDisplay(this int value)
    {
        return value.ToString("N0");
    }

    /// <summary>
    /// Formats integers so they have commas every three decimal places. Shorthand for "N0".
    /// </summary>
    /// <returns>Null if value is null, unless dashIfNull is true</returns>
    [Pure]
    public static string? ToDisplay(this int? value, bool dashIfNull = false)
    {
        if (value is null)
            return dashIfNull ? "-" : null;

        // Format the non-nullable value
        return value.Value.ToString("N0");
    }

    [Pure]
    public static char ToChar(this int value, bool isCaps = false)
    {
        return (char)((isCaps ? 'A' : 'a') + (value - 1));
    }

    /// <summary>
    /// Assumes value is always less than 16
    /// </summary>
    [Pure]
    public static char ToHexChar(int value)
    {
        return (char)(value < 10 ? value + '0' : value - 10 + 'A');
    }

    /// <summary>
    /// Converts a Unix timestamp to a <see cref="DateTime"/> in UTC. (Also available as a <see cref="long"/> extension)
    /// </summary>
    /// <remarks>
    /// This method extends the <see cref="int"/> type, allowing a Unix timestamp
    /// (which counts the number of seconds since the Unix Epoch at 00:00:00 UTC on 1 January 1970)
    /// to be directly converted into a <see cref="DateTime"/> object.
    /// The result is a UTC <see cref="DateTime"/> representing the same moment in time as the Unix timestamp.
    /// </remarks>
    /// <param name="unixTime">The Unix timestamp to convert, represented as the number of seconds since the Unix Epoch.</param>
    /// <returns>A UTC <see cref="DateTime"/> object representing the same moment in time as the specified Unix timestamp.</returns>
    /// <example>
    /// This example shows how to convert a Unix timestamp to a <see cref="DateTime"/>:
    /// <code>
    /// long unixTimestamp = 1588305600;
    /// DateTime dateTime = unixTimestamp.ToDateTimeFromUnixTime();
    /// Console.WriteLine(dateTime); // Output: 5/1/2020 12:00:00 AM (Depending on the system's time zone, output might vary)
    /// </code>
    /// </example>
    public static DateTime ToDateTimeFromUnixTime(this int unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
    }
}