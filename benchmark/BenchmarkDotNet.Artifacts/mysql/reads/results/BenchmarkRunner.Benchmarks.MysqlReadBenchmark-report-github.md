```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-CYAUUP : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [79]** |  **5.755 s** | **0.1121 s** | **0.0742 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.11 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [79] |  9.784 s | 0.2544 s | 0.1330 s |  1.70 |    0.02 | 94000.0000 | 48000.0000 | 2000.0000 | 1491.61 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [79] |  9.836 s | 0.1640 s | 0.0857 s |  1.70 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1490.66 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **2.716 s** | **0.0179 s** | **0.0118 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.26 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 14.948 s | 2.9508 s | 1.9518 s |  5.50 |    0.72 | 50000.0000 | 10000.0000 | 1000.0000 |  783.63 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 15.070 s | 2.4533 s | 1.6227 s |  5.55 |    0.58 | 50000.0000 | 11000.0000 | 3000.0000 |  775.64 MB |        4.35 |
