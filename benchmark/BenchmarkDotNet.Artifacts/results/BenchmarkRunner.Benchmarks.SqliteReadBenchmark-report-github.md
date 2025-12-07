```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-JZDQJD : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|-------------------------------------------- |------ |---------:|--------:|--------:|------:|--------:|---------:|---------:|---------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **500**   | **107.6 ms** | **0.99 ms** | **0.66 ms** |  **1.00** |    **0.00** |        **-** |        **-** |        **-** |  **236.12 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 500   | 107.4 ms | 1.09 ms | 0.72 ms |  1.00 |    0.01 |        - |        - |        - |  364.64 KB |        1.54 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 500   | 108.1 ms | 3.21 ms | 1.68 ms |  1.00 |    0.02 |        - |        - |        - |     363 KB |        1.54 |
|                                             |       |          |         |         |       |         |          |          |          |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **2000**  | **111.1 ms** | **0.76 ms** | **0.45 ms** |  **1.00** |    **0.00** |        **-** |        **-** |        **-** |  **935.69 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 2000  | 110.5 ms | 0.75 ms | 0.45 ms |  1.00 |    0.01 |        - |        - |        - | 1363.88 KB |        1.46 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 2000  | 111.2 ms | 1.46 ms | 0.76 ms |  1.00 |    0.01 |        - |        - |        - | 1363.17 KB |        1.46 |
|                                             |       |          |         |         |       |         |          |          |          |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **117.7 ms** | **1.42 ms** | **0.84 ms** |  **1.00** |    **0.00** | **200.0000** | **200.0000** | **200.0000** | **2815.41 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 125.3 ms | 7.99 ms | 5.29 ms |  1.07 |    0.04 | 400.0000 |        - |        - | 3410.82 KB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 125.8 ms | 7.74 ms | 4.61 ms |  1.07 |    0.04 | 250.0000 |        - |        - | 3410.93 KB |        1.21 |
