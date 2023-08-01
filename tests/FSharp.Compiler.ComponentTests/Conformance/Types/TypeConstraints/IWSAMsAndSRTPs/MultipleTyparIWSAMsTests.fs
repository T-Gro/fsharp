module Conformance.Types.TypesAndTypeConstraints_IWSAM_Regressions

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``Issue 15713 - typecheck error for multiple implementations`` () =
    Fsx """
open System.Numerics

type Vector = Vector of x: double * y: double * z: double with
    
    //factor
    static member factorAsterisk (Vector (x, y, z), f : double) : Vector =
        Vector (x * f, y * f, z * f)

    //cross product
    static member crossProductAsterisk (Vector (x1, y1, z1), Vector (x2, y2, z2)) : Vector =
        Vector (y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2)

    interface IMultiplyOperators<Vector, double, Vector> with
        static member (*) (a:Vector, f:double) : Vector = Vector.factorAsterisk(a,f)
    interface IMultiplyOperators<Vector, Vector, Vector> with
        static member (*) (a:Vector, b:Vector) : Vector = Vector.crossProductAsterisk(a,b)
    """
    |> typecheck
    |> shouldSucceed