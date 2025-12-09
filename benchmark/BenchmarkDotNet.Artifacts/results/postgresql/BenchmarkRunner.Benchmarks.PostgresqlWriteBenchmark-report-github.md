```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-BMUQQB : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  Categories=Write  

```
| Method                   | BatchSize | Mean        | Error      | StdDev     | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated     | Alloc Ratio |
|------------------------- |---------- |------------:|-----------:|-----------:|------:|--------:|------------:|-----------:|----------:|--------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **1000**      |    **10.39 ms** |   **0.784 ms** |   **0.466 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |     **212.29 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 1000      |    93.17 ms |  54.513 ms |  36.057 ms |  9.62 |    3.36 |   2000.0000 |  1000.0000 |         - |   20991.53 KB |       98.88 |
|                          |           |             |            |            |       |         |             |            |           |               |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **10000**     |    **88.13 ms** |   **2.294 ms** |   **1.200 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |     **472.61 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 10000     |   401.59 ms |  34.856 ms |  20.742 ms |  4.51 |    0.16 |  25000.0000 |  6000.0000 | 2000.0000 |  209619.81 KB |      443.54 |
|                          |           |             |            |            |       |         |             |            |           |               |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **50000**     |   **440.21 ms** |  **10.816 ms** |   **7.154 ms** |  **1.00** |    **0.00** |           **-** |          **-** |         **-** |    **2347.61 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 50000     | 2,222.82 ms | 225.492 ms | 149.149 ms |  5.05 |    0.36 | 121000.0000 | 16000.0000 | 3000.0000 | 1046836.47 KB |      445.92 |
