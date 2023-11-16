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

#pragma warning disable IDE0060

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class StorageBackedAttribute : Attribute
    {
        /// <summary>
        /// The property will be backed in the storage using the specific key using the property name as storage key
        /// </summary>
        public StorageBackedAttribute() { }

        /// <summary>
        /// The property will be backed in the storage using the specific key
        /// </summary>
        /// <param name="storageKey">Storage key</param>
        public StorageBackedAttribute(byte storageKey) { }

        /// <summary>
        /// The property will be backed in the storage using the specific key
        /// </summary>
        /// <param name="storageKey">Storage key</param>
        public StorageBackedAttribute(string storageKey) { }
    }
}
