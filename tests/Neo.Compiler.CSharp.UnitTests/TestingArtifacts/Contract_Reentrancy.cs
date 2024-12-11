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
    /// INITSLOT 0100 [64 datoshi]
    /// TRY 5C00 [4 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// ENDTRY 15 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancy")]
    public abstract void HasReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFNANA
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromCall")]
    public abstract void HasReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1JFEQwBAdswQZv2Z85B5j8YhEA=
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromSingleBasicBlock")]
    public abstract void HasReentrancyFromSingleBasicBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwBAdswQZv2Z85B5j8YhAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRUA=
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancy")]
    public abstract void NoReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00D2BYNCs1NP7//1g0X0A=
    /// LDSFLD0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 1B [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 0F [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL 2B [512 datoshi]
    /// CALL_L 34FEFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL 5F [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyByAttribute")]
    public abstract void NoReentrancyByAttribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EQwBAdswQZv2Z85B5j8YhDSIQA==
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// CALL 88 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCZaCxAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFQBEMAQHbMEGb9mfOQeY/GIRA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 5A [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion
}
