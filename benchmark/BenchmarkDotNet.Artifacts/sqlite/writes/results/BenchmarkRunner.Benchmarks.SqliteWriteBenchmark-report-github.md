```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-WIOPCN : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args         | Mean    | Error   | StdDev  | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------- |------------- |--------:|--------:|--------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **R=3.0K,B=100** | **10.98 s** | **0.184 s** | **0.122 s** |  **1.00** |    **0.00** |  **39000.0000** |   **5000.0000** |  **644.78 MB** |        **1.00** |
|                          |              |         |         |         |       |         |             |             |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **R=3.0K,B=50**  | **13.79 s** | **0.285 s** | **0.169 s** |  **1.00** |    **0.00** |  **40000.0000** |   **2000.0000** |  **650.17 MB** |        **1.00** |
|                          |              |         |         |         |       |         |             |             |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **R=3.0K,B=500** | **17.83 s** | **0.333 s** | **0.220 s** |     **?** |       **?** | **224000.0000** | **112000.0000** | **3590.75 MB** |           **?** |
