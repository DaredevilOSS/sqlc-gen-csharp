```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-FCFSBV : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated   | Alloc Ratio |
|------------------------- |--------------------- |----------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **96.07 ms** | **20.07 ms** | **13.28 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |   **592.28 KB** |        **1.00** |
|                          |                      |           |          |          |       |         |           |           |           |             |             |
| &#39;SQLC - AddOrderItems&#39;   | Write(...)000 } [62] |  91.39 ms | 15.91 ms | 10.52 ms |  1.00 |    0.00 |         - |         - |         - |   681.64 KB |        1.00 |
|                          |                      |           |          |          |       |         |           |           |           |             |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **416.71 ms** | **35.09 ms** | **23.21 ms** |     **?** |       **?** | **3000.0000** | **2000.0000** | **1000.0000** | **54517.56 KB** |           **?** |
