```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-DDSKTY : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | ConcurrentQueries | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |------------------ |-----------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **1**                 |   **183.9 ms** |  **0.90 ms** |  **0.53 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |      **4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 1                 |   188.8 ms |  6.99 ms |  3.66 ms |  1.03 |    0.02 |         - |         - |         - |   3.51 MB |        0.88 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 1                 |   194.7 ms | 16.39 ms | 10.84 ms |  1.06 |    0.06 |         - |         - |         - |   3.51 MB |        0.88 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10**                | **1,839.2 ms** | **13.95 ms** |  **8.30 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** |   **37.5 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10                | 1,893.9 ms |  9.75 ms |  6.45 ms |  1.03 |    0.00 | 2000.0000 | 1000.0000 |         - |   34.8 MB |        0.93 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10                | 1,890.3 ms |  5.28 ms |  3.14 ms |  1.03 |    0.00 | 2000.0000 | 1000.0000 |         - |  34.78 MB |        0.93 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **1**                 |   **183.9 ms** |  **0.98 ms** |  **0.51 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |      **4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 1                 |   192.7 ms | 11.64 ms |  7.70 ms |  1.05 |    0.04 |         - |         - |         - |   3.51 MB |        0.88 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 1                 |   194.7 ms | 15.25 ms | 10.09 ms |  1.07 |    0.05 |         - |         - |         - |   3.51 MB |        0.88 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **10**                | **1,831.9 ms** |  **8.46 ms** |  **5.04 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** |   **37.5 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 10                | 1,888.3 ms |  5.78 ms |  3.02 ms |  1.03 |    0.00 | 2000.0000 | 1000.0000 |         - |  34.79 MB |        0.93 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 10                | 1,881.7 ms |  4.14 ms |  2.46 ms |  1.03 |    0.00 | 2000.0000 | 1000.0000 |         - |  34.78 MB |        0.93 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **1**                 |   **185.4 ms** |  **2.88 ms** |  **1.90 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |      **4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 1                 |   199.0 ms | 17.65 ms | 11.67 ms |  1.07 |    0.06 |         - |         - |         - |   3.51 MB |        0.88 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 1                 |   198.7 ms | 16.90 ms | 11.18 ms |  1.07 |    0.06 |         - |         - |         - |   3.51 MB |        0.88 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **10**                | **1,840.4 ms** |  **2.82 ms** |  **1.47 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** |   **37.5 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 10                | 1,892.4 ms |  4.15 ms |  2.17 ms |  1.03 |    0.00 | 2000.0000 | 1000.0000 |         - |   34.8 MB |        0.93 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 10                | 1,910.3 ms | 29.51 ms | 17.56 ms |  1.04 |    0.01 | 2000.0000 | 1000.0000 |         - |  34.78 MB |        0.93 |
