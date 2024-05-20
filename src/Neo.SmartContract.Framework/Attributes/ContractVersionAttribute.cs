// Copyright (C) 2015-2024 The Neo Project.
//
// ContractVersionAttribute.cs file belongs to the neo project and is free
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
    public class ContractVersionAttribute : ManifestExtraAttribute
    {
        /// <summary>
        /// Specifies the version of the contract in the manifest.
        /// </summary>
        /// <param name="value">Version of the contract</param>
        /// <remarks>The version is different from the Update Counter.</remarks>
        public ContractVersionAttribute(string value) : base(AttributeType[nameof(ContractVersionAttribute)], value)
        {
        }
    }
}
