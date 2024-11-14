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
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : TRY 5C00 [4 datoshi]
    /// 06 : PUSHNULL [1 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1E : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 34 : PUSH4 [1 datoshi]
    /// 35 : PACK [2048 datoshi]
    /// 36 : PUSH15 [1 datoshi]
    /// 37 : PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 41 : PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// 57 : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 5C : DROP [2 datoshi]
    /// 5D : ENDTRY 15 [4 datoshi]
    /// 5F : STLOC0 [2 datoshi]
    /// 60 : PUSH1 [1 datoshi]
    /// 61 : PUSHDATA1 01 [8 datoshi]
    /// 64 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 66 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 6B : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 70 : ENDTRY 02 [4 datoshi]
    /// 72 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancy")]
    public abstract void HasReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFNANA
    /// 00 : PUSHNULL [1 datoshi]
    /// 01 : PUSH0 [1 datoshi]
    /// 02 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 18 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 2E : PUSH4 [1 datoshi]
    /// 2F : PACK [2048 datoshi]
    /// 30 : PUSH15 [1 datoshi]
    /// 31 : PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 3B : PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// 51 : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 56 : DROP [2 datoshi]
    /// 57 : CALL 03 [512 datoshi]
    /// 59 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromCall")]
    public abstract void HasReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1JFEQwBAdswQZv2Z85B5j8YhEA=
    /// 00 : PUSHNULL [1 datoshi]
    /// 01 : PUSH0 [1 datoshi]
    /// 02 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 18 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 2E : PUSH4 [1 datoshi]
    /// 2F : PACK [2048 datoshi]
    /// 30 : PUSH15 [1 datoshi]
    /// 31 : PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 3B : PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// 51 : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 56 : DROP [2 datoshi]
    /// 57 : PUSH1 [1 datoshi]
    /// 58 : PUSHDATA1 01 [8 datoshi]
    /// 5B : CONVERT 30 'Buffer' [8192 datoshi]
    /// 5D : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 62 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 67 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromSingleBasicBlock")]
    public abstract void HasReentrancyFromSingleBasicBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwBAdswQZv2Z85B5j8YhAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRUA=
    /// 00 : PUSH1 [1 datoshi]
    /// 01 : PUSHDATA1 01 [8 datoshi]
    /// 04 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0B : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : PUSHNULL [1 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 28 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 3E : PUSH4 [1 datoshi]
    /// 3F : PACK [2048 datoshi]
    /// 40 : PUSH15 [1 datoshi]
    /// 41 : PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 4B : PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// 61 : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 66 : DROP [2 datoshi]
    /// 67 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancy")]
    public abstract void NoReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00D2BYNCs1NP7//1g0X0A=
    /// 00 : LDSFLD0 [2 datoshi]
    /// 01 : ISNULL [2 datoshi]
    /// 02 : JMPIFNOT 1B [2 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// 15 : PUSHINT16 FF00 [1 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : PICK [2 datoshi]
    /// 1A : CALL 0F [512 datoshi]
    /// 1C : STSFLD0 [2 datoshi]
    /// 1D : LDSFLD0 [2 datoshi]
    /// 1E : CALL 2B [512 datoshi]
    /// 20 : CALL_L 34FEFFFF [512 datoshi]
    /// 25 : LDSFLD0 [2 datoshi]
    /// 26 : CALL 5F [512 datoshi]
    /// 28 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyByAttribute")]
    public abstract void NoReentrancyByAttribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwBAdswQZv2Z85B5j8YhDSIQA==
    /// 00 : PUSH1 [1 datoshi]
    /// 01 : PUSHDATA1 01 [8 datoshi]
    /// 04 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0B : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : CALL 88 [512 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCZaCxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFQBEMAQHbMEGb9mfOQeY/GIRA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : JMPIFNOT 5A [2 datoshi]
    /// 06 : PUSHNULL [1 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1E : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 34 : PUSH4 [1 datoshi]
    /// 35 : PACK [2048 datoshi]
    /// 36 : PUSH15 [1 datoshi]
    /// 37 : PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 41 : PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// 57 : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 5C : DROP [2 datoshi]
    /// 5D : RET [0 datoshi]
    /// 5E : PUSH1 [1 datoshi]
    /// 5F : PUSHDATA1 01 [8 datoshi]
    /// 62 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 64 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 69 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 6E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion
}
