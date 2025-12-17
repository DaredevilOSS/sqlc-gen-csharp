```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-NQWZCO : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=75, Q=2K**  |  **7.561 s** | **0.0641 s** | **0.0424 s** |  **1.00** | **39000.0000** | **38000.0000** | **2000.0000** |  **910.15 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=75, Q=2K  |  8.336 s | 0.0663 s | 0.0439 s |  1.10 | 65000.0000 | 64000.0000 | 3000.0000 | 1026.48 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=75, Q=2K  |  8.339 s | 0.0687 s | 0.0409 s |  1.10 | 65000.0000 | 64000.0000 | 3000.0000 | 1022.94 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=150, Q=4K** | **10.536 s** | **0.0684 s** | **0.0453 s** |  **1.00** |  **6000.0000** |  **5000.0000** | **1000.0000** |  **131.29 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=150, Q=4K | 11.139 s | 0.0588 s | 0.0389 s |  1.06 | 26000.0000 | 10000.0000 | 2000.0000 |  387.95 MB |        2.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=150, Q=4K | 11.116 s | 0.0620 s | 0.0410 s |  1.06 | 25000.0000 | 10000.0000 | 2000.0000 |  382.91 MB |        2.92 |
