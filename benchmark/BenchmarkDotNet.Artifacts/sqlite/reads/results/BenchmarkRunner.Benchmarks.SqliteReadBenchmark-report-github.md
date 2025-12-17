```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-HZVUTS : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)500 } [78]** |  **9.291 s** | **0.0306 s** | **0.0182 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.07 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)500 } [78] | 11.217 s | 0.0346 s | 0.0229 s |  1.21 | 72000.0000 | 71000.0000 | 4000.0000 | 1129.34 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)500 } [78] | 10.964 s | 0.0950 s | 0.0628 s |  1.18 | 72000.0000 | 71000.0000 | 4000.0000 | 1126.37 MB |        1.12 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **9.192 s** | **0.0342 s** | **0.0203 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.41 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 10.327 s | 0.0193 s | 0.0127 s |  1.12 | 22000.0000 |  8000.0000 | 2000.0000 |  344.46 MB |        2.70 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 10.266 s | 0.0371 s | 0.0246 s |  1.12 | 21000.0000 |  8000.0000 | 2000.0000 |  338.71 MB |        2.66 |
