using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Reentrancy(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Reentrancy"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""hasReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""hasReentrancyFromSingleBasicBlock"",""parameters"":[],""returntype"":""Void"",""offset"":115,""safe"":false},{""name"":""hasReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":219,""safe"":false},{""name"":""noReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":309,""safe"":false},{""name"":""noReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":413,""safe"":false},{""name"":""noReentrancyFromJump"",""parameters"":[{""name"":""input"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":432,""safe"":false},{""name"":""noReentrancyByAttribute"",""parameters"":[],""returntype"":""Void"",""offset"":543,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":696,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP27AlcBADtcAAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRT0VcBEMAQHbMEGb9mfOQeY/GIQ9AkALEAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkURDAEB2zBBm/ZnzkHmPxiEQAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRTQDQBEMAQHbMEGb9mfOQeY/GIQLEAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkVAEQwBAdswQZv2Z85B5j8YhDSIQFcAAXgmWgsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRUARDAEB2zBBm/ZnzkHmPxiEQFjYJhsLCxLADAtub1JlZW50cmFudAH/ABJNNA9gWDQrNTT+//9YNF9AVwADekp4EVHQRUGb9mfOeRGIThBR0FASwEp4EFHQRUBXAQF4Ec54EM7BRVOLUEGSXegxcGjYJBQMD0FscmVhZHkgZW50ZXJlZOAReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFYBQMghjhM="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAO1wACxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1JFPRVwEQwBAdswQZv2Z85B5j8YhD0CQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.TRY 5C00 [4 datoshi]
    /// 06 : OpCode.PUSHNULL [1 datoshi]
    /// 07 : OpCode.PUSH0 [1 datoshi]
    /// 08 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1E : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 34 : OpCode.PUSH4 [1 datoshi]
    /// 35 : OpCode.PACK [2048 datoshi]
    /// 36 : OpCode.PUSH15 [1 datoshi]
    /// 37 : OpCode.PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 41 : OpCode.PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// 57 : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 5C : OpCode.DROP [2 datoshi]
    /// 5D : OpCode.ENDTRY 15 [4 datoshi]
    /// 5F : OpCode.STLOC0 [2 datoshi]
    /// 60 : OpCode.PUSH1 [1 datoshi]
    /// 61 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 64 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 66 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 6B : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 70 : OpCode.ENDTRY 02 [4 datoshi]
    /// 72 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancy")]
    public abstract void HasReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFNANA
    /// 00 : OpCode.PUSHNULL [1 datoshi]
    /// 01 : OpCode.PUSH0 [1 datoshi]
    /// 02 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 18 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 2E : OpCode.PUSH4 [1 datoshi]
    /// 2F : OpCode.PACK [2048 datoshi]
    /// 30 : OpCode.PUSH15 [1 datoshi]
    /// 31 : OpCode.PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 3B : OpCode.PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// 51 : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 56 : OpCode.DROP [2 datoshi]
    /// 57 : OpCode.CALL 03 [512 datoshi]
    /// 59 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromCall")]
    public abstract void HasReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1JFEQwBAdswQZv2Z85B5j8YhEA=
    /// 00 : OpCode.PUSHNULL [1 datoshi]
    /// 01 : OpCode.PUSH0 [1 datoshi]
    /// 02 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 18 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 2E : OpCode.PUSH4 [1 datoshi]
    /// 2F : OpCode.PACK [2048 datoshi]
    /// 30 : OpCode.PUSH15 [1 datoshi]
    /// 31 : OpCode.PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 3B : OpCode.PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// 51 : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 56 : OpCode.DROP [2 datoshi]
    /// 57 : OpCode.PUSH1 [1 datoshi]
    /// 58 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 5B : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 5D : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 62 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 67 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromSingleBasicBlock")]
    public abstract void HasReentrancyFromSingleBasicBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwBAdswQZv2Z85B5j8YhAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRUA=
    /// 00 : OpCode.PUSH1 [1 datoshi]
    /// 01 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 04 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0B : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : OpCode.PUSHNULL [1 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 28 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 3E : OpCode.PUSH4 [1 datoshi]
    /// 3F : OpCode.PACK [2048 datoshi]
    /// 40 : OpCode.PUSH15 [1 datoshi]
    /// 41 : OpCode.PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 4B : OpCode.PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// 61 : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 66 : OpCode.DROP [2 datoshi]
    /// 67 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancy")]
    public abstract void NoReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00D2BYNCs1NP7//1g0X0A=
    /// 00 : OpCode.LDSFLD0 [2 datoshi]
    /// 01 : OpCode.ISNULL [2 datoshi]
    /// 02 : OpCode.JMPIFNOT 1B [2 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// 15 : OpCode.PUSHINT16 FF00 [1 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PICK [2 datoshi]
    /// 1A : OpCode.CALL 0F [512 datoshi]
    /// 1C : OpCode.STSFLD0 [2 datoshi]
    /// 1D : OpCode.LDSFLD0 [2 datoshi]
    /// 1E : OpCode.CALL 2B [512 datoshi]
    /// 20 : OpCode.CALL_L 34FEFFFF [512 datoshi]
    /// 25 : OpCode.LDSFLD0 [2 datoshi]
    /// 26 : OpCode.CALL 5F [512 datoshi]
    /// 28 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyByAttribute")]
    public abstract void NoReentrancyByAttribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwBAdswQZv2Z85B5j8YhDSIQA==
    /// 00 : OpCode.PUSH1 [1 datoshi]
    /// 01 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 04 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0B : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : OpCode.CALL 88 [512 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCZaCxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFQBEMAQHbMEGb9mfOQeY/GIRA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.JMPIFNOT 5A [2 datoshi]
    /// 06 : OpCode.PUSHNULL [1 datoshi]
    /// 07 : OpCode.PUSH0 [1 datoshi]
    /// 08 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1E : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 34 : OpCode.PUSH4 [1 datoshi]
    /// 35 : OpCode.PACK [2048 datoshi]
    /// 36 : OpCode.PUSH15 [1 datoshi]
    /// 37 : OpCode.PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 41 : OpCode.PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// 57 : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 5C : OpCode.DROP [2 datoshi]
    /// 5D : OpCode.RET [0 datoshi]
    /// 5E : OpCode.PUSH1 [1 datoshi]
    /// 5F : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 62 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 64 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 69 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 6E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion
}
