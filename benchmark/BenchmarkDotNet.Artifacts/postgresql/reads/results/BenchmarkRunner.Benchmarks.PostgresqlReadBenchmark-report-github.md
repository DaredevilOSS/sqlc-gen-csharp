```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-JIISVR : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=75, Q=2K**  |  **7.638 s** | **0.0777 s** | **0.0514 s** |  **1.00** | **39000.0000** | **38000.0000** | **2000.0000** |  **910.04 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=75, Q=2K  |  8.418 s | 0.0727 s | 0.0481 s |  1.10 | 65000.0000 | 64000.0000 | 3000.0000 | 1027.15 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=75, Q=2K  |  8.396 s | 0.0622 s | 0.0411 s |  1.10 | 65000.0000 | 64000.0000 | 3000.0000 | 1023.74 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=150, Q=4K** | **10.655 s** | **0.0433 s** | **0.0287 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.29 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=150, Q=4K | 11.253 s | 0.1116 s | 0.0738 s |  1.06 | 26000.0000 | 10000.0000 | 2000.0000 |  387.37 MB |        2.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=150, Q=4K | 11.283 s | 0.0505 s | 0.0300 s |  1.06 | 25000.0000 | 10000.0000 | 2000.0000 |  379.69 MB |        2.89 |
