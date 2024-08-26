using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":471,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":486,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":501,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":117,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":163,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":172,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":175,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":231,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":240,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":293,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":337,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":516,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":528,""safe"":true},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":468,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0dAlcAAXgQziQrDBJXaXRob3V0Q29uc3RydWN0b3JB9rRr4kGSXegxStgmBEUQSngQUdBAVwABeDQDQFcAAUBXAAJ4EHnQeQwSV2l0aG91dENvbnN0cnVjdG9yQZv2Z85B5j8YhEBXAAJ5Sng000VAVwABeDSOQFjYJhcMAQFB9rRr4kGSXegxStgmBEUQSmBAVwABeGB4DAEBQZv2Z85B5j8YhEBXAAF4SjTnRUA0yUBZ2CYcDAZ0ZXN0TWVB9rRr4kGSXegxStgmBEUQSmFAVwABeGF4DAZ0ZXN0TWVBm/ZnzkHmPxiEQFcAAXhKNOJFQDS/QFrYJi8MGVByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQSmJAVwABeGJ4DBlQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQZv2Z85B5j8YhEA0okBXAAF4Ec4kOwwiTm9uU3RhdGljUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckH2tGviQZJd6DFK2CYERRBKeBFR0EBXAAJ4EXnQeQwiTm9uU3RhdGljUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckGb9mfOQeY/GIRAVwABeDSDQFYDQBAQEsBKNVb+//8jH/7//xAQEsBKNUf+//8jdP7//xAQEsBKNTj+//8jb/7//xAQEsBKNSn+//8iiBAQEsBKNR3+//8is0DDecSY"));

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
    [DisplayName("getWithKey")]
    public abstract BigInteger? GetWithKey();

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getWithoutConstructor")]
    public abstract BigInteger? GetWithoutConstructor();

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getWithString")]
    public abstract BigInteger? GetWithString();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("putWithKey")]
    public abstract void PutWithKey(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("putWithoutConstructor")]
    public abstract void PutWithoutConstructor(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("putWithString")]
    public abstract void PutWithString(BigInteger? value);

    #endregion

}
