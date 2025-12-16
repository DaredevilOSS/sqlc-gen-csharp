```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EBRLFP : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean    | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |--------------------- |--------:|---------:|---------:|------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)500 } [77]** | **3.204 s** | **0.0157 s** | **0.0104 s** |  **1.00** | **18000.0000** | **17000.0000** | **4000.0000** | **310.03 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)500 } [77] | 3.751 s | 0.0200 s | 0.0132 s |  1.17 | 25000.0000 | 24000.0000 | 3000.0000 | 373.81 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)500 } [77] | 3.754 s | 0.0245 s | 0.0162 s |  1.17 | 25000.0000 | 24000.0000 | 3000.0000 | 372.68 MB |        1.20 |
|                                             |                      |         |          |          |       |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [76]** | **3.063 s** | **0.0075 s** | **0.0050 s** |  **1.00** |  **2000.0000** |  **1000.0000** | **1000.0000** |  **39.12 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [76] | 3.434 s | 0.0123 s | 0.0082 s |  1.12 |  7000.0000 |  2000.0000 |         - | 114.16 MB |        2.92 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [76] | 3.448 s | 0.0082 s | 0.0043 s |  1.13 |  7000.0000 |  2000.0000 |         - | 113.04 MB |        2.89 |
