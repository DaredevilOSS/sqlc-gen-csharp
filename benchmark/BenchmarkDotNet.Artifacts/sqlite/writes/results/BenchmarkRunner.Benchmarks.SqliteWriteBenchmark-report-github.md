```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-WTRYZO : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean    | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |--------:|---------:|---------:|------:|--------:|-----------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)100 } [61]** | **5.930 s** | **0.1318 s** | **0.0872 s** |  **1.00** |    **0.00** | **17000.0000** |  **3000.0000** |  **429.76 MB** |        **1.00** |
|                          |                      |         |          |          |       |         |            |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...) 50 } [60]** | **7.492 s** | **0.5229 s** | **0.3112 s** |  **1.00** |    **0.00** | **17000.0000** |  **1000.0000** |   **433.4 MB** |        **1.00** |
|                          |                      |         |          |          |       |         |            |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **8.240 s** | **0.0549 s** | **0.0287 s** |     **?** |       **?** | **99000.0000** | **49000.0000** | **2393.84 MB** |           **?** |
