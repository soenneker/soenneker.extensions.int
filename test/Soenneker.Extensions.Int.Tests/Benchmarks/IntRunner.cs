﻿using System.Threading.Tasks;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Soenneker.Benchmarking.Extensions.Summary;
using Soenneker.Tests.Benchmark;
using Xunit;

namespace Soenneker.Extensions.Int.Tests.Benchmarks;

public class IntRunner : BenchmarkTest
{
    public IntRunner(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    //[Fact]
    public async ValueTask ToDisplay()
    {
        Summary summary = BenchmarkRunner.Run<ToDisplayBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog(OutputHelper, CancellationToken);
    }
}