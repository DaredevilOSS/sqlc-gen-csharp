```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-DGYZYR : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=75, Q=2K**  |  **7.738 s** | **0.0388 s** | **0.0231 s** |  **1.00** | **39000.0000** | **38000.0000** | **2000.0000** |     **910 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=75, Q=2K  |  8.591 s | 0.0784 s | 0.0519 s |  1.11 | 63000.0000 | 62000.0000 | 1000.0000 | 1026.64 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=75, Q=2K  |  8.609 s | 0.0668 s | 0.0442 s |  1.11 | 65000.0000 | 64000.0000 | 3000.0000 | 1023.09 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=150, Q=4K** | **10.777 s** | **0.0595 s** | **0.0393 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.24 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=150, Q=4K | 11.440 s | 0.1026 s | 0.0678 s |  1.06 | 26000.0000 | 10000.0000 | 2000.0000 |  387.36 MB |        2.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=150, Q=4K | 11.440 s | 0.0599 s | 0.0396 s |  1.06 | 25000.0000 | 10000.0000 | 2000.0000 |  380.58 MB |        2.90 |
