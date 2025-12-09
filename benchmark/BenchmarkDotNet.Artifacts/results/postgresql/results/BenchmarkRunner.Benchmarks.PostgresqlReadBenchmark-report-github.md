```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-QEIBGC : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | ConcurrentQueries | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated   | Alloc Ratio |
|-------------------------------------------- |------ |------------------ |-----------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|------------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10**                | **4,061.6 ms** | **44.02 ms** | **23.02 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** | **36775.21 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10                |   248.4 ms | 49.94 ms | 33.03 ms |  0.06 |    0.01 |         - |         - |         - |   688.27 KB |        0.02 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10                |   246.4 ms | 53.51 ms | 35.40 ms |  0.06 |    0.01 |         - |         - |         - |   682.33 KB |        0.02 |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **100**               |         **NA** |       **NA** |       **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **10**                |   **233.7 ms** | **20.84 ms** | **13.78 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |    **51.24 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 10                |   258.5 ms | 46.40 ms | 30.69 ms |  1.11 |    0.13 |         - |         - |         - |   685.26 KB |       13.37 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 10                |   253.4 ms | 43.48 ms | 28.76 ms |  1.09 |    0.14 |         - |         - |         - |   678.73 KB |       13.25 |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **100**               |         **NA** |       **NA** |       **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **10**                |   **241.2 ms** | **11.20 ms** |  **5.86 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |    **51.55 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 10                |   257.6 ms | 52.06 ms | 34.44 ms |  1.11 |    0.15 |         - |         - |         - |   683.49 KB |       13.26 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 10                |   253.2 ms | 41.24 ms | 27.28 ms |  1.05 |    0.13 |         - |         - |         - |   686.09 KB |       13.31 |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **100**               |         **NA** |       **NA** |       **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |

Benchmarks with issues:
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-QEIBGC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
