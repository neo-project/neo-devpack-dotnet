using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Reentrancy(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Reentrancy"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""hasReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""hasReentrancyFromSingleBasicBlock"",""parameters"":[],""returntype"":""Void"",""offset"":117,""safe"":false},{""name"":""hasReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":223,""safe"":false},{""name"":""noReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":314,""safe"":false},{""name"":""noReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":420,""safe"":false},{""name"":""noReentrancyFromJump"",""parameters"":[{""name"":""input"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":440,""safe"":false},{""name"":""noReentrancyByAttribute"",""parameters"":[],""returntype"":""Void"",""offset"":553,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":702,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3BAlcBADtdAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAEAtUFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkU9FnBBm/ZnzgwBAdswEVNB5j8YhD0CQAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAEAtUFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkVBm/ZnzgwBAdswEVNB5j8YhEAMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABALVBTAHwwIdHJhbnNmZXIMFM924ovQBixKR47jVWEBExnzz6TSQWJ9W1JFNANAQZv2Z84MAQHbMBFTQeY/GIQMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABALVBTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1JFQEGb9mfODAEB2zARU0HmPxiENIVAVwABeCZbDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRUBBm/ZnzgwBAdswEVNB5j8YhEBY2CYbCwsSwAwLbm9SZWVudHJhbnQB/wASTTQPYFg0JzUs/v//WDRbQFcAA3p4EVHQQZv2Z855EYhOEFHQUBLAeBBR0EBXAQF4Ec54EM7BRVOLUEGSXegxcGjYJBQMD0FscmVhZHkgZW50ZXJlZOAReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFYBQAXmC9Y=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAO10ADBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRT0WcEGb9mfODAEB2zARU0HmPxiEPQJA
    /// INITSLOT 0100 [64 datoshi]
    /// TRY 5D00 [4 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
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
    /// Script: DBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRTQDQA==
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
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
    /// Script: DBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRUGb9mfODAEB2zARU0HmPxiEQA==
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("hasReentrancyFromSingleBasicBlock")]
    public abstract void HasReentrancyFromSingleBasicBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZv2Z84MAQHbMBFTQeY/GIQMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABALVBTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1JFQA==
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
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
    /// Script: WNgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00D2BYNCc1LP7//1g0W0A=
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
    /// CALL 27 [512 datoshi]
    /// CALL_L 2CFEFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL 5B [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyByAttribute")]
    public abstract void NoReentrancyByAttribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZv2Z84MAQHbMBFTQeY/GIQ0hUA=
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// CALL 85 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCZbDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRUBBm/ZnzgwBAdswEVNB5j8YhEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 5B [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion
}
