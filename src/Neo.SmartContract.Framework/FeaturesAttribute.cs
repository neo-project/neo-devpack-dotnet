using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FeaturesAttribute : Attribute
    {
        /// <summary>
        /// Smart contract features
        /// </summary>
        public ContractFeatures Features { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="features">Specify the smart contract features</param>
        public FeaturesAttribute(ContractFeatures features)
        {
            Features = features;
        }
    }
}
