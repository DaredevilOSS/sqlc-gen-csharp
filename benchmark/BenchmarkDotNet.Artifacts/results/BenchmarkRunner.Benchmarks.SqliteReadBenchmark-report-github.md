```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-VDLBPN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|-------------------------------------------- |------ |-----:|------:|------:|--------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **500**   |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 500   |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 500   |   NA |    NA |     ? |       ? |           ? |
|                                             |       |      |       |       |         |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **2000**  |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 2000  |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 2000  |   NA |    NA |     ? |       ? |           ? |
|                                             |       |      |       |       |         |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=500]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=500]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=500]
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=2000]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=2000]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=2000]
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=5000]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=5000]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [Limit=5000]
