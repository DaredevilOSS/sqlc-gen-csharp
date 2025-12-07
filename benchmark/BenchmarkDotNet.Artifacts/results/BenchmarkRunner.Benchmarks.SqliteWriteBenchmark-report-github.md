```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-VDLBPN : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=8  WarmupCount=2  
Categories=Write  

```
| Method                   | BatchSize | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|------------------------- |---------- |-----:|------:|------:|--------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **100**       |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore - AddOrderItems&#39; | 100       |   NA |    NA |     ? |       ? |           ? |
|                          |           |      |       |       |         |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200**       |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore - AddOrderItems&#39; | 200       |   NA |    NA |     ? |       ? |           ? |
|                          |           |      |       |       |         |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **500**       |   **NA** |    **NA** |     **?** |       **?** |           **?** |
| &#39;EFCore - AddOrderItems&#39; | 500       |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  SqliteWriteBenchmark.'SQLC - AddOrderItems': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [BatchSize=100]
  SqliteWriteBenchmark.'EFCore - AddOrderItems': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [BatchSize=100]
  SqliteWriteBenchmark.'SQLC - AddOrderItems': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [BatchSize=200]
  SqliteWriteBenchmark.'EFCore - AddOrderItems': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [BatchSize=200]
  SqliteWriteBenchmark.'SQLC - AddOrderItems': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [BatchSize=500]
  SqliteWriteBenchmark.'EFCore - AddOrderItems': Job-VDLBPN(Runtime=.NET 8.0, IterationCount=8, WarmupCount=2) [BatchSize=500]
