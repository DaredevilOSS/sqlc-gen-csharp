```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NVWEVN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean        | Error      | StdDev     | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated     | Alloc Ratio |
|------------------------- |---------- |------------:|-----------:|-----------:|------:|--------:|------------:|-----------:|----------:|--------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **1000**      |    **11.00 ms** |   **1.099 ms** |   **0.727 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |     **214.16 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 1000      |    87.67 ms |  24.251 ms |  16.040 ms |  7.98 |    1.41 |   2000.0000 |  1000.0000 |         - |   20650.91 KB |       96.43 |
|                          |           |             |            |            |       |         |             |            |           |               |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **10000**     |    **90.14 ms** |   **1.946 ms** |   **1.158 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |     **472.91 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 10000     |   393.80 ms |  45.986 ms |  27.366 ms |  4.37 |    0.34 |  24000.0000 |  6000.0000 | 2000.0000 |  202275.55 KB |      427.72 |
|                          |           |             |            |            |       |         |             |            |           |               |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **50000**     |   **461.89 ms** |  **15.254 ms** |  **10.089 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **2347.91 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 50000     | 2,140.85 ms | 188.790 ms | 124.873 ms |  4.64 |    0.28 | 116000.0000 | 16000.0000 | 2000.0000 | 1097310.06 KB |      467.36 |
