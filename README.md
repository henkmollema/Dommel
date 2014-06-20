Dommel
======
[![Build status](https://ci.appveyor.com/api/projects/status/kynsbfu97f9s5bj7)](https://ci.appveyor.com/project/HenkMollema/dommel)

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. [Dapper](https://github.com/StackExchange/dapper-dot-net) is used for query execution and object mapping. The functionality is basically the same as [Dapper.Contrib](https://github.com/StackExchange/dapper-dot-net/tree/master/Dapper.Contrib) (and Dapper.Rainbow) but since it's not updated any time lately and it has some shortcomings, I decided to create a similar tool.

One of the things I got stuck with using Dapper.Contrib, was mapping of column names. For Dapper, I solved this by creating [Dapper.FluentMap](https://github.com/HenkMollema/Dapper-FluentMap), but this doesn't work for Dapper.Contrib.

### To-do
* Improve documentation in readme.
* Add functionality to customize column name mapping of POCO properties.

### Donwload
NuGet package comming soon.

### The API

##### Retrieving entities:

```csharp
using (IDbConnection con = new SqlConnection())
{
   var product = con.Get<Product>(1);
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

### Extensibility points
Currently the API exists of 2 extension points:
##### `ITableNameResolver`

##### `IKeyPropertyResolver`
