// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The IL Binary writer.
module internal FSharp.Compiler.AbstractIL.ILBinaryWriter

open Internal.Utilities
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.AbstractIL.StrongNameSign

type options =
    { ilg: ILGlobals
      outfile: string
      pdbfile: string option
      portablePDB: bool
      embeddedPDB: bool
      embedAllSource: bool
      embedSourceList: string list
      allGivenSources: ILSourceDocument list
      sourceLink: string
      checksumAlgorithm: HashAlgorithm
      signer: ILStrongNameSigner option
      emitTailcalls: bool
      deterministic: bool
      dumpDebugInfo: bool
      referenceAssemblyOnly: bool
      referenceAssemblyAttribOpt: ILAttribute option
      referenceAssemblySignatureHash: int option
      pathMap: PathMap }

/// Write a binary to the file system.
val WriteILBinaryFile: options: options * inputModule: ILModuleDef * (ILAssemblyRef -> ILAssemblyRef) -> unit

/// Write a binary to an array of bytes suitable for dynamic loading.
val WriteILBinaryInMemory:
    options: options * inputModule: ILModuleDef * (ILAssemblyRef -> ILAssemblyRef) -> byte[] * byte[] option
