```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-YQWXVR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | BatchSize | Mean        | Error      | StdDev    | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated     | Alloc Ratio |
|------------------------- |---------- |------------:|-----------:|----------:|------:|--------:|------------:|-----------:|----------:|--------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **1000**      |    **10.85 ms** |   **1.255 ms** |  **0.747 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |     **217.54 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 1000      |    84.14 ms |  49.606 ms | 32.811 ms |  8.10 |    2.69 |   2000.0000 |  1000.0000 |         - |   20649.34 KB |       94.92 |
|                          |           |             |            |           |       |         |             |            |           |               |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **10000**     |    **88.05 ms** |   **1.826 ms** |  **1.208 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |     **472.73 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 10000     |   392.02 ms |  17.168 ms | 10.216 ms |  4.45 |    0.16 |  24000.0000 |  6000.0000 | 2000.0000 |  202271.43 KB |      427.88 |
|                          |           |             |            |           |       |         |             |            |           |               |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **50000**     |   **446.02 ms** |  **10.278 ms** |  **6.798 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **2348.52 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 50000     | 2,239.60 ms | 128.430 ms | 84.949 ms |  5.02 |    0.18 | 117000.0000 | 19000.0000 | 3000.0000 | 1097311.15 KB |      467.23 |
