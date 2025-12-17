```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ORMWPU : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=100, Q=2K** |  **6.147 s** | **0.1465 s** | **0.0969 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.69 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=100, Q=2K | 10.719 s | 0.6145 s | 0.4064 s |  1.74 |    0.07 | 94000.0000 | 48000.0000 | 2000.0000 | 1491.14 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=100, Q=2K | 10.540 s | 0.4190 s | 0.2771 s |  1.72 |    0.06 | 94000.0000 | 48000.0000 | 2000.0000 | 1490.26 MB |        1.20 |
|                                             |                   |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=200, Q=4K** |  **2.800 s** | **0.0389 s** | **0.0257 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.26 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=200, Q=4K | 16.186 s | 3.8553 s | 2.5500 s |  5.78 |    0.89 | 50000.0000 | 10000.0000 | 1000.0000 |  784.77 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=200, Q=4K | 15.297 s | 1.3578 s | 0.8080 s |  5.47 |    0.27 | 49000.0000 | 10000.0000 | 1000.0000 |  776.32 MB |        4.35 |
