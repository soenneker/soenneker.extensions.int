using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Soenneker.Benchmarking.Extensions.Summary;
using Soenneker.Tests.Benchmark;

namespace Soenneker.Extensions.Int.Tests.Benchmarks;

public class IntRunner : BenchmarkTest
{
    public IntRunner() : base()
    {
    }

    //[Test]
    public async System.Threading.Tasks.ValueTask ToDisplay()
    {
        Summary summary = BenchmarkRunner.Run<ToDisplayBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog(OutputHelper, CancellationToken);
    }
}