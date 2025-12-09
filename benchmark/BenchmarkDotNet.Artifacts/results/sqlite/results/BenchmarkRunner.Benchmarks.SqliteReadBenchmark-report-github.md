```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FGCJKJ : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Read  

```
| Method                                      | Limit | ConcurrentQueries | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated | Alloc Ratio |
|-------------------------------------------- |------ |------------------ |-----------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **1**                 |   **163.5 ms** |  **2.33 ms** |  **1.39 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |      **4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 1                 |   172.8 ms | 10.87 ms |  7.19 ms |  1.06 |    0.04 |         - |         - |         - |   3.51 MB |        0.88 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 1                 |   176.9 ms | 18.09 ms | 11.97 ms |  1.09 |    0.08 |         - |         - |         - |   3.52 MB |        0.88 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **5000**  | **10**                | **1,622.3 ms** |  **8.12 ms** |  **4.83 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** |   **37.5 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 5000  | 10                | 1,686.1 ms | 14.45 ms |  8.60 ms |  1.04 |    0.01 | 1000.0000 |         - |         - |  34.79 MB |        0.93 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 5000  | 10                | 1,668.0 ms | 12.04 ms |  7.16 ms |  1.03 |    0.00 | 1000.0000 |         - |         - |  34.78 MB |        0.93 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **1**                 |   **162.7 ms** |  **2.08 ms** |  **1.09 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |      **4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 1                 |   174.1 ms | 13.13 ms |  7.81 ms |  1.08 |    0.05 |         - |         - |         - |   3.51 MB |        0.88 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 1                 |   177.3 ms | 20.54 ms | 13.59 ms |  1.11 |    0.09 |         - |         - |         - |   3.51 MB |        0.88 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **10000** | **10**                | **1,657.6 ms** | **38.48 ms** | **25.45 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** |   **37.5 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 10000 | 10                | 1,685.6 ms | 19.44 ms | 12.86 ms |  1.02 |    0.01 | 1000.0000 |         - |         - |   34.8 MB |        0.93 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 10000 | 10                | 1,686.8 ms | 14.38 ms |  9.51 ms |  1.02 |    0.02 | 1000.0000 |         - |         - |  34.79 MB |        0.93 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **1**                 |   **164.0 ms** |  **5.79 ms** |  **3.83 ms** |  **1.00** |    **0.00** |         **-** |         **-** |         **-** |      **4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 1                 |   178.0 ms | 13.64 ms |  9.02 ms |  1.09 |    0.05 |         - |         - |         - |   3.51 MB |        0.88 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 1                 |   177.9 ms |  8.98 ms |  5.94 ms |  1.09 |    0.04 |         - |         - |         - |   3.51 MB |        0.88 |
|                                             |       |                   |            |          |          |       |         |           |           |           |           |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **20000** | **10**                | **1,636.5 ms** | **21.21 ms** | **14.03 ms** |  **1.00** |    **0.00** | **2000.0000** | **2000.0000** | **2000.0000** |   **37.5 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 20000 | 10                | 1,688.6 ms | 14.40 ms |  8.57 ms |  1.03 |    0.01 | 1000.0000 |         - |         - |  34.81 MB |        0.93 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 20000 | 10                | 1,713.6 ms | 29.45 ms | 19.48 ms |  1.05 |    0.01 | 1000.0000 |         - |         - |  34.78 MB |        0.93 |
