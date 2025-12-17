```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-OVVOTJ : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=100, Q=2K** |  **6.180 s** | **0.1594 s** | **0.1055 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.64 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=100, Q=2K | 10.794 s | 0.7980 s | 0.5278 s |  1.75 |    0.08 | 94000.0000 | 48000.0000 | 2000.0000 | 1492.87 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=100, Q=2K | 10.626 s | 0.2649 s | 0.1576 s |  1.72 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1489.05 MB |        1.20 |
|                                             |                   |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=200, Q=4K** |  **2.942 s** | **0.0336 s** | **0.0200 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.25 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=200, Q=4K | 16.741 s | 2.1171 s | 1.4003 s |  5.67 |    0.52 | 50000.0000 | 10000.0000 | 1000.0000 |  783.88 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=200, Q=4K | 15.781 s | 0.8646 s | 0.4522 s |  5.36 |    0.17 | 48000.0000 |  9000.0000 |         - |  776.08 MB |        4.35 |
