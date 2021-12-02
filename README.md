# Dommel
CRUD operations with Dapper made simple.

| Windows | Linux | NuGet | MyGet | Test Coverage |
| ------- | ----- | ----- | ----- | ------------- |
| [![AppVeyor](https://img.shields.io/appveyor/ci/henkmollema/dommel/master.svg?style=flat-square)](https://ci.appveyor.com/project/henkmollema/dommel) | [![Travis](https://img.shields.io/travis/com/henkmollema/Dommel?style=flat-square)](https://app.travis-ci.com/github/henkmollema/Dommel) | [![NuGet](https://img.shields.io/nuget/vpre/Dommel.svg?style=flat-square)](https://www.nuget.org/packages/Dommel) | [![MyGet Pre Release](https://img.shields.io/myget/dommel-ci/vpre/Dommel.svg?style=flat-square)](https://www.myget.org/feed/dommel-ci/package/nuget/Dommel) | [![codecov](https://codecov.io/gh/henkmollema/Dommel/branch/master/graph/badge.svg)](https://codecov.io/gh/henkmollema/Dommel) |

<hr>

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. The SQL queries are generated based on your POCO entities. Dommel also supports LINQ expressions which are being translated to SQL expressions. [Dapper](https://github.com/StackExchange/Dapper) is used for query execution and object mapping.

There are several extensibility points available to change the behavior of resolving table names, column names, the key property and POCO properties. See [Extensibility](https://github.com/henkmollema/Dommel#extensibility) for more details.

## Installing Dommel

Dommel is available on [NuGet](https://www.nuget.org/packages/Dommel).

### Install using the .NET CLI:
```
dotnet add package Dommel
```

### Install using the NuGet Package Manager:
```
Install-Package Dommel
```

## Using Dommel

### Retrieving entities by ID
```cs
var product = await connection.GetAsync<Product>(1);
```

### Retrieving all entities in a table
```cs
var products = await connection.GetAllAsync<Product>();
```

### Selecting entities using a predicate
Dommel allows you to specify a predicate which is being translated into a SQL expression. The arguments in the lambda expression are added as parameters to the command.
```cs
var products = await connection.SelectAsync<Product>(p => p.Name == "Awesome bike" && p.Created < new DateTime(2014, 12, 31) && p.InStock > 5);
```

There is also a `FirstOrDefaultAsync<T>(...)` method available to select the entity matching the predicate.

#### Like-queries
It is possible to generate `LIKE`-queries using `Contains()`, `StartsWith()` or `EndsWith()` on string properties:

```cs
var products = await connection.SelectAsync<Product>(p => p.Name.Contains("bike"));
var products = await connection.SelectAsync<Product>(p => p.Name.StartsWith("bike"));
var products = await connection.SelectAsync<Product>(p => p.Name.EndsWith("bike"));
```

### Inserting entities
```cs
var product = new Product { Name = "Awesome bike", InStock = 4 };
var id = await connection.InsertAsync(product);
```

### Updating entities
```cs
var product = await connection.GetAsync<Product>(1);
product.Name = "New name";
await connection.UpdateAsync(product);
```

### Removing entities
```cs
var product = await connection.GetAsync<Product>(1);
await connection.DeleteAsync(product);
```

### Multi mapping

#### One-to-one relations

Dommel is able to generate join-queries based on the specified multi mapping function. Consider the following POCO's:

```cs
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; }

    // Maps to the foreign key column
    public int CategoryId { get; set; }

    // The navigation property
    public Category Category { get; set; }
}


public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

The `Product` with its associated `Category` can be queried toegether using the `Get<T1, T2, ..., TResult>()` method:

```cs
var product = await product.GetAsync<Product, Category, Product>(1, (product, category) =>
{
    product.Category = category;
    return product;
});
```

##### Foreign key columns

`CategoryId` is automatically used as foreign key between `Product` and `Category`. This follows a simple convention: joining table name + `Id` (`Category` + `Id`). You can override this behavior by using the `[ForeignKey("ForeignKeyColumnName")]` attribute or by implementing your own `IForeignKeyPropertyResolver`.

#### One-to-many relations

One-to-many relationships work in a similar way, expect that the foreign key is defined on the _joined_ type rather than the _source_ type. For example:

```cs
public class Order
{
    public int Id { get; set; }

    // The navigation property
    public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}

public class OrderLine
{
    public int Id { get; set; }

    // The foreign key column to the Order table
    public int OrderId { get; set; }

    public string Description { get; set; }
}
```

The `Order` with its child `OrderLine`'s can be queried toegether using the `Get<T1, T2, ..., TResult>()` method:

```cs
var product = await product.GetAsync<Order, OrderLine, Order>(1, (order, line) =>
{
    // Naive mapping example, in reality it requires more gymnastics
    order.OrderLines.Add(line);
    return order;
});
```

### Automatic multi mapping

> Note: this is an experimental feature.

Dommel is able to create simple join-expressions for retrieving parent-child entities. One-to-one and one-to-many relations are supported. It works the samy way as regular mapping, except there is no need to specify a function which performs the mapping of the objects. Using the same POCO's as the previous examples:

Retrieving a `Product` and its associated `Category`:

```cs
var product = product.Get<Product, Category, Product>(1);
```

Retrieving one `Order` and with its child `OrderLine`'s:

```cs
var product = product.Get<Order, OrderLine, Order>(1);
```

#### Entity equality
When joining with two or more tables with a one-to-many relationship, you are required to override `Equals(object obj)` method or implement the `IEquatable<T>` interface on your POCO's so Dommel can determine whether an entity is already added to the collection. For example:

```cs
public class OrderLine : IEquatable<OrderLine>
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Description { get; set; }

    public bool Equals(OrderLine other) => Id == other.Id;
}
```

### Combining `Select` and multi-mapping
It's possible to combine `Select` queries and multi-mapping. For example:
```cs
var products = await connection.SelectAsync<Product, Category, Product>(x => x.Name.StartsWith("bike"));
```
This is applicable for `Select`, `SelectAsync`, `FirstOrDefault` and `FirstOrDefaultAsync`. Both with manual and automatic multi-mapping.

### From-queries
With `From`-queries you can create more complex queries on a certain table by providing access to the `SqlExpression<T>`. For example:
```cs
var products = await connection.FromAsync<Product>(sql => sql
    .Where(x => x.Name.StartsWith("bike") && x.DeletedOn == null)
    .OrWhere(x => x.InStock > 5)
    .OrderBy(x => x.DateCreated)
    .Page(1, 25)
    .Select()));
```

## Async and non-async
All Dommel methods have async and non-async variants, such as as `Get` & `GetAsync`, `GetAll` & `GetAllAsync`, `Select` & `SelectAsync`, `Insert` & `InsertAsync`, `Update` & `UpdateAsync`, `Delete` & `DeleteAsync`, etc.

## Query builders

Dommel supports building specialized queries for a certain RDBMS. By default, query builders for the following RDMBS are included: SQL Server, SQL Server CE, SQLite, MySQL and Postgres. The query builder to be used is determined by the connection type. To add or overwrite an existing query builder, use the `AddSqlBuilder()`  method:

```cs
DommelMapper.AddSqlBuilder(typeof(SqlConnection), new CustomSqlBuilder());
```

<hr>

## Extensibility
#### `ITableNameResolver`
Implement this interface if you want to customize the resolving of table names when building SQL queries.

```cs
public class CustomTableNameResolver : ITableNameResolver
{
    public string ResolveTableName(Type type)
    {
        // Every table has prefix 'tbl'.
        return $"tbl{type.Name}";
    }
}
```

Use the `SetTableNameResolver()` method to register the custom implementation:
```cs
SetTableNameResolver(new CustomTableNameResolver());
```

#### `IKeyPropertyResolver`
Implement this interface if you want to customize the resolving of the key property of an entity. By default, Dommel will search for a property with the `[Key]` attribute, or a column with the name 'Id'.

If you, for example, have the naming convention of `{TypeName}Id` for key properties, you would implement the `IKeyPropertyResolver` like this:

```cs
public class CustomKeyPropertyResolver : IKeyPropertyResolver
{
    public ColumnPropertyInfo[] ResolveKeyProperties(Type type)
    {
        return new [] { new ColumnPropertyInfo(type.GetProperties().Single(p => p.Name == $"{type.Name}Id"), isKey: true) };
    }
}
```

Use the `SetKeyPropertyResolver()` method to register the custom implementation:

```cs
DommelMapper.SetKeyPropertyResolver(new CustomKeyPropertyResolver());
```

#### `IForeignKeyPropertyResolver`
Implement this interface if you want to customize the resolving of the foreign key property from one entity to another. By default Dommel will search for a property of `{TypeName}Id` or the column name specified using the `[ForeignKey]` attribute.

> This is a rather advanced interface. Providing your own implementation requires quite some knowledge of the way Dommel handles foreign key relationships. Consider subclassing `DefaultForeignKeyPropertyResolver` and override `ResolveForeignKeyProperty()`.

Use the `SetForeignKeyPropertyResolver()` method to register the custom implementation:

```cs
DommelMapper.SetForeignKeyPropertyResolver(new CustomForeignKeyPropertyResolver());
```

#### `IColumnNameResolver`
Implement this interface if you want to customize the resolving of column names for when building SQL queries. This is useful when your naming conventions for database columns are different than your POCO properties.

```cs
public class CustomColumnNameResolver : IColumnNameResolver
{
    public string ResolveColumnName(PropertyInfo propertyInfo)
    {
        // Every column has prefix 'fld' and is uppercase.
        return $"fld{propertyInfo.Name.ToUpper()}";
    }
}
```

Use the `SetColumnNameResolver()` method to register the custom implementation:

```cs
DommelMapper.SetColumnNameResolver(new CustomColumnNameResolver());
```

The [Dapper.FluentMap.Dommel](https://www.nuget.org/packages/Dapper.FluentMap.Dommel) extension implements these interfaces using the configured mapping of Dapper.FluentMap. Also see: [Dapper.FluentMap](https://github.com/HenkMollema/Dapper-FluentMap#dommel).
