```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-VDLBPN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean      | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |----------:|---------:|---------:|------:|--------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **2000**  | **197.02 ms** | **1.077 ms** | **0.563 ms** |  **1.00** |    **0.00** | **348.86 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 2000  |  21.33 ms | 0.167 ms | 0.074 ms |  0.11 |    0.00 |  22.66 KB |        0.06 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 2000  |  20.82 ms | 1.280 ms | 0.568 ms |  0.11 |    0.00 |  21.49 KB |        0.06 |
|                                             |       |           |          |          |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  |  **23.05 ms** | **0.112 ms** | **0.050 ms** |  **1.00** |    **0.00** |   **5.06 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |  20.95 ms | 0.240 ms | 0.106 ms |  0.91 |    0.00 |  23.12 KB |        4.57 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |  21.40 ms | 0.335 ms | 0.149 ms |  0.93 |    0.01 |  21.67 KB |        4.28 |
|                                             |       |           |          |          |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** |  **23.14 ms** | **0.654 ms** | **0.291 ms** |  **1.00** |    **0.00** |   **5.29 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 |  23.27 ms | 2.716 ms | 1.420 ms |  0.99 |    0.06 |  22.16 KB |        4.19 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 |  21.85 ms | 0.314 ms | 0.164 ms |  0.94 |    0.02 |  21.46 KB |        4.06 |
