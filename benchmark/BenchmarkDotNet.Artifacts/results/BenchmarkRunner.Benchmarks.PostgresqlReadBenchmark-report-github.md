```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YURDSC : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean      | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |----------:|---------:|---------:|------:|--------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **2000**  | **203.14 ms** | **5.403 ms** | **2.399 ms** |  **1.00** |    **0.00** | **692.16 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 2000  |  21.24 ms | 0.175 ms | 0.078 ms |  0.10 |    0.00 |  22.35 KB |        0.03 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 2000  |  21.32 ms | 0.207 ms | 0.108 ms |  0.10 |    0.00 |  22.13 KB |        0.03 |
|                                             |       |           |          |          |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  |  **23.66 ms** | **0.203 ms** | **0.106 ms** |  **1.00** |    **0.00** |   **5.17 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |  22.30 ms | 1.182 ms | 0.525 ms |  0.94 |    0.02 |  22.21 KB |        4.29 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |  21.04 ms | 0.499 ms | 0.222 ms |  0.89 |    0.01 |     22 KB |        4.25 |
|                                             |       |           |          |          |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** |  **24.61 ms** | **1.265 ms** | **0.662 ms** |  **1.00** |    **0.00** |   **5.21 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 |  21.27 ms | 0.190 ms | 0.099 ms |  0.86 |    0.02 |  22.37 KB |        4.29 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 |  21.32 ms | 0.350 ms | 0.155 ms |  0.87 |    0.03 |  22.01 KB |        4.22 |
