using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.Tests;
public class UpdateTests
{
    private static readonly MySqlSqlBuilder MySqlSqlBuilder = new();

    [Fact]
    public void CreateUpdateQuery()
    {
        var sql = DommelMapper.BuildUpdateMultipleQuery<Product>(
            MySqlSqlBuilder,
            x => x.SetProperty(x => x.Name, "Blah").SetProperty(x => x.CategoryId, x => x.CategoryId * 2),
            p => p.Id == 1,
            out var parameters);

        Assert.Equal("update `Products` set `Products`.`CategoryId` = `Products`.`CategoryId` * @p2, `Products`.`FullName` = @p3 where `Products`.`Id` = @p1", sql);
        Assert.Collection(parameters.ParameterNames,
            x => Assert.Equal(1, parameters.Get<int>(x)),
            x => Assert.Equal(2, parameters.Get<int>(x)),
            x => Assert.Equal("Blah", parameters.Get<string>(x)));

    }
}
