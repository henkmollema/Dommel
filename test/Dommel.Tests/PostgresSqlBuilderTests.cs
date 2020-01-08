using System;
using Xunit;

namespace Dommel.Tests
{
    public class PostgresSqlBuilderTests
    {
        [Fact]
        public void BuildInsert_ThrowsWhenTypeIsNull()
        {
            var builder = new PostgresSqlBuilder();
            Assert.Throws<ArgumentNullException>("type", () => builder.BuildInsert(null!, null!, null!, null!));
        }
    }
}
