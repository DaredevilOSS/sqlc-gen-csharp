```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-GAUMSY : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean    | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |--------------------- |--------:|---------:|---------:|------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)500 } [77]** | **3.162 s** | **0.0107 s** | **0.0071 s** |  **1.00** | **18000.0000** | **17000.0000** | **4000.0000** | **310.05 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)500 } [77] | 3.715 s | 0.0325 s | 0.0194 s |  1.17 | 25000.0000 | 24000.0000 | 3000.0000 | 373.79 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)500 } [77] | 3.733 s | 0.0277 s | 0.0183 s |  1.18 | 25000.0000 | 24000.0000 | 3000.0000 | 372.94 MB |        1.20 |
|                                             |                      |         |          |          |       |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [76]** | **3.067 s** | **0.0079 s** | **0.0052 s** |  **1.00** |  **2000.0000** |  **1000.0000** | **1000.0000** |  **39.12 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [76] | 3.425 s | 0.0067 s | 0.0040 s |  1.12 |  7000.0000 |  2000.0000 |         - |  113.9 MB |        2.91 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [76] | 3.424 s | 0.0129 s | 0.0086 s |  1.12 |  6000.0000 |  2000.0000 |         - | 112.49 MB |        2.88 |
