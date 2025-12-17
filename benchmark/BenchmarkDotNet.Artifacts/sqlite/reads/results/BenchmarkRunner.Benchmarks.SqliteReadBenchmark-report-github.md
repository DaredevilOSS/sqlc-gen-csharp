```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EZDFKY : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=50, Q=1K**  |  **9.391 s** | **0.0702 s** | **0.0464 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.05 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=50, Q=1K  | 11.124 s | 0.0352 s | 0.0184 s |  1.18 | 72000.0000 | 71000.0000 | 4000.0000 | 1128.62 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=50, Q=1K  | 10.992 s | 0.0415 s | 0.0274 s |  1.17 | 72000.0000 | 71000.0000 | 4000.0000 | 1127.25 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=100, Q=3K** |  **9.479 s** | **0.0211 s** | **0.0140 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.39 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=100, Q=3K | 10.388 s | 0.0407 s | 0.0269 s |  1.10 | 22000.0000 |  8000.0000 | 2000.0000 |  345.79 MB |        2.71 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=100, Q=3K | 10.373 s | 0.0693 s | 0.0412 s |  1.09 | 22000.0000 |  8000.0000 | 2000.0000 |  340.75 MB |        2.67 |
