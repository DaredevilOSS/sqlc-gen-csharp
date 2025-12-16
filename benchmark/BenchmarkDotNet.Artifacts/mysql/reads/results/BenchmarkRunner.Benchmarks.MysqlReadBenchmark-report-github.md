```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-XUHQLN : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [79]** |  **6.197 s** | **0.2318 s** | **0.1533 s** |  **1.00** |    **0.00** | **46000.0000** | **24000.0000** | **2000.0000** | **1243.78 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [79] | 10.883 s | 0.7858 s | 0.5197 s |  1.76 |    0.10 | 94000.0000 | 48000.0000 | 2000.0000 | 1492.04 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [79] | 10.427 s | 0.4359 s | 0.2594 s |  1.68 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.34 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **2.826 s** | **0.0168 s** | **0.0111 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.25 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 15.921 s | 3.3698 s | 2.2289 s |  5.63 |    0.79 | 50000.0000 | 10000.0000 | 1000.0000 |   785.3 MB |        4.41 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 16.141 s | 1.6014 s | 1.0592 s |  5.71 |    0.37 | 49000.0000 | 10000.0000 | 1000.0000 |  776.41 MB |        4.36 |
