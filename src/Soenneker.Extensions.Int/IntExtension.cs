using System;
using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Soenneker.Extensions.Int;

/// <summary>
/// A collection of useful int (Int32) extension methods
/// </summary>
public static class IntExtension
{
    // Using an array keeps it JIT-friendly and simple; range guard in Pow10 lets JIT remove bounds checks.
    private static readonly decimal[] _pow10Table =
    [
        1m, 10m, 100m, 1000m, 10000m, 100000m, 1000000m, 10000000m,
        100000000m, 1000000000m, 10000000000m, 100000000000m,
        1000000000000m, 10000000000000m, 100000000000000m, 1000000000000000m,
        10000000000000000m, 100000000000000000m, 1000000000000000000m,
        10000000000000000000m, 100000000000000000000m, 1000000000000000000000m,
        10000000000000000000000m, 100000000000000000000000m,
        1000000000000000000000000m, 10000000000000000000000000m,
        100000000000000000000000000m, 1000000000000000000000000000m,
        10000000000000000000000000000m
    ];

    /// <summary>Formats integers with thousands separators (comma), shorthand for "N0" invariant.</summary>
    /// <returns>Formats integers with thousands separators (comma), shorthand for "N0" invariant.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToDisplay(this int value)
    {
        // Enough for "-2,147,483,648"
        Span<char> buf = stackalloc char[16];

        if (value.TryFormat(buf, out int written, format: "N0", provider: CultureInfo.InvariantCulture))
            return new string(buf[..written]);

        // Fallback: allocate once via string.Format (should not happen for int)
        return string.Format(CultureInfo.InvariantCulture, "{0:N0}", value);
    }

    /// <summary>Formats integers with thousands separators (comma), shorthand for "N0" invariant.</summary>
    /// <returns>Null if value is null, unless dashIfNull is true.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ToDisplay(this int? value, bool dashIfNull = false)
    {
        if (!value.HasValue)
            return dashIfNull ? "-" : null;

        return value.Value.ToDisplay();
    }

    /// <summary>
    /// Maps an integer from 1 through 26 to its alphabetic character.
    /// </summary>
    /// <param name="value">The one-based alphabet position to convert.</param>
    /// <param name="isCaps">True to return an uppercase letter; false for lowercase.</param>
    /// <returns>The corresponding alphabetic character.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToChar(this int value, bool isCaps = false)
    {
        // Assumes 1..26 input; use unchecked to avoid extra range costs.
        return unchecked((char)((isCaps ? 'A' : 'a') + (value - 1)));
    }

    /// <summary>Assumes value is always less than 16.</summary>
    /// <returns>Assumes value is always less than 16.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToHexChar(int value)
    {
        // No branching on sign thanks to assumption; keeps it to two very cheap paths.
        return (char)(value < 10 ? value + '0' : value - 10 + 'A');
    }

    /// <summary>Fast power of 10 calculation. Exponent must be between 0 and 28.</summary>
    /// <returns>Fast power of 10 calculation. Exponent must be between 0 and 28.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Pow10(int exponent)
    {
        // Simple range guard enables the JIT to eliminate bounds checks on the indexed load.
        if ((uint)exponent >= 29u)
            throw new ArgumentOutOfRangeException(nameof(exponent), "Exponent must be between 0 and 28.");
        return _pow10Table[exponent];
    }

    /// <summary>
    /// Converts a Unix timestamp (seconds) to UTC <see cref="DateTime"/>.
    /// </summary>
    /// <returns>Converts a Unix timestamp (seconds) to UTC <see cref="DateTime"/>.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ToDateTimeFromUnixTime(this int unixTime)
    {
        return DateTime.UnixEpoch.AddSeconds(unixTime);
    }

    /// <summary>
    /// Converts a Unix timestamp (seconds) to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <returns>Converts a Unix timestamp (seconds) to <see cref="DateTimeOffset"/>.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToDateTimeOffsetFromUnixTime(this int unixTime)
    {
        return DateTimeOffset.UnixEpoch.AddSeconds(unixTime);
    }

    /// <summary>
    /// Converts a 32-bit integer into a deterministic GUID string in "D" format.
    /// </summary>
    /// <returns>Converts a 32-bit integer into a deterministic GUID string in "D" format.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToGuidString(this int value)
    {
        Span<byte> bytes = stackalloc byte[16];

        // 0..3: value (little-endian)
        BinaryPrimitives.WriteInt32LittleEndian(bytes, value);

        // 4..7: mixed/hash
        int hash = HashInteger(value);
        BinaryPrimitives.WriteInt32LittleEndian(bytes[4..], hash);

        // 8..15: large-prime mix (same constant as SplitMix64; good bit diffusion)
        long mixedValue = unchecked(value * 6364136223846793005L);
        BinaryPrimitives.WriteInt64LittleEndian(bytes[8..], mixedValue);

        return new Guid(bytes).ToString("D");
    }

    /// <summary>
    /// Applies uniform random jitter within ±(percent·|value|), with a minimum absolute delta.
    /// </summary>
    /// <returns>Applies uniform random jitter within ±(percent·|value|), with a minimum absolute delta.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ApplyJitter(this int value, double percent = 0.1, int minDelta = 1)
    {
        if ((uint)minDelta > int.MaxValue) // cheap validation (also filters negatives)
            throw new ArgumentOutOfRangeException(nameof(minDelta));

        if (double.IsNaN(percent) || percent is < 0.0 or > 1.0)
            throw new ArgumentOutOfRangeException(nameof(percent), "Must be between 0.0 and 1.0");

        // Long arithmetic keeps int.MinValue, the upper bound, and the final addition representable.
        long proportionalDelta = (long)Math.Round(Math.Abs((long)value) * percent);
        long delta = Math.Max(minDelta, proportionalDelta);
        long minimumOffset = Math.Max(-delta, int.MinValue - (long)value);
        long maximumOffset = Math.Min(delta, int.MaxValue - (long)value);
        long offset = System.Random.Shared.NextInt64(minimumOffset, maximumOffset + 1);

        return (int)(value + offset);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int HashInteger(int value)
    {
        unchecked
        {
            // 32-bit mix (Murmur3 finalizer style)
            value ^= value >>> 16;
            value *= unchecked((int)0x85EBCA6B);
            value ^= value >>> 13;
            value *= unchecked((int)0xC2B2AE35);
            value ^= value >>> 16;
            return value;
        }
    }
}
