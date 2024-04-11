// Copyright (C) 2015-2024 The Neo Project.
//
// ContractDescriptionAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractDescriptionAttribute : ManifestExtraAttribute
    {
        /// <summary>
        /// Specifies the description of the contract in the manifest.
        /// </summary>
        /// <param name="value">Description of the contract.</param>
        public ContractDescriptionAttribute(string value) : base(AttributeType[nameof(ContractDescriptionAttribute)], value)
        {
        }
    }
}
