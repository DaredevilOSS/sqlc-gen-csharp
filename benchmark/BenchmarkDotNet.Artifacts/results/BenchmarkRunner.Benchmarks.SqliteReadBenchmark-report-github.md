```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NVWEVN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | Mean     | Error   | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |---------:|--------:|--------:|------:|--------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **118.8 ms** | **2.74 ms** | **1.81 ms** |  **1.00** |    **0.00** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 125.8 ms | 7.87 ms | 5.21 ms |  1.06 |    0.04 |   3.34 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 124.4 ms | 9.41 ms | 6.23 ms |  1.05 |    0.05 |   3.33 MB |        1.21 |
|                                             |       |          |         |         |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **118.3 ms** | **1.60 ms** | **0.95 ms** |  **1.00** |    **0.00** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 125.1 ms | 6.04 ms | 3.99 ms |  1.06 |    0.03 |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 124.3 ms | 7.21 ms | 4.77 ms |  1.05 |    0.04 |   3.33 MB |        1.21 |
|                                             |       |          |         |         |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **117.9 ms** | **2.33 ms** | **1.54 ms** |  **1.00** |    **0.00** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 124.6 ms | 8.86 ms | 5.86 ms |  1.06 |    0.05 |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 125.5 ms | 9.21 ms | 6.09 ms |  1.06 |    0.05 |   3.33 MB |        1.21 |
