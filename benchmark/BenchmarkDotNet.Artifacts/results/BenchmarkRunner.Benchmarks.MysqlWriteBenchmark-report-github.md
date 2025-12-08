```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YQWXVR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean        | Error      | StdDev     | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|------------------------- |---------- |------------:|-----------:|-----------:|------:|--------:|------------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **1000**      |    **11.66 ms** |   **4.377 ms** |   **2.604 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **1.29 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 1000      |   157.50 ms |  37.330 ms |  22.214 ms | 14.12 |    3.37 |   2000.0000 |  1000.0000 |         - |   21.78 MB |       16.94 |
|                          |           |             |            |            |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **10000**     |    **65.39 ms** |   **6.938 ms** |   **4.589 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **3.22 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 10000     |   811.12 ms |  59.715 ms |  35.535 ms | 12.49 |    0.91 |  27000.0000 |  6000.0000 | 2000.0000 |  215.72 MB |       66.90 |
|                          |           |             |            |            |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **50000**     |   **257.16 ms** |  **39.840 ms** |  **26.352 ms** |  **1.00** |    **0.00** |   **1000.0000** |          **-** |         **-** |   **12.29 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 50000     | 4,349.57 ms | 276.442 ms | 164.506 ms | 17.22 |    1.96 | 135000.0000 | 19000.0000 | 3000.0000 | 1153.17 MB |       93.85 |
