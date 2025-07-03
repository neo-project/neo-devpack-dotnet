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

        public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@""));

        public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"");

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