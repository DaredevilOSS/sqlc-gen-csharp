```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-TNXKWK : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |---------:|--------:|--------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **117.4 ms** | **0.53 ms** | **0.28 ms** |  **1.00** |    **0.00** | **200.0000** | **200.0000** | **200.0000** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 117.8 ms | 0.94 ms | 0.62 ms |  1.00 |    0.01 | 400.0000 | 200.0000 |        - |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 120.0 ms | 3.01 ms | 1.79 ms |  1.02 |    0.02 | 250.0000 |        - |        - |   3.33 MB |        1.21 |
|                                             |       |          |         |         |       |         |          |          |          |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **117.9 ms** | **1.61 ms** | **0.96 ms** |  **1.00** |    **0.00** | **200.0000** | **200.0000** | **200.0000** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 118.1 ms | 0.97 ms | 0.64 ms |  1.00 |    0.01 | 250.0000 |        - |        - |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 118.6 ms | 1.64 ms | 1.09 ms |  1.00 |    0.01 | 250.0000 |        - |        - |   3.33 MB |        1.21 |
|                                             |       |          |         |         |       |         |          |          |          |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **117.8 ms** | **0.83 ms** | **0.49 ms** |  **1.00** |    **0.00** | **200.0000** | **200.0000** | **200.0000** |   **2.75 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 118.0 ms | 1.18 ms | 0.78 ms |  1.00 |    0.01 | 250.0000 |        - |        - |   3.33 MB |        1.21 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 117.6 ms | 0.88 ms | 0.52 ms |  1.00 |    0.00 | 400.0000 | 200.0000 |        - |   3.33 MB |        1.21 |
