# AGENTS.md

This file provides guidance to coding agents (Claude Code, GitHub Copilot, etc.) when working with code in this repository.

## Build, Test, and Lint

```powershell
# Build
dotnet build Dommel.sln -c Release

# Run all unit tests (no database required)
dotnet test test/Dommel.Tests

# Run all integration tests (requires local SQL Server, MySQL, PostgreSQL)
dotnet test test/Dommel.IntegrationTests

# Run a single test by name
dotnet test test/Dommel.Tests --filter "FullyQualifiedName~CountTests.BuildCountAllSql"

# Full CI build (restore, build, test all four projects, coverage, pack)
./build.ps1
```

The four test projects are `Dommel.Tests`, `Dommel.IntegrationTests`, `Dommel.Json.Tests`, and `Dommel.Json.IntegrationTests`.

## Architecture

Dommel is a Dapper extension library that generates CRUD SQL from POCO entities via `IDbConnection` extension methods. [Dapper](https://github.com/StackExchange/Dapper) handles query execution and object mapping.

**`DommelMapper`** is a single `static partial class` split across many files in `src/Dommel/` — one per CRUD concern (`Get.cs`, `Insert.cs`, `Update.cs`, `Delete.cs`, `Select.cs`, `Count.cs`, `Any.cs`, `From.cs`, `Project.cs`, `Scalar.cs`, plus multi-map files). Each file adds extension methods to `IDbConnection`.

**Core flow for every CRUD operation:**
1. Extension method receives `IDbConnection` (+ optional `IDbTransaction`, `CancellationToken`)
2. Calls an `internal static Build*Query()` method that checks `QueryCache` (a `ConcurrentDictionary<QueryCacheKey, string>`, see `Cache.cs`)
3. On cache miss: uses `Resolvers` to resolve table names, column names, and key properties, then builds the SQL string
4. Delegates to Dapper for execution and mapping

**Key collaborators:**
- **`ISqlBuilder`** — Abstraction for DB-specific SQL (identifier quoting, insert-ID retrieval, paging, LIKE). Implementations: `SqlServerSqlBuilder`, `MySqlSqlBuilder`, `PostgresSqlBuilder`, `SqliteSqlBuilder`, `SqlServerCeSqlBuilder`. Looked up by `connection.GetType().Name` (lowercase) in a dictionary; register new ones via `DommelMapper.AddSqlBuilder()`.
- **`Resolvers`** — Static caching layer (`ConcurrentDictionary`) over the resolver interfaces (`ITableNameResolver`, `IColumnNameResolver`, `IKeyPropertyResolver`, `IPropertyResolver`, `IForeignKeyPropertyResolver`). All replaceable via `DommelMapper.SetTableNameResolver()` etc. New resolvers must be idempotent since results are cached.
- **`SqlExpression<T>`** — Translates LINQ `Expression<Func<T, bool>>` to SQL WHERE clauses with auto-numbered parameters (`@p1`, `@p2`). Designed for subclassing (virtual methods).

**Dommel.Json** (`src/Dommel.Json/`) is a companion package adding JSON column support. It replaces all SQL builders with JSON-aware variants (implementing `IJsonSqlBuilder`), swaps the `SqlExpressionFactory` to produce `JsonSqlExpression<T>` (which overrides `VisitMemberAccess` to emit DB-specific JSON path queries), and registers Dapper type handlers for `[JsonData]`-annotated properties.

## Key Conventions

- **Extension method pattern**: Every public API method is an `IDbConnection` extension method on the `DommelMapper` partial class, with a sync and an async variant. The async variant accepts an optional `CancellationToken`; `IDbTransaction? transaction = null` is a standard parameter.

- **Entity mapping attributes**:
  - `[Key]` or a property named `Id` — marks the key property (defaults to `DatabaseGeneratedOption.Identity`)
  - `[DatabaseGenerated]` — controls identity/computed/none behavior
  - `[Table]` / `[Column]` — custom name mapping (from `System.ComponentModel.DataAnnotations.Schema`)
  - `[Ignore]` (Dommel's own, `IgnoreAttribute.cs`) or `[NotMapped]` — exclude property from mapping
  - `[ForeignKey]` — navigation property resolution for multi-map queries

- **Target frameworks**: Libraries multi-target `netstandard2.0`, `net8.0`, `net9.0`, `net10.0`. Test projects target `net10.0` only. Nullable reference types are enabled project-wide via `Directory.Build.props`; the package version is set there (`VersionPrefix`).

## Testing structure

- **Unit tests** (`Dommel.Tests`, `Dommel.Json.Tests`): Call `internal static Build*` methods directly, passing a concrete `ISqlBuilder`, and assert on the generated SQL string. Test model classes are inline or in `Models.cs`. Framework is xUnit.
- **Integration tests** (`Dommel.IntegrationTests`, `Dommel.Json.IntegrationTests`): Use `[Theory]` + `[ClassData(typeof(DatabaseTestData))]` to run each test against all configured database drivers (SQL Server, MySQL, PostgreSQL). Tests share a `DatabaseFixture` via xUnit `[Collection("Database")]` that handles DB setup/seeding.
