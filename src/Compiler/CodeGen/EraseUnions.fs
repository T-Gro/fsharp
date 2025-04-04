// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Erase discriminated unions.
module internal FSharp.Compiler.AbstractIL.ILX.EraseUnions

open FSharp.Compiler.IlxGenSupport

open System.Collections.Generic
open System.Reflection
open Internal.Utilities.Library
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.Features
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types

[<Literal>]
let TagNil = 0

[<Literal>]
let TagCons = 1

[<Literal>]
let ALT_NAME_CONS = "Cons"

type DiscriminationTechnique =
    /// Indicates a special representation for the F# list type where the "empty" value has a tail field of value null
    | TailOrNull

    /// Indicates a type with either number of cases < 4, and not a single-class type with an integer tag (IntegerTag)
    | RuntimeTypes

    /// Indicates a type with a single case, e.g. ``type X = ABC of string * int``
    | SingleCase

    /// Indicates a type with either cases >= 4, or a type like
    //     type X = A | B | C
    //  or type X = A | B | C of string
    // where at most one case is non-nullary.  These can be represented using a single
    // class (no subclasses), but an integer tag is stored to discriminate between the objects.
    | IntegerTag

// A potentially useful additional representation trades an extra integer tag in the root type
// for faster discrimination, and in the important single-non-nullary constructor case
//
//     type Tree = Tip | Node of int * Tree * Tree
//
// it also flattens so the fields for "Node" are stored in the base class, meaning that no type casts
// are needed to access the data.
//
// However, it can't be enabled because it suppresses the generation
// of C#-facing nested types for the non-nullary case. This could be enabled
// in a binary compatible way by ensuring we continue to generate the C# facing types and use
// them as the instance types, but still store all field elements in the base type. Additional
// accessors would be needed to access these fields directly, akin to HeadOrDefault and TailOrNull.

// This functor helps us make representation decisions for F# union type compilation
type UnionReprDecisions<'Union, 'Alt, 'Type>
    (
        getAlternatives: 'Union -> 'Alt[],
        nullPermitted: 'Union -> bool,
        isNullary: 'Alt -> bool,
        isList: 'Union -> bool,
        isStruct: 'Union -> bool,
        nameOfAlt: 'Alt -> string,
        makeRootType: 'Union -> 'Type,
        makeNestedType: 'Union * string -> 'Type
    ) =

    static let TaggingThresholdFixedConstant = 4

    member repr.RepresentAllAlternativesAsConstantFieldsInRootClass cu =
        cu |> getAlternatives |> Array.forall isNullary

    member repr.DiscriminationTechnique cu =
        if isList cu then
            TailOrNull
        else
            let alts = getAlternatives cu

            if alts.Length = 1 then
                SingleCase
            elif
                not (isStruct cu)
                && alts.Length < TaggingThresholdFixedConstant
                && not (repr.RepresentAllAlternativesAsConstantFieldsInRootClass cu)
            then
                RuntimeTypes
            else
                IntegerTag

    // WARNING: this must match IsUnionTypeWithNullAsTrueValue in the F# compiler
    member repr.RepresentAlternativeAsNull(cu, alt) =
        let alts = getAlternatives cu

        nullPermitted cu
        && (repr.DiscriminationTechnique cu = RuntimeTypes)
        && (* don't use null for tags, lists or single-case  *) Array.existsOne isNullary alts
        && Array.exists (isNullary >> not) alts
        && isNullary alt (* is this the one? *)

    member repr.RepresentOneAlternativeAsNull cu =
        let alts = getAlternatives cu

        nullPermitted cu
        && alts |> Array.existsOne (fun alt -> repr.RepresentAlternativeAsNull(cu, alt))

    member repr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull(cu, alt) =
        // Check all nullary constructors are being represented without using sub-classes
        let alts = getAlternatives cu

        not (isStruct cu)
        && not (isNullary alt)
        && (alts
            |> Array.forall (fun alt2 -> not (isNullary alt2) || repr.RepresentAlternativeAsNull(cu, alt2)))
        &&
        // Check this is the one and only non-nullary constructor
        Array.existsOne (isNullary >> not) alts

    member repr.RepresentAlternativeAsStructValue cu = isStruct cu

    member repr.RepresentAlternativeAsFreshInstancesOfRootClass(cu, alt) =
        not (isStruct cu)
        && ((isList // Check all nullary constructors are being represented without using sub-classes
                 cu
             && nameOfAlt alt = ALT_NAME_CONS)
            || repr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull(cu, alt))

    member repr.RepresentAlternativeAsConstantFieldInTaggedRootClass(cu, alt) =
        not (isStruct cu)
        && isNullary alt
        && not (repr.RepresentAlternativeAsNull(cu, alt))
        && (repr.DiscriminationTechnique cu <> RuntimeTypes)

    member repr.Flatten cu = isStruct cu

    member repr.OptimizeAlternativeToRootClass(cu, alt) =
        // The list type always collapses to the root class
        isList cu
        ||
        // Structs are always flattened
        repr.Flatten cu
        || repr.RepresentAllAlternativesAsConstantFieldsInRootClass cu
        || repr.RepresentAlternativeAsConstantFieldInTaggedRootClass(cu, alt)
        || repr.RepresentAlternativeAsStructValue(cu)
        || repr.RepresentAlternativeAsFreshInstancesOfRootClass(cu, alt)

    member repr.MaintainPossiblyUniqueConstantFieldForAlternative(cu, alt) =
        not (isStruct cu)
        && not (repr.RepresentAlternativeAsNull(cu, alt))
        && isNullary alt

    member repr.TypeForAlternative(cuspec, alt) =
        if
            repr.OptimizeAlternativeToRootClass(cuspec, alt)
            || repr.RepresentAlternativeAsNull(cuspec, alt)
        then
            makeRootType cuspec
        else
            let altName = nameOfAlt alt
            // Add "_" if the thing is nullary or if it is 'List._Cons', which is special because it clashes with the name of the static method "Cons"
            let nm =
                if isNullary alt || isList cuspec then
                    "_" + altName
                else
                    altName

            makeNestedType (cuspec, nm)

let baseTyOfUnionSpec (cuspec: IlxUnionSpec) =
    mkILNamedTy cuspec.Boxity cuspec.TypeRef cuspec.GenericArgs

let mkMakerName (cuspec: IlxUnionSpec) nm =
    match cuspec.HasHelpers with
    | SpecialFSharpListHelpers
    | SpecialFSharpOptionHelpers -> nm // Leave 'Some', 'None', 'Cons', 'Empty' as is
    | AllHelpers
    | NoHelpers -> "New" + nm

let mkCasesTypeRef (cuspec: IlxUnionSpec) = cuspec.TypeRef

let cuspecRepr =
    UnionReprDecisions(
        (fun (cuspec: IlxUnionSpec) -> cuspec.AlternativesArray),
        (fun (cuspec: IlxUnionSpec) -> cuspec.IsNullPermitted),
        (fun (alt: IlxUnionCase) -> alt.IsNullary),
        (fun cuspec -> cuspec.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers),
        (fun cuspec -> cuspec.Boxity = ILBoxity.AsValue),
        (fun (alt: IlxUnionCase) -> alt.Name),
        (fun cuspec -> cuspec.DeclaringType),
        (fun (cuspec, nm) -> mkILNamedTy cuspec.Boxity (mkILTyRefInTyRef (mkCasesTypeRef cuspec, nm)) cuspec.GenericArgs)
    )

type NoTypesGeneratedViaThisReprDecider = NoTypesGeneratedViaThisReprDecider

let cudefRepr =
    UnionReprDecisions(
        (fun (_td, cud) -> cud.UnionCases),
        (fun (_td, cud) -> cud.IsNullPermitted),
        (fun (alt: IlxUnionCase) -> alt.IsNullary),
        (fun (_td, cud) -> cud.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers),
        (fun (td: ILTypeDef, _cud) -> td.IsStruct),
        (fun (alt: IlxUnionCase) -> alt.Name),
        (fun (_td, _cud) -> NoTypesGeneratedViaThisReprDecider),
        (fun ((_td, _cud), _nm) -> NoTypesGeneratedViaThisReprDecider)
    )

let mkTesterName nm = "Is" + nm

let tagPropertyName = "Tag"

let mkUnionCaseFieldId (fdef: IlxUnionCaseField) =
    // Use the lower case name of a field or constructor as the field/parameter name if it differs from the uppercase name
    fdef.LowerName, fdef.Type

let inline getFieldsNullability (g: TcGlobals) (ilf: ILFieldDef) =
    if g.checkNullness then
        ilf.CustomAttrs.AsArray()
        |> Array.tryFind (IsILAttrib g.attrib_NullableAttribute)
    else
        None

let mkUnionCaseFieldIdAndAttrs g fdef =
    let nm, t = mkUnionCaseFieldId fdef
    let attrs = getFieldsNullability g fdef.ILField
    nm, t, attrs |> Option.toList

let refToFieldInTy ty (nm, fldTy) = mkILFieldSpecInTy (ty, nm, fldTy)

let formalTypeArgs (baseTy: ILType) =
    List.mapi (fun i _ -> mkILTyvarTy (uint16 i)) baseTy.GenericArgs

let constFieldName nm = "_unique_" + nm

let constFormalFieldTy (baseTy: ILType) =
    mkILNamedTy baseTy.Boxity baseTy.TypeRef (formalTypeArgs baseTy)

let mkConstFieldSpecFromId (baseTy: ILType) constFieldId = refToFieldInTy baseTy constFieldId

let mkConstFieldSpec nm (baseTy: ILType) =
    mkConstFieldSpecFromId baseTy (constFieldName nm, constFormalFieldTy baseTy)

let tyForAlt cuspec alt =
    cuspecRepr.TypeForAlternative(cuspec, alt)

let GetILTypeForAlternative cuspec alt =
    cuspecRepr.TypeForAlternative(cuspec, cuspec.Alternative alt)

let mkTagFieldType (ilg: ILGlobals) _cuspec = ilg.typ_Int32

let mkTagFieldFormalType (ilg: ILGlobals) _cuspec = ilg.typ_Int32

let mkTagFieldId ilg cuspec = "_tag", mkTagFieldType ilg cuspec

let altOfUnionSpec (cuspec: IlxUnionSpec) cidx =
    try
        cuspec.Alternative cidx
    with _ ->
        failwith ("alternative " + string cidx + " not found")

// Nullary cases on types with helpers do not reveal their underlying type even when
// using runtime type discrimination, because the underlying type is never needed from
// C# code and pollutes the visible API surface. In this case we must discriminate by
// calling the IsFoo helper. This only applies to discriminations outside the
// assembly where the type is defined (indicated by 'avoidHelpers' flag - if this is true
// then the reference is intra-assembly).
let doesRuntimeTypeDiscriminateUseHelper avoidHelpers (cuspec: IlxUnionSpec) (alt: IlxUnionCase) =
    not avoidHelpers
    && alt.IsNullary
    && cuspec.HasHelpers = IlxUnionHasHelpers.AllHelpers

let mkRuntimeTypeDiscriminate (ilg: ILGlobals) avoidHelpers cuspec alt altName altTy =
    let useHelper = doesRuntimeTypeDiscriminateUseHelper avoidHelpers cuspec alt

    if useHelper then
        let baseTy = baseTyOfUnionSpec cuspec

        [
            mkNormalCall (mkILNonGenericInstanceMethSpecInTy (baseTy, "get_" + mkTesterName altName, [], ilg.typ_Bool))
        ]
    else
        [ I_isinst altTy; AI_ldnull; AI_cgt_un ]

let mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy after =
    let useHelper = doesRuntimeTypeDiscriminateUseHelper avoidHelpers cuspec alt

    match after with
    | I_brcmp(BI_brfalse, _)
    | I_brcmp(BI_brtrue, _) when not useHelper -> [ I_isinst altTy; after ]
    | _ -> mkRuntimeTypeDiscriminate ilg avoidHelpers cuspec alt altName altTy @ [ after ]

let mkGetTagFromField ilg cuspec baseTy =
    mkNormalLdfld (refToFieldInTy baseTy (mkTagFieldId ilg cuspec))

let mkSetTagToField ilg cuspec baseTy =
    mkNormalStfld (refToFieldInTy baseTy (mkTagFieldId ilg cuspec))

let adjustFieldName hasHelpers nm =
    match hasHelpers, nm with
    | SpecialFSharpListHelpers, "Head" -> "HeadOrDefault"
    | SpecialFSharpListHelpers, "Tail" -> "TailOrNull"
    | _ -> nm

let mkLdData (avoidHelpers, cuspec, cidx, fidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let fieldDef = alt.FieldDef fidx

    if avoidHelpers then
        mkNormalLdfld (mkILFieldSpecInTy (altTy, fieldDef.LowerName, fieldDef.Type))
    else
        mkNormalCall (
            mkILNonGenericInstanceMethSpecInTy (altTy, "get_" + adjustFieldName cuspec.HasHelpers fieldDef.Name, [], fieldDef.Type)
        )

let mkLdDataAddr (avoidHelpers, cuspec, cidx, fidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let fieldDef = alt.FieldDef fidx

    if avoidHelpers then
        mkNormalLdflda (mkILFieldSpecInTy (altTy, fieldDef.LowerName, fieldDef.Type))
    else
        failwith (sprintf "can't load address using helpers, for fieldDef %s" fieldDef.LowerName)

let mkGetTailOrNull avoidHelpers cuspec =
    mkLdData (avoidHelpers, cuspec, 1, 1) (* tail is in alternative 1, field number 1 *)

let mkGetTagFromHelpers ilg (cuspec: IlxUnionSpec) =
    let baseTy = baseTyOfUnionSpec cuspec

    if cuspecRepr.RepresentOneAlternativeAsNull cuspec then
        mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "Get" + tagPropertyName, [ baseTy ], mkTagFieldFormalType ilg cuspec))
    else
        mkNormalCall (mkILNonGenericInstanceMethSpecInTy (baseTy, "get_" + tagPropertyName, [], mkTagFieldFormalType ilg cuspec))

let mkGetTag ilg (cuspec: IlxUnionSpec) =
    match cuspec.HasHelpers with
    | AllHelpers -> mkGetTagFromHelpers ilg cuspec
    | _hasHelpers -> mkGetTagFromField ilg cuspec (baseTyOfUnionSpec cuspec)

let mkCeqThen after =
    match after with
    | I_brcmp(BI_brfalse, a) -> [ I_brcmp(BI_bne_un, a) ]
    | I_brcmp(BI_brtrue, a) -> [ I_brcmp(BI_beq, a) ]
    | _ -> [ AI_ceq; after ]

let mkTagDiscriminate ilg cuspec _baseTy cidx =
    [ mkGetTag ilg cuspec; mkLdcInt32 cidx; AI_ceq ]

let mkTagDiscriminateThen ilg cuspec cidx after =
    [ mkGetTag ilg cuspec; mkLdcInt32 cidx ] @ mkCeqThen after

let convNewDataInstrInternal ilg cuspec cidx =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let altName = alt.Name

    if cuspecRepr.RepresentAlternativeAsNull(cuspec, alt) then
        [ AI_ldnull ]
    elif cuspecRepr.MaintainPossiblyUniqueConstantFieldForAlternative(cuspec, alt) then
        let baseTy = baseTyOfUnionSpec cuspec
        [ I_ldsfld(Nonvolatile, mkConstFieldSpec altName baseTy) ]
    elif cuspecRepr.RepresentAlternativeAsFreshInstancesOfRootClass(cuspec, alt) then
        let baseTy = baseTyOfUnionSpec cuspec

        let instrs, tagfields =
            match cuspecRepr.DiscriminationTechnique cuspec with
            | IntegerTag -> [ mkLdcInt32 cidx ], [ mkTagFieldType ilg cuspec ]
            | _ -> [], []

        let ctorFieldTys = alt.FieldTypes |> Array.toList

        instrs
        @ [ mkNormalNewobj (mkILCtorMethSpecForTy (baseTy, (ctorFieldTys @ tagfields))) ]
    elif
        cuspecRepr.RepresentAlternativeAsStructValue cuspec
        && cuspecRepr.DiscriminationTechnique cuspec = IntegerTag
    then
        // Structs with fields should be created using maker methods (mkMakerName), only field-less cases are created this way
        assert (alt.IsNullary)
        let baseTy = baseTyOfUnionSpec cuspec
        let tagField = [ mkTagFieldType ilg cuspec ]
        [ mkLdcInt32 cidx; mkNormalNewobj (mkILCtorMethSpecForTy (baseTy, tagField)) ]
    else
        [ mkNormalNewobj (mkILCtorMethSpecForTy (altTy, Array.toList alt.FieldTypes)) ]

// The stdata 'instruction' is only ever used for the F# "List" type within FSharp.Core.dll
let mkStData (cuspec, cidx, fidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let fieldDef = alt.FieldDef fidx
    mkNormalStfld (mkILFieldSpecInTy (altTy, fieldDef.LowerName, fieldDef.Type))

let mkNewData ilg (cuspec, cidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altName = alt.Name
    let baseTy = baseTyOfUnionSpec cuspec

    let viaMakerCall () =
        [
            mkNormalCall (
                mkILNonGenericStaticMethSpecInTy (
                    baseTy,
                    mkMakerName cuspec altName,
                    Array.toList alt.FieldTypes,
                    constFormalFieldTy baseTy
                )
            )
        ]

    let viaGetAltNameProperty () =
        [
            mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "get_" + altName, [], constFormalFieldTy baseTy))
        ]

    // If helpers exist, use them
    match cuspec.HasHelpers with
    | AllHelpers
    | SpecialFSharpListHelpers
    | SpecialFSharpOptionHelpers ->
        if cuspecRepr.RepresentAlternativeAsNull(cuspec, alt) then
            [ AI_ldnull ]
        elif alt.IsNullary then
            viaGetAltNameProperty ()
        else
            viaMakerCall ()

    | NoHelpers when (not alt.IsNullary) && cuspecRepr.RepresentAlternativeAsStructValue cuspec -> viaMakerCall ()
    | NoHelpers when cuspecRepr.MaintainPossiblyUniqueConstantFieldForAlternative(cuspec, alt) -> viaGetAltNameProperty ()
    | NoHelpers -> convNewDataInstrInternal ilg cuspec cidx

let mkIsData ilg (avoidHelpers, cuspec, cidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let altName = alt.Name

    if cuspecRepr.RepresentAlternativeAsNull(cuspec, alt) then
        [ AI_ldnull; AI_ceq ]
    elif cuspecRepr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull(cuspec, alt) then
        // in this case we can use a null test
        [ AI_ldnull; AI_cgt_un ]
    else
        match cuspecRepr.DiscriminationTechnique cuspec with
        | SingleCase -> [ mkLdcInt32 1 ]
        | RuntimeTypes -> mkRuntimeTypeDiscriminate ilg avoidHelpers cuspec alt altName altTy
        | IntegerTag -> mkTagDiscriminate ilg cuspec (baseTyOfUnionSpec cuspec) cidx
        | TailOrNull ->
            match cidx with
            | TagNil -> [ mkGetTailOrNull avoidHelpers cuspec; AI_ldnull; AI_ceq ]
            | TagCons -> [ mkGetTailOrNull avoidHelpers cuspec; AI_ldnull; AI_cgt_un ]
            | _ -> failwith "mkIsData - unexpected"

type ICodeGen<'Mark> =
    abstract CodeLabel: 'Mark -> ILCodeLabel
    abstract GenerateDelayMark: unit -> 'Mark
    abstract GenLocal: ILType -> uint16
    abstract SetMarkToHere: 'Mark -> unit
    abstract EmitInstr: ILInstr -> unit
    abstract EmitInstrs: ILInstr list -> unit
    abstract MkInvalidCastExnNewobj: unit -> ILInstr

let genWith g : ILCode =
    let instrs = ResizeArray()
    let lab2pc = Dictionary()

    g
        { new ICodeGen<ILCodeLabel> with
            member _.CodeLabel(m) = m
            member _.GenerateDelayMark() = generateCodeLabel ()
            member _.GenLocal(ilTy) = failwith "not needed"
            member _.SetMarkToHere(m) = lab2pc[m] <- instrs.Count
            member _.EmitInstr x = instrs.Add x

            member cg.EmitInstrs xs =
                for i in xs do
                    cg.EmitInstr i

            member _.MkInvalidCastExnNewobj() = failwith "not needed"
        }

    {
        Labels = lab2pc
        Instrs = instrs.ToArray()
        Exceptions = []
        Locals = []
    }

let mkBrIsData ilg sense (avoidHelpers, cuspec, cidx, tg) =
    let neg = (if sense then BI_brfalse else BI_brtrue)
    let pos = (if sense then BI_brtrue else BI_brfalse)
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let altName = alt.Name

    if cuspecRepr.RepresentAlternativeAsNull(cuspec, alt) then
        [ I_brcmp(neg, tg) ]
    elif cuspecRepr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull(cuspec, alt) then
        // in this case we can use a null test
        [ I_brcmp(pos, tg) ]
    else
        match cuspecRepr.DiscriminationTechnique cuspec with
        | SingleCase -> []
        | RuntimeTypes -> mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy (I_brcmp(pos, tg))
        | IntegerTag -> mkTagDiscriminateThen ilg cuspec cidx (I_brcmp(pos, tg))
        | TailOrNull ->
            match cidx with
            | TagNil -> [ mkGetTailOrNull avoidHelpers cuspec; I_brcmp(neg, tg) ]
            | TagCons -> [ mkGetTailOrNull avoidHelpers cuspec; I_brcmp(pos, tg) ]
            | _ -> failwith "mkBrIsData - unexpected"

let emitLdDataTagPrim ilg ldOpt (cg: ICodeGen<'Mark>) (avoidHelpers, cuspec: IlxUnionSpec) =
    // If helpers exist, use them
    match cuspec.HasHelpers with
    | SpecialFSharpListHelpers
    | AllHelpers when not avoidHelpers ->
        ldOpt |> Option.iter cg.EmitInstr
        cg.EmitInstr(mkGetTagFromHelpers ilg cuspec)
    | _ ->

        let alts = cuspec.Alternatives

        match cuspecRepr.DiscriminationTechnique cuspec with
        | TailOrNull ->
            // leaves 1 if cons, 0 if not
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstrs [ mkGetTailOrNull avoidHelpers cuspec; AI_ldnull; AI_cgt_un ]
        | IntegerTag ->
            let baseTy = baseTyOfUnionSpec cuspec
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstr(mkGetTagFromField ilg cuspec baseTy)
        | SingleCase ->
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstrs [ AI_pop; mkLdcInt32 0 ]
        | RuntimeTypes ->
            let baseTy = baseTyOfUnionSpec cuspec

            let ld =
                match ldOpt with
                | None ->
                    let locn = cg.GenLocal baseTy
                    // Add on a branch to the first input label.  This gets optimized away by the printer/emitter.
                    cg.EmitInstr(mkStloc locn)
                    mkLdloc locn
                | Some i -> i

            let outlab = cg.GenerateDelayMark()

            let emitCase cidx =
                let alt = altOfUnionSpec cuspec cidx
                let internalLab = cg.GenerateDelayMark()
                let failLab = cg.GenerateDelayMark()
                let cmpNull = cuspecRepr.RepresentAlternativeAsNull(cuspec, alt)

                let test =
                    I_brcmp((if cmpNull then BI_brtrue else BI_brfalse), cg.CodeLabel failLab)

                let testBlock =
                    if
                        cmpNull
                        || cuspecRepr.RepresentAlternativeAsFreshInstancesOfRootClass(cuspec, alt)
                    then
                        [ test ]
                    else
                        let altName = alt.Name
                        let altTy = tyForAlt cuspec alt
                        mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy test

                cg.EmitInstrs(ld :: testBlock)
                cg.SetMarkToHere internalLab
                cg.EmitInstrs [ mkLdcInt32 cidx; I_br(cg.CodeLabel outlab) ]
                cg.SetMarkToHere failLab

            // Make the blocks for the remaining tests.
            for n in alts.Length - 1 .. -1 .. 1 do
                emitCase n

            // Make the block for the last test.
            cg.EmitInstr(mkLdcInt32 0)
            cg.SetMarkToHere outlab

let emitLdDataTag ilg (cg: ICodeGen<'Mark>) (avoidHelpers, cuspec: IlxUnionSpec) =
    emitLdDataTagPrim ilg None cg (avoidHelpers, cuspec)

let emitCastData ilg (cg: ICodeGen<'Mark>) (canfail, avoidHelpers, cuspec, cidx) =
    let alt = altOfUnionSpec cuspec cidx

    if cuspecRepr.RepresentAlternativeAsNull(cuspec, alt) then
        if canfail then
            let outlab = cg.GenerateDelayMark()
            let internal1 = cg.GenerateDelayMark()
            cg.EmitInstrs [ AI_dup; I_brcmp(BI_brfalse, cg.CodeLabel outlab) ]
            cg.SetMarkToHere internal1
            cg.EmitInstrs [ cg.MkInvalidCastExnNewobj(); I_throw ]
            cg.SetMarkToHere outlab
        else
            // If it can't fail, it's still verifiable just to leave the value on the stack unchecked
            ()
    elif cuspecRepr.Flatten cuspec then
        if canfail then
            let outlab = cg.GenerateDelayMark()
            let internal1 = cg.GenerateDelayMark()
            cg.EmitInstr AI_dup
            emitLdDataTagPrim ilg None cg (avoidHelpers, cuspec)
            cg.EmitInstrs [ mkLdcInt32 cidx; I_brcmp(BI_beq, cg.CodeLabel outlab) ]
            cg.SetMarkToHere internal1
            cg.EmitInstrs [ cg.MkInvalidCastExnNewobj(); I_throw ]
            cg.SetMarkToHere outlab
        else
            // If it can't fail, it's still verifiable just to leave the value on the stack unchecked
            ()
    elif cuspecRepr.OptimizeAlternativeToRootClass(cuspec, alt) then
        ()
    else
        let altTy = tyForAlt cuspec alt
        cg.EmitInstr(I_castclass altTy)

let emitDataSwitch ilg (cg: ICodeGen<'Mark>) (avoidHelpers, cuspec, cases) =
    let baseTy = baseTyOfUnionSpec cuspec

    match cuspecRepr.DiscriminationTechnique cuspec with
    | RuntimeTypes ->
        let locn = cg.GenLocal baseTy

        cg.EmitInstr(mkStloc locn)

        for cidx, tg in cases do
            let alt = altOfUnionSpec cuspec cidx
            let altTy = tyForAlt cuspec alt
            let altName = alt.Name
            let failLab = cg.GenerateDelayMark()
            let cmpNull = cuspecRepr.RepresentAlternativeAsNull(cuspec, alt)

            cg.EmitInstr(mkLdloc locn)
            let testInstr = I_brcmp((if cmpNull then BI_brfalse else BI_brtrue), tg)

            if
                cmpNull
                || cuspecRepr.RepresentAlternativeAsFreshInstancesOfRootClass(cuspec, alt)
            then
                cg.EmitInstr testInstr
            else
                cg.EmitInstrs(mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy testInstr)

            cg.SetMarkToHere failLab

    | IntegerTag ->
        match cases with
        | [] -> cg.EmitInstr AI_pop
        | _ ->
            // Use a dictionary to avoid quadratic lookup in case list
            let dict = Dictionary<int, _>()

            for i, case in cases do
                dict[i] <- case

            let failLab = cg.GenerateDelayMark()

            let emitCase i _ =
                match dict.TryGetValue i with
                | true, res -> res
                | _ -> cg.CodeLabel failLab

            let dests = Array.mapi emitCase cuspec.AlternativesArray
            cg.EmitInstr(mkGetTag ilg cuspec)
            cg.EmitInstr(I_switch(Array.toList dests))
            cg.SetMarkToHere failLab

    | SingleCase ->
        match cases with
        | [ (0, tg) ] -> cg.EmitInstrs [ AI_pop; I_br tg ]
        | [] -> cg.EmitInstr AI_pop
        | _ -> failwith "unexpected: strange switch on single-case unions should not be present"

    | TailOrNull -> failwith "unexpected: switches on lists should have been eliminated to brisdata tests"

//---------------------------------------------------
// Generate the union classes

let mkMethodsAndPropertiesForFields
    (addMethodGeneratedAttrs, addPropertyGeneratedAttrs)
    (g: TcGlobals)
    access
    attr
    imports
    hasHelpers
    (ilTy: ILType)
    (fields: IlxUnionCaseField[])
    =
    let basicProps =
        fields
        |> Array.map (fun field ->
            ILPropertyDef(
                name = adjustFieldName hasHelpers field.Name,
                attributes = PropertyAttributes.None,
                setMethod = None,
                getMethod =
                    Some(
                        mkILMethRef (
                            ilTy.TypeRef,
                            ILCallingConv.Instance,
                            "get_" + adjustFieldName hasHelpers field.Name,
                            0,
                            [],
                            field.Type
                        )
                    ),
                callingConv = ILThisConvention.Instance,
                propertyType = field.Type,
                init = None,
                args = [],
                customAttrs = field.ILField.CustomAttrs
            )
            |> addPropertyGeneratedAttrs)
        |> Array.toList

    let basicMethods =
        [
            for field in fields do
                let fspec = mkILFieldSpecInTy (ilTy, field.LowerName, field.Type)

                let ilReturn = mkILReturn field.Type

                let ilReturn =
                    match getFieldsNullability g field.ILField with
                    | None -> ilReturn
                    | Some a -> ilReturn.WithCustomAttrs(mkILCustomAttrsFromArray [| a |])

                yield
                    mkILNonGenericInstanceMethod (
                        "get_" + adjustFieldName hasHelpers field.Name,
                        access,
                        [],
                        ilReturn,
                        mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkLdarg 0us; mkNormalLdfld fspec ], attr, imports)
                    )
                    |> addMethodGeneratedAttrs

        ]

    basicProps, basicMethods

let convAlternativeDef
    (
        addMethodGeneratedAttrs,
        addPropertyGeneratedAttrs,
        addPropertyNeverAttrs,
        addFieldGeneratedAttrs,
        addFieldNeverAttrs,
        mkDebuggerTypeProxyAttribute
    )
    (g: TcGlobals)
    num
    (td: ILTypeDef)
    (cud: IlxUnionInfo)
    info
    cuspec
    (baseTy: ILType)
    (alt: IlxUnionCase)
    =

    let imports = cud.DebugImports
    let attr = cud.DebugPoint
    let altName = alt.Name
    let fields = alt.FieldDefs
    let altTy = tyForAlt cuspec alt
    let repr = cudefRepr

    // Attributes on unions get attached to the construction methods in the helpers
    let addAltAttribs (mdef: ILMethodDef) =
        mdef.With(customAttrs = alt.altCustomAttrs)

    // The stdata instruction is only ever used for the F# "List" type
    //
    // Microsoft.FSharp.Collections.List`1 is indeed logically immutable, but we use mutation on this type internally
    // within FSharp.Core.dll on fresh unpublished cons cells.
    let isTotallyImmutable = (cud.HasHelpers <> SpecialFSharpListHelpers)

    let makeNonNullaryMakerMethod () =
        let locals, ilInstrs =
            if repr.RepresentAlternativeAsStructValue info then
                let local = mkILLocal baseTy None
                let ldloca = I_ldloca(0us)

                let ilInstrs =
                    [
                        ldloca
                        ILInstr.I_initobj baseTy
                        if (repr.DiscriminationTechnique info) = IntegerTag && num <> 0 then
                            ldloca
                            mkLdcInt32 num
                            mkSetTagToField g.ilg cuspec baseTy
                        for i in 0 .. fields.Length - 1 do
                            ldloca
                            mkLdarg (uint16 i)
                            mkNormalStfld (mkILFieldSpecInTy (baseTy, fields[i].LowerName, fields[i].Type))
                        mkLdloc 0us
                    ]

                [ local ], ilInstrs
            else
                let ilInstrs =
                    [
                        for i in 0 .. fields.Length - 1 do
                            mkLdarg (uint16 i)
                        yield! convNewDataInstrInternal g.ilg cuspec num
                    ]

                [], ilInstrs

        let mdef =
            mkILNonGenericStaticMethod (
                mkMakerName cuspec altName,
                cud.HelpersAccessibility,
                fields
                |> Array.map (fun fd ->
                    let plainParam = mkILParamNamed (fd.LowerName, fd.Type)

                    match getFieldsNullability g fd.ILField with
                    | None -> plainParam
                    | Some a ->
                        { plainParam with
                            CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrsFromArray [| a |])
                        })

                |> Array.toList,
                mkILReturn baseTy,
                mkMethodBody (true, locals, fields.Length + locals.Length, nonBranchingInstrsToCode ilInstrs, attr, imports)
            )
            |> addAltAttribs
            |> addMethodGeneratedAttrs

        mdef

    let altUniqObjMeths =

        // This method is only generated if helpers are not available. It fetches the unique object for the alternative
        // without exposing direct access to the underlying field
        match cud.HasHelpers with
        | AllHelpers
        | SpecialFSharpOptionHelpers
        | SpecialFSharpListHelpers -> []
        | _ ->
            if
                alt.IsNullary
                && repr.MaintainPossiblyUniqueConstantFieldForAlternative(info, alt)
            then
                let methName = "get_" + altName

                let meth =
                    mkILNonGenericStaticMethod (
                        methName,
                        cud.UnionCasesAccessibility,
                        [],
                        mkILReturn (baseTy),
                        mkMethodBody (
                            true,
                            [],
                            fields.Length,
                            nonBranchingInstrsToCode [ I_ldsfld(Nonvolatile, mkConstFieldSpec altName baseTy) ],
                            attr,
                            imports
                        )
                    )
                    |> addMethodGeneratedAttrs

                [ meth ]

            else
                []

    let baseMakerMeths, baseMakerProps =

        match cud.HasHelpers with
        | AllHelpers
        | SpecialFSharpOptionHelpers
        | SpecialFSharpListHelpers ->

            let baseTesterMeths, baseTesterProps =
                if cud.UnionCases.Length <= 1 then
                    [], []
                elif repr.RepresentOneAlternativeAsNull info then
                    [], []
                else
                    let additionalAttributes =
                        if
                            g.checkNullness
                            && g.langFeatureNullness
                            && repr.RepresentAlternativeAsStructValue info
                            && not alt.IsNullary
                        then
                            let notnullfields =
                                alt.FieldDefs
                                // Fields that are nullable even from F# perspective has an [Nullable] attribute on them
                                // Non-nullable fields are implicit in F#, therefore not annotated separately
                                |> Array.filter (fun f -> TryFindILAttribute g.attrib_NullableAttribute f.ILField.CustomAttrs |> not)

                            let fieldNames =
                                notnullfields
                                |> Array.map (fun f -> f.LowerName)
                                |> Array.append (notnullfields |> Array.map (fun f -> f.Name))

                            if fieldNames |> Array.isEmpty then
                                emptyILCustomAttrs
                            else
                                mkILCustomAttrsFromArray [| GetNotNullWhenTrueAttribute g fieldNames |]

                        else
                            emptyILCustomAttrs

                    [
                        (mkILNonGenericInstanceMethod (
                            "get_" + mkTesterName altName,
                            cud.HelpersAccessibility,
                            [],
                            mkILReturn g.ilg.typ_Bool,
                            mkMethodBody (
                                true,
                                [],
                                2,
                                nonBranchingInstrsToCode ([ mkLdarg0 ] @ mkIsData g.ilg (true, cuspec, num)),
                                attr,
                                imports
                            )
                        ))
                            .With(customAttrs = additionalAttributes)
                        |> addMethodGeneratedAttrs
                    ],
                    [
                        ILPropertyDef(
                            name = mkTesterName altName,
                            attributes = PropertyAttributes.None,
                            setMethod = None,
                            getMethod =
                                Some(
                                    mkILMethRef (
                                        baseTy.TypeRef,
                                        ILCallingConv.Instance,
                                        "get_" + mkTesterName altName,
                                        0,
                                        [],
                                        g.ilg.typ_Bool
                                    )
                                ),
                            callingConv = ILThisConvention.Instance,
                            propertyType = g.ilg.typ_Bool,
                            init = None,
                            args = [],
                            customAttrs = additionalAttributes
                        )
                        |> addPropertyGeneratedAttrs
                        |> addPropertyNeverAttrs
                    ]

            let baseMakerMeths, baseMakerProps =

                if alt.IsNullary then
                    let attributes =
                        if
                            g.checkNullness
                            && g.langFeatureNullness
                            && repr.RepresentAlternativeAsNull(info, alt)
                        then
                            let noTypars = td.GenericParams.Length

                            GetNullableAttribute
                                g
                                [
                                    yield NullnessInfo.WithNull // The top-level value itself, e.g. option, is nullable
                                    yield! List.replicate noTypars NullnessInfo.AmbivalentToNull
                                ] // The typars are not (i.e. do not change option<string> into option<string?>
                            |> Array.singleton
                            |> mkILCustomAttrsFromArray
                        else
                            emptyILCustomAttrs

                    let nullaryMeth =
                        mkILNonGenericStaticMethod (
                            "get_" + altName,
                            cud.HelpersAccessibility,
                            [],
                            (mkILReturn baseTy).WithCustomAttrs attributes,
                            mkMethodBody (
                                true,
                                [],
                                fields.Length,
                                nonBranchingInstrsToCode (convNewDataInstrInternal g.ilg cuspec num),
                                attr,
                                imports
                            )
                        )
                        |> addAltAttribs
                        |> addMethodGeneratedAttrs

                    let nullaryProp =

                        ILPropertyDef(
                            name = altName,
                            attributes = PropertyAttributes.None,
                            setMethod = None,
                            getMethod = Some(mkILMethRef (baseTy.TypeRef, ILCallingConv.Static, "get_" + altName, 0, [], baseTy)),
                            callingConv = ILThisConvention.Static,
                            propertyType = baseTy,
                            init = None,
                            args = [],
                            customAttrs = attributes
                        )
                        |> addPropertyGeneratedAttrs
                        |> addPropertyNeverAttrs

                    [ nullaryMeth ], [ nullaryProp ]

                else
                    [ makeNonNullaryMakerMethod () ], []

            (baseMakerMeths @ baseTesterMeths), (baseMakerProps @ baseTesterProps)

        | NoHelpers when not (alt.IsNullary) && cuspecRepr.RepresentAlternativeAsStructValue(cuspec) ->
            // For non-nullary struct DUs, maker method is used to create their values.
            [ makeNonNullaryMakerMethod () ], []
        | NoHelpers -> [], []

    let typeDefs, altDebugTypeDefs, altNullaryFields =
        if repr.RepresentAlternativeAsNull(info, alt) then
            [], [], []
        elif repr.RepresentAlternativeAsFreshInstancesOfRootClass(info, alt) then
            [], [], []
        elif repr.RepresentAlternativeAsStructValue info then
            [], [], []
        else
            let altNullaryFields =
                if repr.MaintainPossiblyUniqueConstantFieldForAlternative(info, alt) then
                    let basic: ILFieldDef =
                        mkILStaticField (constFieldName altName, baseTy, None, None, ILMemberAccess.Assembly)
                        |> addFieldNeverAttrs
                        |> addFieldGeneratedAttrs

                    let uniqObjField = basic.WithInitOnly(true)
                    let inRootClass = cuspecRepr.OptimizeAlternativeToRootClass(cuspec, alt)
                    [ (info, alt, altTy, num, uniqObjField, inRootClass) ]
                else
                    []

            let typeDefs, altDebugTypeDefs =
                if repr.OptimizeAlternativeToRootClass(info, alt) then
                    [], []
                else

                    let altDebugTypeDefs, debugAttrs =
                        if not cud.GenerateDebugProxies then
                            [], []
                        else

                            let debugProxyTypeName = altTy.TypeSpec.Name + "@DebugTypeProxy"

                            let debugProxyTy =
                                mkILBoxedTy
                                    (mkILNestedTyRef (altTy.TypeSpec.Scope, altTy.TypeSpec.Enclosing, debugProxyTypeName))
                                    altTy.GenericArgs

                            let debugProxyFieldName = "_obj"

                            let debugProxyFields =
                                [
                                    mkILInstanceField (debugProxyFieldName, altTy, None, ILMemberAccess.Assembly)
                                    |> addFieldNeverAttrs
                                    |> addFieldGeneratedAttrs
                                ]

                            let debugProxyCode =
                                [
                                    mkLdarg0
                                    mkNormalCall (mkILCtorMethSpecForTy (g.ilg.typ_Object, []))
                                    mkLdarg0
                                    mkLdarg 1us
                                    mkNormalStfld (mkILFieldSpecInTy (debugProxyTy, debugProxyFieldName, altTy))
                                ]
                                |> nonBranchingInstrsToCode

                            let debugProxyCtor =
                                (mkILCtor (
                                    ILMemberAccess.Public (* must always be public - see jared parson blog entry on implementing debugger type proxy *) ,
                                    [ mkILParamNamed ("obj", altTy) ],
                                    mkMethodBody (false, [], 3, debugProxyCode, None, imports)
                                ))
                                    .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x660 baseTy ])
                                |> addMethodGeneratedAttrs

                            let debugProxyGetterMeths =
                                fields
                                |> Array.map (fun field ->
                                    let fldName, fldTy = mkUnionCaseFieldId field

                                    let instrs =
                                        [
                                            mkLdarg0
                                            (if td.IsStruct then mkNormalLdflda else mkNormalLdfld) (
                                                mkILFieldSpecInTy (debugProxyTy, debugProxyFieldName, altTy)
                                            )
                                            mkNormalLdfld (mkILFieldSpecInTy (altTy, fldName, fldTy))
                                        ]
                                        |> nonBranchingInstrsToCode

                                    let mbody = mkMethodBody (true, [], 2, instrs, None, imports)

                                    mkILNonGenericInstanceMethod (
                                        "get_" + field.Name,
                                        ILMemberAccess.Public,
                                        [],
                                        mkILReturn field.Type,
                                        mbody
                                    )
                                    |> addMethodGeneratedAttrs)
                                |> Array.toList

                            let debugProxyGetterProps =
                                fields
                                |> Array.map (fun fdef ->
                                    ILPropertyDef(
                                        name = fdef.Name,
                                        attributes = PropertyAttributes.None,
                                        setMethod = None,
                                        getMethod =
                                            Some(
                                                mkILMethRef (
                                                    debugProxyTy.TypeRef,
                                                    ILCallingConv.Instance,
                                                    "get_" + fdef.Name,
                                                    0,
                                                    [],
                                                    fdef.Type
                                                )
                                            ),
                                        callingConv = ILThisConvention.Instance,
                                        propertyType = fdef.Type,
                                        init = None,
                                        args = [],
                                        customAttrs = fdef.ILField.CustomAttrs
                                    )
                                    |> addPropertyGeneratedAttrs)
                                |> Array.toList

                            let debugProxyTypeDef =
                                mkILGenericClass (
                                    debugProxyTypeName,
                                    ILTypeDefAccess.Nested ILMemberAccess.Assembly,
                                    td.GenericParams,
                                    g.ilg.typ_Object,
                                    [],
                                    mkILMethods ([ debugProxyCtor ] @ debugProxyGetterMeths),
                                    mkILFields debugProxyFields,
                                    emptyILTypeDefs,
                                    mkILProperties debugProxyGetterProps,
                                    emptyILEvents,
                                    emptyILCustomAttrs,
                                    ILTypeInit.BeforeField
                                )

                            [ debugProxyTypeDef.WithSpecialName(true) ],
                            ([ mkDebuggerTypeProxyAttribute debugProxyTy ] @ cud.DebugDisplayAttributes)

                    let altTypeDef =
                        let basicFields =
                            fields
                            |> Array.map (fun field ->
                                let fldName, fldTy, attrs = mkUnionCaseFieldIdAndAttrs g field
                                let fdef = mkILInstanceField (fldName, fldTy, None, ILMemberAccess.Assembly)

                                let fdef =
                                    match attrs with
                                    | [] -> fdef
                                    | attrs -> fdef.With(customAttrs = mkILCustomAttrs attrs)

                                    |> addFieldNeverAttrs
                                    |> addFieldGeneratedAttrs

                                fdef.WithInitOnly(isTotallyImmutable))

                            |> Array.toList

                        let basicProps, basicMethods =
                            mkMethodsAndPropertiesForFields
                                (addMethodGeneratedAttrs, addPropertyGeneratedAttrs)
                                g
                                cud.UnionCasesAccessibility
                                attr
                                imports
                                cud.HasHelpers
                                altTy
                                fields

                        let basicCtorInstrs =
                            [
                                yield mkLdarg0
                                match repr.DiscriminationTechnique info with
                                | IntegerTag ->
                                    yield mkLdcInt32 num
                                    yield mkNormalCall (mkILCtorMethSpecForTy (baseTy, [ mkTagFieldType g.ilg cuspec ]))
                                | SingleCase
                                | RuntimeTypes -> yield mkNormalCall (mkILCtorMethSpecForTy (baseTy, []))
                                | TailOrNull -> failwith "unreachable"
                            ]

                        let basicCtorAccess =
                            (if cuspec.HasHelpers = AllHelpers then
                                 ILMemberAccess.Assembly
                             else
                                 cud.UnionCasesAccessibility)

                        let basicCtorFields =
                            basicFields
                            |> List.map (fun fdef ->
                                let nullableAttr = getFieldsNullability g fdef |> Option.toList
                                fdef.Name, fdef.FieldType, nullableAttr)

                        let basicCtorMeth =
                            (mkILStorageCtor (basicCtorInstrs, altTy, basicCtorFields, basicCtorAccess, attr, imports))
                                .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x660 baseTy ])
                            |> addMethodGeneratedAttrs

                        let attrs =
                            if g.checkNullness && g.langFeatureNullness then
                                GetNullableContextAttribute g 1uy :: debugAttrs
                            else
                                debugAttrs

                        let altTypeDef =
                            mkILGenericClass (
                                altTy.TypeSpec.Name,
                                // Types for nullary's become private, they also have names like _Empty
                                ILTypeDefAccess.Nested(
                                    if alt.IsNullary && cud.HasHelpers = IlxUnionHasHelpers.AllHelpers then
                                        ILMemberAccess.Assembly
                                    else
                                        cud.UnionCasesAccessibility
                                ),
                                td.GenericParams,
                                baseTy,
                                [],
                                mkILMethods ([ basicCtorMeth ] @ basicMethods),
                                mkILFields basicFields,
                                emptyILTypeDefs,
                                mkILProperties basicProps,
                                emptyILEvents,
                                mkILCustomAttrs attrs,
                                ILTypeInit.BeforeField
                            )

                        altTypeDef.WithSpecialName(true).WithSerializable(td.IsSerializable)

                    [ altTypeDef ], altDebugTypeDefs

            typeDefs, altDebugTypeDefs, altNullaryFields

    baseMakerMeths, baseMakerProps, altUniqObjMeths, typeDefs, altDebugTypeDefs, altNullaryFields

let mkClassUnionDef
    (
        addMethodGeneratedAttrs,
        addPropertyGeneratedAttrs,
        addPropertyNeverAttrs,
        addFieldGeneratedAttrs: ILFieldDef -> ILFieldDef,
        addFieldNeverAttrs: ILFieldDef -> ILFieldDef,
        mkDebuggerTypeProxyAttribute
    )
    (g: TcGlobals)
    tref
    (td: ILTypeDef)
    cud
    =
    let boxity = if td.IsStruct then ILBoxity.AsValue else ILBoxity.AsObject
    let baseTy = mkILFormalNamedTy boxity tref td.GenericParams

    let cuspec =
        IlxUnionSpec(IlxUnionRef(boxity, baseTy.TypeRef, cud.UnionCases, cud.IsNullPermitted, cud.HasHelpers), baseTy.GenericArgs)

    let info = (td, cud)
    let repr = cudefRepr
    let isTotallyImmutable = (cud.HasHelpers <> SpecialFSharpListHelpers)

    let results =
        cud.UnionCases
        |> List.ofArray
        |> List.mapi (fun i alt ->
            convAlternativeDef
                (addMethodGeneratedAttrs,
                 addPropertyGeneratedAttrs,
                 addPropertyNeverAttrs,
                 addFieldGeneratedAttrs,
                 addFieldNeverAttrs,
                 mkDebuggerTypeProxyAttribute)
                g
                i
                td
                cud
                info
                cuspec
                baseTy
                alt)

    let baseMethsFromAlt = results |> List.collect (fun (a, _, _, _, _, _) -> a)
    let basePropsFromAlt = results |> List.collect (fun (_, a, _, _, _, _) -> a)
    let altUniqObjMeths = results |> List.collect (fun (_, _, a, _, _, _) -> a)
    let altTypeDefs = results |> List.collect (fun (_, _, _, a, _, _) -> a)
    let altDebugTypeDefs = results |> List.collect (fun (_, _, _, _, a, _) -> a)
    let altNullaryFields = results |> List.collect (fun (_, _, _, _, _, a) -> a)

    let tagFieldsInObject =
        match repr.DiscriminationTechnique info with
        | SingleCase
        | RuntimeTypes
        | TailOrNull -> []
        | IntegerTag -> [ let n, t = mkTagFieldId g.ilg cuspec in n, t, [] ]

    let isStruct = td.IsStruct

    let ctorAccess =
        if cuspec.HasHelpers = AllHelpers then
            ILMemberAccess.Assembly
        else
            cud.UnionCasesAccessibility

    let selfFields, selfMeths, selfProps =

        [
            let minNullaryIdx =
                cud.UnionCases
                |> Array.tryFindIndex (fun t -> t.IsNullary)
                |> Option.defaultValue -1

            let fieldsEmitted = new HashSet<_>()

            for cidx, alt in Array.indexed cud.UnionCases do
                if
                    repr.RepresentAlternativeAsFreshInstancesOfRootClass(info, alt)
                    || repr.RepresentAlternativeAsStructValue info
                then

                    let baseInit =
                        if isStruct then
                            None
                        else
                            match td.Extends.Value with
                            | None -> Some g.ilg.typ_Object.TypeSpec
                            | Some ilTy -> Some ilTy.TypeSpec

                    let ctor =
                        // Structs with fields are created using static makers methods
                        // Structs without fields can share constructor for the 'tag' value, we just create one
                        if isStruct && not (cidx = minNullaryIdx) then
                            []
                        else
                            let fields =
                                alt.FieldDefs |> Array.map (mkUnionCaseFieldIdAndAttrs g) |> Array.toList

                            [
                                (mkILSimpleStorageCtor (
                                    baseInit,
                                    baseTy,
                                    [],
                                    (fields @ tagFieldsInObject),
                                    ctorAccess,
                                    cud.DebugPoint,
                                    cud.DebugImports
                                ))
                                    .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x660 baseTy ])
                                |> addMethodGeneratedAttrs
                            ]

                    let fieldDefs =
                        // Since structs are flattened out for all cases together, all boxed fields are potentially nullable
                        if
                            isStruct
                            && cud.UnionCases.Length > 1
                            && g.checkNullness
                            && g.langFeatureNullness
                        then
                            alt.FieldDefs
                            |> Array.map (fun field ->
                                if field.Type.IsNominal && field.Type.Boxity = AsValue then
                                    field
                                else
                                    let attrs =
                                        let existingAttrs = field.ILField.CustomAttrs.AsArray()

                                        let nullableIdx =
                                            existingAttrs |> Array.tryFindIndex (IsILAttrib g.attrib_NullableAttribute)

                                        match nullableIdx with
                                        | None ->
                                            existingAttrs
                                            |> Array.append [| GetNullableAttribute g [ NullnessInfo.WithNull ] |]
                                        | Some idx ->
                                            let replacementAttr =
                                                match existingAttrs[idx] with
                                                (*
                                                 The attribute carries either a single byte, or a list of bytes for the fields itself and all its generic type arguments
                                                 The way we lay out DUs does not affect nullability of the typars of a field, therefore we just change the very first byte
                                                 If the field was already declared as nullable (value = 2uy) or ambivalent(value = 0uy), we can keep it that way
                                                 If it was marked as non-nullable within that UnionCase, we have to convert it to WithNull (2uy) due to other cases being possible
                                                *)
                                                | Encoded(method, _data, [ ILAttribElem.Byte 1uy ]) ->
                                                    mkILCustomAttribMethRef (method, [ ILAttribElem.Byte 2uy ], [])
                                                | Encoded(method,
                                                          _data,
                                                          [ ILAttribElem.Array(elemType, (ILAttribElem.Byte 1uy) :: otherElems) ]) ->
                                                    mkILCustomAttribMethRef (
                                                        method,
                                                        [ ILAttribElem.Array(elemType, (ILAttribElem.Byte 2uy) :: otherElems) ],
                                                        []
                                                    )
                                                | attrAsBefore -> attrAsBefore

                                            existingAttrs |> Array.replace idx replacementAttr

                                    field.ILField.With(customAttrs = mkILCustomAttrsFromArray attrs)
                                    |> IlxUnionCaseField)
                        else
                            alt.FieldDefs

                    let fieldsToBeAddedIntoType =
                        fieldDefs
                        |> Array.filter (fun f -> fieldsEmitted.Add(struct (f.LowerName, f.Type)))

                    let fields =
                        fieldsToBeAddedIntoType
                        |> Array.map (mkUnionCaseFieldIdAndAttrs g)
                        |> Array.toList

                    let props, meths =
                        mkMethodsAndPropertiesForFields
                            (addMethodGeneratedAttrs, addPropertyGeneratedAttrs)
                            g
                            cud.UnionCasesAccessibility
                            cud.DebugPoint
                            cud.DebugImports
                            cud.HasHelpers
                            baseTy
                            fieldsToBeAddedIntoType

                    yield (fields, (ctor @ meths), props)
        ]
        |> List.unzip3
        |> (fun (a, b, c) -> List.concat a, List.concat b, List.concat c)

    let selfAndTagFields =
        [
            for fldName, fldTy, attrs in (selfFields @ tagFieldsInObject) do
                let fdef =
                    let fdef = mkILInstanceField (fldName, fldTy, None, ILMemberAccess.Assembly)

                    match attrs with
                    | [] -> fdef
                    | attrs -> fdef.With(customAttrs = mkILCustomAttrs attrs)

                    |> addFieldNeverAttrs
                    |> addFieldGeneratedAttrs

                yield fdef.WithInitOnly(not isStruct && isTotallyImmutable)
        ]

    let ctorMeths =
        if
            (List.isEmpty selfFields
             && List.isEmpty tagFieldsInObject
             && not (List.isEmpty selfMeths))
            || isStruct
            || cud.UnionCases
               |> Array.forall (fun alt -> repr.RepresentAlternativeAsFreshInstancesOfRootClass(info, alt))
        then

            [] (* no need for a second ctor in these cases *)

        else
            let baseTySpec =
                (match td.Extends.Value with
                 | None -> g.ilg.typ_Object
                 | Some ilTy -> ilTy)
                    .TypeSpec

            [
                (mkILSimpleStorageCtor (
                    Some baseTySpec,
                    baseTy,
                    [],
                    tagFieldsInObject,
                    ILMemberAccess.Assembly,
                    cud.DebugPoint,
                    cud.DebugImports
                ))
                    .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x7E0 baseTy ])
                |> addMethodGeneratedAttrs
            ]

    // Now initialize the constant fields wherever they are stored...
    let addConstFieldInit cd =
        if List.isEmpty altNullaryFields then
            cd
        else
            prependInstrsToClassCtor
                [
                    for info, _alt, altTy, fidx, fd, inRootClass in altNullaryFields do
                        let constFieldId = (fd.Name, baseTy)
                        let constFieldSpec = mkConstFieldSpecFromId baseTy constFieldId

                        match repr.DiscriminationTechnique info with
                        | SingleCase
                        | RuntimeTypes
                        | TailOrNull -> yield mkNormalNewobj (mkILCtorMethSpecForTy (altTy, []))
                        | IntegerTag ->
                            if inRootClass then
                                yield mkLdcInt32 fidx
                                yield mkNormalNewobj (mkILCtorMethSpecForTy (altTy, [ mkTagFieldType g.ilg cuspec ]))
                            else
                                yield mkNormalNewobj (mkILCtorMethSpecForTy (altTy, []))

                        yield mkNormalStsfld constFieldSpec
                ]
                cud.DebugPoint
                cud.DebugImports
                cd

    let tagMeths, tagProps, tagEnumFields =
        let tagFieldType = mkTagFieldType g.ilg cuspec

        let tagEnumFields =
            cud.UnionCases
            |> Array.mapi (fun num alt -> mkILLiteralField (alt.Name, tagFieldType, ILFieldInit.Int32 num, None, ILMemberAccess.Public))
            |> Array.toList

        let tagMeths, tagProps =

            let code =
                genWith (fun cg ->
                    emitLdDataTagPrim g.ilg (Some mkLdarg0) cg (true, cuspec)
                    cg.EmitInstr I_ret)

            let body = mkMethodBody (true, [], 2, code, cud.DebugPoint, cud.DebugImports)
            // // If we are using NULL as a representation for an element of this type then we cannot
            // // use an instance method
            if (repr.RepresentOneAlternativeAsNull info) then
                [
                    mkILNonGenericStaticMethod (
                        "Get" + tagPropertyName,
                        cud.HelpersAccessibility,
                        [ mkILParamAnon baseTy ],
                        mkILReturn tagFieldType,
                        body
                    )
                    |> addMethodGeneratedAttrs
                ],
                []

            else
                [
                    mkILNonGenericInstanceMethod ("get_" + tagPropertyName, cud.HelpersAccessibility, [], mkILReturn tagFieldType, body)
                    |> addMethodGeneratedAttrs
                ],

                [
                    ILPropertyDef(
                        name = tagPropertyName,
                        attributes = PropertyAttributes.None,
                        setMethod = None,
                        getMethod =
                            Some(mkILMethRef (baseTy.TypeRef, ILCallingConv.Instance, "get_" + tagPropertyName, 0, [], tagFieldType)),
                        callingConv = ILThisConvention.Instance,
                        propertyType = tagFieldType,
                        init = None,
                        args = [],
                        customAttrs = emptyILCustomAttrs
                    )
                    |> addPropertyGeneratedAttrs
                    |> addPropertyNeverAttrs
                ]

        tagMeths, tagProps, tagEnumFields

    // The class can be abstract if each alternative is represented by a derived type
    let isAbstract = (altTypeDefs.Length = cud.UnionCases.Length)

    let existingMeths = td.Methods.AsList()
    let existingProps = td.Properties.AsList()

    let enumTypeDef =
        // The nested Tags type is elided if there is only one tag
        // The Tag property is NOT elided if there is only one tag
        if tagEnumFields.Length <= 1 then
            None
        else
            let tdef =
                ILTypeDef(
                    name = "Tags",
                    nestedTypes = emptyILTypeDefs,
                    genericParams = td.GenericParams,
                    attributes = enum 0,
                    layout = ILTypeDefLayout.Auto,
                    implements = [],
                    extends = Some g.ilg.typ_Object,
                    methods = emptyILMethods,
                    securityDecls = emptyILSecurityDecls,
                    fields = mkILFields tagEnumFields,
                    methodImpls = emptyILMethodImpls,
                    events = emptyILEvents,
                    properties = emptyILProperties,
                    customAttrs = emptyILCustomAttrsStored
                )
                    .WithNestedAccess(cud.UnionCasesAccessibility)
                    .WithAbstract(true)
                    .WithSealed(true)
                    .WithImport(false)
                    .WithEncoding(ILDefaultPInvokeEncoding.Ansi)
                    .WithHasSecurity(false)

            Some tdef

    let baseTypeDef =
        td
            .WithInitSemantics(ILTypeInit.BeforeField)
            .With(
                nestedTypes =
                    mkILTypeDefs (
                        Option.toList enumTypeDef
                        @ altTypeDefs
                        @ altDebugTypeDefs
                        @ td.NestedTypes.AsList()
                    ),
                extends =
                    (match td.Extends.Value with
                     | None -> Some g.ilg.typ_Object |> notlazy
                     | _ -> td.Extends),
                methods =
                    mkILMethods (
                        ctorMeths
                        @ baseMethsFromAlt
                        @ selfMeths
                        @ tagMeths
                        @ altUniqObjMeths
                        @ existingMeths
                    ),
                fields =
                    mkILFields (
                        selfAndTagFields
                        @ List.map (fun (_, _, _, _, fdef, _) -> fdef) altNullaryFields
                        @ td.Fields.AsList()
                    ),
                properties = mkILProperties (tagProps @ basePropsFromAlt @ selfProps @ existingProps),
                customAttrs =
                    if cud.IsNullPermitted && g.checkNullness && g.langFeatureNullness then
                        td.CustomAttrs.AsArray()
                        |> Array.append [| GetNullableAttribute g [ NullnessInfo.WithNull ] |]
                        |> mkILCustomAttrsFromArray
                        |> storeILCustomAttrs
                    else
                        td.CustomAttrsStored
            )
        // The .cctor goes on the Cases type since that's where the constant fields for nullary constructors live
        |> addConstFieldInit

    baseTypeDef.WithAbstract(isAbstract).WithSealed(altTypeDefs.IsEmpty)
