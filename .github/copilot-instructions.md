# Copilot Instructions for Dommel

## Build, Test, and Lint

```powershell
# Build
dotnet build Dommel.sln -c Release

# Run all unit tests
dotnet test test/Dommel.Tests

# Run all integration tests (requires local SQL Server, MySQL, PostgreSQL)
dotnet test test/Dommel.IntegrationTests

# Run a single test by name
dotnet test test/Dommel.Tests --filter "FullyQualifiedName~CountTests.BuildCountAllSql"

# Full CI build (restore, build, test all projects, coverage, pack)
./build.ps1
```

## Architecture

Dommel is a Dapper extension library that generates CRUD SQL from POCO entities via `IDbConnection` extension methods.

**`DommelMapper`** is a single `static partial class` split across many files — one per CRUD concern (`Get.cs`, `Insert.cs`, `Update.cs`, `Delete.cs`, `Select.cs`, `Count.cs`, `Any.cs`, `From.cs`, `Project.cs`, plus multi-map files). Each file adds extension methods to `IDbConnection` following sync + async pairs with `CancellationToken`.

**Core flow for every CRUD operation:**
1. Extension method receives `IDbConnection` (+ optional `IDbTransaction`, `CancellationToken`)
2. Calls an `internal static Build*Query()` method that checks `QueryCache` (a `ConcurrentDictionary<QueryCacheKey, string>`)
3. On cache miss: uses `Resolvers` to resolve table names, column names, and key properties, then builds the SQL string
4. Delegates to Dapper for execution and mapping

**Key collaborators:**
- **`ISqlBuilder`** — Abstraction for DB-specific SQL (identifier quoting, insert-ID retrieval, paging, LIKE). Implementations: `SqlServerSqlBuilder`, `MySqlSqlBuilder`, `PostgresSqlBuilder`, `SqliteSqlBuilder`, `SqlServerCeSqlBuilder`. Looked up by `connection.GetType().Name`.
- **`Resolvers`** — Static caching layer over the resolver interfaces (`ITableNameResolver`, `IColumnNameResolver`, `IKeyPropertyResolver`, `IPropertyResolver`, `IForeignKeyPropertyResolver`). All resolution results are cached in `ConcurrentDictionary` instances.
- **`SqlExpression<T>`** — Translates LINQ `Expression<Func<T, bool>>` to SQL WHERE clauses with auto-numbered parameters (`@p1`, `@p2`). Designed for subclassing (virtual methods).

**Dommel.Json** is a companion package that adds JSON column support. It replaces all SQL builders with JSON-aware variants (implementing `IJsonSqlBuilder`), swaps the `SqlExpressionFactory` to produce `JsonSqlExpression<T>` (which overrides `VisitMemberAccess` to emit DB-specific JSON path queries), and registers Dapper type handlers for `[JsonData]`-annotated properties.

## Key Conventions

### Extension method pattern
Every public API method is an `IDbConnection` extension method in the `DommelMapper` partial class. Each has a sync and async variant. The async variant accepts an optional `CancellationToken`. An `IDbTransaction? transaction = null` parameter is standard.

### Resolver + caching pattern
Table/column/key/property resolution goes through `Resolvers` (static caching wrapper) → configured resolver interface instance (stored on `DommelMapper`). All resolvers are replaceable via `DommelMapper.SetTableNameResolver()`, etc. New resolvers must be idempotent since results are cached.

### SQL builder registration
SQL builders are registered by connection type name (lowercase) in a `Dictionary<string, ISqlBuilder>`. To add a new database, implement `ISqlBuilder` and register via `DommelMapper.AddSqlBuilder()`.

### Attributes for entity mapping
- `[Key]` or a property named `Id` — marks the key property (defaults to `DatabaseGeneratedOption.Identity`)
- `[DatabaseGenerated]` — controls identity/computed/none behavior
- `[Table]` / `[Column]` — custom name mapping (from `System.ComponentModel.DataAnnotations.Schema`)
- `[Ignore]` (Dommel's own) or `[NotMapped]` — exclude property from mapping
- `[ForeignKey]` — navigation property resolution for multi-map queries

### Target frameworks
The libraries multi-target `netstandard2.0`, `net8.0`, `net9.0`, and `net10.0`. Test projects target `net10.0` only.

### Nullable reference types
Enabled project-wide via `Directory.Build.props`. All public APIs use nullable annotations consistently.

### Testing structure
- **Unit tests** (`Dommel.Tests`): Call `internal static Build*` methods directly, passing a concrete `ISqlBuilder`. Assert generated SQL strings. Test model classes are defined inline or in `Models.cs`.
- **Integration tests** (`Dommel.IntegrationTests`): Use `[Theory]` + `[ClassData(typeof(DatabaseTestData))]` to run each test against all configured database drivers (SQL Server, MySQL, PostgreSQL). Tests share a `DatabaseFixture` via xUnit `[Collection("Database")]` that handles DB setup/seeding. Test framework is xUnit.
