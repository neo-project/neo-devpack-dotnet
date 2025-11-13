// Copyright (C) 2015-2025 The Neo Project.
//
// StrategyAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.Optimizer
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StrategyAttribute : Attribute
    {
        /// <summary>
        /// Strategy name
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Greater num to be executed first
        /// </summary>
        public int Priority = 0;
    }
}
