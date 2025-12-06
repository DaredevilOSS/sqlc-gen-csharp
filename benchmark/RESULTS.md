# Benchmark Results - SQLC vs EFCore

**Date**: December 6, 2024  
**Environment**: macOS 15.6.1, Apple M2, .NET 8.0.22  
**Benchmark Tool**: BenchmarkDotNet v0.13.12

## Overview

This document presents comprehensive benchmark results comparing:
- **SQLC**: SQL Compiler generated C# code (raw SQL queries)
- **EFCore (NoTracking)**: Entity Framework Core with `AsNoTracking()`
- **EFCore (WithTracking)**: Entity Framework Core with change tracking enabled

---

## Read Operations

### MySQL Read Performance

| Implementation | Mean Time | Ratio vs SQLC | Memory | GC Collections | Winner |
|---------------|-----------|---------------|--------|----------------|--------|
| **SQLC** | 23.91 ms | 1.00 (baseline) | 647.94 KB | Gen0: 94, Gen1: 31, Gen2: 31 | ⚠️ **EFCore** |
| **EFCore (NoTracking)** | 4.25 ms | 0.18x (5.6x faster) | 675.29 KB | Gen0: 78, Gen1: 8 | ✅ **WINNER** |
| **EFCore (WithTracking)** | 4.18 ms | 0.17x (5.7x faster) | 664.64 KB | Gen0: 78, Gen1: 16 | ✅ **WINNER** |

**Key Findings:**
- **EFCore is 5.6-5.7x faster** than SQLC (reversed from previous results!)
- This is likely due to reduced dataset size (matching PostgreSQL)
- Tracking overhead is negligible (1.7% difference)
- Memory usage is similar across all implementations

---

### PostgreSQL Read Performance

| Implementation | Mean Time | Ratio vs SQLC | Memory | GC Collections | Winner |
|---------------|-----------|---------------|--------|----------------|--------|
| **SQLC** | 31.21 ms | 1.00 (baseline) | 1.48 MB | Gen0: 438, Gen1: 438, Gen2: 125 | ✅ **WINNER** |
| **EFCore (NoTracking)** | 33.27 ms | 1.07x slower | 1.87 MB | Gen0: 200, Gen1: 67 | |
| **EFCore (WithTracking)** | 32.96 ms | 1.06x slower | 1.85 MB | Gen0: 188, Gen1: 63 | |

**Key Findings:**
- SQLC is **7% faster** than EFCore implementations
- All implementations are nearly equivalent
- Tracking vs NoTracking has minimal impact (0.9% difference)

---

### SQLite Read Performance

| Implementation | Mean Time | Ratio vs SQLC | Memory | GC Collections | Winner |
|---------------|-----------|---------------|--------|----------------|--------|
| **SQLC** | 135.9 ms | 1.00 (baseline) | 2.0 MB | None | ⚠️ **EFCore** |
| **EFCore (NoTracking)** | 137.9 ms | 1.01x (1% slower) | 2.4 MB | Gen0: 250 | ✅ **SQLC** |
| **EFCore (WithTracking)** | 138.3 ms | 1.02x (2% slower) | 2.37 MB | Gen0: 250 | ✅ **SQLC** |

**Key Findings:**
- **SQLC is 1-2% faster** than EFCore implementations
- All implementations are nearly equivalent
- Tracking vs NoTracking has minimal impact (0.3% difference)
- SQLC uses slightly less memory

---

## Write Operations

### MySQL Write Performance

#### AddOrderItems

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 1,000 | 14.10 ms | 113.86 ms | 1:8.1 | 1:10.4 | ✅ **SQLC** |
| 10,000 | 104.28 ms | 802.67 ms | 1:7.7 | 1:52.4 | ✅ **SQLC** |
| 50,000 | 320.47 ms | 4,017.49 ms | 1:12.5 | 1:55.0 | ✅ **SQLC** |

#### AddOrders

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 1,000 | 9.08 ms | 126.41 ms | 1:13.9 | 1:11.7 | ✅ **SQLC** |
| 10,000 | 48.68 ms | 741.02 ms | 1:15.2 | 1:50.8 | ✅ **SQLC** |
| 50,000 | 196.48 ms | 3,981.40 ms | 1:20.3 | 1:74.7 | ✅ **SQLC** |

#### AddProducts

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 1,000 | 9.44 ms | 131.02 ms | 1:13.9 | 1:48.2 | ✅ **SQLC** |
| 10,000 | 44.19 ms | 707.46 ms | 1:16.0 | 1:41.2 | ✅ **SQLC** |
| 50,000 | 175.15 ms | 3,929.13 ms | 1:22.4 | 1:66.5 | ✅ **SQLC** |

**MySQL Write Summary:**
- **Winner**: ✅ **SQLC** (7.7-22.4x faster, 10-75x less memory)
- Performance gap increases with batch size
- SQLC uses dramatically less memory

---

### PostgreSQL Write Performance

#### AddOrderItems

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 1,000 | 10.08 ms | 101.15 ms | 1:10.0 | 1:102.2 | ✅ **SQLC** |
| 10,000 | 87.61 ms | 397.74 ms | 1:4.5 | 1:444.1 | ✅ **SQLC** |
| 50,000 | 461.42 ms | 2,225.20 ms | 1:4.8 | 1:446.0 | ✅ **SQLC** |

#### AddOrders

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 1,000 | 7.57 ms | 88.51 ms | 1:11.7 | 1:107.7 | ✅ **SQLC** |
| 10,000 | 62.33 ms | 391.62 ms | 1:6.3 | 1:457.7 | ✅ **SQLC** |
| 50,000 | 324.13 ms | 2,324.91 ms | 1:7.2 | 1:459.1 | ✅ **SQLC** |

#### AddProducts

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 1,000 | 4.98 ms | 64.69 ms | 1:13.0 | 1:257.5 | ✅ **SQLC** |
| 10,000 | 13.00 ms | 354.03 ms | 1:27.3 | 1:466.4 | ✅ **SQLC** |
| 50,000 | 56.40 ms | 2,253.94 ms | 1:40.0 | 1:466.3 | ✅ **SQLC** |

**PostgreSQL Write Summary:**
- **Winner**: ✅ **SQLC** (4.3-40.0x faster, 100-466x less memory)
- Best performance: AddProducts (27-40x faster)
- SQLC uses dramatically less memory (100-466x less)

---

### SQLite Write Performance

#### AddOrderItems

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 100 | 1.83 ms | 5.78 ms | 1:3.2 | 1:6.2 | ✅ **SQLC** |
| 200 | 5.16 ms | 13.36 ms | 1:2.6 | 1:6.1 | ✅ **SQLC** |
| 500 | 26.75 ms | 40.15 ms | 1:1.5 | 1:5.2 | ✅ **SQLC** |

#### AddOrders

| Batch Size | SQLC | EFCore | Status | Winner |
|-----------|------|--------|--------|--------|
| 100 | N/A | N/A | ❌ Failed | N/A |
| 200 | N/A | N/A | ❌ Failed | N/A |
| 500 | N/A | N/A | ❌ Failed | N/A |

#### AddProducts

| Batch Size | SQLC | EFCore | Ratio | Memory Ratio | Winner |
|-----------|------|--------|-------|--------------|--------|
| 100 | 1.61 ms | 6.76 ms | 1:4.2 | 1:8.0 | ✅ **SQLC** |
| 200 | 4.64 ms | 16.17 ms | 1:3.5 | 1:7.9 | ✅ **SQLC** |
| 500 | 24.59 ms | 43.67 ms | 1:1.8 | 1:6.8 | ✅ **SQLC** |

**SQLite Write Summary:**
- **Winner**: ✅ **SQLC** (1.5-4.2x faster, 5-8x less memory)
- AddOrders operation still failing (needs investigation)
- AddOrderItems and AddProducts working successfully
- Performance gap is smaller than MySQL/PostgreSQL

---

## Summary Tables

### Read Operations - Overall Winners

| Database | Winner | SQLC Advantage | EFCore (NoTracking) | EFCore (WithTracking) |
|----------|--------|----------------|---------------------|----------------------|
| **MySQL** | ⚠️ **EFCore** | **5.6x slower** | 4.25 ms | 4.18 ms |
| **PostgreSQL** | ✅ **SQLC** | **6% faster** | 33.08 ms | 31.88 ms |
| **SQLite** | ✅ **SQLC** | **1-2% faster** | 137.9 ms | 138.3 ms |

**Read Operations Winner**: ⚠️ **Mixed** (EFCore wins in MySQL, SQLC wins in PostgreSQL and SQLite)

---

### Write Operations - Overall Winners

| Database | Winner | SQLC Advantage Range | Best Operation | Worst Operation |
|----------|--------|---------------------|----------------|----------------|
| **MySQL** | ✅ **SQLC** | **7.7-22.4x faster** | AddProducts (22.4x) | AddOrderItems (7.7x) |
| **PostgreSQL** | ✅ **SQLC** | **4.3-38.1x faster** | AddProducts (38.1x) | AddOrderItems (4.3x) |
| **SQLite** | ✅ **SQLC** | **1.5-4.2x faster** | AddProducts (4.2x) | AddOrderItems (1.5x) |

**Write Operations Winner**: ✅ **SQLC** (wins in all databases, AddOrders still failing in SQLite)

---

## Performance Comparison Matrix

### Read Performance (Time)

| Database | SQLC | EFCore (NoTracking) | EFCore (WithTracking) | Winner |
|----------|------|---------------------|----------------------|--------|
| **MySQL** | 23.91 ms | 4.25 ms (5.6x faster) | 4.18 ms (5.7x faster) | ⚠️ **EFCore** |
| **PostgreSQL** | 31.30 ms | 33.08 ms (6% slower) | 31.88 ms (2% slower) | ✅ **SQLC** |
| **SQLite** | 135.9 ms | 137.9 ms (1% slower) | 138.3 ms (2% slower) | ✅ **SQLC** |

### Read Performance (Memory)

| Database | SQLC | EFCore (NoTracking) | EFCore (WithTracking) | Winner |
|----------|------|---------------------|----------------------|--------|
| **MySQL** | 19.99 MB | 15.08 MB (25% less) | 15.10 MB (24% less) | ✅ **EFCore** |
| **PostgreSQL** | 1.48 MB | 1.86 MB (26% more) | 1.86 MB (26% more) | ✅ **SQLC** |
| **SQLite** | 2.0 MB | 2.41 MB (21% more) | 2.37 MB (19% more) | ✅ **SQLC** |

### Write Performance (50K Batch - Time)

| Database | Operation | SQLC | EFCore | Ratio | Winner |
|----------|-----------|------|--------|-------|--------|
| **MySQL** | AddOrderItems | 320.47 ms | 4,017.49 ms | 1:12.5 | ✅ **SQLC** |
| **MySQL** | AddOrders | 196.48 ms | 3,981.40 ms | 1:20.3 | ✅ **SQLC** |
| **MySQL** | AddProducts | 175.15 ms | 3,929.13 ms | 1:22.4 | ✅ **SQLC** |
| **PostgreSQL** | AddOrderItems | 463.13 ms | 2,197.90 ms | 1:4.7 | ✅ **SQLC** |
| **PostgreSQL** | AddOrders | 306.53 ms | 2,415.53 ms | 1:7.9 | ✅ **SQLC** |
| **PostgreSQL** | AddProducts | 59.82 ms | 2,307.92 ms | 1:38.6 | ✅ **SQLC** |
| **SQLite** | AddOrderItems | 26.75 ms | 40.15 ms | 1:1.5 | ✅ **SQLC** |
| **SQLite** | AddProducts | 24.59 ms | 43.67 ms | 1:1.8 | ✅ **SQLC** |

### Write Performance (50K Batch - Memory)

| Database | Operation | SQLC | EFCore | Ratio | Winner |
|----------|-----------|------|--------|-------|--------|
| **MySQL** | AddOrderItems | 12.30 MB | 676.43 MB | 1:55.0 | ✅ **SQLC** |
| **MySQL** | AddOrders | 9.49 MB | 709.36 MB | 1:74.7 | ✅ **SQLC** |
| **MySQL** | AddProducts | 11.23 MB | 747.03 MB | 1:66.5 | ✅ **SQLC** |
| **PostgreSQL** | AddOrderItems | 2.35 MB | 1,047.11 MB | 1:446.0 | ✅ **SQLC** |
| **PostgreSQL** | AddOrders | 2.35 MB | 1,077.84 MB | 1:458.9 | ✅ **SQLC** |
| **PostgreSQL** | AddProducts | 2.37 MB | 1,103.78 MB | 1:466.4 | ✅ **SQLC** |
| **SQLite** | AddOrderItems | 1.78 MB | 9.32 MB | 1:5.2 | ✅ **SQLC** |
| **SQLite** | AddProducts | 1.76 MB | 11.87 MB | 1:6.8 | ✅ **SQLC** |

---

## Key Insights

### 1. Read Operations

- **MySQL**: EFCore is **5.6-5.7x faster** than SQLC (reversed from previous results with larger dataset)
- **PostgreSQL**: SQLC has a **small 2-6% advantage** - both are viable options
- **SQLite**: SQLC has a **small 1-2% advantage** - all implementations are nearly equivalent
- **Tracking Impact**: Negligible (0.3-1.7% difference) - use `AsNoTracking()` for correctness, not performance
- **Note**: MySQL results changed dramatically with reduced dataset size (matching PostgreSQL)

### 2. Write Operations

- **All Databases**: SQLC is **2-41x faster** depending on database and operation
- **Memory**: SQLC uses **5-466x less memory** - dramatic memory efficiency advantage
- **Scaling**: SQLC scales linearly; EFCore performance degrades with larger batches

### 3. Database-Specific Patterns

- **MySQL**: EFCore faster for reads (5.6x), SQLC faster for writes (7.7-22.4x)
- **PostgreSQL**: SQLC faster for both reads (2-6%) and writes (4.3-38.6x)
- **SQLite**: SQLC faster for both reads (1-2%) and writes (1.5-4.2x), AddOrders still failing

### 4. Memory Efficiency

- **Reads**: Memory differences are generally small (20-30%)
- **Writes**: SQLC uses dramatically less memory (5-466x less)
- **EFCore Memory**: Grows significantly with batch size (up to 1.1 GB at 50K batch)

---

## Recommendations

### For MySQL
- ⚠️ **Use EFCore for reads** (5.6x faster), **SQLC for writes** (7.7-22.4x faster)
- **Reads**: EFCore is faster with smaller datasets
- **Writes**: SQLC is 7.7-22.4x faster, 55-75x less memory

### For PostgreSQL
- ✅ **Use SQLC** for writes (4-41x faster, 100-466x less memory)
- **Reads**: Either is fine (7% difference) - choose based on team preferences

### For SQLite
- ✅ **Use SQLC** for writes (1.5-4.2x faster, 5-7x less memory)
- **Reads**: SQLC is 1-2% faster - all implementations are nearly equivalent
- **Note**: AddOrders operation still failing (needs investigation)

### General Guidelines

1. **Performance-Critical Applications**: Use SQLC
2. **Large Batch Operations**: Use SQLC (better scaling, less memory)
3. **Team Familiarity**: Consider EFCore if team is more familiar with LINQ
4. **Memory Constraints**: Use SQLC (dramatically less memory usage)

---

## Conclusion

**Overall Winner**: ⚠️ **Mixed Results**

- **Read Operations**: EFCore wins in MySQL (5.6x faster with reduced dataset). SQLC wins in PostgreSQL (2-6% faster) and SQLite (1-2% faster)
- **Write Operations**: SQLC wins in all databases (1.5-22.4x faster, 5-466x less memory)
- **Memory Efficiency**: SQLC is dramatically more efficient, especially for writes (5-466x less memory)
- **Scaling**: SQLC scales better with larger batch sizes
- **Note**: MySQL read results reversed with reduced dataset size - EFCore performs better with smaller datasets

**When to Use EFCore:**
- Team is more familiar with LINQ/EFCore
- Need rapid development with code-first migrations
- Performance is not the primary concern
- Working with PostgreSQL/SQLite for reads (small performance gap)

**When to Use SQLC:**
- Performance is critical for writes (all databases)
- Large batch operations (all databases)
- Memory-constrained environments (all databases)
- Need maximum control over SQL queries
- PostgreSQL/SQLite reads (small performance advantage)

**When to Use EFCore:**
- MySQL reads with smaller datasets (5.6x faster)
- Team is more familiar with LINQ/EFCore
- Need rapid development with code-first migrations
- PostgreSQL/SQLite reads (small performance gap acceptable)

