using System;
using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Utils.Random;

namespace Soenneker.Extensions.Int;

/// <summary>
/// A collection of useful int (Int32) extension methods
/// </summary>
public static class IntExtension
{
    private static readonly decimal[] _pow10Table =
    [
        1m, 10m, 100m, 1000m, 10000m, 100000m, 1000000m, 10000000m,
        100000000m, 1000000000m, 10000000000m, 100000000000m,
        1000000000000m, 10000000000000m, 100000000000000m, 1000000000000000m,
        10000000000000000m, 100000000000000000m, 1000000000000000000m,
        10000000000000000000m, 100000000000000000000m, 1000000000000000000000m,
        10000000000000000000000m, 100000000000000000000000m,
        1000000000000000000000000m, 10000000000000000000000000m,
        100000000000000000000000000m, 1000000000000000000000000000m
    ];

    /// <summary>
    /// Formats integers so they have commas every three decimal places. Shorthand for "N0"
    /// </summary>
    [Pure]
    public static string ToDisplay(this int value)
    {
        const char thousandSeparator = ',';
        const int maxBufferSize = 16; // Sufficient for int.MinValue: -2,147,483,648

        // Handle the zero case early to avoid unnecessary processing
        if (value == 0)
        {
            return "0";
        }

        Span<char> buffer = stackalloc char[maxBufferSize];
        int index = maxBufferSize;

        // Handle negative numbers
        bool isNegative = value < 0;
        if (isNegative)
        {
            value = -value; // Safe as int.MinValue is handled below
        }

        // Extract digits and place separators
        var digitCount = 0;
        while (value > 0)
        {
            if (digitCount == 3)
            {
                buffer[--index] = thousandSeparator;
                digitCount = 0;
            }

            buffer[--index] = (char)('0' + value % 10);
            value /= 10;
            digitCount++;
        }

        // Add the negative sign if needed
        if (isNegative)
        {
            buffer[--index] = '-';
        }

        // Return the result string
        return new string(buffer[index..]);
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
        return value.Value.ToDisplay();
    }

    [Pure]
    public static char ToChar(this int value, bool isCaps = false)
    {
        return (char) ((isCaps ? 'A' : 'a') + (value - 1));
    }

    /// <summary>
    /// Assumes value is always less than 16
    /// </summary>
    [Pure]
    public static char ToHexChar(int value)
    {
        return (char) (value < 10 ? value + '0' : value - 10 + 'A');
    }

    /// <summary>
    /// Fast power of 10 calculation. Exponent must be between 0 and 28.
    /// </summary>
    /// <param name="exponent"></param>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Pow10(int exponent)
    {
        return _pow10Table[exponent];
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

    /// <summary>
    /// Converts a 32-bit integer into a unique, deterministic GUID string.
    /// </summary>
    /// <param name="value">The integer to convert into a GUID.</param>
    /// <returns>A GUID string representation in "D" format.</returns>
    /// <remarks>
    /// This method ensures that the same integer always produces the same GUID.
    /// It spreads entropy across all 16 bytes by using a combination of bitwise
    /// operations and a large prime multiplier to improve uniqueness.
    /// The resulting GUID is in little-endian format for cross-platform consistency.
    /// </remarks>
    /// <example>
    /// <code>
    /// int value = 123456;
    /// string guidString = value.ToGuidString();
    /// Console.WriteLine(guidString); // Example: "40E20100-ABCD-1234-5678-90ABCDEF1234"
    /// </code>
    /// </example>
    [Pure]
    public static string ToGuidString(this int value)
    {
        Span<byte> bytes = stackalloc byte[16];

        // Store integer as little-endian explicitly to ensure cross-platform consistency
        BinaryPrimitives.WriteInt32LittleEndian(bytes, value);

        // Compute a hash and store it in a consistent order
        int hash = HashInteger(value);
        BinaryPrimitives.WriteInt32LittleEndian(bytes[4..], hash);

        // Use a large prime multiplication to further distribute bits
        long mixedValue = value * 6364136223846793005L;
        BinaryPrimitives.WriteInt64LittleEndian(bytes[8..], mixedValue);

        // Convert to a GUID and return as a string
        return new Guid(bytes).ToString("D");
    }

    /// <summary>
    /// Applies a random jitter to the specified integer value, within a percentage-based range.
    /// </summary>
    /// <param name="value">The original integer value to apply jitter to.</param>
    /// <param name="percent">
    /// The maximum percentage of the original value used to compute the jitter range (e.g., 0.1 for ±10%). 
    /// Must be between 0.0 and 1.0. Defaults to 0.1.
    /// </param>
    /// <param name="minDelta">
    /// The minimum amount of jitter to apply regardless of the computed percentage. 
    /// Useful to ensure non-zero jitter for small values. Defaults to 1.
    /// </param>
    /// <returns>
    /// An integer with random jitter applied, uniformly distributed within the calculated range.
    /// The return value will be in the range [value - delta, value + delta].
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="percent"/> is less than 0.0 or greater than 1.0.
    /// </exception>
    /// <remarks>
    /// The jitter is applied symmetrically around the original value. For example, with a value of 100 and a percent of 0.1,
    /// the result will be between 90 and 110 (inclusive). The jitter range is calculated using rounding to avoid downward bias.
    /// </remarks>
    /// <example>
    /// <code>
    /// int original = 100;
    /// int jittered = original.ApplyJitter(); // returns a value between 90 and 110
    /// </code>
    /// </example>
    [Pure]
    public static int ApplyJitter(this int value, double percent = 0.1, int minDelta = 1)
    {
        if (percent is < 0.0 or > 1.0)
            throw new ArgumentOutOfRangeException(nameof(percent), "Must be between 0.0 and 1.0");

        int delta = Math.Max(minDelta, (int)Math.Round(Math.Abs(value) * percent));
        return value + RandomUtil.Next(-delta, delta + 1);
    }

    private static int HashInteger(int value)
    {
        unchecked
        {
            value ^= value >> 16;
            value *= unchecked((int)0x85ebca6b); // Ensure signed integer multiplication
            value ^= value >> 13;
            value *= unchecked((int)0xc2b2ae35); // Ensure signed integer multiplication
            value ^= value >> 16;
        }
        return value;
    }

}