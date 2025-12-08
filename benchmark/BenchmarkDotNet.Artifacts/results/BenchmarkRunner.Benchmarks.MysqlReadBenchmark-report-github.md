```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-KOIWQR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=8  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | Mean      | Error      | StdDev     | Median   | Ratio | RatioSD | Gen0      | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |----------:|-----------:|-----------:|---------:|------:|--------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10.467 ms** |  **0.6702 ms** |  **0.3505 ms** | **10.42 ms** |  **1.00** |    **0.00** |         **-** |   **2.73 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |  9.912 ms |  0.5659 ms |  0.2960 ms | 10.01 ms |  0.95 |    0.02 |         - |   3.01 MB |        1.11 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10.363 ms |  2.3849 ms |  1.2473 ms | 10.67 ms |  0.99 |    0.15 |         - |      3 MB |        1.10 |
|                                             |       |           |            |            |          |       |         |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **19.610 ms** |  **1.8452 ms** |  **0.9651 ms** | **19.53 ms** |  **1.00** |    **0.00** |         **-** |   **5.41 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 15.459 ms |  1.2221 ms |  0.6392 ms | 15.53 ms |  0.79 |    0.06 |         - |   5.87 MB |        1.08 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 21.082 ms |  3.8873 ms |  2.0332 ms | 20.69 ms |  1.08 |    0.13 |         - |   5.87 MB |        1.09 |
|                                             |       |           |            |            |          |       |         |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **38.896 ms** | **23.7991 ms** | **12.4474 ms** | **30.64 ms** |  **1.00** |    **0.00** |         **-** |  **10.95 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 33.703 ms |  2.6060 ms |  1.1571 ms | 33.35 ms |  0.97 |    0.24 | 1000.0000 |  11.59 MB |        1.06 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 51.777 ms |  3.1317 ms |  1.3905 ms | 51.31 ms |  1.49 |    0.39 | 1000.0000 |  11.49 MB |        1.05 |
