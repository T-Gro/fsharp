




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested public beforefieldinit Key
           extends [runtime]System.Object
           implements class [runtime]System.IEquatable`1<class assembly/HashMicroPerfAndCodeGenerationTests/Key>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<class assembly/HashMicroPerfAndCodeGenerationTests/Key>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly int32 item1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly int32 item2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class assembly/HashMicroPerfAndCodeGenerationTests/Key NewKey(int32 item1, int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void assembly/HashMicroPerfAndCodeGenerationTests/Key::.ctor(int32,
                                                                                                 int32)
        IL_0007:  ret
      } 

      .method assembly specialname rtspecialname instance void  .ctor(int32 item1, int32 item2) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 2E 48 61 73 68 30 36 2B 48 61   
                                                                                                                                                       73 68 4D 69 63 72 6F 50 65 72 66 41 6E 64 43 6F   
                                                                                                                                                       64 65 47 65 6E 65 72 61 74 69 6F 6E 54 65 73 74   
                                                                                                                                                       73 2B 4B 65 79 00 00 )                            
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0014:  ret
      } 

      .method public hidebysig instance int32 get_Item1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Item2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Tag() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldc.i4.0
        IL_0003:  ret
      } 

      .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/HashMicroPerfAndCodeGenerationTests/Key>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/HashMicroPerfAndCodeGenerationTests/Key,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 int32 V_2,
                 class [runtime]System.Collections.IComparer V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0062

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0060

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.3
        IL_0012:  ldloc.0
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  stloc.s    V_4
        IL_001a:  ldloc.1
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0020:  stloc.s    V_5
        IL_0022:  ldloc.s    V_4
        IL_0024:  ldloc.s    V_5
        IL_0026:  cgt
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  clt
        IL_002e:  sub
        IL_002f:  stloc.2
        IL_0030:  ldloc.2
        IL_0031:  ldc.i4.0
        IL_0032:  bge.s      IL_0036

        IL_0034:  ldloc.2
        IL_0035:  ret

        IL_0036:  ldloc.2
        IL_0037:  ldc.i4.0
        IL_0038:  ble.s      IL_003c

        IL_003a:  ldloc.2
        IL_003b:  ret

        IL_003c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0041:  stloc.3
        IL_0042:  ldloc.0
        IL_0043:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.1
        IL_004b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0050:  stloc.s    V_5
        IL_0052:  ldloc.s    V_4
        IL_0054:  ldloc.s    V_5
        IL_0056:  cgt
        IL_0058:  ldloc.s    V_4
        IL_005a:  ldloc.s    V_5
        IL_005c:  clt
        IL_005e:  sub
        IL_005f:  ret

        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.1
        IL_0063:  brfalse.s  IL_0067

        IL_0065:  ldc.i4.m1
        IL_0066:  ret

        IL_0067:  ldc.i4.0
        IL_0068:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0007:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::CompareTo(class assembly/HashMicroPerfAndCodeGenerationTests/Key)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_2,
                 int32 V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_0062

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0010:  brfalse.s  IL_0060

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.2
        IL_0021:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0026:  stloc.s    V_5
        IL_0028:  ldloc.s    V_4
        IL_002a:  ldloc.s    V_5
        IL_002c:  cgt
        IL_002e:  ldloc.s    V_4
        IL_0030:  ldloc.s    V_5
        IL_0032:  clt
        IL_0034:  sub
        IL_0035:  stloc.3
        IL_0036:  ldloc.3
        IL_0037:  ldc.i4.0
        IL_0038:  bge.s      IL_003c

        IL_003a:  ldloc.3
        IL_003b:  ret

        IL_003c:  ldloc.3
        IL_003d:  ldc.i4.0
        IL_003e:  ble.s      IL_0042

        IL_0040:  ldloc.3
        IL_0041:  ret

        IL_0042:  ldloc.1
        IL_0043:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0048:  stloc.s    V_4
        IL_004a:  ldloc.2
        IL_004b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0050:  stloc.s    V_5
        IL_0052:  ldloc.s    V_4
        IL_0054:  ldloc.s    V_5
        IL_0056:  cgt
        IL_0058:  ldloc.s    V_4
        IL_005a:  ldloc.s    V_5
        IL_005c:  clt
        IL_005e:  sub
        IL_005f:  ret

        IL_0060:  ldc.i4.1
        IL_0061:  ret

        IL_0062:  ldarg.1
        IL_0063:  unbox.any  assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0068:  brfalse.s  IL_006c

        IL_006a:  ldc.i4.m1
        IL_006b:  ret

        IL_006c:  ldc.i4.0
        IL_006d:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0037

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldarg.0
        IL_0006:  pop
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldloc.1
        IL_0011:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0016:  ldloc.0
        IL_0017:  ldc.i4.6
        IL_0018:  shl
        IL_0019:  ldloc.0
        IL_001a:  ldc.i4.2
        IL_001b:  shr
        IL_001c:  add
        IL_001d:  add
        IL_001e:  add
        IL_001f:  stloc.0
        IL_0020:  ldc.i4     0x9e3779b9
        IL_0025:  ldloc.1
        IL_0026:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_002b:  ldloc.0
        IL_002c:  ldc.i4.6
        IL_002d:  shl
        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.2
        IL_0030:  shr
        IL_0031:  add
        IL_0032:  add
        IL_0033:  add
        IL_0034:  stloc.0
        IL_0035:  ldloc.0
        IL_0036:  ret

        IL_0037:  ldc.i4.0
        IL_0038:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002d

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002b

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0026:  ceq
        IL_0028:  ret

        IL_0029:  ldc.i4.0
        IL_002a:  ret

        IL_002b:  ldc.i4.0
        IL_002c:  ret

        IL_002d:  ldarg.1
        IL_002e:  ldnull
        IL_002f:  cgt.un
        IL_0031:  ldc.i4.0
        IL_0032:  ceq
        IL_0034:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0013

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key,
                                                                                                  class [runtime]System.Collections.IEqualityComparer)
        IL_0012:  ret

        IL_0013:  ldc.i4.0
        IL_0014:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0,
                 class assembly/HashMicroPerfAndCodeGenerationTests/Key V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_002d

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_002b

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0012:  ldloc.1
        IL_0013:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item1
        IL_0018:  bne.un.s   IL_0029

        IL_001a:  ldloc.0
        IL_001b:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0020:  ldloc.1
        IL_0021:  ldfld      int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::item2
        IL_0026:  ceq
        IL_0028:  ret

        IL_0029:  ldc.i4.0
        IL_002a:  ret

        IL_002b:  ldc.i4.0
        IL_002c:  ret

        IL_002d:  ldarg.1
        IL_002e:  ldnull
        IL_002f:  cgt.un
        IL_0031:  ldc.i4.0
        IL_0032:  ceq
        IL_0034:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/HashMicroPerfAndCodeGenerationTests/Key V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     assembly/HashMicroPerfAndCodeGenerationTests/Key
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  callvirt   instance bool assembly/HashMicroPerfAndCodeGenerationTests/Key::Equals(class assembly/HashMicroPerfAndCodeGenerationTests/Key)
        IL_0011:  ret

        IL_0012:  ldc.i4.0
        IL_0013:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::get_Tag()
      } 
      .property instance int32 Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::get_Item1()
      } 
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/HashMicroPerfAndCodeGenerationTests/Key::get_Item2()
      } 
    } 

    .method public static void  f5a() cil managed
    {
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0016

      IL_0005:  ldc.i4.1
      IL_0006:  ldc.i4.2
      IL_0007:  call       class assembly/HashMicroPerfAndCodeGenerationTests/Key assembly/HashMicroPerfAndCodeGenerationTests/Key::NewKey(int32,
                                                                                                                                       int32)
      IL_000c:  callvirt   instance int32 [runtime]System.Object::GetHashCode()
      IL_0011:  stloc.1
      IL_0012:  ldloc.0
      IL_0013:  ldc.i4.1
      IL_0014:  add
      IL_0015:  stloc.0
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4     0x989681
      IL_001c:  blt.s      IL_0005

      IL_001e:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






