using BenchmarkDotNet.Attributes;

namespace Soenneker.Extensions.Int.Tests.Benchmarks;

[MemoryDiagnoser]
public class ToDisplayBenchmarks
{
    private int value;

    [GlobalSetup]
    public void Setup()
    {
        value = 123456789;
    }

    [Benchmark]
    public string ToDisplay_ToString()
    {
        return value.ToString("N0");
    }

    [Benchmark]
    public string ToDisplay()
    {
        return value.ToDisplay();
    }
}