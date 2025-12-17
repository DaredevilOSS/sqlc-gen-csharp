```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-RYAXLS : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)es=2K [37]** |  **6.053 s** | **0.1175 s** | **0.0777 s** |  **1.00** |    **0.00** | **47000.0000** | **25000.0000** | **3000.0000** | **1243.73 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)es=2K [37] | 10.189 s | 0.2491 s | 0.1303 s |  1.68 |    0.02 | 94000.0000 | 48000.0000 | 2000.0000 | 1491.39 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)es=2K [37] | 10.215 s | 0.0877 s | 0.0522 s |  1.69 |    0.02 | 94000.0000 | 48000.0000 | 2000.0000 |  1489.3 MB |        1.20 |
|                                             |                      |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)es=4K [37]** |  **2.797 s** | **0.0174 s** | **0.0115 s** |  **1.00** |    **0.00** | **10000.0000** |  **6000.0000** | **2000.0000** |   **178.3 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)es=4K [37] | 15.598 s | 3.2388 s | 2.1423 s |  5.58 |    0.77 | 50000.0000 | 10000.0000 | 1000.0000 |   783.9 MB |        4.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)es=4K [37] | 15.579 s | 2.4339 s | 1.6099 s |  5.57 |    0.57 | 50000.0000 | 11000.0000 | 2000.0000 |  776.11 MB |        4.35 |
