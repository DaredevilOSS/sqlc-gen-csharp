```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-BMUQQB : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  Categories=Read  

```
| Method                                      | Limit | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **2.074 μs** | **1.3828 μs** | **0.8229 μs** | **1.792 μs** |  **1.00** |    **0.00** |    **1872 B** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 1.714 μs | 0.6764 μs | 0.3538 μs | 1.667 μs |  0.97 |    0.45 |    1792 B |        0.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 1.891 μs | 1.3561 μs | 0.7092 μs | 1.688 μs |  1.01 |    0.43 |    1792 B |        0.96 |
|                                             |       |          |           |           |          |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **1.895 μs** | **1.1294 μs** | **0.7470 μs** | **1.916 μs** |  **1.00** |    **0.00** |    **1872 B** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 2.720 μs | 2.3335 μs | 1.5435 μs | 2.042 μs |  1.76 |    1.36 |    1792 B |        0.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 3.066 μs | 2.0087 μs | 1.3286 μs | 3.271 μs |  1.95 |    1.27 |    1792 B |        0.96 |
|                                             |       |          |           |           |          |       |         |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **2.005 μs** | **1.1535 μs** | **0.7630 μs** | **1.771 μs** |  **1.00** |    **0.00** |     **576 B** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 1.719 μs | 0.8812 μs | 0.4609 μs | 1.708 μs |  0.95 |    0.43 |    1792 B |        3.11 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 3.074 μs | 2.9087 μs | 1.7309 μs | 2.208 μs |  1.91 |    1.42 |    1792 B |        3.11 |
