// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler
{
    class PermissionBuilder
    {
        private readonly HashSet<(string Hash, string Method)> _normalItems = new();
        private readonly HashSet<string> _wildcardHashes = new();
        private readonly HashSet<string> _wildcardMethods = new();
        private bool _isWildcard;

        public void Add(string hash, string[] methods)
        {
            if (methods.Length == 0)
                Add(hash, "*");
            else
                foreach (string method in methods)
                    Add(hash, method);
        }

        public void Add(string hash, string method)
        {
            if (_isWildcard) return;
            if (hash == "*")
            {
                if (method == "*")
                {
                    _isWildcard = true;
                }
                else
                {
                    if (_wildcardMethods.Add(method))
                        _normalItems.RemoveWhere(p => p.Method == method);
                }
            }
            else
            {
                if (method == "*")
                {
                    if (_wildcardHashes.Add(hash))
                        _normalItems.RemoveWhere(p => p.Hash == hash);
                }
                else
                {
                    if (!_wildcardHashes.Contains(hash) && !_wildcardMethods.Contains(method))
                        _normalItems.Add((hash, method));
                }
            }
        }

        public JArray ToJson()
        {
            JArray permissions = new();
            if (_isWildcard)
            {
                permissions.Add(new JObject
                {
                    ["contract"] = "*",
                    ["methods"] = "*"
                });
            }
            else
            {
                foreach (var group in _normalItems.GroupBy(p => p.Hash, p => p.Method).OrderBy(p => p.Key))
                    permissions.Add(new JObject
                    {
                        ["contract"] = group.Key,
                        ["methods"] = new JArray(group.OrderBy(p => p).Select(p => (JString)p!))
                    });
                foreach (string hash in _wildcardHashes.OrderBy(p => p))
                    permissions.Add(new JObject
                    {
                        ["contract"] = hash,
                        ["methods"] = "*"
                    });
                if (_wildcardMethods.Count > 0)
                    permissions.Add(new JObject
                    {
                        ["contract"] = "*",
                        ["methods"] = new JArray(_wildcardMethods.OrderBy(p => p).Select(p => (JString)p!))
                    });
            }
            return permissions;
        }
    }
}
