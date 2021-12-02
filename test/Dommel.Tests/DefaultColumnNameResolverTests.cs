using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests;

public class DefaultColumnNameResolverTests
{
    private static readonly DefaultColumnNameResolver Resolver = new DefaultColumnNameResolver();

    [Fact]
    public void ResolvesName()
    {
        var name = Resolver.ResolveColumnName(typeof(Foo).GetProperty("Bar")!);
        Assert.Equal("Bar", name);
    }

    [Fact]
    public void ResolvesColumnAttribute()
    {
        var name = Resolver.ResolveColumnName(typeof(Bar).GetProperty("FooBarBaz")!);
        Assert.Equal("foo_bar_baz", name);
    }

    private class Foo
    {
        public string? Bar { get; set; }
    }

    private class Bar
    {
        [Column("foo_bar_baz")]
        public string? FooBarBaz { get; set; }
    }
}
