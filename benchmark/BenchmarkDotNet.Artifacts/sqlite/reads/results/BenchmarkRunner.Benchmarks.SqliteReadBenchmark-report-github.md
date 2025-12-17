```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EHHQBB : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=50, Q=1K**  |  **9.540 s** | **0.0328 s** | **0.0195 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.09 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=50, Q=1K  | 11.339 s | 0.0371 s | 0.0245 s |  1.19 | 72000.0000 | 71000.0000 | 4000.0000 | 1129.55 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=50, Q=1K  | 11.197 s | 0.0869 s | 0.0575 s |  1.17 | 72000.0000 | 71000.0000 | 4000.0000 | 1126.92 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=100, Q=3K** |  **9.170 s** | **0.0209 s** | **0.0138 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.38 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=100, Q=3K | 10.266 s | 0.0198 s | 0.0131 s |  1.12 | 22000.0000 |  8000.0000 | 2000.0000 |  343.46 MB |        2.70 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=100, Q=3K | 10.201 s | 0.0128 s | 0.0076 s |  1.11 | 22000.0000 |  8000.0000 | 2000.0000 |   339.6 MB |        2.67 |
