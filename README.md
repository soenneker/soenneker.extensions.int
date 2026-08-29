[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Int.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Int/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.Int/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.Int/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Int.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Int/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.Int/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.Int/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Int
A  collection of useful int (Int32) extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.Int
```

## Quick start

```csharp
using Soenneker.Extensions.Int;

int value = 42;
var result = value.ToDisplay();
```

## Common operations

- `ToDisplay()` - Formats integers with thousands separators (comma), shorthand for "N0" invariant.
- `ToChar()` - Casts the integer to a UTF-16 `char`; the number is treated as a character code, not decimal text.
- `ToHexChar()` - Assumes value is always less than 16.
- `Pow10()` - Fast power of 10 calculation. Exponent must be between 0 and 28.
- `ToDateTimeFromUnixTime()` - Converts a Unix timestamp (seconds) to UTC `DateTime`.
- `ToDateTimeOffsetFromUnixTime()` - Converts a Unix timestamp (seconds) to `DateTimeOffset`.
- `ToGuidString()` - Converts a 32-bit integer into a deterministic GUID string in "D" format.
- `ApplyJitter()` - Applies uniform random jitter within ±(percent·|value|), with a minimum absolute delta.
