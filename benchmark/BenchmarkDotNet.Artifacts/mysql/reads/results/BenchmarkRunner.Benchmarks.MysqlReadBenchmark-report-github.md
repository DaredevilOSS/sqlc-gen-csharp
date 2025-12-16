```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-WZNYFO : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [79]** |  **6.050 s** | **0.1067 s** | **0.0706 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.64 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [79] | 10.474 s | 0.2883 s | 0.1508 s |  1.73 |    0.04 | 94000.0000 | 48000.0000 | 2000.0000 | 1491.74 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [79] | 10.403 s | 0.2523 s | 0.1501 s |  1.72 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.05 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **2.855 s** | **0.0139 s** | **0.0082 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.32 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 16.168 s | 2.8108 s | 1.8592 s |  5.56 |    0.59 | 50000.0000 | 10000.0000 | 1000.0000 |  784.36 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 15.073 s | 1.1206 s | 0.5861 s |  5.28 |    0.21 | 50000.0000 | 11000.0000 | 2000.0000 |  776.56 MB |        4.35 |
