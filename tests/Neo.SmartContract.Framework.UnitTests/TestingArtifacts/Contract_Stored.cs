using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":72,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":81,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":84,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":130,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":139,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":142,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":198,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":207,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":260,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":304,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":360,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":411,""safe"":true},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":414,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2hAQwSV2l0aG91dENvbnN0cnVjdG9yQfa0a+JBkl3oMUrYJgRFEEBXAAF4DBJXaXRob3V0Q29uc3RydWN0b3JBm/ZnzkHmPxiEQFcAAXhKNNhFQDSvQFjYJhcMAQFB9rRr4kGSXegxStgmBEUQSmBAVwABeGB4DAEBQZv2Z85B5j8YhEBXAAF4SjTnRUA0yUBZ2CYcDAZ0ZXN0TWVB9rRr4kGSXegxStgmBEUQSmFAVwABeGF4DAZ0ZXN0TWVBm/ZnzkHmPxiEQFcAAXhKNOJFQDS/QFrYJi8MGVByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQSmJAVwABeGJ4DBlQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQZv2Z85B5j8YhEA0okAMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQQFcAAXgMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJBm/ZnzkHmPxiEQDSYQFYDQEp0AoE="));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? NonStaticPrivateGetterPublicSetter { [DisplayName("getNonStaticPrivateGetterPublicSetter")] get; [DisplayName("setNonStaticPrivateGetterPublicSetter")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? PrivateGetterPublicSetter { [DisplayName("getPrivateGetterPublicSetter")] get; [DisplayName("setPrivateGetterPublicSetter")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithKey { [DisplayName("withKey")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithoutConstructor { [DisplayName("withoutConstructor")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithString { [DisplayName("withString")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NMlA
    /// 00 : OpCode.CALL C9 [512 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithKey")]
    public abstract BigInteger? GetWithKey();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NK9A
    /// 00 : OpCode.CALL AF [512 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithoutConstructor")]
    public abstract BigInteger? GetWithoutConstructor();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NL9A
    /// 00 : OpCode.CALL BF [512 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithString")]
    public abstract BigInteger? GetWithString();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo050VA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.CALL E7 [512 datoshi]
    /// 07 : OpCode.DROP [2 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithKey")]
    public abstract void PutWithKey(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo02EVA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.CALL D8 [512 datoshi]
    /// 07 : OpCode.DROP [2 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithoutConstructor")]
    public abstract void PutWithoutConstructor(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo04kVA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.CALL E2 [512 datoshi]
    /// 07 : OpCode.DROP [2 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithString")]
    public abstract void PutWithString(BigInteger? value);

    #endregion
}
