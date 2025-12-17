```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ZANJET : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Gen2       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [68]** |  **4.723 s** | **0.0597 s** | **0.0395 s** |  **1.00** |    **0.00** |   **2000.0000** |           **-** |          **-** |    **41.8 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |            |             |
| &#39;SQLC - AddOrderItems&#39;   | Write(...)000 } [68] |  4.167 s | 0.0351 s | 0.0232 s |  1.00 |    0.00 |   2000.0000 |           - |          - |   41.55 MB |        1.00 |
|                          |                      |          |          |          |       |         |             |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [67]** | **23.212 s** | **0.1357 s** | **0.0710 s** |     **?** |       **?** | **192000.0000** | **191000.0000** | **12000.0000** | **3180.28 MB** |           **?** |
