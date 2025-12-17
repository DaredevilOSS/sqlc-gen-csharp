```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EVLVTR : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)1,500 [41]** |  **9.569 s** | **0.0833 s** | **0.0551 s** |  **1.00** |    **0.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.09 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)1,500 [41] | 11.715 s | 0.3787 s | 0.2505 s |  1.22 |    0.03 | 69000.0000 | 68000.0000 | 1000.0000 | 1128.19 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)1,500 [41] | 11.334 s | 0.2682 s | 0.1774 s |  1.18 |    0.02 | 72000.0000 | 71000.0000 | 4000.0000 | 1126.06 MB |        1.11 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)3,000 [40]** |  **9.335 s** | **0.0952 s** | **0.0630 s** |  **1.00** |    **0.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.38 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)3,000 [40] | 10.505 s | 0.2054 s | 0.1359 s |  1.13 |    0.01 | 22000.0000 |  8000.0000 | 2000.0000 |  344.02 MB |        2.70 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)3,000 [40] | 10.378 s | 0.1681 s | 0.1112 s |  1.11 |    0.01 | 21000.0000 |  8000.0000 | 2000.0000 |  338.45 MB |        2.66 |
