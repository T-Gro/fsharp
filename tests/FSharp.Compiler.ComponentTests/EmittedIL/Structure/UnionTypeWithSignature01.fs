// #Regression #NoMT #CodeGen #Interop 
// Regression test for FSHARP1.0:4040
// "Signature files do not prevent compiler-generated public constructors from leaking out of discriminated unions"
// Note that the corresponding .fsi file is missing the "| C of int" part of the DU
#light

namespace N

type T = | C of int

module M =
 
 open System
 open System.Reflection

 open CodeGenHelper
 
 let res = Assembly.GetExecutingAssembly() 
           |> getType "N.T" 
           |> getMembers
           |> Array.tryFind (fun a -> a.Name = "C")
           
 if not (res.IsNone) then
     raise (Exception($"Oops: not(res.IsNone)"))
