```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ERTZWV : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)es=2K [36]** |  **7.683 s** | **0.0410 s** | **0.0271 s** |  **1.00** | **39000.0000** | **38000.0000** | **2000.0000** |  **909.73 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)es=2K [36] |  8.628 s | 0.0937 s | 0.0620 s |  1.12 | 65000.0000 | 64000.0000 | 3000.0000 | 1026.08 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)es=2K [36] |  8.689 s | 0.0934 s | 0.0556 s |  1.13 | 65000.0000 | 64000.0000 | 3000.0000 | 1022.91 MB |        1.12 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)es=4K [37]** | **10.658 s** | **0.1233 s** | **0.0816 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.28 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)es=4K [37] | 11.241 s | 0.0443 s | 0.0264 s |  1.05 | 26000.0000 | 10000.0000 | 2000.0000 |  386.58 MB |        2.94 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)es=4K [37] | 11.224 s | 0.0364 s | 0.0216 s |  1.05 | 25000.0000 | 10000.0000 | 2000.0000 |  380.08 MB |        2.90 |
