```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YQWXVR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | Mean     | Error    | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |---------:|---------:|--------:|------:|--------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **119.1 ms** |  **2.49 ms** | **1.64 ms** |  **1.00** |    **0.00** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 129.3 ms |  4.79 ms | 2.85 ms |  1.08 |    0.03 |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 129.0 ms | 12.22 ms | 8.09 ms |  1.08 |    0.06 |   3.33 MB |        1.21 |
|                                             |       |          |          |         |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **119.2 ms** |  **2.66 ms** | **1.39 ms** |  **1.00** |    **0.00** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 126.1 ms |  7.91 ms | 5.23 ms |  1.07 |    0.04 |   3.34 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 126.3 ms |  6.83 ms | 4.52 ms |  1.07 |    0.03 |   3.33 MB |        1.21 |
|                                             |       |          |          |         |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **120.0 ms** |  **2.58 ms** | **1.71 ms** |  **1.00** |    **0.00** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 126.0 ms |  8.01 ms | 5.30 ms |  1.05 |    0.04 |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 126.0 ms |  8.14 ms | 5.39 ms |  1.05 |    0.04 |   3.33 MB |        1.21 |
