using Neo.IO.Json;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler
{
    class PermissionBuilder
    {
        private readonly HashSet<(string Hash, string Method)> normalItems = new();
        private readonly HashSet<string> wildcardHashes = new();
        private readonly HashSet<string> wildcardMethods = new();
        private bool isWildcard;

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
            if (isWildcard) return;
            if (hash == "*")
            {
                if (method == "*")
                {
                    isWildcard = true;
                }
                else
                {
                    if (wildcardMethods.Add(method))
                        normalItems.RemoveWhere(p => p.Method == method);
                }
            }
            else
            {
                if (method == "*")
                {
                    if (wildcardHashes.Add(hash))
                        normalItems.RemoveWhere(p => p.Hash == hash);
                }
                else
                {
                    if (!wildcardHashes.Contains(hash) && !wildcardMethods.Contains(method))
                        normalItems.Add((hash, method));
                }
            }
        }

        public JArray ToJson()
        {
            JArray permissions = new();
            if (isWildcard)
            {
                permissions.Add(new JObject
                {
                    ["contract"] = "*",
                    ["methods"] = "*"
                });
            }
            else
            {
                foreach (var group in normalItems.GroupBy(p => p.Hash, p => p.Method).OrderBy(p => p.Key))
                    permissions.Add(new JObject
                    {
                        ["contract"] = group.Key,
                        ["methods"] = new JArray(group.OrderBy(p => p).Select(p => (JString)p))
                    });
                foreach (string hash in wildcardHashes.OrderBy(p => p))
                    permissions.Add(new JObject
                    {
                        ["contract"] = hash,
                        ["methods"] = "*"
                    });
                if (wildcardMethods.Count > 0)
                    permissions.Add(new JObject
                    {
                        ["contract"] = "*",
                        ["methods"] = new JArray(wildcardMethods.OrderBy(p => p).Select(p => (JString)p))
                    });
            }
            return permissions;
        }
    }
}
