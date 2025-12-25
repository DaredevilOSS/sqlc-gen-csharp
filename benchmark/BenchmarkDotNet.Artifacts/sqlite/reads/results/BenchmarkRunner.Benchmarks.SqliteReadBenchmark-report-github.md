```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YXOMNI : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|-------------------------------------------- |------------------ |-----:|------:|------:|--------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=50, Q=1K**  |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=50, Q=1K  |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=50, Q=1K  |   NA |    NA |     ? |       ? |           ? |
|                                             |                   |      |       |       |         |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=100, Q=3K** |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=100, Q=3K |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=100, Q=3K |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-YXOMNI(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=L=1K, C=50, Q=1K]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-YXOMNI(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=L=1K, C=50, Q=1K]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-YXOMNI(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=L=1K, C=50, Q=1K]
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-YXOMNI(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=L=50, C=100, Q=3K]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-YXOMNI(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=L=50, C=100, Q=3K]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-YXOMNI(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=L=50, C=100, Q=3K]
