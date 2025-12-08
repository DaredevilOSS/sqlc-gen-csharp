```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YVFTVT : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=8  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0      | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |----------:|----------:|----------:|------:|--------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  |  **9.498 ms** | **0.4534 ms** | **0.2013 ms** |  **1.00** |    **0.00** |         **-** |   **2.81 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10.387 ms | 1.4703 ms | 0.7690 ms |  1.11 |    0.05 |         - |   2.96 MB |        1.05 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  |  9.708 ms | 2.0297 ms | 1.0615 ms |  1.02 |    0.12 |         - |      3 MB |        1.06 |
|                                             |       |           |           |           |       |         |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **17.683 ms** | **6.8399 ms** | **3.5774 ms** |  **1.00** |    **0.00** |         **-** |   **5.53 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 15.200 ms | 0.8059 ms | 0.3578 ms |  0.92 |    0.15 |         - |   5.86 MB |        1.06 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 24.200 ms | 2.6178 ms | 1.3691 ms |  1.42 |    0.29 |         - |    5.8 MB |        1.05 |
|                                             |       |           |           |           |       |         |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **31.957 ms** | **7.4290 ms** | **3.2985 ms** |  **1.00** |    **0.00** |         **-** |  **10.94 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 36.138 ms | 5.8442 ms | 2.5949 ms |  1.15 |    0.18 | 1000.0000 |  11.65 MB |        1.07 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 35.142 ms | 2.9059 ms | 1.0363 ms |  1.11 |    0.16 | 1000.0000 |  11.62 MB |        1.06 |
