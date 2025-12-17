```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-MKTQLE : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=100, Q=2K** |  **6.040 s** | **0.1213 s** | **0.0803 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** |  **1243.4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=100, Q=2K | 10.158 s | 0.2126 s | 0.1112 s |  1.68 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1492.19 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=100, Q=2K | 10.192 s | 0.2091 s | 0.1383 s |  1.69 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.34 MB |        1.20 |
|                                             |                   |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=200, Q=4K** |  **2.849 s** | **0.0446 s** | **0.0295 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.09 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=200, Q=4K | 15.472 s | 3.6460 s | 2.4116 s |  5.43 |    0.84 | 50000.0000 | 10000.0000 | 1000.0000 |  783.42 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=200, Q=4K | 15.419 s | 2.5600 s | 1.6933 s |  5.42 |    0.63 | 50000.0000 | 11000.0000 | 2000.0000 |   775.5 MB |        4.35 |
