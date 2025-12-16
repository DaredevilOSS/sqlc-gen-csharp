```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-DMZBJH : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated   | Alloc Ratio |
|------------------------- |--------------------- |----------:|----------:|----------:|------:|--------:|----------:|----------:|----------:|------------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **86.84 ms** |  **14.93 ms** |  **9.876 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |   **592.41 KB** |        **1.00** |
|                          |                      |           |           |           |       |         |           |           |           |             |             |
| &#39;SQLC - AddOrderItems&#39;   | Write(...)000 } [62] | 107.59 ms |  23.77 ms | 15.725 ms |  1.00 |    0.00 |         - |         - |         - |   681.64 KB |        1.00 |
|                          |                      |           |           |           |       |         |           |           |           |             |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **430.24 ms** | **110.37 ms** | **65.679 ms** |     **?** |       **?** | **3000.0000** | **2000.0000** | **1000.0000** | **54517.56 KB** |           **?** |
