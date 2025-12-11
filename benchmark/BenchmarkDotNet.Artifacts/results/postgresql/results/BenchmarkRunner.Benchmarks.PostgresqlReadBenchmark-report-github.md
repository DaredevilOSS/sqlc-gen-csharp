```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-PFIHHF : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | ConcurrentQueries | Mean     | Error   | StdDev  | Ratio | Gen0      | Gen1      | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |------------------ |---------:|--------:|--------:|------:|----------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **50**    | **10**                | **416.58 s** | **1.678 s** | **1.110 s** |  **1.00** | **2000.0000** | **1000.0000** | **1000.0000** |   **36.7 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 50    | 10                |  30.57 s | 0.362 s | 0.240 s |  0.07 | 4000.0000 | 1000.0000 |         - |  65.87 MB |        1.80 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 50    | 10                |  31.02 s | 0.474 s | 0.314 s |  0.07 | 4000.0000 | 1000.0000 |         - |  65.71 MB |        1.79 |
|                                             |       |                   |          |         |         |       |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **50**    | **50**                |  **35.92 s** | **0.343 s** | **0.227 s** |  **1.00** |         **-** |         **-** |         **-** |   **4.59 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 50    | 50                |  31.02 s | 0.316 s | 0.209 s |  0.86 | 4000.0000 | 1000.0000 |         - |  66.09 MB |       14.41 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 50    | 50                |  31.10 s | 0.163 s | 0.108 s |  0.87 | 4000.0000 | 1000.0000 |         - |  65.19 MB |       14.21 |
|                                             |       |                   |          |         |         |       |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **500**   | **10**                |  **34.57 s** | **0.260 s** | **0.172 s** |  **1.00** |         **-** |         **-** |         **-** |   **4.59 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 500   | 10                |  30.13 s | 0.440 s | 0.291 s |  0.87 | 4000.0000 | 1000.0000 |         - |  65.94 MB |       14.36 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 500   | 10                |  29.47 s | 0.431 s | 0.285 s |  0.85 | 4000.0000 | 1000.0000 |         - |  65.37 MB |       14.23 |
|                                             |       |                   |          |         |         |       |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **500**   | **50**                |  **35.93 s** | **0.353 s** | **0.233 s** |  **1.00** |         **-** |         **-** |         **-** |   **4.59 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 500   | 50                |  31.13 s | 0.113 s | 0.075 s |  0.87 | 4000.0000 | 1000.0000 |         - |  66.08 MB |       14.41 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 500   | 50                |  31.16 s | 0.227 s | 0.150 s |  0.87 | 4000.0000 | 1000.0000 |         - |  66.18 MB |       14.43 |
|                                             |       |                   |          |         |         |       |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10**                |  **36.25 s** | **0.289 s** | **0.191 s** |  **1.00** |         **-** |         **-** |         **-** |   **4.59 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10                |  31.27 s | 0.341 s | 0.226 s |  0.86 | 4000.0000 | 1000.0000 |         - |  66.19 MB |       14.43 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10                |  30.84 s | 0.109 s | 0.065 s |  0.85 | 4000.0000 | 1000.0000 |         - |  65.44 MB |       14.26 |
|                                             |       |                   |          |         |         |       |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **50**                |  **35.72 s** | **0.232 s** | **0.154 s** |  **1.00** |         **-** |         **-** |         **-** |   **4.58 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 50                |  31.15 s | 0.137 s | 0.091 s |  0.87 | 4000.0000 | 1000.0000 |         - |  66.02 MB |       14.40 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 50                |  31.38 s | 0.207 s | 0.137 s |  0.88 | 4000.0000 | 1000.0000 |         - |  65.37 MB |       14.26 |
