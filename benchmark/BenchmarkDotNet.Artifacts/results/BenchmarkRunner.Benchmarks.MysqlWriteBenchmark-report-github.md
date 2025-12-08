```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-QPNTSL : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean        | Error      | StdDev     | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|------------------------- |---------- |------------:|-----------:|-----------:|------:|--------:|------------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **1000**      |    **11.10 ms** |   **4.309 ms** |   **2.564 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **1.32 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 1000      |   148.53 ms |  45.042 ms |  29.793 ms | 14.06 |    2.63 |   2000.0000 |  1000.0000 |         - |   21.96 MB |       16.65 |
|                          |           |             |            |            |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **10000**     |    **65.09 ms** |  **10.445 ms** |   **6.215 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **3.22 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 10000     |   827.94 ms |  55.793 ms |  36.903 ms | 12.79 |    1.09 |  27000.0000 |  6000.0000 | 2000.0000 |  216.51 MB |       67.15 |
|                          |           |             |            |            |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **50000**     |   **289.06 ms** |  **53.912 ms** |  **35.659 ms** |  **1.00** |    **0.00** |   **1000.0000** |          **-** |         **-** |   **12.32 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 50000     | 4,320.53 ms | 306.867 ms | 202.974 ms | 15.18 |    2.21 | 136000.0000 | 19000.0000 | 3000.0000 | 1154.15 MB |       93.69 |
