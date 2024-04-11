// Copyright (C) 2015-2024 The Neo Project.
//
// ContractAuthorAttribute.cs file belongs to the neo project and is free
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
    public class ContractAuthorAttribute : ManifestExtraAttribute
    {
        /// <summary>
        ///     Specifies the author of the contract in the manifest.
        /// </summary>
        /// <param name="author">The name of the contract author</param>
        public ContractAuthorAttribute(string author) : base(AttributeType[nameof(ContractAuthorAttribute)], author)
        {
        }

        /// <summary>
        /// Specifies the author and email of the contract in the manifest.
        /// </summary>
        /// <param name="author">The name of the contract author</param>
        /// <param name="email">The email of the contract author</param>
        public ContractAuthorAttribute(string author, string email) : base(AttributeType[nameof(ContractAuthorAttribute)], author, email)
        {
        }
    }
}
