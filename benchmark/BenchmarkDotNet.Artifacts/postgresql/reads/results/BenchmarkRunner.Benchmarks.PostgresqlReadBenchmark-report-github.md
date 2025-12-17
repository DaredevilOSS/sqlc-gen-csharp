```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-UDCPCP : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [78]** |  **7.682 s** | **0.0613 s** | **0.0405 s** |  **1.00** | **42000.0000** | **41000.0000** | **5000.0000** |  **910.02 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [78] |  8.497 s | 0.0897 s | 0.0593 s |  1.11 | 65000.0000 | 64000.0000 | 3000.0000 | 1026.25 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [78] |  8.513 s | 0.0774 s | 0.0512 s |  1.11 | 65000.0000 | 64000.0000 | 3000.0000 | 1023.01 MB |        1.12 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** | **10.780 s** | **0.0850 s** | **0.0562 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.29 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 11.345 s | 0.0234 s | 0.0123 s |  1.05 | 26000.0000 | 10000.0000 | 2000.0000 |  388.39 MB |        2.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 11.357 s | 0.0573 s | 0.0379 s |  1.05 | 25000.0000 | 10000.0000 | 2000.0000 |  380.55 MB |        2.90 |
