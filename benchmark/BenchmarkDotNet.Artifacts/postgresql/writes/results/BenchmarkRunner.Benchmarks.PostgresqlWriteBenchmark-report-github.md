```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-JDYEDI : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Gen2       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Recor(...)ze=1K [26]** |  **5.233 s** | **0.0272 s** | **0.0162 s** |  **1.00** |    **0.00** |   **2000.0000** |           **-** |          **-** |    **41.8 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Recor(...)ze=2K [26]** |  **5.226 s** | **0.0274 s** | **0.0163 s** |  **1.00** |    **0.00** |   **2000.0000** |           **-** |          **-** |   **41.55 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Recor(...)e=500 [27]** | **23.429 s** | **0.1490 s** | **0.0986 s** |     **?** |       **?** | **192000.0000** | **191000.0000** | **12000.0000** | **3180.28 MB** |           **?** |
