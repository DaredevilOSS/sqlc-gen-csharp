```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-SDNUPU : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=50, Q=1K**  |  **9.155 s** | **0.0186 s** | **0.0123 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.08 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=50, Q=1K  | 11.159 s | 0.0309 s | 0.0184 s |  1.22 | 72000.0000 | 71000.0000 | 4000.0000 | 1128.85 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=50, Q=1K  | 11.083 s | 0.0178 s | 0.0106 s |  1.21 | 72000.0000 | 71000.0000 | 4000.0000 |    1126 MB |        1.11 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=100, Q=3K** |  **9.074 s** | **0.0072 s** | **0.0048 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.37 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=100, Q=3K | 10.478 s | 0.0238 s | 0.0157 s |  1.15 | 22000.0000 |  8000.0000 | 2000.0000 |  344.22 MB |        2.70 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=100, Q=3K | 10.263 s | 0.0213 s | 0.0127 s |  1.13 | 22000.0000 |  8000.0000 | 2000.0000 |  338.85 MB |        2.66 |
