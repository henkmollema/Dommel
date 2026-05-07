# Dommel
CRUD operations with Dapper made simple.

| Build | NuGet |
| ----- | ----- |
| [![CI](https://github.com/henkmollema/Dommel/actions/workflows/ci.yml/badge.svg)](https://github.com/henkmollema/Dommel/actions/workflows/ci.yml) | [![NuGet](https://img.shields.io/nuget/vpre/Dommel.svg?style=flat-square)](https://www.nuget.org/packages/Dommel) |

<hr>

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. The SQL queries are generated based on your POCO entities. Dommel also supports LINQ expressions which are being translated to SQL expressions. [Dapper](https://github.com/StackExchange/Dapper) is used for query execution and object mapping.

There are several extensibility points available to change the behavior of resolving table names, column names, the key property and POCO properties. See [Extensibility](https://www.learndapper.com/extensions/dommel#extensibility) for more details.

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

## Documentation

The documentation is available at **[Learn Dapper](https://www.learndapper.com/extensions/dommel)**.

## Sponsors
[Dapper Plus](https://dapper-plus.net/) and [Entity Framework Extensions](https://entityframework-extensions.net/) are major sponsors and are proud to contribute to the development of Dommel.

[![Dapper Plus](https://raw.githubusercontent.com/henkmollema/Dommel/refs/heads/master/assets/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert)

[![Entity Framework Extensions](https://raw.githubusercontent.com/henkmollema/Dommel/refs/heads/master/assets/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert)
