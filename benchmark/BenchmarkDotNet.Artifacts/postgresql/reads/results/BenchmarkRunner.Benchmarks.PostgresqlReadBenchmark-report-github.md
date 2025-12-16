```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-BETGST : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean    | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |--------------------- |--------:|---------:|---------:|------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [78]** | **3.850 s** | **0.0604 s** | **0.0359 s** |  **1.00** | **20000.0000** | **19000.0000** | **2000.0000** | **455.08 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [78] | 4.256 s | 0.0411 s | 0.0272 s |  1.11 | 33000.0000 | 32000.0000 | 2000.0000 | 513.32 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [78] | 4.219 s | 0.0463 s | 0.0306 s |  1.10 | 33000.0000 | 32000.0000 | 2000.0000 | 511.26 MB |        1.12 |
|                                             |                      |         |          |          |       |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** | **5.389 s** | **0.0256 s** | **0.0152 s** |  **1.00** |  **3000.0000** |  **2000.0000** | **1000.0000** |  **65.64 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 5.683 s | 0.0587 s | 0.0388 s |  1.05 | 13000.0000 |  5000.0000 | 1000.0000 | 193.35 MB |        2.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 5.672 s | 0.0384 s | 0.0254 s |  1.05 | 12000.0000 |  5000.0000 | 1000.0000 | 190.88 MB |        2.91 |
