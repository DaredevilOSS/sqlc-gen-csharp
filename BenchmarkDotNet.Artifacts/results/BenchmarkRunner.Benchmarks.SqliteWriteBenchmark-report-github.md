```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-QWPDYV : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  

```
| Method                   | Categories       | BatchSize | Mean      | Error      | StdDev    | Median    | Ratio | RatioSD | Gen0      | Allocated   | Alloc Ratio |
|------------------------- |----------------- |---------- |----------:|-----------:|----------:|----------:|------:|--------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **100**       |  **1.830 ms** |  **0.0869 ms** | **0.0517 ms** |  **1.832 ms** |  **1.00** |    **0.00** |         **-** |   **304.64 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 100       |  5.783 ms |  1.3300 ms | 0.7914 ms |  5.681 ms |  3.17 |    0.49 |         - |     1873 KB |        6.15 |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **200**       |  **5.161 ms** |  **0.2002 ms** | **0.1191 ms** |  **5.118 ms** |  **1.00** |    **0.00** |         **-** |   **614.32 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 200       | 13.362 ms |  5.9198 ms | 3.9156 ms | 13.653 ms |  2.56 |    0.82 |         - |  3738.32 KB |        6.09 |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **500**       | **26.748 ms** |  **0.5167 ms** | **0.3418 ms** | **26.702 ms** |  **1.00** |    **0.00** |         **-** |  **1780.48 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 500       | 40.154 ms | 11.3167 ms | 7.4853 ms | 39.692 ms |  1.50 |    0.28 | 1000.0000 |  9320.73 KB |        5.23 |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **100**       |        **NA** |         **NA** |        **NA** |        **NA** |     **?** |       **?** |        **NA** |          **NA** |           **?** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 100       |        NA |         NA |        NA |        NA |     ? |       ? |        NA |          NA |           ? |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **200**       |        **NA** |         **NA** |        **NA** |        **NA** |     **?** |       **?** |        **NA** |          **NA** |           **?** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 200       |        NA |         NA |        NA |        NA |     ? |       ? |        NA |          NA |           ? |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **500**       |        **NA** |         **NA** |        **NA** |        **NA** |     **?** |       **?** |        **NA** |          **NA** |           **?** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 500       |        NA |         NA |        NA |        NA |     ? |       ? |        NA |          NA |           ? |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **100**       |  **1.607 ms** |  **0.0485 ms** | **0.0254 ms** |  **1.610 ms** |  **1.00** |    **0.00** |         **-** |   **298.88 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 100       |  6.762 ms |  1.3223 ms | 0.7869 ms |  6.695 ms |  4.10 |    0.40 |         - |  2383.54 KB |        7.97 |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **200**       |  **4.636 ms** |  **0.1366 ms** | **0.0813 ms** |  **4.613 ms** |  **1.00** |    **0.00** |         **-** |   **603.66 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 200       | 16.167 ms | 11.6998 ms | 7.7387 ms | 11.984 ms |  3.62 |    1.65 |         - |  4761.46 KB |        7.89 |
|                          |                  |           |           |            |           |           |       |         |           |             |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **500**       | **24.585 ms** |  **1.0643 ms** | **0.7040 ms** | **24.220 ms** |  **1.00** |    **0.00** |         **-** |  **1755.19 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 500       | 43.667 ms |  9.6761 ms | 6.4001 ms | 43.411 ms |  1.78 |    0.28 | 1000.0000 | 11871.17 KB |        6.76 |

Benchmarks with issues:
  SqliteWriteBenchmark.'SQLC - AddOrders': Job-QWPDYV(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=100]
  SqliteWriteBenchmark.'EFCore - AddOrders': Job-QWPDYV(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=100]
  SqliteWriteBenchmark.'SQLC - AddOrders': Job-QWPDYV(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=200]
  SqliteWriteBenchmark.'EFCore - AddOrders': Job-QWPDYV(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=200]
  SqliteWriteBenchmark.'SQLC - AddOrders': Job-QWPDYV(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=500]
  SqliteWriteBenchmark.'EFCore - AddOrders': Job-QWPDYV(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=500]
