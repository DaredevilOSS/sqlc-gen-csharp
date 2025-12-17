```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-JCYNVC : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)2,000 [41]** |  **7.706 s** | **0.0965 s** | **0.0638 s** |  **1.00** | **39000.0000** | **38000.0000** | **2000.0000** |  **909.86 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)2,000 [41] |  8.609 s | 0.0696 s | 0.0460 s |  1.12 | 65000.0000 | 64000.0000 | 3000.0000 | 1025.87 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)2,000 [41] |  8.587 s | 0.1204 s | 0.0796 s |  1.11 | 65000.0000 | 64000.0000 | 3000.0000 | 1023.15 MB |        1.12 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)4,000 [40]** | **10.786 s** | **0.0996 s** | **0.0659 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.26 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)4,000 [40] | 11.311 s | 0.0589 s | 0.0390 s |  1.05 | 26000.0000 | 10000.0000 | 2000.0000 |  387.94 MB |        2.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)4,000 [40] | 11.284 s | 0.0394 s | 0.0260 s |  1.05 | 25000.0000 | 10000.0000 | 2000.0000 |  379.93 MB |        2.89 |
