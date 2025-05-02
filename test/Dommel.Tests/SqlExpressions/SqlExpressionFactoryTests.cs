using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Xunit;
using Xunit.Abstractions;

namespace Dommel.Tests.SqlExpressions;
public class SqlExpressionFactoryTests
{
    private readonly ITestOutputHelper _outputHelper;

    public SqlExpressionFactoryTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void Run_Benchmarks()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger)
            .WithOptions(ConfigOptions.Default);

        BenchmarkRunner.Run<SqlExpressionFactoryBenchmarks>(config);

        // write benchmark summary
        _outputHelper.WriteLine(logger.GetLog());
    }
}
