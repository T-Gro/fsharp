// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test.Compiler

module GuardedOrPatternComplexity =

    // https://github.com/dotnet/fsharp/issues/18425
    let private runsWith expected source =
        source
        |> FSharp
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains expected

    [<Fact>]
    let ``Issue 18425 - guarded shared-or partial active pattern match runs the guard once per structural match`` () =
        let disjuncts =
            [ for k in 1..24 -> sprintf "    | (A p, E %d _)" k ]
            |> String.concat "\n"

        """module Test
let mutable guards = 0
let (|A|_|) (x: int) = if x % 2 = 0 then Some(x / 2) else None
let (|E|_|) (n: int) (x: int) = if x = n then Some x else None
let g (p: int) = guards <- guards + 1; p > 1000
let f (a: int) (b: int) =
    match a, b with
__DISJUNCTS__
        when g p -> p
    | _ -> -1
[<EntryPoint>]
let main _ =
    let r1 = f 8 3
    let r2 = f 4000 1
    printfn "r1=%d r2=%d guards=%d" r1 r2 guards
    0
"""
            .Replace("__DISJUNCTS__", disjuncts)
        |> runsWith "r1=-1 r2=2000 guards=2"

    [<Fact>]
    let ``Issue 18425 - shared guard binding a variable at different positions is not over-fused`` () =
        """module Test
let (|Z|_|) (v: int) = if v = 0 then Some() else None
let (|Pos|_|) (v: int) = if v > 100 then Some v else None
let f (t: int*int*int*int*int*int*int*int) =
    match t with
    | (Pos x, Z, Z, Z, Z, Z, Z, Z)
    | (Z, Pos x, Z, Z, Z, Z, Z, Z)
    | (Z, Z, Pos x, Z, Z, Z, Z, Z)
    | (Z, Z, Z, Pos x, Z, Z, Z, Z)
    | (Z, Z, Z, Z, Pos x, Z, Z, Z)
    | (Z, Z, Z, Z, Z, Pos x, Z, Z)
    | (Z, Z, Z, Z, Z, Z, Pos x, Z)
    | (Z, Z, Z, Z, Z, Z, Z, Pos x) when x > 100 -> x
    | _ -> -1
[<EntryPoint>]
let main _ =
    printfn "%d %d %d %d" (f (150,0,0,0,0,0,0,0)) (f (0,0,0,160,0,0,0,0)) (f (0,0,0,0,0,0,0,170)) (f (1,2,3,4,5,6,7,8))
    0
"""
        |> runsWith "150 160 170 -1"

    // Bodies that cannot move into a lambda must keep being inlined past the promotion threshold.
    [<Fact>]
    let ``Issue 18425 - guarded shared-or with an unliftable body stays inline`` () =
        """module Test
let (|E|_|) (n: int) (x: int) = if x = n then Some x else None
let (|Msg|_|) (n: int) (e: exn) = if e.Message = string n then Some() else None
let returnsByref (arr: int[]) (b: int) : byref<int> =
    match b with
    | E 1 _ | E 2 _ | E 3 _ | E 4 _ | E 5 _ | E 6 _ | E 7 _ | E 8 _ when arr.Length > 2 -> &arr[0]
    | _ -> &arr[1]
let capturesByrefLike (buffer: byref<int>) x =
    match x with
    | E 1 _ | E 2 _ | E 3 _ | E 4 _ | E 5 _ | E 6 _ | E 7 _ | E 8 _ when System.Environment.TickCount >= System.Int32.MinValue -> buffer
    | _ -> 0
let reraises () =
    try failwith "1" with
    | (Msg 1 | Msg 2 | Msg 3 | Msg 4 | Msg 5 | Msg 6 | Msg 7 | Msg 8) when System.Environment.TickCount >= System.Int32.MinValue -> 1
    | _ -> reraise()
[<EntryPoint>]
let main _ =
    let arr = [| 10; 20; 30 |]
    (returnsByref arr 3) <- 99
    (returnsByref arr 42) <- 77
    let mutable slot = 5
    printfn "%d %d %d %d" arr[0] arr[1] (capturesByrefLike &slot 3) (reraises ())
    0
"""
        |> runsWith "99 77 5 1"
