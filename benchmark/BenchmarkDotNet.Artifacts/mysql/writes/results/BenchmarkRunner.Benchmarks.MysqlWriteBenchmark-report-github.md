```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-LVPCZC : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Recor(...)1,000 [32]** |  **4.748 s** | **0.2712 s** | **0.1794 s** |  **1.00** |    **0.00** |   **6000.0000** |   **3000.0000** |  **109.58 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Recor(...)5,000 [32]** |  **3.745 s** | **0.1045 s** | **0.0622 s** |  **1.00** |    **0.00** |   **4000.0000** |   **1000.0000** |    **91.8 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Recor(...)e=500 [30]** | **78.369 s** | **0.7358 s** | **0.4867 s** |     **?** |       **?** | **267000.0000** | **136000.0000** | **4259.57 MB** |           **?** |
