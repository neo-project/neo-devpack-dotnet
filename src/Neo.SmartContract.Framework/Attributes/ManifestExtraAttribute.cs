// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ManifestExtraAttribute : Attribute
    {
        public ManifestExtraAttribute(string key, string value)
        {
        }

        public ManifestExtraAttribute(string key, string value, string value2)
        {
        }

        internal static readonly Dictionary<string, string> AttributeType = new()
        {
            { nameof(ContractAuthorAttribute), "Author" },
            { nameof(ContractEmailAttribute), "E-mail" },
            { nameof(ContractDescriptionAttribute), "Description" },
            { nameof(ContractVersionAttribute), "Version" },
        };
    }
}
