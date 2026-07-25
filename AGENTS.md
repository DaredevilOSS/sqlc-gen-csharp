# AGENTS.md - SQLC Gen C# Codebase Guide

## 🎯 Project Overview

**sqlc-gen-csharp** is a .NET plugin for [sqlc](https://github.com/sqlc-dev/sqlc) that generates type-safe C# data access code from SQL queries.

**Key Purpose:** Generate C# classes (queries, models, utils) from SQL queries using database drivers (Npgsql, MySqlConnector, Microsoft.Data.Sqlite) or optionally Dapper.

**Supported Databases:**
- PostgreSQL (Npgsql driver)
- MySQL (MySqlConnector driver)
- SQLite (Microsoft.Data.Sqlite driver)

---

## 📁 Core Architecture

### Main Entry Point
| File | Purpose |
|------|---------|
| `SqlcGenCsharp/PluginRunner.cs` | Main plugin entry point - handles requests |
| `CodeGenerator/CodeGenerator.cs` | Orchestrates code generation flow |

### Generator Components
| File | Purpose |
|------|---------|
| `CodeGenerator/Generators/QueriesGen.cs` | Generates query classes with CRUD methods |
| `CodeGenerator/Generators/ModelsGen.cs` | Generates POCOs for database tables |
| `CodeGenerator/Generators/UtilsGen.cs` | Generates utility classes |
| `CodeGenerator/Generators/CsprojGen.cs` | Generates .csproj files |

### Database Drivers
| File | Purpose |
|------|---------|
| `Drivers/DbDriver.cs` | Abstract base driver class |
| `Drivers/NpgsqlDriver.cs` | PostgreSQL driver implementation |
| `Drivers/MySqlConnectorDriver.cs` | MySQL driver implementation |
| `Drivers/SqliteDriver.cs` | SQLite driver implementation |
| `Drivers/EnumDbDriver.cs` | Generates enum types |

---

## 🏗️ Code Generation Flow

```
1. Generate() method in CodeGenerator.cs
   ↓
2. InitGenerators() - Parse options, instantiate drivers
   ↓
3. InstantiateDriver() - Select driver based on DriverName
   ↓
4. GetFileQueries() - Group queries by table
   ↓
5. GenerateFiles() - Call appropriate generators
   ↓
6. Return GenerateResponse with generated files
```

### Driver Instantiation
```csharp
DbDriver InstantiateDriver()
{
    return Options.DriverName switch
    {
        DriverName.MySqlConnector => new MySqlConnectorDriver(...),
        DriverName.Npgsql => new NpgsqlDriver(...),
        DriverName.Sqlite => new SqliteDriver(...),
        _ => throw new ArgumentException("unknown driver")
    }
}
```

---

## 🗄️ Database-Specific Implementation Differences

### PostgreSQL
- **`:execlastid`**: Uses `RETURNING` clause
- **`:copyfrom`**: Uses `COPY FROM` binary command
- **Supported types**: Boolean, integer types, decimal, date/time, JSON/JSONB, arrays, enums, etc.

### MySQL
- **`:execlastid`** (Driver): Uses `MySqlConnector.LastInsertedId` property
- **`:execlastid`** (Dapper): Appends `SELECT LAST_INSERT_ID()` to query
- **`:copyfrom`**: Uses `LOAD DATA` from CSV file
- **Supported types**: Boolean, integer types, decimal, date/time, JSON, geometry

### SQLite
- **`:execlastid`**: Uses `RETURNING` clause (integer only)
- **`:copyfrom`**: Uses multi-VALUES clause
- **Supported types**: Limited - integer, real, text, blob
- **Override support**: Can override to DateTime, bool, etc.

---

## ⚙️ Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `overrideDriverVersion` | 2.3.6 (MySql), 8.0.3 (Npgsql), 8.0.10 (Sqlite) | Override DB driver version |
| `targetFramework` | net8.0 | .NET Standard 2.0/2.1 or .NET 8.0 |
| `generateCsproj` | true | Generate .csproj file |
| `namespaceName` | Project name | Override generated namespace |
| `useDapper` | false | Use Dapper wrapper |
| `overrideDapperVersion` | 2.1.66 | Override Dapper version |
| `useCentralPackageManagement` | false | Support central package management |
| `withAsyncSuffix` | true | Append "Async" to method names |
| `overrides` | - | Per-column type overrides |

---

## 🔧 Query Annotations Support

| Annotation | PostgreSQL | MySQL | SQLite | Description |
|------------|-----------|-------|--------|-------------|
| `:one` | ✅ | ✅ | ✅ | Returns 0...1 records |
| `:many` | ✅ | ✅ | ✅ | Returns 0...n records |
| `:exec` | ✅ | ✅ | ✅ | DML/DDDL - no return |
| `:execrows` | ✅ | ✅ | ✅ | Returns affected rows count |
| `:execlastid` | ✅ | ✅ | ✅ | INSERT with returned ID |
| `:copyfrom` | ✅ | ✅ | ✅ | Batch insert |

### Macro Annotations
| Macro | PostgreSQL | MySQL | SQLite | Description |
|-------|-----------|-------|--------|-------------|
| `sqlc.arg` | ✅ | ✅ | ✅ | Named parameters |
| `sqlc.narg` | ✅ | ✅ | ✅ | Nullable named parameters |
| `sqlc.slice` | 🚫 | ✅ | ✅ | Dynamic query for arrays |
| `sqlc.embed` | ✅ | ✅ | ✅ | Embed existing models |

---

## 🎨 Code Generation Patterns

### Query Class Structure
```csharp
public partial class AuthorsQueries
{
    private readonly string _connectionString;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    
    // Constructor with connection string
    public AuthorsQueries(string connectionString) 
    {
        _connectionString = connectionString;
    }
    
    // Query methods
    public Task<Author?> GetAuthorAsync(int id) => ...
    public Task<IEnumerable<Author>> GetAuthorsAsync() => ...
    public Task<int> CreateAuthorAsync(CreateAuthorParams params) => ...
}
```

### Model Structure
```csharp
public partial class Author
{
    public int AuthorID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
}
```

### Type Mapping
- **Column types** are mapped to C# types via `ColumnMappings`
- **Overrides** can be applied per column: `overrideOption.Column == "GetAuthor:Name"`
- **Nullable types** in .NET Core are inferred from `notNull` column attribute

---

## 🧪 Testing & Development

### Local Development
```bash
# Prerequisites
dotnet --version # .NET 8.0
buf --version
# WASM libs for .NET WASI (see WASM docs)

# Generate protobuf
make protobuf-generate

# Generate code
make sqlc-generate-process
make sqlc-generate-wasm

# Test plugins
make test-process-plugin
make test-wasm-plugin
```

### Pre-commit Setup
```bash
pip install pre-commit
pre-commit install
```

---

## 📝 Common Patterns

### Type Handler Registration (Dapper)
```csharp
public static void ConfigureSqlMapper()
{
    SqlMapper.TypeHandler<Instant>.Register(new NodaInstantTypeHandler());
    // Other type handlers...
}
```

### Parameter Binding
```csharp
command.Parameters.AddWithValue("@column_name", 
    param.Value ?? (object)DBNull.Value);
```

### Slice Arguments (MySQL/SQLite)
```csharp
for (int i = 0; i < paramArray.Length; i++)
    command.Parameters.AddWithValue($"@paramArg{i}", paramArray[i]);
```

---

## 🐛 Known Issues & TODOs

1. **2D arrays** in batch inserts work but may have edge cases
2. **Time with time zone** not supported (PostgreSQL)
3. **Array columns** override handling marked as TODO
4. **Geometry types** (MySQL) marked with ⚠️ - partial support
5. **macaddr8 conversion** requires explicit cast in INSERT

---

## 📚 Key Files to Know

- **`CodeGenerator/CodeGenerator.cs`** - Main orchestration
- **`Drivers/DbDriver.cs`** - Base driver with shared logic
- **`Drivers/NpgsqlDriver.cs`** - Most complex driver (33KB)
- **`Drivers/MySqlConnectorDriver.cs`** - MySQL-specific logic
- **`Drivers/SqliteDriver.cs`** - Simplest driver
- **`Extensions/*`** - Helper methods for code generation

---

## 🚀 Quick Reference

### Generate Code
```bash
make sqlc-generate-process  # Process-based plugin
make sqlc-generate-wasm     # WASM plugin
```

### Test
```bash
make test-process-plugin
make test-wasm-plugin
```

### Release
The release flow uses three automated workflows:

1. **Release Assistant** (`release-assistant.yml`) - runs on every PR to `main`. Validates that version-worthy PRs have a `major`/`minor`/`patch` label. Sets `release-assistant/requirements` status check.

2. **Generate Release PR** (`gen-release-pr.yml`) - triggered when Build completes on `main`. Aggregates all labeled, merged PRs since the last release, computes the new version, regenerates docs with the wasm sha, and opens a `release-prep` PR.

3. **Release** (`release.yml`) - triggered when the `release-prep` PR is merged. Creates the release tag, reconstructs release notes from merged PR titles, and uploads the wasm asset.

---

## 📖 External References

- [SQLC Documentation](https://docs.sqlc.dev/)
- [Npgsql](https://www.npgsql.org/)
- [MySqlConnector](https://mysqlconnector.net/)
- [Dapper](https://github.com/DapperLib/Dapper)
- [NodaTime](https://nodatime.org/)

---
