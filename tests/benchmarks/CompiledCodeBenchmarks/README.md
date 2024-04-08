# Compiled Code Benchmarks

This benchmarks project is meant to be used to test the performance of code generated by the F# compiler. It is set up so that, by default, it will build and run your benchmarks in two jobs:

- The first will be built using the local compiler targeting the current language version and will be used as the baseline.
- The second will be built using the local compiler targeting the `preview` language version.

Note that the intent is generally that you use this project to benchmark your changes locally. Most of the time, you will not need to check in your benchmarks.

#### Build the repo with the `Release` config

```pwsh
./Build.cmd -c Release
```

#### Run the benchmarks

```pwsh
dotnet run --project .\tests\benchmarks\CompiledCodeBenchmarks\MicroPerf\MicroPerf.fsproj -c Release
```

The benchmark switcher will prompt you to choose which benchmark or benchmarks you want to run.

#### Sample output

```console
| Job     | Categories                                                                             | start | finish | step | Mean           | Error         | StdDev        | Median         | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|-------- |--------------------------------------------------------------------------------------- |------ |------- |----- |---------------:|--------------:|--------------:|---------------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| Current | UInt32,[|127u..1u|],ComputedCollections,Arrays,IntegralRanges                          | ?     | ?      | ?    |      24.046 ns |     0.4269 ns |     0.3993 ns |      23.985 ns |  1.00 |    0.00 | 0.0004 |      - |      - |      96 B |        1.00 |
| Preview | UInt32,[|127u..1u|],ComputedCollections,Arrays,IntegralRanges                          | ?     | ?      | ?    |       1.729 ns |     0.0804 ns |     0.0752 ns |       1.725 ns |  0.07 |    0.00 |      - |      - |      - |         - |        0.00 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|127u..2u..1u|],ComputedCollections,Arrays,IntegralRanges                      | ?     | ?      | ?    |      22.817 ns |     0.2053 ns |     0.1920 ns |      22.760 ns |  1.00 |    0.00 | 0.0004 |      - |      - |      96 B |        1.00 |
| Preview | UInt32,[|127u..2u..1u|],ComputedCollections,Arrays,IntegralRanges                      | ?     | ?      | ?    |       3.161 ns |     0.1053 ns |     0.0985 ns |       3.172 ns |  0.14 |    0.00 |      - |      - |      - |         - |        0.00 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|1u..127u|],ComputedCollections,Arrays,IntegralRanges                          | ?     | ?      | ?    |     361.493 ns |     4.3161 ns |     3.8261 ns |     361.798 ns |  1.00 |    0.00 | 0.0072 |      - |      - |    1768 B |        1.00 |
| Preview | UInt32,[|1u..127u|],ComputedCollections,Arrays,IntegralRanges                          | ?     | ?      | ?    |      96.560 ns |     1.9609 ns |     3.6347 ns |      94.721 ns |  0.27 |    0.01 | 0.0021 |      - |      - |     536 B |        0.30 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|1u..2u..127u|],ComputedCollections,Arrays,IntegralRanges                      | ?     | ?      | ?    |     263.240 ns |     3.4600 ns |     2.8893 ns |     264.086 ns |  1.00 |    0.00 | 0.0029 |      - |      - |     712 B |        1.00 |
| Preview | UInt32,[|1u..2u..127u|],ComputedCollections,Arrays,IntegralRanges                      | ?     | ?      | ?    |      58.053 ns |     1.1757 ns |     1.6481 ns |      57.840 ns |  0.22 |    0.01 | 0.0011 |      - |      - |     280 B |        0.39 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|1u..2u..32767u|],ComputedCollections,Arrays,IntegralRanges                    | ?     | ?      | ?    |  40,529.790 ns |   272.6267 ns |   241.6764 ns |  40,486.288 ns |  1.00 |    0.00 | 0.4883 |      - |      - |  131464 B |        1.00 |
| Preview | UInt32,[|1u..2u..32767u|],ComputedCollections,Arrays,IntegralRanges                    | ?     | ?      | ?    |   7,787.907 ns |   152.9334 ns |   176.1183 ns |   7,737.320 ns |  0.19 |    0.00 | 0.2747 |      - |      - |   65560 B |        0.50 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|1u..32767u|],ComputedCollections,Arrays,IntegralRanges                        | ?     | ?      | ?    | 256,084.235 ns | 5,074.6636 ns | 6,598.4961 ns | 257,729.980 ns |  1.00 |    0.00 | 8.3008 | 8.3008 | 8.3008 |  393680 B |        1.00 |
| Preview | UInt32,[|1u..32767u|],ComputedCollections,Arrays,IntegralRanges                        | ?     | ?      | ?    |  77,660.979 ns | 1,541.8822 ns | 4,399.0768 ns |  77,866.278 ns |  0.31 |    0.02 | 2.8076 | 2.8076 | 2.8076 |  131088 B |        0.33 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|for n in start..finish -> n|],ComputedCollections,Arrays,IntegralRanges       | 0     | 32767  | ?    | 281,373.636 ns | 5,097.5675 ns | 4,518.8608 ns | 282,881.763 ns |  1.00 |    0.00 | 8.7891 | 8.7891 | 8.7891 |  393741 B |        1.00 |
| Preview | UInt32,[|for n in start..finish -> n|],ComputedCollections,Arrays,IntegralRanges       | 0     | 32767  | ?    |  77,629.964 ns | 1,545.8980 ns | 4,509.4572 ns |  77,968.518 ns |  0.29 |    0.02 | 3.0518 | 3.0518 | 3.0518 |  131090 B |        0.33 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|for n in start..step..finish -> n|],ComputedCollections,Arrays,IntegralRanges | 0     | 32767  | 2    |  69,948.064 ns | 1,078.6284 ns | 1,154.1203 ns |  69,834.222 ns |  1.00 |    0.00 | 0.7324 |      - |      - |  197056 B |        1.00 |
| Preview | UInt32,[|for n in start..step..finish -> n|],ComputedCollections,Arrays,IntegralRanges | 0     | 32767  | 2    |   7,700.286 ns |   115.4058 ns |   107.9507 ns |   7,679.921 ns |  0.11 |    0.00 | 0.2747 |      - |      - |   65560 B |        0.33 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|start..finish|],ComputedCollections,Arrays,IntegralRanges                     | 0     | 32767  | ?    | 148,726.931 ns | 2,956.8132 ns | 4,603.4019 ns | 148,672.632 ns |  1.00 |    0.00 | 4.8828 | 4.3945 | 4.3945 |  262584 B |        1.00 |
| Preview | UInt32,[|start..finish|],ComputedCollections,Arrays,IntegralRanges                     | 0     | 32767  | ?    |  77,915.564 ns | 1,554.2518 ns | 3,476.3069 ns |  77,861.060 ns |  0.52 |    0.03 | 4.0283 | 4.0283 | 4.0283 |  131095 B |        0.50 |
|         |                                                                                        |       |        |      |                |               |               |                |       |         |        |        |        |           |             |
| Current | UInt32,[|start..step..finish|],ComputedCollections,Arrays,IntegralRanges               | 0     | 32767  | 2    |  38,456.304 ns |   682.2118 ns |   638.1413 ns |  38,380.719 ns |  1.00 |    0.00 | 0.4883 |      - |      - |  131464 B |        1.00 |
| Preview | UInt32,[|start..step..finish|],ComputedCollections,Arrays,IntegralRanges               | 0     | 32767  | 2    |   7,791.339 ns |    93.7728 ns |    87.7152 ns |   7,789.114 ns |  0.20 |    0.00 | 0.2747 |      - |      - |   65560 B |        0.50 |
```