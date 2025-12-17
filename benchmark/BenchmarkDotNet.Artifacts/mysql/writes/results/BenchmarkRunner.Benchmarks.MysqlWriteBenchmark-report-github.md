```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-NSCXXI : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args          | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------- |-------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=1K**  |  **4.630 s** | **0.1233 s** | **0.0734 s** |  **1.00** |    **0.00** |   **6000.0000** |   **3000.0000** |  **109.57 MB** |        **1.00** |
|                          |               |          |          |          |       |         |             |             |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **R=300K, B=500** | **77.129 s** | **0.2735 s** | **0.1628 s** |     **?** |       **?** | **267000.0000** | **136000.0000** | **4257.75 MB** |           **?** |
|                          |               |          |          |          |       |         |             |             |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=5K**  |  **3.898 s** | **0.0851 s** | **0.0563 s** |  **1.00** |    **0.00** |   **4000.0000** |   **1000.0000** |   **91.79 MB** |        **1.00** |
