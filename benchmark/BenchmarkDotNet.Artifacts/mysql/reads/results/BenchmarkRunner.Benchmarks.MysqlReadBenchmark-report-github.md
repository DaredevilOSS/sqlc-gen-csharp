```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ENWQTZ : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [79]** |  **5.993 s** | **0.1203 s** | **0.0796 s** |  **1.00** |    **0.00** | **48000.0000** | **26000.0000** | **4000.0000** | **1243.49 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [79] |  9.986 s | 0.2038 s | 0.1213 s |  1.66 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 |  1490.3 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [79] | 10.129 s | 0.2950 s | 0.1756 s |  1.69 |    0.04 | 94000.0000 | 48000.0000 | 2000.0000 | 1488.16 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **2.818 s** | **0.0497 s** | **0.0329 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |   **178.2 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 15.342 s | 3.3305 s | 2.2029 s |  5.45 |    0.81 | 50000.0000 | 10000.0000 | 1000.0000 |  784.33 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 15.237 s | 2.4033 s | 1.5896 s |  5.41 |    0.55 | 49000.0000 | 10000.0000 | 1000.0000 |  776.86 MB |        4.36 |
