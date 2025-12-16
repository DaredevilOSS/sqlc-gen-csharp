```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EMIWUW : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean    | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |--------------------- |--------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [78]** | **3.834 s** | **0.0966 s** | **0.0639 s** |  **1.00** |    **0.00** | **20000.0000** | **19000.0000** | **2000.0000** | **455.03 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [78] | 4.230 s | 0.0436 s | 0.0260 s |  1.10 |    0.02 | 33000.0000 | 32000.0000 | 2000.0000 | 513.46 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [78] | 4.261 s | 0.0772 s | 0.0511 s |  1.11 |    0.02 | 33000.0000 | 32000.0000 | 2000.0000 | 511.39 MB |        1.12 |
|                                             |                      |         |          |          |       |         |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** | **5.330 s** | **0.0424 s** | **0.0280 s** |  **1.00** |    **0.00** |  **3000.0000** |  **2000.0000** | **1000.0000** |  **65.67 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 5.635 s | 0.0843 s | 0.0557 s |  1.06 |    0.01 | 13000.0000 |  5000.0000 | 1000.0000 | 193.62 MB |        2.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 5.612 s | 0.0866 s | 0.0515 s |  1.05 |    0.01 | 12000.0000 |  5000.0000 | 1000.0000 | 190.47 MB |        2.90 |
