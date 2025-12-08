```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NVWEVN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | Mean      | Error    | StdDev   | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------------------------------------- |------ |----------:|---------:|---------:|------:|--------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **199.95 ms** | **2.720 ms** | **1.799 ms** |  **1.00** |    **0.00** | **2343.92 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |  24.33 ms | 0.801 ms | 0.530 ms |  0.12 |    0.00 |   25.83 KB |        0.01 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |  24.50 ms | 0.849 ms | 0.562 ms |  0.12 |    0.00 |    28.4 KB |        0.01 |
|                                             |       |           |          |          |       |         |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** |  **26.17 ms** | **0.968 ms** | **0.576 ms** |  **1.00** |    **0.00** |    **7.33 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 |  23.81 ms | 0.773 ms | 0.511 ms |  0.91 |    0.03 |   25.84 KB |        3.53 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 |  24.34 ms | 1.225 ms | 0.810 ms |  0.93 |    0.04 |   25.22 KB |        3.44 |
|                                             |       |           |          |          |       |         |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** |  **26.33 ms** | **0.806 ms** | **0.533 ms** |  **1.00** |    **0.00** |    **7.02 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 |  24.24 ms | 0.643 ms | 0.382 ms |  0.92 |    0.01 |   25.62 KB |        3.65 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 |  24.49 ms | 1.097 ms | 0.726 ms |  0.93 |    0.03 |   27.95 KB |        3.98 |
