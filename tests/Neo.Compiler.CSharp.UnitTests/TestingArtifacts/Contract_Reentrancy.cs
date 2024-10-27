using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Reentrancy(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Reentrancy"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""hasReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""hasReentrancyFromSingleBasicBlock"",""parameters"":[],""returntype"":""Void"",""offset"":115,""safe"":false},{""name"":""hasReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":219,""safe"":false},{""name"":""noReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":309,""safe"":false},{""name"":""noReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":413,""safe"":false},{""name"":""noReentrancyFromJump"",""parameters"":[{""name"":""input"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":432,""safe"":false},{""name"":""noReentrancyByAttribute"",""parameters"":[],""returntype"":""Void"",""offset"":543,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":694,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP25AlcBADtcAAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRT0VcBEMAQHbMEGb9mfOQeY/GIQ9AkALEAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkURDAEB2zBBm/ZnzkHmPxiEQAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRTQDQBEMAQHbMEGb9mfOQeY/GIQLEAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkVAEQwBAdswQZv2Z85B5j8YhDSIQFcAAXgmWgsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRUARDAEB2zBBm/ZnzkHmPxiEQFjYJhsLCxLADAtub1JlZW50cmFudAH/ABJNNA9gWDQrNTT+//9YNF1AVwADekp4EVHQRUGb9mfOeRGIThBR0FASwEp4EFHQRUBXAQF4Ec54EM7BRVOLUEGSXegxcGjYDA9BbHJlYWR5IGVudGVyZWThEXgRzngQzsFFU4tQQeY/GIRAVwABeBHOeBDOwUVTi1BBL1jF7UBWAUBhsDXX"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAO1wACxAMAAAAAAAAAAAAAAAAAAAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MdHJhbnNmZXIM9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkU9FXARDAHbMEGb9mfOQeY/GIQ9AkA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.TRY 5C00
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 1E : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 34 : OpCode.PUSH4
    /// 35 : OpCode.PACK
    /// 36 : OpCode.PUSH15
    /// 37 : OpCode.PUSHDATA1 7472616E73666572
    /// 41 : OpCode.PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF
    /// 57 : OpCode.SYSCALL 627D5B52
    /// 5C : OpCode.DROP
    /// 5D : OpCode.ENDTRY 15
    /// 5F : OpCode.STLOC0
    /// 60 : OpCode.PUSH1
    /// 61 : OpCode.PUSHDATA1 01
    /// 64 : OpCode.CONVERT 30
    /// 66 : OpCode.SYSCALL 9BF667CE
    /// 6B : OpCode.SYSCALL E63F1884
    /// 70 : OpCode.ENDTRY 02
    /// 72 : OpCode.RET
    /// </remarks>
    [DisplayName("hasReentrancy")]
    public abstract void HasReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMAAAAAAAAAAAAAAAAAAAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MdHJhbnNmZXIMz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUkU0A0A=
    /// 00 : OpCode.PUSHNULL
    /// 01 : OpCode.PUSH0
    /// 02 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 18 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 2E : OpCode.PUSH4
    /// 2F : OpCode.PACK
    /// 30 : OpCode.PUSH15
    /// 31 : OpCode.PUSHDATA1 7472616E73666572
    /// 3B : OpCode.PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2
    /// 51 : OpCode.SYSCALL 627D5B52
    /// 56 : OpCode.DROP
    /// 57 : OpCode.CALL 03
    /// 59 : OpCode.RET
    /// </remarks>
    [DisplayName("hasReentrancyFromCall")]
    public abstract void HasReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMAAAAAAAAAAAAAAAAAAAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MdHJhbnNmZXIM9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkURDAHbMEGb9mfOQeY/GIRA
    /// 00 : OpCode.PUSHNULL
    /// 01 : OpCode.PUSH0
    /// 02 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 18 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 2E : OpCode.PUSH4
    /// 2F : OpCode.PACK
    /// 30 : OpCode.PUSH15
    /// 31 : OpCode.PUSHDATA1 7472616E73666572
    /// 3B : OpCode.PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF
    /// 51 : OpCode.SYSCALL 627D5B52
    /// 56 : OpCode.DROP
    /// 57 : OpCode.PUSH1
    /// 58 : OpCode.PUSHDATA1 01
    /// 5B : OpCode.CONVERT 30
    /// 5D : OpCode.SYSCALL 9BF667CE
    /// 62 : OpCode.SYSCALL E63F1884
    /// 67 : OpCode.RET
    /// </remarks>
    [DisplayName("hasReentrancyFromSingleBasicBlock")]
    public abstract void HasReentrancyFromSingleBasicBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwB2zBBm/ZnzkHmPxiECxAMAAAAAAAAAAAAAAAAAAAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MdHJhbnNmZXIM9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkVA
    /// 00 : OpCode.PUSH1
    /// 01 : OpCode.PUSHDATA1 01
    /// 04 : OpCode.CONVERT 30
    /// 06 : OpCode.SYSCALL 9BF667CE
    /// 0B : OpCode.SYSCALL E63F1884
    /// 10 : OpCode.PUSHNULL
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 28 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 3E : OpCode.PUSH4
    /// 3F : OpCode.PACK
    /// 40 : OpCode.PUSH15
    /// 41 : OpCode.PUSHDATA1 7472616E73666572
    /// 4B : OpCode.PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF
    /// 61 : OpCode.SYSCALL 627D5B52
    /// 66 : OpCode.DROP
    /// 67 : OpCode.RET
    /// </remarks>
    [DisplayName("noReentrancy")]
    public abstract void NoReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmGwsLEsAMbm9SZWVudHJhbnQB/wASTTQPYFg0KzU0/v//WDRdQA==
    /// 00 : OpCode.LDSFLD0
    /// 01 : OpCode.ISNULL
    /// 02 : OpCode.JMPIFNOT 1B
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PACK
    /// 08 : OpCode.PUSHDATA1 6E6F5265656E7472616E74
    /// 15 : OpCode.PUSHINT16 FF00
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.PICK
    /// 1A : OpCode.CALL 0F
    /// 1C : OpCode.STSFLD0
    /// 1D : OpCode.LDSFLD0
    /// 1E : OpCode.CALL 2B
    /// 20 : OpCode.CALL_L 34FEFFFF
    /// 25 : OpCode.LDSFLD0
    /// 26 : OpCode.CALL 5D
    /// 28 : OpCode.RET
    /// </remarks>
    [DisplayName("noReentrancyByAttribute")]
    public abstract void NoReentrancyByAttribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwB2zBBm/ZnzkHmPxiENIhA
    /// 00 : OpCode.PUSH1
    /// 01 : OpCode.PUSHDATA1 01
    /// 04 : OpCode.CONVERT 30
    /// 06 : OpCode.SYSCALL 9BF667CE
    /// 0B : OpCode.SYSCALL E63F1884
    /// 10 : OpCode.CALL 88
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCZaCxAMAAAAAAAAAAAAAAAAAAAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MdHJhbnNmZXIMz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUkVAEQwB2zBBm/ZnzkHmPxiEQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.JMPIFNOT 5A
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 1E : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 34 : OpCode.PUSH4
    /// 35 : OpCode.PACK
    /// 36 : OpCode.PUSH15
    /// 37 : OpCode.PUSHDATA1 7472616E73666572
    /// 41 : OpCode.PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2
    /// 57 : OpCode.SYSCALL 627D5B52
    /// 5C : OpCode.DROP
    /// 5D : OpCode.RET
    /// 5E : OpCode.PUSH1
    /// 5F : OpCode.PUSHDATA1 01
    /// 62 : OpCode.CONVERT 30
    /// 64 : OpCode.SYSCALL 9BF667CE
    /// 69 : OpCode.SYSCALL E63F1884
    /// 6E : OpCode.RET
    /// </remarks>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion
}
