```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-GXXNTG : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | ConcurrentQueries | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated   | Alloc Ratio |
|-------------------------------------------- |------ |------------------ |-----------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|------------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10**                | **4,039.8 ms** | **28.31 ms** | **14.81 ms** |  **1.00** |    **0.00** | **3000.0000** | **3000.0000** | **3000.0000** | **36781.55 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10                |   237.3 ms | 43.36 ms | 28.68 ms |  0.06 |    0.01 |         - |         - |         - |    683.5 KB |        0.02 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10                |   243.7 ms | 49.59 ms | 32.80 ms |  0.06 |    0.01 |         - |         - |         - |   675.38 KB |        0.02 |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **100**               |         **NA** |       **NA** |       **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **10**                |   **230.6 ms** | **14.90 ms** |  **9.85 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |    **51.24 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 10                |   244.1 ms | 50.28 ms | 33.26 ms |  1.06 |    0.13 |         - |         - |         - |   685.92 KB |       13.39 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 10                |   250.7 ms | 41.98 ms | 27.76 ms |  1.09 |    0.12 |         - |         - |         - |   686.66 KB |       13.40 |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **100**               |         **NA** |       **NA** |       **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **10**                |   **233.9 ms** | **18.26 ms** | **12.08 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |    **51.24 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 10                |   252.5 ms | 51.00 ms | 33.73 ms |  1.08 |    0.15 |         - |         - |         - |   684.88 KB |       13.37 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 10                |   248.1 ms | 51.85 ms | 34.30 ms |  1.06 |    0.16 |         - |         - |         - |   678.34 KB |       13.24 |
|                                             |       |                   |            |          |          |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **100**               |         **NA** |       **NA** |       **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 100               |         NA |       NA |       NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |

Benchmarks with issues:
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-GXXNTG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
