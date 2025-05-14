# Dommel
CRUD operations with Dapper made simple.

| Build | NuGet | MyGet | Test Coverage |
| ----- | ----- | ----- | ------------- |
| [![Travis](https://img.shields.io/travis/com/henkmollema/Dommel?style=flat-square)](https://app.travis-ci.com/github/henkmollema/Dommel) | [![NuGet](https://img.shields.io/nuget/vpre/Dommel.svg?style=flat-square)](https://www.nuget.org/packages/Dommel) | [![MyGet Pre Release](https://img.shields.io/myget/dommel-ci/vpre/Dommel.svg?style=flat-square)](https://www.myget.org/feed/dommel-ci/package/nuget/Dommel) | [![codecov](https://codecov.io/gh/henkmollema/Dommel/branch/master/graph/badge.svg)](https://codecov.io/gh/henkmollema/Dommel) |

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

![dapper-plus-sponsor](https://github.com/user-attachments/assets/e14fc116-436e-4746-a97b-27890a0b773b)

![entity-framework-extensions-sponsor](https://github.com/user-attachments/assets/f98d302c-9a1f-4073-9ac8-671e8628cc86)
