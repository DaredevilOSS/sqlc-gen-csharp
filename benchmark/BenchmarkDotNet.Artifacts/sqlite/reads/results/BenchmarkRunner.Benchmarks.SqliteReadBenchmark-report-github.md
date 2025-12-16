```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-DXXSNC : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean    | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |--------------------- |--------:|---------:|---------:|------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)500 } [77]** | **3.142 s** | **0.0212 s** | **0.0140 s** |  **1.00** | **18000.0000** | **17000.0000** | **4000.0000** | **310.02 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)500 } [77] | 3.770 s | 0.0225 s | 0.0134 s |  1.20 | 25000.0000 | 24000.0000 | 3000.0000 |    374 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)500 } [77] | 3.730 s | 0.0180 s | 0.0107 s |  1.19 | 25000.0000 | 24000.0000 | 3000.0000 | 372.99 MB |        1.20 |
|                                             |                      |         |          |          |       |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [76]** | **3.051 s** | **0.0086 s** | **0.0045 s** |  **1.00** |  **2000.0000** |  **1000.0000** | **1000.0000** |  **39.12 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [76] | 3.460 s | 0.0333 s | 0.0220 s |  1.13 |  7000.0000 |  2000.0000 |         - |  114.3 MB |        2.92 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [76] | 3.462 s | 0.0306 s | 0.0182 s |  1.13 |  6000.0000 |  2000.0000 |         - | 112.46 MB |        2.87 |
