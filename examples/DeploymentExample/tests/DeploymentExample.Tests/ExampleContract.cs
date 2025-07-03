using Neo;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Tests
{
    public abstract class ExampleContract : SmartContract, IContractInfo
    {
        #region Compiled data

        public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErZGFmMGZjMTI0M2I0N2Q5MzU2ZTYzNTU4ODJiYjhiZWIyMDcuLi4AAAL9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAP/aP6Q0bqUyolj8SX3a3bZDfJ/f8HZGVzdHJveQAAAA8AAP0RA1cBAnmqJlV4cGhK2SgkBkUJIgbKABSzqiYFCCIFaBCzJhoMFUludmFsaWQgb3duZXIgYWRkcmVzczpoDAEB2zBBm/ZnzkHmPxiEEAwBAtswQZv2Z85B5j8YhEBK2SgkBkUJIgbKABSzQBCzQEHmPxiEQEGb9mfOQEHmPxiEQFcBAAwBAdswQZv2Z85Bkl3oMXBoStgkA8oAFJcmEGhK2CQJSsoAFCgDOiIYDBQAAAAAAAAAAAAAAAAAAAAAAAAAACICQEGSXegxQMpADBQAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAQF4StkoJAZFCSIGygAUs6omBQgiBXgQsyYeDBlJbnZhbGlkIG5ldyBvd25lciBhZGRyZXNzOjVk////cGhB+CfsjKomJgwhT25seSBvd25lciBjYW4gdHJhbnNmZXIgb3duZXJzaGlwOngMAQHbMEGb9mfOQeY/GIR4aBLADAxPd25lckNoYW5nZWRBlQFvYQgiAkBB+CfsjEBXAwBBOVNuPHBoC5cmBQgiCWhB+CfsjKomEQwMVW5hdXRob3JpemVkOjQ+cWkRnnJqDAEC2zBBm/ZnzkHmPxiEamgSwAwSQ291bnRlckluY3JlbWVudGVkQZUBb2FqIgJAQTlTbjxAVwEADAEC2zBBm/ZnzkGSXegxcGhK2CQDyhC3Jg9oStgmBkUQIgTbISIDECICQErYJgZFECIE2yFAVwACeHmgIgJAVwEAyHAMEURlcGxveW1lbnRFeGFtcGxlSgwEbmFtZWhT0EUMBTEuMC4wSgwHdmVyc2lvbmhT0EU1H/7//0oMBW93bmVyaFPQRTV1////SgwHY291bnRlcmhT0EVoIgJAyEDQQDXz/f//Qfgn7IwiAkBXAAM14/3//0H4J+yMqiYjDB5Pbmx5IG93bmVyIGNhbiB1cGRhdGUgY29udHJhY3Q6enl4NwAACCICQDcAAEA1p/3//0H4J+yMqiYkDB9Pbmx5IG93bmVyIGNhbiBkZXN0cm95IGNvbnRyYWN0OjcBAAgiAkA3AQBA87Vgog=="));

        public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""DeploymentExample.ExampleContract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":126,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":226,""safe"":false},{""name"":""increment"",""parameters"":[],""returntype"":""Integer"",""offset"":382,""safe"":false},{""name"":""getCounter"",""parameters"":[],""returntype"":""Integer"",""offset"":485,""safe"":true},{""name"":""multiply"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":542,""safe"":true},{""name"":""getInfo"",""parameters"":[],""returntype"":""Map"",""offset"":551,""safe"":true},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":651,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":664,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Boolean"",""offset"":727,""safe"":false}],""events"":[{""name"":""CounterIncremented"",""parameters"":[{""name"":""arg1"",""type"":""Hash160""},{""name"":""arg2"",""type"":""Integer""}]},{""name"":""OwnerChanged"",""parameters"":[{""name"":""arg1"",""type"":""Hash160""},{""name"":""arg2"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet"",""Author"":""Neo Community"",""Email"":""developer@neo.org"",""Description"":""Example contract demonstrating deployment workflow"",""Version"":""1.0.0"",""nef"":{""optimization"":""Basic""}}}");

        #endregion

        #region Events

        [DisplayName("CounterIncremented")]
        public event OnCounterIncrementedDelegate? OnCounterIncremented;
        public delegate void OnCounterIncrementedDelegate(UInt160 caller, BigInteger newValue);

        [DisplayName("OwnerChanged")]
        public event OnOwnerChangedDelegate? OnOwnerChanged;
        public delegate void OnOwnerChangedDelegate(UInt160 oldOwner, UInt160 newOwner);

        #endregion

        #region Properties

        /// <summary>
        /// Safe property
        /// </summary>
        public abstract BigInteger? Counter { [DisplayName("getCounter")] get; }

        /// <summary>
        /// Safe property
        /// </summary>
        public abstract UInt160? Owner { [DisplayName("getOwner")] get; }

        #endregion

        #region Safe methods

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("getCounter")]
        public abstract BigInteger GetCounter();

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("getInfo")]
        public abstract IDictionary<string, object>? GetInfo();

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("getOwner")]
        public abstract UInt160 GetOwner();

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("multiply")]
        public abstract BigInteger Multiply(BigInteger a, BigInteger b);

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("verify")]
        public abstract bool Verify();

        #endregion

        #region Unsafe methods

        /// <summary>
        /// Unsafe method
        /// </summary>
        [DisplayName("destroy")]
        public abstract bool Destroy();

        /// <summary>
        /// Unsafe method
        /// </summary>
        [DisplayName("increment")]
        public abstract BigInteger Increment();

        /// <summary>
        /// Unsafe method
        /// </summary>
        [DisplayName("setOwner")]
        public abstract bool SetOwner(UInt160 newOwner);

        /// <summary>
        /// Unsafe method
        /// </summary>
        [DisplayName("update")]
        public abstract bool Update(byte[] nefFile, string manifest, object? data = null);

        #endregion

        #region Events

        public class OnCounterIncrementedEvent
        {
            public UInt160 Caller { get; set; } = null!;
            public BigInteger NewValue { get; set; }
        }

        public class OnOwnerChangedEvent
        {
            public UInt160 OldOwner { get; set; } = null!;
            public UInt160 NewOwner { get; set; } = null!;
        }

        #endregion

        #region Constructor

        protected ExampleContract(Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest) : base(nef, manifest)
        {
        }

        #endregion
    }
}