```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-NKYZDL : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params              | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |-------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1.0K,C=100,Q=2.0K** |  **5.755 s** | **0.0978 s** | **0.0647 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.39 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1.0K,C=100,Q=2.0K |  9.619 s | 0.1822 s | 0.1085 s |  1.67 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1493.51 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1.0K,C=100,Q=2.0K |  9.648 s | 0.1764 s | 0.1049 s |  1.67 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.07 MB |        1.20 |
|                                             |                     |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50,C=200,Q=4.0K**   |  **2.686 s** | **0.0432 s** | **0.0286 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.26 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50,C=200,Q=4.0K   | 14.870 s | 3.2039 s | 2.1192 s |  5.54 |    0.79 | 51000.0000 | 11000.0000 | 2000.0000 |  786.39 MB |        4.41 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50,C=200,Q=4.0K   | 14.967 s | 2.5572 s | 1.6914 s |  5.57 |    0.62 | 50000.0000 | 11000.0000 | 2000.0000 |  776.83 MB |        4.36 |
