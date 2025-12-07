```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-VDLBPN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |---------:|---------:|---------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **2000**  | **14.19 ms** | **1.777 ms** | **0.789 ms** |  **1.00** |    **0.00** | **875.0000** | **875.0000** | **328.1250** |   **2.74 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 2000  | 14.46 ms | 1.755 ms | 0.779 ms |  1.02 |    0.09 | 359.3750 | 125.0000 |  15.6250 |   2.92 MB |        1.07 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 2000  | 14.10 ms | 0.642 ms | 0.336 ms |  1.00 |    0.06 | 359.3750 | 156.2500 |        - |   2.92 MB |        1.06 |
|                                             |       |          |          |          |       |         |          |          |          |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **11.99 ms** | **0.912 ms** | **0.477 ms** |  **1.00** |    **0.00** | **796.8750** | **796.8750** | **328.1250** |   **2.74 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 14.17 ms | 2.081 ms | 1.088 ms |  1.18 |    0.11 | 359.3750 | 156.2500 |        - |   2.92 MB |        1.07 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 13.67 ms | 1.357 ms | 0.710 ms |  1.14 |    0.07 | 359.3750 | 156.2500 |        - |   2.92 MB |        1.07 |
|                                             |       |          |          |          |       |         |          |          |          |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **13.39 ms** | **1.368 ms** | **0.716 ms** |  **1.00** |    **0.00** | **875.0000** | **875.0000** | **328.1250** |   **2.74 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 16.60 ms | 2.981 ms | 1.559 ms |  1.24 |    0.07 | 359.3750 | 156.2500 |        - |   2.92 MB |        1.07 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 14.44 ms | 2.600 ms | 1.360 ms |  1.08 |    0.06 | 359.3750 | 156.2500 |        - |   2.92 MB |        1.07 |
