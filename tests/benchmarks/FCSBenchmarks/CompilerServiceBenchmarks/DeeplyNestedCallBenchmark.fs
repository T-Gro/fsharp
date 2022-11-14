
module FSharp.Compiler.Benchmarks.DeeplyNestedCallBenchmark

open System
open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Benchmarks
open Microsoft.CodeAnalysis.Text
open BenchmarkDotNet.Order
open BenchmarkDotNet.Mathematics


let flags = [| "--langversion:Preview";"--simpleresolution";"--targetprofile:netcore";"--noframework";@"-o:C:\temp\DeeplyNestedBenchmark.dll" |]

let assemblies =
    let mainAssemblyLocation = typeof<System.Object>.Assembly.Location
    let frameworkDirectory = Path.GetDirectoryName(mainAssemblyLocation)
    Directory.EnumerateFiles(frameworkDirectory)
    |> Seq.filter (fun x ->
        let name = Path.GetFileName(x)
        (name.StartsWith("System.") && name.EndsWith(".dll") && not(name.Contains("Native"))) ||
        name.Contains("netstandard") ||
        name.Contains("mscorlib")
    )
    |> Array.ofSeq
    |> Array.append [|typeof<Async>.Assembly.Location|]
        
let refs =
    assemblies
    |> Array.map (fun x ->
        $"-r:{x}"
    )

let mainFilePath = Path.Combine(__SOURCE_DIRECTORY__, @"..\deeplyNestedProgram.fs")


let checker = FSharpChecker.Create()

[<MemoryDiagnoser>]
[<Orderer(SummaryOrderPolicy.FastestToSlowest)>]
[<RankColumn(NumeralSystem.Roman)>]
type DeeplyNestedFileBenchmark() =    

    let compile bonusFlags =
        let diag,errCode = 
             checker.Compile([| yield "fsc.exe"; yield! flags; yield! bonusFlags; yield! refs; yield mainFilePath |])
            |> Async.RunSynchronously

        printfn "ErrorCode: %i" errCode
        for d in diag do
            printfn "%i : %s" d.ErrorNumber d.Message

    [<Benchmark>]
    member x.WithoutOptimizations() = compile ["--optimize-"]

    [<Benchmark>]
    member x.Optimized() = compile ["--optimize+"]

