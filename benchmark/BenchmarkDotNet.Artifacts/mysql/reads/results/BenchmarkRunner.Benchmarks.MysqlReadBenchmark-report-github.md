```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-VONIUY : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=100, Q=2K** |  **5.965 s** | **0.0947 s** | **0.0626 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=100, Q=2K | 10.225 s | 0.4405 s | 0.2914 s |  1.71 |    0.05 | 94000.0000 | 48000.0000 | 2000.0000 | 1493.02 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=100, Q=2K | 10.122 s | 0.1344 s | 0.0703 s |  1.69 |    0.01 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.65 MB |        1.20 |
|                                             |                   |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=200, Q=4K** |  **2.796 s** | **0.0177 s** | **0.0106 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.21 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=200, Q=4K | 15.288 s | 3.2086 s | 2.1223 s |  5.36 |    0.71 | 50000.0000 | 10000.0000 | 1000.0000 |  782.82 MB |        4.39 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=200, Q=4K | 15.183 s | 2.1360 s | 1.4129 s |  5.40 |    0.52 | 49000.0000 | 10000.0000 | 1000.0000 |  777.64 MB |        4.36 |
