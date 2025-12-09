```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-ZKKNBP : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | ConcurrentQueries | Mean        | Error      | StdDev    | Median      | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated   | Alloc Ratio |
|-------------------------------------------- |------ |------------------ |------------:|-----------:|----------:|------------:|------:|--------:|----------:|----------:|----------:|------------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10**                | **1,597.66 ms** |  **42.232 ms** | **27.934 ms** | **1,594.13 ms** |  **1.00** |    **0.00** | **7000.0000** | **7000.0000** | **2000.0000** | **36759.23 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10                |    95.00 ms |   9.281 ms |  6.139 ms |    93.17 ms |  0.06 |    0.00 |         - |         - |         - |   697.94 KB |        0.02 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10                |   103.31 ms |  13.069 ms |  7.777 ms |   103.05 ms |  0.06 |    0.00 |         - |         - |         - |   689.33 KB |        0.02 |
|                                             |       |                   |             |            |           |             |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **100**               |          **NA** |         **NA** |        **NA** |          **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 100               |          NA |         NA |        NA |          NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 100               |          NA |         NA |        NA |          NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
|                                             |       |                   |             |            |           |             |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **10**                |   **133.28 ms** |  **70.891 ms** | **46.890 ms** |   **106.00 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |    **54.76 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 10                |   432.95 ms | 151.720 ms | 90.286 ms |   436.72 ms |  3.34 |    0.71 |         - |         - |         - |   692.41 KB |       12.64 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 10                |   150.00 ms |  15.610 ms | 10.325 ms |   153.43 ms |  1.23 |    0.36 |         - |         - |         - |    681.3 KB |       12.44 |
|                                             |       |                   |             |            |           |             |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **100**               |          **NA** |         **NA** |        **NA** |          **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 100               |          NA |         NA |        NA |          NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 100               |          NA |         NA |        NA |          NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
|                                             |       |                   |             |            |           |             |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **10**                |   **192.21 ms** |  **30.792 ms** | **20.367 ms** |   **186.00 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |    **53.49 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 10                |   222.49 ms |  72.407 ms | 43.088 ms |   207.51 ms |  1.19 |    0.24 |         - |         - |         - |   688.34 KB |       12.87 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 10                |   179.84 ms |  28.632 ms | 18.938 ms |   171.05 ms |  0.94 |    0.11 |         - |         - |         - |   681.13 KB |       12.73 |
|                                             |       |                   |             |            |           |             |       |         |           |           |           |             |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **100**               |          **NA** |         **NA** |        **NA** |          **NA** |     **?** |       **?** |        **NA** |        **NA** |        **NA** |          **NA** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 100               |          NA |         NA |        NA |          NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 100               |          NA |         NA |        NA |          NA |     ? |       ? |        NA |        NA |        NA |          NA |           ? |

Benchmarks with issues:
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=5000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=10000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-ZKKNBP(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [Limit=20000, ConcurrentQueries=100]
