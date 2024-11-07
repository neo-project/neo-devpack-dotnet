using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":72,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":81,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":84,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":132,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":141,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":144,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":202,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":211,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":266,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":310,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":366,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":417,""safe"":true},{""name"":""multiWithStringStstic"",""parameters"":[],""returntype"":""Integer"",""offset"":420,""safe"":true},{""name"":""multiWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":496,""safe"":true},{""name"":""testStaticMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":550,""safe"":false},{""name"":""testStaticMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":586,""safe"":false},{""name"":""testMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":592,""safe"":false},{""name"":""testMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":622,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":625,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP10AgwSV2l0aG91dENvbnN0cnVjdG9yQfa0a+JBkl3oMUrYJgRFEEBXAAF4DBJXaXRob3V0Q29uc3RydWN0b3JBm/ZnzkHmPxiEQFcAAXhKNNhFQDSvQFhK2CYYRQwBAUH2tGviQZJd6DFK2CYERRBKYEBXAAF4YHgMAQFBm/ZnzkHmPxiEQFcAAXhKNOdFQDTHQFlK2CYdRQwGdGVzdE1lQfa0a+JBkl3oMUrYJgRFEEphQFcAAXhheAwGdGVzdE1lQZv2Z85B5j8YhEBXAAF4SjTiRUA0vUBaStgmMEUMGVByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQSmJAVwABeGJ4DBlQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQZv2Z85B5j8YhEA0oEAMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQQFcAAXgMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJBm/ZnzkHmPxiEQDSYQFtK2CYmRQwPdGVzdFN0YXRpY011bHRpQfa0a+JBkl3oMUrYJgRFEEpjQFcAAXhjeAwPdGVzdFN0YXRpY011bHRpQZv2Z85B5j8YhEAMCXRlc3RNdWx0aUH2tGviQZJd6DFK2CYERRBAVwABeAwJdGVzdE11bHRpQZv2Z85B5j8YhEA1fv///0qcNKFFNXT///9KnDSXRTVq////Spw0jUU1YP///0A1Wv///0B4SjSeTpw0tkV4SjSVTpw0rUV4SjSMTpw0pEU0hUA0gkBWBEDMfR8r"));

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
    public abstract BigInteger? MultiWithString { [DisplayName("multiWithString")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? MultiWithStringStstic { [DisplayName("multiWithStringStstic")] get; }

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

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NIJA
    /// 00 : OpCode.CALL 82 [512 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiGet")]
    public abstract BigInteger? TestMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: eEo0nk6cNLZFeEo0lU6cNK1FeEo0jE6cNKRFNIVA
    /// 00 : OpCode.LDARG0 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.CALL 9E [512 datoshi]
    /// 04 : OpCode.TUCK [2 datoshi]
    /// 05 : OpCode.INC [4 datoshi]
    /// 06 : OpCode.CALL B6 [512 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.CALL 95 [512 datoshi]
    /// 0D : OpCode.TUCK [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.CALL AD [512 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.LDARG0 [2 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.CALL 8C [512 datoshi]
    /// 16 : OpCode.TUCK [2 datoshi]
    /// 17 : OpCode.INC [4 datoshi]
    /// 18 : OpCode.CALL A4 [512 datoshi]
    /// 1A : OpCode.DROP [2 datoshi]
    /// 1B : OpCode.CALL 85 [512 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiSet")]
    public abstract BigInteger? TestMultiSet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NVr///9A
    /// 00 : OpCode.CALL_L 5AFFFFFF [512 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiGet")]
    public abstract BigInteger? TestStaticMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NX7///9KnDShRTV0////Spw0l0U1av///0qcNI1FNWD///9A
    /// 00 : OpCode.CALL_L 7EFFFFFF [512 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.INC [4 datoshi]
    /// 07 : OpCode.CALL A1 [512 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.CALL_L 74FFFFFF [512 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.INC [4 datoshi]
    /// 11 : OpCode.CALL 97 [512 datoshi]
    /// 13 : OpCode.DROP [2 datoshi]
    /// 14 : OpCode.CALL_L 6AFFFFFF [512 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.INC [4 datoshi]
    /// 1B : OpCode.CALL 8D [512 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.CALL_L 60FFFFFF [512 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiSet")]
    public abstract BigInteger? TestStaticMultiSet();

    #endregion
}
