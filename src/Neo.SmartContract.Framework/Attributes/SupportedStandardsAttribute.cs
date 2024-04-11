// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedStandardsAttribute : Attribute
    {
        public SupportedStandardsAttribute(params string[] supportedStandards)
        {
        }

        public SupportedStandardsAttribute(params NepStandard[] supportedStandards)
        {
        }
    }
}
