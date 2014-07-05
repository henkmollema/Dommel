Dommel
======
[![Build status](https://ci.appveyor.com/api/projects/status/kynsbfu97f9s5bj7)](https://ci.appveyor.com/project/HenkMollema/dommel)

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. [Dapper](https://github.com/StackExchange/dapper-dot-net) is used for query execution and object mapping. The functionality is basically the same as [Dapper.Contrib](https://github.com/StackExchange/dapper-dot-net/tree/master/Dapper.Contrib) (and Dapper.Rainbow) but since it has not been updated any time lately and it has some shortcomings, Dommel was born.

<hr>

### Download
[![Download Dommel on NuGet](http://i.imgur.com/g9ZIbID.png "Download Dommel on NuGet")](https://www.nuget.org/packages/Dommel)

<hr>


### The API

##### Retrieving entities by id

```csharp
using (IDbConnection con = new SqlConnection())
{
   var product = con.Get<Product>(1);
}
```

##### Retrieving all entities in a table
```csharp
using (IDbConnection con = new SqlConnection())
{
   var products = con.GetAll<Product>().ToList();
}
```

##### Inserting entities

```csharp
using (var IDbConnection con = new SqlConnection())
{
   var product = new Product { Name = "Awesome bike", InStock = 4 };
   int id = con.Insert(product);
}
```

##### Updating entities

```csharp
using (IDbConnection con = new SqlConnection())
{
   var product = con.Get<Product>(1);
   product.LastUpdate = DateTime.Now;
   con.Update(product);
}
```

##### Removing entities

```csharp
using (IDbConnection con = new SqlConnection())
{
   var product = con.Get<Product>(1);
   con.Delete(product);
}
```

<hr>

### Extensibility
##### `ITableNameResolver`
Implement this interface if you want to customize the resolving of table names when building SQL queries.
```csharp
public class CustomTableNameResolver : Dommel.ITableNameResolver
{
    public string ResolveTableName(Type type)
    {
        // Every table has prefix 'tbl'.
        return "tbl" + type.Name;
    }
}
```

Use the `SetTableNameResolver()` method to register the custom implementation:
```csharp
Dommel.SetTableNameResolver(new CustomTableNameResolver());
```

##### `IKeyPropertyResolver`
Implement this interface if you want to customize the resolving of the key property of an entity. By default, Dommel will search for a property with the `[Key]` attribute, or a column with the name 'Id'.

If you, for example, have the naming convention of `{TypeName}Id` for key properties, you would implement the `IKeyPropertyResolver` like this:
```csharp
public class CustomKeyPropertyResolver : Dommel.IKeyPropertyResolver
{
    public PropertyInfo ResolveKeyProperty(Type type)
    {
        return type.GetProperties().Single(p => p.Name == string.Format("{0}Id", type.Name));
    }
}
```

Use the `SetKeyPropertyResolver()` method to register the custom implementation:
```csharp
Dommel.SetKeyPropertyResolver(new CustomKeyPropertyResolver());
```

##### `IColumnNameResolver`
Implement this interface if you want to customize the resolving of column names for when building SQL queries. This is useful when your naming conventions for database columns are different than your POCO properties.

```csharp
public class CustomColumnNameResolver : Dommel.IColumnNameResolver
{
    public string ResolveColumnName(PropertyInfo propertyInfo)
    {
        // Every column has prefix 'fld' and is uppercase.
        return "fld" + propertyInfo.Name.ToUpper();
    }
}
```

Use the `SetColumnNameResolver()` method to register the custom implementation:
```csharp
Dommel.SetColumnNameResolver(new CustomColumnNameResolver());
```

The [Dapper.FluentMap.Dommel](https://www.nuget.org/packages/Dapper.FluentMap.Dommel) extension implements these interfaces using the configured mapping. Also see: [Dapper.FluentMap](https://github.com/HenkMollema/Dapper-FluentMap#dommel).
