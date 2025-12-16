```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-EXWNVP : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)500 } [78]** |  **9.594 s** | **0.0395 s** | **0.0261 s** |  **1.00** | **33000.0000** | **32000.0000** | **5000.0000** | **1010.07 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)500 } [78] | 11.871 s | 0.1069 s | 0.0636 s |  1.24 | 46000.0000 | 45000.0000 | 1000.0000 |  1128.6 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)500 } [78] | 11.624 s | 0.1451 s | 0.0960 s |  1.21 | 46000.0000 | 45000.0000 | 1000.0000 | 1126.81 MB |        1.12 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **ReadB(...)000 } [77]** |  **8.849 s** | **0.0325 s** | **0.0194 s** |  **1.00** |  **5000.0000** |  **4000.0000** | **2000.0000** |  **127.38 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | ReadB(...)000 } [77] | 10.022 s | 0.0737 s | 0.0488 s |  1.13 | 15000.0000 |  8000.0000 | 1000.0000 |  344.91 MB |        2.71 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | ReadB(...)000 } [77] | 10.016 s | 0.0307 s | 0.0182 s |  1.13 | 14000.0000 |  7000.0000 | 1000.0000 |  338.65 MB |        2.66 |
