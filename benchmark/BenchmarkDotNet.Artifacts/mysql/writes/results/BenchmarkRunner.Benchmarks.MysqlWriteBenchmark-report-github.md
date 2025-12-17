```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-RVUIEV : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Recor(...)ze=1K [26]** |  **4.856 s** | **0.2556 s** | **0.1691 s** |  **1.00** |    **0.00** |   **6000.0000** |   **3000.0000** |  **109.55 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Recor(...)e=500 [27]** | **76.030 s** | **0.6331 s** | **0.4187 s** |     **?** |       **?** | **267000.0000** | **137000.0000** | **4261.56 MB** |           **?** |
|                          |                      |          |          |          |       |         |             |             |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Recor(...)ze=5K [26]** |  **3.662 s** | **0.0633 s** | **0.0377 s** |  **1.00** |    **0.00** |   **4000.0000** |   **1000.0000** |   **91.79 MB** |        **1.00** |
