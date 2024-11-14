using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":80,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":89,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":92,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":138,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":147,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":150,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":206,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":215,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":268,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":312,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":374,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":427,""safe"":true},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":430,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2xAVjYJigMEldpdGhvdXRDb25zdHJ1Y3RvckH2tGviQZJd6DFK2CYERRBKYEBXAAF4YHgMEldpdGhvdXRDb25zdHJ1Y3RvckGb9mfOQeY/GIRAVwABeEo01kVANKdAWdgmFwwBAUH2tGviQZJd6DFK2CYERRBKYUBXAAF4YXgMAQFBm/ZnzkHmPxiEQFcAAXhKNOdFQDTJQFrYJhwMBnRlc3RNZUH2tGviQZJd6DFK2CYERRBKYkBXAAF4YngMBnRlc3RNZUGb9mfOQeY/GIRAVwABeEo04kVANL9AW9gmLwwZUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckH2tGviQZJd6DFK2CYERRBKY0BXAAF4Y3gMGVByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJBm/ZnzkHmPxiEQDSiQFzYJjgMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQSmRAVwABeGR4DCJOb25TdGF0aWNQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQZv2Z85B5j8YhEA0kEBWBUCpJ7GT"));

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
    /// 00 : CALL C9 [512 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithKey")]
    public abstract BigInteger? GetWithKey();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NKdA
    /// 00 : CALL A7 [512 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithoutConstructor")]
    public abstract BigInteger? GetWithoutConstructor();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NL9A
    /// 00 : CALL BF [512 datoshi]
    /// 02 : RET [0 datoshi]
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
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL E7 [512 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithKey")]
    public abstract void PutWithKey(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo01kVA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL D6 [512 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithoutConstructor")]
    public abstract void PutWithoutConstructor(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo04kVA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL E2 [512 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithString")]
    public abstract void PutWithString(BigInteger? value);

    #endregion
}
