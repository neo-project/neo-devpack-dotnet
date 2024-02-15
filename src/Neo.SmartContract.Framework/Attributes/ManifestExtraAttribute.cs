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

        internal static readonly Dictionary<string, string> AttributeType = new Dictionary<string, string>
        {
            { nameof(AuthorAttribute), "Author" },
            { nameof(EmailAttribute), "E-mail" },
            { nameof(DescriptionAttribute), "Description" },
            { nameof(VersionAttribute), "Version" },
        };
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorAttribute : ManifestExtraAttribute
    {
        public AuthorAttribute(string value) : base(AttributeType[nameof(AuthorAttribute)], value)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EmailAttribute : ManifestExtraAttribute
    {
        public EmailAttribute(string value) : base(AttributeType[nameof(EmailAttribute)], value)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : ManifestExtraAttribute
    {
        public DescriptionAttribute(string value) : base(AttributeType[nameof(DescriptionAttribute)], value)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : ManifestExtraAttribute
    {
        public VersionAttribute(string value) : base(AttributeType[nameof(VersionAttribute)], value)
        {
        }
    }
}
