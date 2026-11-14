// Copyright (C) 2015-2026 The Neo Project.
//
// StoredAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

#pragma warning disable IDE0060

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class StoredAttribute : Attribute
    {
        /// <summary>
        /// Stores the property using its name as the storage key.
        /// If the property hides an inherited stored property with the same name using <c>new</c>,
        /// both properties use the same key and writing one overwrites the other's stored value.
        /// Use an overload that accepts a byte or string key to give the hiding property a distinct key.
        /// </summary>
        public StoredAttribute() { }

        /// <summary>
        /// The property will be backed in the storage using the specific key
        /// </summary>
        /// <param name="storageKey">Storage key</param>
        public StoredAttribute(byte storageKey) { }

        /// <summary>
        /// The property will be backed in the storage using the specific key
        /// </summary>
        /// <param name="storageKey">Storage key</param>
        public StoredAttribute(string storageKey) { }
    }
}
