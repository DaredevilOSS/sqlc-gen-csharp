```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-BMUQQB : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  Categories=Read  

```
| Method                                      | Limit | Mean    | Error   | StdDev  | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |--------:|--------:|--------:|------:|--------:|-----------:|-----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **11.71 s** | **0.034 s** | **0.021 s** |  **1.00** |    **0.00** | **29000.0000** | **16000.0000** | **5000.0000** | **354.92 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 12.07 s | 0.153 s | 0.101 s |  1.03 |    0.01 | 48000.0000 | 21000.0000 | 7000.0000 | 341.09 MB |        0.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 11.94 s | 0.032 s | 0.019 s |  1.02 |    0.00 | 48000.0000 | 21000.0000 | 7000.0000 | 340.92 MB |        0.96 |
|                                             |       |         |         |         |       |         |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **11.69 s** | **0.046 s** | **0.024 s** |  **1.00** |    **0.00** | **29000.0000** | **16000.0000** | **5000.0000** | **354.93 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 12.03 s | 0.160 s | 0.106 s |  1.03 |    0.01 | 48000.0000 | 21000.0000 | 7000.0000 | 341.06 MB |        0.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 11.89 s | 0.047 s | 0.025 s |  1.02 |    0.00 | 48000.0000 | 21000.0000 | 7000.0000 | 340.96 MB |        0.96 |
|                                             |       |         |         |         |       |         |            |            |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **11.94 s** | **0.454 s** | **0.270 s** |  **1.00** |    **0.00** | **29000.0000** | **16000.0000** | **5000.0000** | **354.93 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 12.80 s | 0.713 s | 0.472 s |  1.08 |    0.05 | 48000.0000 | 21000.0000 | 7000.0000 | 341.16 MB |        0.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 12.55 s | 0.702 s | 0.464 s |  1.05 |    0.04 | 48000.0000 | 21000.0000 | 7000.0000 | 340.93 MB |        0.96 |
