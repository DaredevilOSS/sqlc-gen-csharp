```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-YTZYKE : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args          | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------- |-------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=100** |  **9.243 s** | **0.1342 s** | **0.0798 s** |  **1.00** |    **0.00** |  **39000.0000** |   **5000.0000** |  **644.78 MB** |        **1.00** |
|                          |               |          |          |          |       |         |             |             |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=50**  | **11.252 s** | **0.3859 s** | **0.2553 s** |  **1.00** |    **0.00** |  **40000.0000** |   **3000.0000** |  **650.17 MB** |        **1.00** |
|                          |               |          |          |          |       |         |             |             |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **R=300K, B=500** | **16.045 s** | **0.0791 s** | **0.0414 s** |     **?** |       **?** | **224000.0000** | **112000.0000** | **3590.75 MB** |           **?** |
