```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YURDSC : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|-------------------------------------------- |------ |----------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **2000**  |  **5.079 ms** | **0.3119 ms** | **0.1631 ms** |  **1.00** |    **0.00** | **164.0625** | **156.2500** |  **70.3125** |  **909.95 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 2000  |  5.703 ms | 0.3457 ms | 0.1535 ms |  1.13 |    0.03 | 140.6250 |  54.6875 |        - | 1202.77 KB |        1.32 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 2000  |  5.744 ms | 0.4685 ms | 0.2080 ms |  1.14 |    0.04 | 140.6250 |   7.8125 |        - | 1200.63 KB |        1.32 |
|                                             |       |           |           |           |       |         |          |          |          |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **11.826 ms** | **0.4759 ms** | **0.2489 ms** |  **1.00** |    **0.00** | **875.0000** | **875.0000** | **328.1250** | **2803.25 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 13.649 ms | 1.6713 ms | 0.8741 ms |  1.15 |    0.08 | 359.3750 | 156.2500 |        - | 2989.07 KB |        1.07 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 12.283 ms | 0.9034 ms | 0.4725 ms |  1.04 |    0.05 | 359.3750 | 156.2500 |        - | 2988.28 KB |        1.07 |
|                                             |       |           |           |           |       |         |          |          |          |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **23.626 ms** | **0.7159 ms** | **0.3179 ms** |  **1.00** |    **0.00** | **968.7500** | **937.5000** | **593.7500** | **5609.38 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 29.880 ms | 3.3924 ms | 1.7743 ms |  1.26 |    0.07 | 781.2500 | 375.0000 | 187.5000 | 5944.67 KB |        1.06 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 25.265 ms | 1.6904 ms | 0.8841 ms |  1.07 |    0.05 | 781.2500 | 375.0000 | 187.5000 | 5945.48 KB |        1.06 |
