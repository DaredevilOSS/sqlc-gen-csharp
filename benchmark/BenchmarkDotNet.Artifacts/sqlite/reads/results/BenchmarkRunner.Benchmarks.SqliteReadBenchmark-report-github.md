```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-RUBHRK : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|-------------------------------------------- |--------------------- |-----:|------:|------:|--------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)500 } [77]** |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)500 } [77] |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)500 } [77] |   NA |    NA |     ? |       ? |           ? |
|                                             |                      |      |       |       |         |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [76]** |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [76] |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [76] |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-RUBHRK(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)500 } [77]]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-RUBHRK(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)500 } [77]]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-RUBHRK(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)500 } [77]]
  SqliteReadBenchmark.'SQLC - GetCustomerOrders': Job-RUBHRK(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [76]]
  SqliteReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-RUBHRK(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [76]]
  SqliteReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-RUBHRK(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [76]]
