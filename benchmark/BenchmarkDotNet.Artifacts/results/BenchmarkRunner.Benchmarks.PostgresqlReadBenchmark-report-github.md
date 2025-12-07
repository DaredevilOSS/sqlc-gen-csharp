```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-TNXKWK : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
|-------------------------------------------- |------ |----------:|---------:|---------:|------:|--------:|---------:|---------:|---------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **209.58 ms** | **2.459 ms** | **1.463 ms** |  **1.00** |    **0.00** | **333.3333** | **333.3333** | **333.3333** | **2343.08 KB** |       **1.000** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  |  21.59 ms | 0.190 ms | 0.113 ms |  0.10 |    0.00 |        - |        - |        - |   22.49 KB |       0.010 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |  21.54 ms | 0.214 ms | 0.141 ms |  0.10 |    0.00 |        - |        - |        - |   22.05 KB |       0.009 |
|                                             |       |           |          |          |       |         |          |          |          |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** |  **23.99 ms** | **1.177 ms** | **0.701 ms** |  **1.00** |    **0.00** |        **-** |        **-** |        **-** |    **5.17 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 |  21.52 ms | 0.241 ms | 0.143 ms |  0.90 |    0.03 |        - |        - |        - |   22.68 KB |        4.39 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 |  20.92 ms | 0.327 ms | 0.216 ms |  0.87 |    0.03 |        - |        - |        - |    21.7 KB |        4.20 |
|                                             |       |           |          |          |       |         |          |          |          |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** |  **23.53 ms** | **0.167 ms** | **0.100 ms** |  **1.00** |    **0.00** |        **-** |        **-** |        **-** |    **5.17 KB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 |  21.32 ms | 0.269 ms | 0.178 ms |  0.90 |    0.01 |        - |        - |        - |   22.26 KB |        4.30 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 |  21.60 ms | 0.175 ms | 0.115 ms |  0.92 |    0.01 |        - |        - |        - |   21.81 KB |        4.22 |
