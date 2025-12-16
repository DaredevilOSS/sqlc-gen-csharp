```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-GGUEWM : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Gen2       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **3.437 s** | **0.0559 s** | **0.0333 s** |  **1.00** |    **0.00** |   **1000.0000** |           **-** |          **-** |   **27.88 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |            |             |
| &#39;SQLC - AddOrderItems&#39;   | Write(...)000 } [62] |  2.839 s | 0.0523 s | 0.0311 s |  1.00 |    0.00 |   1000.0000 |           - |          - |   27.72 MB |        1.00 |
|                          |                      |          |          |          |       |         |             |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **15.958 s** | **0.1519 s** | **0.1005 s** |     **?** |       **?** | **136000.0000** | **135000.0000** | **16000.0000** | **2120.24 MB** |           **?** |
