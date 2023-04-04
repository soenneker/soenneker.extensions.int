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
        if (value == null)
        {
            if (dashIfNull)
                return "-";
        }

        return value?.ToString("N0");
    }

    [Pure]
    public static char ToChar(this int value, bool isCaps = false)
    {
        var c = (char)((isCaps ? 65 : 97) + (value - 1));
        return c;
    }
}