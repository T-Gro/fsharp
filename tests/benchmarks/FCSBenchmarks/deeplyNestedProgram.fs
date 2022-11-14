// See https://github.com/dotnet/fsharp/pull/12420 and https://github.com/dotnet/fsharp/issues/12322
let f x = printfn "call"; x
(* type ReproBuilder () =
    member _.Delay x = printfn "Delay"; x ()
    member _.Yield  (x) = printfn "Yield"; x
    member _.Combine (x, y) = printfn "Combine"; x + y

let repro = ReproBuilder ()

// The de-sugaring of this is a mass of nested function calls
let reallyBigComputationExpression () =
    repro {
        // Commenting out some of the below is enough to avoid StackOverflow on my machine.
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
    }



let manyPipes () =
    1 |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    *)

let deepCalls () =
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
         1
    ))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
    ))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))