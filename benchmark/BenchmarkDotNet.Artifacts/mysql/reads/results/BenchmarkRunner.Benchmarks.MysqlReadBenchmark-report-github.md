```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-KOYNRB : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|-------------------------------------------- |--------------------- |-----:|------:|------:|--------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [79]** |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [79] |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [79] |   NA |    NA |     ? |       ? |           ? |
|                                             |                      |      |       |       |         |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] |   NA |    NA |     ? |       ? |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  MysqlReadBenchmark.'SQLC - GetCustomerOrders': Job-KOYNRB(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [79]]
  MysqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-KOYNRB(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [79]]
  MysqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-KOYNRB(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [79]]
  MysqlReadBenchmark.'SQLC - GetCustomerOrders': Job-KOYNRB(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [77]]
  MysqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-KOYNRB(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [77]]
  MysqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-KOYNRB(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [77]]
