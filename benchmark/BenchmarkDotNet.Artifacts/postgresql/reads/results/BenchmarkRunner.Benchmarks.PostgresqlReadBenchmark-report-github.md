```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-CMNLSB : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params            | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------ |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1K, C=75, Q=2K**  |  **8.159 s** | **0.0604 s** | **0.0359 s** |  **1.00** | **29000.0000** | **28000.0000** | **5000.0000** |  **909.81 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1K, C=75, Q=2K  |  8.912 s | 0.0497 s | 0.0296 s |  1.09 | 42000.0000 | 41000.0000 | 1000.0000 | 1026.06 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1K, C=75, Q=2K  |  8.943 s | 0.0815 s | 0.0539 s |  1.10 | 42000.0000 | 41000.0000 | 1000.0000 |  1022.7 MB |        1.12 |
|                                             |                   |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50, C=150, Q=4K** | **11.906 s** | **0.0600 s** | **0.0397 s** |  **1.00** |  **5000.0000** |  **4000.0000** | **2000.0000** |  **131.22 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50, C=150, Q=4K | 12.475 s | 0.0693 s | 0.0458 s |  1.05 | 18000.0000 | 10000.0000 | 2000.0000 |  386.65 MB |        2.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50, C=150, Q=4K | 12.424 s | 0.0627 s | 0.0415 s |  1.04 | 16000.0000 |  8000.0000 | 1000.0000 |  380.86 MB |        2.90 |
