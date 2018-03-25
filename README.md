# Dommel
Simple CRUD operations for Dapper.

| Windows | Linux | NuGet | MyGet |
| ------- | ----- | ----- | ----- |
| ![AppVeyor](https://img.shields.io/appveyor/ci/henkmollema/dommel.svg?style=flat-square) | ![Travis](https://img.shields.io/travis/henkmollema/Dommel.svg?style=flat-square) | ![NuGet](https://img.shields.io/nuget/vpre/Dommel.svg?style=flat-square) | ![MyGet Pre Release](https://img.shields.io/myget/dommel-ci/vpre/Dommel.svg?style=flat-square) |

<hr>

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. The SQL queries are generated based on your POCO entities. Dommel also supports LINQ expressions which are being translated to SQL expressions. [Dapper](https://github.com/StackExchange/dapper-dot-net) is used for query execution and object mapping.

Dommel also provides extensibility points to change the bahavior of resolving table names, column names, the key property and POCO properties. See [Extensibility](https://github.com/henkmollema/Dommel#extensibility) for more details.

<hr>

## Download
[![Download Dommel on NuGet](http://i.imgur.com/g9ZIbID.png "Download Dommel on NuGet")](https://www.nuget.org/packages/Dommel)

<hr>

## API

#### Retrieving entities by id
```csharp
using (var con = new SqlConnection())
{
   var product = con.Get<Product>(1);
}
```

#### Retrieving all entities in a table
```csharp
using (var con = new SqlConnection())
{
   var products = con.GetAll<Product>().ToList();
}
```

#### Selecting entities using a predicate
Dommel allows you to specify a predicate which is being translated into a SQL expression. The arguments in the lambda expression are added as parameters to the command.
```csharp
using (var con = new SqlConnection())
{
   var products = con.Select<Product>(p => p.Name == "Awesome bike");
   
   var products = con.Select<Product>(p => p.Created < new DateTime(2014, 12, 31) && p.InStock > 5);
}
```

#### Inserting entities
```csharp
using (var con = new SqlConnection())
{
   var product = new Product { Name = "Awesome bike", InStock = 4 };
   int id = con.Insert(product);
}
```

#### Updating entities
```csharp
using (var con = new SqlConnection())
{
   var product = con.Get<Product>(1);
   product.LastUpdate = DateTime.Now;
   con.Update(product);
}
```

#### Removing entities
```csharp
using (var con = new SqlConnection())
{
   var product = con.Get<Product>(1);
   con.Delete(product);
}
```

<hr>

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

The [Dapper.FluentMap.Dommel](https://www.nuget.org/packages/Dapper.FluentMap.Dommel) extension implements these interfaces using the configured mapping. Also see: [Dapper.FluentMap](https://github.com/HenkMollema/Dapper-FluentMap#dommel).
