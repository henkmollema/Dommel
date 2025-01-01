using BenchmarkDotNet.Attributes;

namespace Dommel.Tests.SqlExpressions;

[MemoryDiagnoser(true)]
public class SqlExpressionFactoryBenchmarks
{
    private static readonly ISqlBuilder DummySqlBuilder = new SqlServerSqlBuilder();

    [Benchmark(Baseline = true)]
    public object ActivatorBasedFactory()
    {
        return DommelMapper.SqlExpressionFactory(typeof(Product), DummySqlBuilder);
    }

    [Benchmark]
    public object CompiledExpressionFactory()
    {
        return DommelMapper.CompiledSqlExpressionFactory(typeof(Product), DummySqlBuilder);
    }
}