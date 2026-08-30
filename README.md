[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Int.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Int/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.Int/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.Int/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Int.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Int/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.Int/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.Int/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Int
Formatting, character conversion, Unix time, deterministic identifiers, powers of ten, and bounded random jitter for `int` values.

## Installation

```bash
dotnet add package Soenneker.Extensions.Int
```

## Display formatting

```csharp
using Soenneker.Extensions.Int;

int value = 42;
string display = value.ToDisplay(); // "42"

int? missing = null;
string? blank = missing.ToDisplay();             // null
string dash = missing.ToDisplay(dashIfNull: true)!; // "-"
```

`ToDisplay()` uses invariant `N0` formatting, including comma group separators and a leading minus sign.

## Character conversion

```csharp
char lower = 1.ToChar();             // 'a'
char upper = 26.ToChar(isCaps: true); // 'Z'
char hex = IntExtension.ToHexChar(15); // 'F'
```

`ToChar()` expects a one-based alphabet position from 1 through 26. `ToHexChar()` expects a hexadecimal digit from 0 through 15. These methods do not validate their input; values outside those domains produce other UTF-16 characters.

## Powers of ten

```csharp
decimal scale = IntExtension.Pow10(6); // 1000000m
```

`Pow10()` returns an exact `decimal` power for exponents 0 through 28. Values outside that range throw `ArgumentOutOfRangeException`.

## Unix timestamps

```csharp
DateTime utc = unixSeconds.ToDateTimeFromUnixTime();
DateTimeOffset instant = unixSeconds.ToDateTimeOffsetFromUnixTime();
```

The input is seconds since the Unix epoch, not milliseconds. The `DateTime` result has `Kind` set to `Utc`; the `DateTimeOffset` result has a zero offset. Because the source is an `int`, its useful range is limited to 32-bit Unix seconds.

## Deterministic GUID text

```csharp
string id = customerNumber.ToGuidString();
```

`ToGuidString()` maps each 32-bit integer deterministically to a lowercase GUID string in `D` format. It is suitable for stable namespacing or compatibility keys. It does not create a random UUID and must not be used as an unpredictable token, secret, or proof of uniqueness outside the 32-bit input domain.

## Random jitter

```csharp
int delayedMilliseconds = 1_000.ApplyJitter(percent: 0.1, minDelta: 1);
// Uniformly selected from 900 through 1100.
```

`ApplyJitter()` chooses an inclusive random offset within `±max(minDelta, round(abs(value) * percent))`. At integer boundaries, the range is clipped so the returned value cannot overflow. `percent` must be from 0 through 1 and `minDelta` must be nonnegative.

The jitter source is appropriate for retry timing and load spreading, not cryptographic use.
