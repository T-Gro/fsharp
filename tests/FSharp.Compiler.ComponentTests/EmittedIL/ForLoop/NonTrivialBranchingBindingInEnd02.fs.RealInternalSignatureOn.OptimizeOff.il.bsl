




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
  .field static assembly int32[] r@6
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32[] w@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int32[] get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::r@6
    IL_0005:  ret
  } 

  .method public specialname static int32[] get_w() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32[] assembly::w@7
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  7
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4)
    IL_0000:  ldc.i4.8
    IL_0001:  ldc.i4.1
    IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0007:  stsfld     int32[] assembly::r@6
    IL_000c:  ldc.i4.5
    IL_000d:  ldc.i4.2
    IL_000e:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<int32>(int32,
                                                                                                    !!0)
    IL_0013:  stsfld     int32[] assembly::w@7
    IL_0018:  call       int32[] assembly::get_r()
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  stloc.2
    IL_0020:  call       int32[] assembly::get_w()
    IL_0025:  ldlen
    IL_0026:  conv.i4
    IL_0027:  stloc.3
    IL_0028:  ldloc.2
    IL_0029:  ldloc.3
    IL_002a:  bge.s      IL_0030

    IL_002c:  ldloc.2
    IL_002d:  nop
    IL_002e:  br.s       IL_0032

    IL_0030:  ldloc.3
    IL_0031:  nop
    IL_0032:  ldc.i4.1
    IL_0033:  sub
    IL_0034:  stloc.1
    IL_0035:  ldc.i4.0
    IL_0036:  stloc.0
    IL_0037:  ldloc.0
    IL_0038:  ldloc.1
    IL_0039:  bgt.s      IL_0067

    IL_003b:  call       int32[] assembly::get_r()
    IL_0040:  ldloc.1
    IL_0041:  call       int32[] assembly::get_r()
    IL_0046:  ldloc.1
    IL_0047:  ldelem     [runtime]System.Int32
    IL_004c:  call       int32[] assembly::get_w()
    IL_0051:  ldloc.1
    IL_0052:  ldelem     [runtime]System.Int32
    IL_0057:  add
    IL_0058:  stelem     [runtime]System.Int32
    IL_005d:  ldloc.1
    IL_005e:  ldc.i4.1
    IL_005f:  sub
    IL_0060:  stloc.1
    IL_0061:  ldloc.1
    IL_0062:  ldloc.0
    IL_0063:  ldc.i4.1
    IL_0064:  sub
    IL_0065:  bne.un.s   IL_003b

    IL_0067:  nop
    IL_0068:  nop
    IL_0069:  call       int32[] assembly::get_r()
    IL_006e:  ldc.i4.0
    IL_006f:  ldelem     [runtime]System.Int32
    IL_0074:  ldc.i4.3
    IL_0075:  bne.un.s   IL_007b

    IL_0077:  ldc.i4.0
    IL_0078:  nop
    IL_0079:  br.s       IL_007d

    IL_007b:  ldc.i4.1
    IL_007c:  nop
    IL_007d:  stloc.s    V_4
    IL_007f:  ldloc.s    V_4
    IL_0081:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0086:  pop
    IL_0087:  ret
  } 

  .property int32[] r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_r()
  } 
  .property int32[] w()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get int32[] assembly::get_w()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






