using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Reentrancy(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Reentrancy"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""hasReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""hasReentrancyFromSingleBasicBlock"",""parameters"":[],""returntype"":""Void"",""offset"":115,""safe"":false},{""name"":""hasReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":219,""safe"":false},{""name"":""noReentrancy"",""parameters"":[],""returntype"":""Void"",""offset"":309,""safe"":false},{""name"":""noReentrancyFromCall"",""parameters"":[],""returntype"":""Void"",""offset"":413,""safe"":false},{""name"":""noReentrancyFromJump"",""parameters"":[{""name"":""input"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":432,""safe"":false},{""name"":""noReentrancyByAttribute"",""parameters"":[],""returntype"":""Void"",""offset"":543,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":708,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3UAlcBADtcAAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70FifVtSRT0VcBEMAQHbMEGb9mfOQeY/GIQ9AkALEAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkURDAEB2zBBm/ZnzkHmPxiEQAsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRTQDQBEMAQHbMEGb9mfOQeY/GIQLEAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUkVAEQwBAdswQZv2Z85B5j8YhDSIQFcAAXgmWgsQDBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtSRUARDAEB2zBBm/ZnzkHmPxiEQFjYJh4LCxLASlnPDAtub1JlZW50cmFudAH/ABJNNBJgWDQ1NTH+//9YNWgAAABAVwADeDQfekp4EVHQRUGb9mfOeRGIThBR0FASwEp4EFHQRUBXAAFAVwEBeBHOeBDOwUVTi1BBkl3oMXBoC5cMD0FscmVhZHkgZW50ZXJlZOEReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFYCCur///8Kqv///xLAYUBpepbw"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("hasReentrancy")]
    public abstract void HasReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("hasReentrancyFromCall")]
    public abstract void HasReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("hasReentrancyFromSingleBasicBlock")]
    public abstract void HasReentrancyFromSingleBasicBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("noReentrancy")]
    public abstract void NoReentrancy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("noReentrancyByAttribute")]
    public abstract void NoReentrancyByAttribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("noReentrancyFromCall")]
    public abstract void NoReentrancyFromCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("noReentrancyFromJump")]
    public abstract void NoReentrancyFromJump(bool? input);

    #endregion

}
