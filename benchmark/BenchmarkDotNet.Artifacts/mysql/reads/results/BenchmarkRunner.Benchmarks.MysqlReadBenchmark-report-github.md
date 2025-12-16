```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ENSYRD : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [79]** |  **6.340 s** | **0.1130 s** | **0.0748 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.82 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [79] | 10.559 s | 0.1676 s | 0.0877 s |  1.66 |    0.02 | 94000.0000 | 48000.0000 | 2000.0000 | 1492.85 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [79] | 10.714 s | 0.3448 s | 0.2280 s |  1.69 |    0.04 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.17 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **2.943 s** | **0.0177 s** | **0.0117 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |   **178.2 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 16.176 s | 3.3217 s | 2.1971 s |  5.50 |    0.75 | 51000.0000 | 11000.0000 | 2000.0000 |  782.11 MB |        4.39 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 15.604 s | 1.8054 s | 1.1942 s |  5.30 |    0.41 | 52000.0000 | 12000.0000 | 3000.0000 |  777.15 MB |        4.36 |
