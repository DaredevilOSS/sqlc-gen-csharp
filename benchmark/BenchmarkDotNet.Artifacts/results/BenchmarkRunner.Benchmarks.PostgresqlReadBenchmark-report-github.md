```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YQWXVR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | Mean      | Error    | StdDev   | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------------------------------------- |------ |----------:|---------:|---------:|------:|--------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **205.60 ms** | **1.872 ms** | **0.979 ms** |  **1.00** |    **0.00** | **2343.38 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |  24.44 ms | 0.749 ms | 0.495 ms |  0.12 |    0.00 |   25.97 KB |        0.01 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |  24.69 ms | 0.863 ms | 0.571 ms |  0.12 |    0.00 |   25.36 KB |        0.01 |
|                                             |       |           |          |          |       |         |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** |  **26.99 ms** | **1.718 ms** | **1.136 ms** |  **1.00** |    **0.00** |    **7.33 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 |  24.80 ms | 0.855 ms | 0.566 ms |  0.92 |    0.04 |   25.53 KB |        3.48 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 |  24.92 ms | 0.975 ms | 0.580 ms |  0.92 |    0.05 |   25.13 KB |        3.43 |
|                                             |       |           |          |          |       |         |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** |  **26.44 ms** | **0.774 ms** | **0.461 ms** |  **1.00** |    **0.00** |    **7.33 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 |  24.54 ms | 0.695 ms | 0.413 ms |  0.93 |    0.01 |   29.29 KB |        4.00 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 |  23.95 ms | 0.790 ms | 0.470 ms |  0.91 |    0.03 |    25.5 KB |        3.48 |
