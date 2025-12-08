```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-BMUQQB : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  Categories=Write  

```
| Method                   | BatchSize | Mean      | Error      | StdDev     | Ratio | RatioSD | Gen0      | Allocated   | Alloc Ratio |
|------------------------- |---------- |----------:|-----------:|-----------:|------:|--------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **100**       |  **1.253 ms** |  **0.0745 ms** |  **0.0390 ms** |  **1.00** |    **0.00** |         **-** |   **256.06 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 100       |  6.476 ms |  1.3908 ms |  0.8276 ms |  5.10 |    0.79 |         - |   2201.9 KB |        8.60 |
|                          |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200**       |  **3.280 ms** |  **0.1038 ms** |  **0.0543 ms** |  **1.00** |    **0.00** |         **-** |   **516.23 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 200       | 15.986 ms |  8.3981 ms |  5.5548 ms |  4.20 |    1.03 |         - |  4396.13 KB |        8.52 |
|                          |           |           |            |            |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **500**       | **16.550 ms** |  **0.6484 ms** |  **0.3859 ms** |  **1.00** |    **0.00** |         **-** |  **1350.37 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 500       | 43.436 ms | 17.0881 ms | 11.3027 ms |  2.53 |    0.65 | 1000.0000 | 10965.26 KB |        8.12 |
