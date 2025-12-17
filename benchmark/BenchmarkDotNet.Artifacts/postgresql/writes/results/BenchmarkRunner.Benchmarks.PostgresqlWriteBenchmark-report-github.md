```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-DNMPGF : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args          | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Gen2       | Allocated  | Alloc Ratio |
|------------------------- |-------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=1K**  |  **5.700 s** | **0.0496 s** | **0.0295 s** |  **1.00** |    **0.00** |   **2000.0000** |           **-** |          **-** |    **41.8 MB** |        **1.00** |
|                          |               |          |          |          |       |         |             |             |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=2K**  |  **4.215 s** | **0.0271 s** | **0.0179 s** |  **1.00** |    **0.00** |   **2000.0000** |           **-** |          **-** |   **41.55 MB** |        **1.00** |
|                          |               |          |          |          |       |         |             |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **R=300K, B=500** | **23.606 s** | **0.1773 s** | **0.1173 s** |     **?** |       **?** | **192000.0000** | **191000.0000** | **12000.0000** | **3180.28 MB** |           **?** |
