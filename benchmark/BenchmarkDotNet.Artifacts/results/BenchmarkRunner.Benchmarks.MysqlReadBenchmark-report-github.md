```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-HATOLI : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Read  

```
| Method                                      | Limit | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2     | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |---------:|---------:|---------:|------:|--------:|----------:|----------:|---------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **12.27 ms** | **0.846 ms** | **0.442 ms** |  **1.00** |    **0.00** |  **687.5000** |  **671.8750** | **312.5000** |   **2.74 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 12.82 ms | 0.835 ms | 0.437 ms |  1.05 |    0.02 |  359.3750 |  156.2500 |        - |   2.92 MB |        1.07 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 13.49 ms | 1.045 ms | 0.547 ms |  1.10 |    0.07 |  359.3750 |  156.2500 |        - |   2.92 MB |        1.07 |
|                                             |       |          |          |          |       |         |           |           |          |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **25.61 ms** | **3.637 ms** | **1.615 ms** |  **1.00** |    **0.00** | **1615.3846** | **1615.3846** | **461.5385** |   **5.46 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 26.78 ms | 1.703 ms | 0.891 ms |  1.05 |    0.07 |  781.2500 |  375.0000 | 187.5000 |   5.81 MB |        1.06 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 26.43 ms | 1.080 ms | 0.479 ms |  1.04 |    0.06 |  781.2500 |  375.0000 | 187.5000 |    5.8 MB |        1.06 |
|                                             |       |          |          |          |       |         |           |           |          |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **57.04 ms** | **3.830 ms** | **2.003 ms** |  **1.00** |    **0.00** | **1800.0000** | **1800.0000** | **800.0000** |  **10.91 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 52.74 ms | 3.330 ms | 1.479 ms |  0.93 |    0.03 | 1600.0000 |  800.0000 | 400.0000 |  11.57 MB |        1.06 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 51.36 ms | 4.815 ms | 2.138 ms |  0.91 |    0.06 | 1600.0000 |  800.0000 | 400.0000 |  11.56 MB |        1.06 |
