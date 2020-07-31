using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace neo
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a contract example")]
    [Features(ContractFeatures.HasStorage)]
    public class TemplateCSharp : SmartContract
    {
        public static bool Main(string operation, object[] args)
        {
            Storage.Put("Hello", "World");
            return true;
        }
    }
}
