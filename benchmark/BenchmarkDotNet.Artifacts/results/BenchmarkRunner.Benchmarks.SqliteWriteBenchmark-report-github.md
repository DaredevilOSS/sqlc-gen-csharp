```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NVWEVN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean      | Error      | StdDev     | Ratio | RatioSD | Gen0      | Allocated   | Alloc Ratio |
|------------------------- |---------- |----------:|-----------:|-----------:|------:|--------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **100**       |  **1.230 ms** |  **0.0960 ms** |  **0.0635 ms** |  **1.00** |    **0.00** |         **-** |   **256.06 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 100       |  6.378 ms |  1.2231 ms |  0.7279 ms |  5.17 |    0.66 |         - |  2134.71 KB |        8.34 |
|                          |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200**       |  **3.334 ms** |  **0.1172 ms** |  **0.0613 ms** |  **1.00** |    **0.00** |         **-** |   **516.23 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 200       | 11.677 ms |  2.8591 ms |  1.4954 ms |  3.50 |    0.41 |         - |  4261.75 KB |        8.26 |
|                          |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **500**       | **16.205 ms** |  **0.3645 ms** |  **0.1906 ms** |  **1.00** |    **0.00** |         **-** |  **1256.63 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 500       | 41.785 ms | 15.5485 ms | 10.2844 ms |  2.50 |    0.71 | 1000.0000 | 10629.32 KB |        8.46 |
