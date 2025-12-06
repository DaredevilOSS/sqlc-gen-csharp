```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-OHJRPL : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  

```
| Method                   | Categories       | BatchSize | Mean      | Error      | StdDev     | Ratio | RatioSD | Gen0      | Allocated   | Alloc Ratio |
|------------------------- |----------------- |---------- |----------:|-----------:|-----------:|------:|--------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **100**       |  **1.279 ms** |  **0.0642 ms** |  **0.0336 ms** |  **1.00** |    **0.00** |         **-** |   **256.06 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 100       |  6.466 ms |  1.1820 ms |  0.7034 ms |  4.96 |    0.59 |         - |   2201.9 KB |        8.60 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **200**       |  **3.324 ms** |  **0.0839 ms** |  **0.0499 ms** |  **1.00** |    **0.00** |         **-** |   **516.23 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 200       | 14.901 ms |  7.6478 ms |  5.0586 ms |  4.33 |    1.51 |         - |  4396.13 KB |        8.52 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **500**       | **16.410 ms** |  **0.7199 ms** |  **0.4762 ms** |  **1.00** |    **0.00** |         **-** |  **1256.63 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 500       | 53.205 ms | 19.4323 ms | 12.8533 ms |  3.25 |    0.80 | 1000.0000 | 10965.26 KB |        8.73 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **100**       |  **1.003 ms** |  **0.1148 ms** |  **0.0683 ms** |  **1.00** |    **0.00** |         **-** |   **220.72 KB** |        **1.00** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 100       |  7.273 ms |  1.5422 ms |  0.9177 ms |  7.29 |    1.10 |         - |  2335.98 KB |       10.58 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **200**       |  **2.328 ms** |  **0.0647 ms** |  **0.0338 ms** |  **1.00** |    **0.00** |         **-** |   **442.61 KB** |        **1.00** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 200       | 16.132 ms |  8.0004 ms |  5.2917 ms |  6.96 |    2.62 |         - |  4666.08 KB |       10.54 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **500**       | **11.177 ms** |  **0.3730 ms** |  **0.2220 ms** |  **1.00** |    **0.00** |         **-** |  **1068.16 KB** |        **1.00** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 500       | 39.528 ms |  7.3799 ms |  4.3917 ms |  3.53 |    0.36 | 1000.0000 | 11632.74 KB |       10.89 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **100**       |  **1.651 ms** |  **0.0672 ms** |  **0.0400 ms** |  **1.00** |    **0.00** |         **-** |   **298.88 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 100       |  6.578 ms |  1.2957 ms |  0.6777 ms |  3.97 |    0.40 |         - |   2383.5 KB |        7.97 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **200**       |  **4.647 ms** |  **0.1589 ms** |  **0.0946 ms** |  **1.00** |    **0.00** |         **-** |   **603.66 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 200       | 17.102 ms |  8.6015 ms |  5.6894 ms |  3.51 |    1.20 |         - |  4761.44 KB |        7.89 |
|                          |                  |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **500**       | **25.256 ms** |  **1.4187 ms** |  **0.9384 ms** |  **1.00** |    **0.00** |         **-** |  **1755.19 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 500       | 36.806 ms |  9.9994 ms |  6.6140 ms |  1.46 |    0.27 | 1000.0000 | 11871.16 KB |        6.76 |
