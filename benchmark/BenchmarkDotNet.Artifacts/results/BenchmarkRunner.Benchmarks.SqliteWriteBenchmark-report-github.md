```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YQWXVR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean      | Error      | StdDev     | Ratio | RatioSD | Gen0      | Allocated   | Alloc Ratio |
|------------------------- |---------- |----------:|-----------:|-----------:|------:|--------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **100**       |  **1.322 ms** |  **0.1745 ms** |  **0.0913 ms** |  **1.00** |    **0.00** |         **-** |   **256.06 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 100       |  6.542 ms |  1.2274 ms |  0.7304 ms |  4.91 |    0.78 |         - |  2134.71 KB |        8.34 |
|                          |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200**       |  **3.361 ms** |  **0.2056 ms** |  **0.1223 ms** |  **1.00** |    **0.00** |         **-** |   **516.22 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 200       | 12.173 ms |  3.2938 ms |  1.7227 ms |  3.61 |    0.51 |         - |  4261.75 KB |        8.26 |
|                          |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **500**       | **16.393 ms** |  **0.3569 ms** |  **0.2124 ms** |  **1.00** |    **0.00** |         **-** |  **1256.62 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 500       | 45.271 ms | 18.6498 ms | 12.3357 ms |  2.69 |    0.77 | 1000.0000 | 10629.32 KB |        8.46 |
