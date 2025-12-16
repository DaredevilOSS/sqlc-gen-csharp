```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-OODZNM : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [78]** |  **7.820 s** | **0.0553 s** | **0.0366 s** |  **1.00** | **39000.0000** | **38000.0000** | **2000.0000** |  **909.83 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [78] |  8.561 s | 0.0499 s | 0.0330 s |  1.09 | 65000.0000 | 64000.0000 | 3000.0000 | 1026.37 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [78] |  8.637 s | 0.0661 s | 0.0437 s |  1.10 | 65000.0000 | 64000.0000 | 3000.0000 | 1023.71 MB |        1.13 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** | **10.879 s** | **0.0943 s** | **0.0624 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.27 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 11.460 s | 0.0586 s | 0.0388 s |  1.05 | 26000.0000 | 10000.0000 | 2000.0000 |  388.98 MB |        2.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 11.488 s | 0.0668 s | 0.0442 s |  1.06 | 25000.0000 | 10000.0000 | 2000.0000 |  380.36 MB |        2.90 |
