```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NVWEVN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean        | Error      | StdDev     | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|------------------------- |---------- |------------:|-----------:|-----------:|------:|--------:|------------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **1000**      |    **10.08 ms** |   **3.001 ms** |   **1.786 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **1.29 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 1000      |   122.39 ms |  15.212 ms |  10.062 ms | 12.45 |    1.81 |   2000.0000 |  1000.0000 |         - |   21.84 MB |       16.97 |
|                          |           |             |            |            |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **10000**     |    **61.56 ms** |   **4.608 ms** |   **2.410 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **3.22 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 10000     |   827.02 ms |  51.118 ms |  33.811 ms | 13.51 |    0.87 |  27000.0000 |  6000.0000 | 2000.0000 |  215.92 MB |       66.97 |
|                          |           |             |            |            |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **50000**     |   **235.08 ms** |   **9.038 ms** |   **5.978 ms** |  **1.00** |    **0.00** |   **1000.0000** |          **-** |         **-** |   **12.29 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 50000     | 4,504.19 ms | 489.666 ms | 291.392 ms | 19.19 |    1.15 | 136000.0000 | 19000.0000 | 3000.0000 | 1156.02 MB |       94.08 |
