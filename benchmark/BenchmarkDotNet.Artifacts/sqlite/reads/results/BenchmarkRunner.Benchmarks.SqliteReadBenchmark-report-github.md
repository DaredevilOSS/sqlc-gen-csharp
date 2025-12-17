```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-FIWNFX : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=50, Q=1K**  |  **9.190 s** | **0.0196 s** | **0.0130 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.04 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=50, Q=1K  | 11.282 s | 0.0257 s | 0.0153 s |  1.23 | 72000.0000 | 71000.0000 | 4000.0000 |  1129.4 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=50, Q=1K  | 11.049 s | 0.0277 s | 0.0165 s |  1.20 | 72000.0000 | 71000.0000 | 4000.0000 | 1126.61 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=100, Q=3K** |  **9.087 s** | **0.0238 s** | **0.0142 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.37 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=100, Q=3K | 10.284 s | 0.0342 s | 0.0226 s |  1.13 | 22000.0000 |  8000.0000 | 2000.0000 |  344.54 MB |        2.70 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=100, Q=3K | 10.276 s | 0.0199 s | 0.0118 s |  1.13 | 22000.0000 |  8000.0000 | 2000.0000 |  340.35 MB |        2.67 |
