```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EERJVO : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)2,000 [42]** |  **5.962 s** | **0.1085 s** | **0.0718 s** |  **1.00** |    **0.00** | **50000.0000** | **28000.0000** | **6000.0000** | **1243.46 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)2,000 [42] |  9.958 s | 0.2342 s | 0.1394 s |  1.67 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1491.38 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)2,000 [42] |  9.929 s | 0.2455 s | 0.1624 s |  1.67 |    0.03 | 94000.0000 | 48000.0000 | 2000.0000 | 1489.03 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)4,000 [40]** |  **2.750 s** | **0.0118 s** | **0.0070 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |  **178.31 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)4,000 [40] | 15.238 s | 3.1630 s | 2.0921 s |  5.43 |    0.71 | 50000.0000 | 10000.0000 | 1000.0000 |  784.01 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)4,000 [40] | 15.149 s | 2.1527 s | 1.4239 s |  5.46 |    0.52 | 50000.0000 | 10000.0000 | 1000.0000 |  777.75 MB |        4.36 |
