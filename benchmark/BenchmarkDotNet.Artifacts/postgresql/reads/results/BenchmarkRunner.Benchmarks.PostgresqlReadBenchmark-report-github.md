```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-JZHRXG : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean    | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |--------------------- |--------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [78]** | **3.834 s** | **0.0530 s** | **0.0350 s** |  **1.00** |    **0.00** | **22000.0000** | **21000.0000** | **4000.0000** | **454.92 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [78] |      NA |       NA |       NA |     ? |       ? |         NA |         NA |        NA |        NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [78] |      NA |       NA |       NA |     ? |       ? |         NA |         NA |        NA |        NA |           ? |
|                                             |                      |         |          |          |       |         |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** | **5.342 s** | **0.0715 s** | **0.0473 s** |  **1.00** |    **0.00** |  **3000.0000** |  **2000.0000** | **1000.0000** |  **65.67 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] |      NA |       NA |       NA |     ? |       ? |         NA |         NA |        NA |        NA |           ? |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] |      NA |       NA |       NA |     ? |       ? |         NA |         NA |        NA |        NA |           ? |

Benchmarks with issues:
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-JZHRXG(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [78]]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-JZHRXG(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [78]]
  PostgresqlReadBenchmark.'EFCore (NoTracking) - GetCustomerOrders': Job-JZHRXG(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [77]]
  PostgresqlReadBenchmark.'EFCore (WithTracking) - GetCustomerOrders': Job-JZHRXG(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [Params=ReadB(...)000 } [77]]
