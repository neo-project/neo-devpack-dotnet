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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Reentrancy"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""hasReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""hasReentrancyFromSingleBasicBlock"",""parameters"":[],""returntype"":""Void"",""offset"":125,""safe"":false},{""name"":""hasReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":242,""safe"":false},{""name"":""noReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":344,""safe"":false},{""name"":""noReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":461,""safe"":false},{""name"":""noReentrancyFromJump"",""parameters"":[{""name"":""input"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":484,""safe"":false},{""name"":""noReentrancyByAttribute"",""parameters"":[],""returntype"":""Void"",""offset"":605,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":759,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0HA1cEADtlAAwU9WPqQLwoPU0OBcSOowWz8qBzQO9wDAh0cmFuc2ZlcnEfcgwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAEAtUFMBza2ppaEFifVtSRT0WcEGb9mfODAEBEY0RU0HmPxiEPQJAVwQADBT1Y+pAvCg9TQ4FxI6jBbPyoHNA73AMCHRyYW5zZmVycR9yDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwHNramloQWJ9W1JFQZv2Z84MAQERjRFTQeY/GIRAVwQADBTPduKL0AYsSkeO41VhARMZ88+k0nAMCHRyYW5zZmVycR9yDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwHNramloQWJ9W1JFNANAVwQAQZv2Z84MAQERjRFTQeY/GIQMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvcAwIdHJhbnNmZXJxH3IMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABALVBTAc2tqaWhBYn1bUkVAQZv2Z84MAQERjRFTQeY/GIQ1ev///0BXBAF4JmMMFM924ovQBixKR47jVWEBExnzz6TScAwIdHJhbnNmZXJxH3IMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABALVBTAc2tqaWhBYn1bUkVAQZv2Z84MAQERjRFTQeY/GIRAWNgmHFkLCxPADAtub1JlZW50cmFudAH/ABJNNA9gWDQnNf/9//9YNF9AVwADengRUdBBm/ZnznkRiE4QUdBQEsB4EFHQQFcDAXgRzngQzsFFU4tQQZJd6DFwaNhxDA9BbHJlYWR5IGVudGVyZWRyaSQEauAReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFYCCur///8Kpf///xLAYUDE7iQ8").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAO2UADBT1Y+pAvCg9TQ4FxI6jBbPyoHNA73AMCHRyYW5zZmVycR9yDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwHNramloQWJ9W1JFPRZwQZv2Z84MAQERjRFTQeY/GIQ9AkA=
    /// INITSLOT 0400 [64 datoshi]
    /// TRY 6500 [4 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
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
    /// Script: VwQADBTPduKL0AYsSkeO41VhARMZ88+k0nAMCHRyYW5zZmVycR9yDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwHNramloQWJ9W1JFNANA
    /// INITSLOT 0400 [64 datoshi]
    /// PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
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
    /// Script: VwQADBT1Y+pAvCg9TQ4FxI6jBbPyoHNA73AMCHRyYW5zZmVycR9yDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwHNramloQWJ9W1JFQZv2Z84MAQERjRFTQeY/GIRA
    /// INITSLOT 0400 [64 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
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
    /// Script: VwQAQZv2Z84MAQERjRFTQeY/GIQMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvcAwIdHJhbnNmZXJxH3IMFAAAAAAAAAAAAAAAAAAAAAAAAAAADBQAAAAAAAAAAAAAAAAAAAAAAAAAABALVBTAc2tqaWhBYn1bUkVA
    /// INITSLOT 0400 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
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
    /// Script: WNgmHFkLCxPADAtub1JlZW50cmFudAH/ABJNNA9gWDQnNf/9//9YNF9A
    /// LDSFLD0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 1C [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 0F [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL 27 [512 datoshi]
    /// CALL_L FFFDFFFF [512 datoshi]
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
    /// Script: QZv2Z84MAQERjRFTQeY/GIQ1ev///0A=
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// CALL_L 7AFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBeCZjDBTPduKL0AYsSkeO41VhARMZ88+k0nAMCHRyYW5zZmVycR9yDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAQC1QUwHNramloQWJ9W1JFQEGb9mfODAEBEY0RU0HmPxiEQA==
    /// INITSLOT 0401 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 63 [2 datoshi]
    /// PUSHDATA1 CF76E28BD0062C4A478EE35561011319F3CFA4D2 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion
}
