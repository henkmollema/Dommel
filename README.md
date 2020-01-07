# Dommel
CRUD operations with Dapper made simple.

| Windows | Linux | NuGet | MyGet | Test Coverage |
| ------- | ----- | ----- | ----- | ------------- |
| [![AppVeyor](https://img.shields.io/appveyor/ci/henkmollema/dommel/master.svg?style=flat-square)](https://ci.appveyor.com/project/henkmollema/dommel) | [![Travis](https://img.shields.io/travis/henkmollema/Dommel.svg?style=flat-square)](https://travis-ci.org/henkmollema/Dommel) | [![NuGet](https://img.shields.io/nuget/vpre/Dommel.svg?style=flat-square)](https://www.nuget.org/packages/Dommel) | [![MyGet Pre Release](https://img.shields.io/myget/dommel-ci/vpre/Dommel.svg?style=flat-square)](https://www.myget.org/feed/dommel-ci/package/nuget/Dommel) | [![codecov](https://codecov.io/gh/henkmollema/Dommel/branch/master/graph/badge.svg)](https://codecov.io/gh/henkmollema/Dommel) |

<hr>

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. The SQL queries are generated based on your POCO entities. Dommel also supports LINQ expressions which are being translated to SQL expressions. [Dapper](https://github.com/StackExchange/Dapper) is used for query execution and object mapping.

There are several extensibility points available to change the behavior of resolving table names, column names, the key property and POCO properties. See [Extensibility](https://github.com/henkmollema/Dommel#extensibility) for more details.

## Download

Dommel is available on [NuGet](https://www.nuget.org/packages/Dommel):

Using the .NET Core CLI:
#### `dotnet add package Dommel`

Using the NuGet Package Manager:
#### `PM> Install-Package Dommel`

## API

### Retrieving entities by ID
```csharp
using (var con = new SqlConnection())
{
   var product = con.Get<Product>(1);
}
```

### Retrieving all entities in a table
```csharp
using (var con = new SqlConnection())
{
   var products = con.GetAll<Product>().ToList();
}
```

### Selecting entities using a predicate
Dommel allows you to specify a predicate which is being translated into a SQL expression. The arguments in the lambda expression are added as parameters to the command.
```csharp
using (var con = new SqlConnection())
{
   var products = con.Select<Product>(p => p.Name == "Awesome bike");

   var products = con.Select<Product>(p => p.Created < new DateTime(2014, 12, 31) && p.InStock > 5);
}
```

#### Like-queries
It is possible to generate `LIKE`-queries using `Contains()`, `StartsWith()` or `EndsWith()` on string properties:

```cs
using (var con = new SqlConnection())
{
   var products = con.Select<Product>(p => p.Name.Contains("bike"));
   var products = con.Select<Product>(p => p.Name.StartsWith("bike"));
   var products = con.Select<Product>(p => p.Name.EndsWith("bike"));
}
```

### Inserting entities
```csharp
using (var con = new SqlConnection())
{
   var product = new Product { Name = "Awesome bike", InStock = 4 };
   int id = con.Insert(product);
}
```

### Updating entities
```csharp
using (var con = new SqlConnection())
{
   var product = con.Get<Product>(1);
   product.LastUpdate = DateTime.UtcNow;
   con.Update(product);
}
```

### Removing entities
```csharp
using (var con = new SqlConnection())
{
   var product = con.Get<Product>(1);
   con.Delete(product);
}
```

### Multi mapping

### One-to-one relations

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
var product = product.Get<Product, Category, Product>(1, (product, category) =>
{
    product.Category = category;
    return product;
});
```

#### Foreign key columns

`CategoryId` is automatically used as foreign key between `Product` and `Category`. This follows a simple convention: joining table name + `Id` (`Category` + `Id`). You can override this behavior by using the `[ForeignKey("ForeignKeyColumnName")]` attribute or by implementing your own `IForeignKeyPropertyResolver`.

### One-to-many relations

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
var product = product.Get<Order, OrderLine, Order>(1, (order, line) =>
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

## Async
All Dommel methods have an async counterparts, such as as `GetAsync`, `GetAllAsync`, `SelectAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync`, etc.

## Query builders

Dommel supports building specialized queries for a certain RDBMS. By default, query builders for the following RDMBS are included: SQL Server, SQL Server CE, SQLite, MySQL and Postgres. The query builder to be used is determined by the connection type. To add or overwrite an existing query builder, use the `AddSqlBuilder()`  method:

```csharp
DommelMapper.AddSqlBuilder(typeof(SqlConnection), new CustomSqlBuilder());
```

<hr>

## Extensibility
#### `ITableNameResolver`
Implement this interface if you want to customize the resolving of table names when building SQL queries.

```csharp
public class CustomTableNameResolver : DommelMapper.ITableNameResolver
{
    public string ResolveTableName(Type type)
    {
        // Every table has prefix 'tbl'.
        return $"tbl{type.Name}";
    }
}
```

Use the `SetTableNameResolver()` method to register the custom implementation:
```csharp
DommelMapper.SetTableNameResolver(new CustomTableNameResolver());
```

#### `IKeyPropertyResolver`
Implement this interface if you want to customize the resolving of the key property of an entity. By default, Dommel will search for a property with the `[Key]` attribute, or a column with the name 'Id'.

If you, for example, have the naming convention of `{TypeName}Id` for key properties, you would implement the `IKeyPropertyResolver` like this:

```csharp
public class CustomKeyPropertyResolver : DommelMapper.IKeyPropertyResolver
{
    public PropertyInfo ResolveKeyProperty(Type type)
    {
        return type.GetProperties().Single(p => p.Name == $"{type.Name}Id");
    }
}
```

Use the `SetKeyPropertyResolver()` method to register the custom implementation:

```csharp
DommelMapper.SetKeyPropertyResolver(new CustomKeyPropertyResolver());
```

#### `IForeignKeyPropertyResolver`
Implement this interface if you want to customize the resolving of the foreign key property from one entity to another. By default Dommel will search for a property of `{TypeName}Id` or the column name specified using the `[ForeignKey]` attribute.

> This is a rather advanced interface. Providing your own implementation requires quite some knowledge of the way Dommel handles foreign key relationships. Consider subclassing `DefaultForeignKeyPropertyResolver` and override `ResolveForeignKeyProperty()`.

Use the `SetForeignKeyPropertyResolver()` method to register the custom implementation:

```csharp
DommelMapper.SetForeignKeyPropertyResolver(new CustomForeignKeyPropertyResolver());
```

#### `IColumnNameResolver`
Implement this interface if you want to customize the resolving of column names for when building SQL queries. This is useful when your naming conventions for database columns are different than your POCO properties.

```csharp
public class CustomColumnNameResolver : DommelMapper.IColumnNameResolver
{
    public string ResolveColumnName(PropertyInfo propertyInfo)
    {
        // Every column has prefix 'fld' and is uppercase.
        return $"fld{propertyInfo.Name.ToUpper()}";
    }
}
```

Use the `SetColumnNameResolver()` method to register the custom implementation:

```csharp
DommelMapper.SetColumnNameResolver(new CustomColumnNameResolver());
```

The [Dapper.FluentMap.Dommel](https://www.nuget.org/packages/Dapper.FluentMap.Dommel) extension implements these interfaces using the configured mapping of Dapper.FluentMap. Also see: [Dapper.FluentMap](https://github.com/HenkMollema/Dapper-FluentMap#dommel).
