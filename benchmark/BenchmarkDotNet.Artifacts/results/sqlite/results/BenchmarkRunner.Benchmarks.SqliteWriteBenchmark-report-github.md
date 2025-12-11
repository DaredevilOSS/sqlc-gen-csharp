```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host] : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=15  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                 | BatchSize | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|----------------------- |---------- |-----:|------:|------:|--------:|------------:|
| &#39;SQLC - AddOrderItems&#39; | 100       |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  SqliteWriteBenchmark.'SQLC - AddOrderItems': Job-YNBJSC(Runtime=.NET 8.0, InvocationCount=1, IterationCount=15, UnrollFactor=1, WarmupCount=2) [BatchSize=100]
