# Plan: Always Use NpgsqlDataSource Without `using` Statements

## Goal
- Always use `NpgsqlDataSource` (reused, long-lived, managed at class level)
- Never wrap `NpgsqlDataSource` in `using` statements
- For `:copyfrom` queries, get a connection from the data source: `GetDataSource().OpenConnection()` and wrap that in `using` (connections are short-lived)

## Current State Analysis

### 1. `NpgsqlDriver.EstablishConnection()` (Line 542-558)
**Current behavior:**
- For `:copyfrom` or Dapper: Returns `var connection = new NpgsqlConnection(...)` ✅ (correct - needs `using`)
- For regular queries: Returns `var connection = GetDataSource()` ❌ (wrong - shouldn't use `using`)

**Issue:** The logic uses `NpgsqlConnection` for Dapper, but Dapper can work with `NpgsqlDataSource` too.

### 2. Generator Methods That Wrap in `using`
All these methods currently wrap `establishConnection` in `using`:
- `ManyDeclareGen.GetDriverNoTxBody()` - Line 92-93
- `ManyDeclareGen.GetDapperNoTxBody()` - Line 56
- `OneDeclareGen.GetDriverNoTxBody()` - Line 88
- `OneDeclareGen.GetDapperNoTxBody()` - Line 56
- `ExecDeclareGen.GetDriverNoTxBody()` - Line 75
- `ExecDeclareGen.GetDapperNoTxBody()` - Line 51
- `ExecRowsDeclareGen.GetDriverNoTxBody()` - Line 74
- `ExecRowsDeclareGen.GetDapperNoTxBody()` - Line 51
- `ExecLastIdDeclareGen.GetDriverNoTxBody()` - Line 73
- `ExecLastIdDeclareGen.GetDapperNoTxBody()` - Line 51

### 3. `NpgsqlDriver.GetCopyFromImpl()` (Line 637-657)
**Current behavior:** Uses `using` for connection - ✅ **CORRECT** (needs to stay this way)

## Solution Strategy

### Option A: Add Flag to `ConnectionGenCommands` (Recommended)
**Pros:**
- Explicit and type-safe
- Clear intent
- Easy to maintain

**Cons:**
- Requires changing the record structure
- All drivers need to specify the flag

### Option B: Detect Pattern in Generators
**Pros:**
- No changes to `ConnectionGenCommands`
- Works automatically

**Cons:**
- String-based detection (fragile)
- Less explicit

### Option C: Separate Methods for DataSource vs Connection
**Pros:**
- Very explicit
- Type-safe

**Cons:**
- More complex API
- Requires more changes

## Recommended Approach: Simple Flag-Based

### Step 1: Update `ConnectionGenCommands` Record
**File:** `Drivers/DbDriver.cs`

Add a required boolean flag:
```csharp
public record ConnectionGenCommands(
    string EstablishConnection, 
    string ConnectionOpen, 
    bool WrapInUsing
);
```

### Step 2: Update `NpgsqlDriver.EstablishConnection()`
**File:** `Drivers/NpgsqlDriver.cs`

```csharp
public override ConnectionGenCommands EstablishConnection(Query query)
{
    var connectionVar = Variable.Connection.AsVarName();
    
    // For :copyfrom, get a connection from the data source (short-lived, needs using)
    if (query.Cmd == ":copyfrom")
    {
        return new ConnectionGenCommands(
            $"var {connectionVar} = GetDataSource().OpenConnection()",
            string.Empty,
            WrapInUsing: true  // Connection is short-lived, should be disposed
        );
    }
    
    // All other queries use NpgsqlDataSource directly (reusable, long-lived)
    return new ConnectionGenCommands(
        $"var {connectionVar} = GetDataSource()",
        string.Empty,
        WrapInUsing: false  // Data source is long-lived, managed at class level
    );
}
```

### Step 3: Update All Generator Methods
**Files:** All `*DeclareGen.cs` files

For each `GetDriverNoTxBody()` and `GetDapperNoTxBody()` method, replace:
```csharp
var (establishConnection, connectionOpen) = dbDriver.EstablishConnection(query);
return $$"""
    using ({{establishConnection}})
    {
        {{connectionOpen.AppendSemicolonUnlessEmpty()}}
        // ... rest of code
    }
""";
```

With:
```csharp
var connectionCommands = dbDriver.EstablishConnection(query);
var establishConnection = connectionCommands.EstablishConnection;
var connectionOpen = connectionCommands.ConnectionOpen;
var wrapInUsing = connectionCommands.WrapInUsing;

return wrapInUsing
    ? $$"""
        using ({{establishConnection}})
        {
            {{connectionOpen.AppendSemicolonUnlessEmpty()}}
            // ... rest of code
        }
        """
    : $$"""
        {{establishConnection}};
        {{connectionOpen.AppendSemicolonUnlessEmpty()}}
        // ... rest of code
        """;
```

### Step 4: Update `GetCopyFromImpl()`
**File:** `Drivers/NpgsqlDriver.cs`

Update to use the new pattern (connection from data source, already open):
```csharp
var connectionCommands = EstablishConnection(query);
var establishConnection = connectionCommands.EstablishConnection;
var connectionOpen = connectionCommands.ConnectionOpen;
var wrapInUsing = connectionCommands.WrapInUsing; // Always true for :copyfrom

return $$"""
    using ({{establishConnection}})
    {
        {{connectionOpen.AppendSemicolonUnlessEmpty()}}
        // Note: OpenConnection() already opens the connection, so OpenAsync() may not be needed
        // ... rest of code
    }
    """;
```

**Note:** `OpenConnection()` returns an already-open connection, so we may be able to remove the `OpenAsync()` call. Need to verify.

## Files to Modify

1. ✅ `Drivers/DbDriver.cs` - Add `WrapInUsing` parameter to `ConnectionGenCommands`
2. ✅ `Drivers/NpgsqlDriver.cs` - Update `EstablishConnection()` and `GetCopyFromImpl()`
3. ✅ `Drivers/Generators/ManyDeclareGen.cs` - Update `GetDriverNoTxBody()` and `GetDapperNoTxBody()`
4. ✅ `Drivers/Generators/OneDeclareGen.cs` - Update `GetDriverNoTxBody()` and `GetDapperNoTxBody()`
5. ✅ `Drivers/Generators/ExecDeclareGen.cs` - Update `GetDriverNoTxBody()` and `GetDapperNoTxBody()`
6. ✅ `Drivers/Generators/ExecRowsDeclareGen.cs` - Update `GetDriverNoTxBody()` and `GetDapperNoTxBody()`
7. ✅ `Drivers/Generators/ExecLastIdDeclareGen.cs` - Update `GetDriverNoTxBody()` and `GetDapperNoTxBody()`

**Note:** Other drivers (SqliteDriver, MySqlConnectorDriver) will need to be updated later to include `WrapInUsing: true` when this pattern is extended to them.

## Testing Strategy

1. **Unit Tests:**
   - Verify `EstablishConnection()` returns correct `WrapInUsing` flag
   - Verify `:copyfrom` queries return `WrapInUsing: true`
   - Verify regular queries return `WrapInUsing: false`

2. **Generated Code Tests:**
   - Regenerate code and verify:
     - Regular queries don't wrap `GetDataSource()` in `using`
     - `:copyfrom` queries still wrap `NpgsqlConnection` in `using`
     - Dapper queries work with `NpgsqlDataSource`

3. **Integration Tests:**
   - Run existing end-to-end tests
   - Run benchmark to verify performance improvement
   - Verify no connection pool leaks

## Expected Generated Code Patterns

### Regular Query (e.g., `:many`, `:one`, `:exec`)
```csharp
public async Task<List<GetCustomerOrdersRow>> GetCustomerOrdersAsync(...)
{
    if (this.Transaction == null)
    {
        var connection = GetDataSource();  // ✅ No using!
        using (var command = connection.CreateCommand(GetCustomerOrdersSql))
        {
            // ... query execution
        }
    }
    // ... transaction path
}
```

### CopyFrom Query
```csharp
public async Task AddProductsAsync(...)
{
    if (this.Transaction == null)
    {
        using (var connection = GetDataSource().OpenConnection())  // ✅ Connection from data source, with using
        {
            // Connection is already open from OpenConnection()
            using (var writer = await connection.BeginBinaryImportAsync(...))
            {
                // ... copy operations
            }
        }
    }
    // ... transaction path
}
```

## Implementation Order

1. ✅ Update `ConnectionGenCommands` record
2. ✅ Update `NpgsqlDriver.EstablishConnection()` with conditional logic
3. ✅ Update all generator methods (start with one, test, then do others)
4. ✅ Update `GetCopyFromImpl()` for consistency
5. ✅ Test with code regeneration
6. ✅ Run benchmarks to verify performance

## Risk Assessment

**Low Risk:**
- `:copyfrom` path is unchanged (still uses `NpgsqlConnection` with `using`)
- Simple flag-based approach is easy to understand

**Medium Risk:**
- Dapper path changes from `NpgsqlConnection` to `NpgsqlDataSource` - need to verify Dapper works with data source
- Multiple generator files need updates - risk of inconsistency

**Mitigation:**
- Test Dapper compatibility first
- Update generators one at a time and test

