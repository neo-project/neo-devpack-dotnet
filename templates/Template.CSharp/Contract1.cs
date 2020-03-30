using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace $safeprojectname$
{
    [ManifestExtraAttribute("Name", "Demo")]
    [ManifestExtraAttribute("Author", "Neo")]
    [ManifestExtraAttribute("Email", "dev@neo.org")]
    [ManifestExtraAttribute("Description", "This is a contract example")]
    [Features(ContractFeatures.HasStorage)]
    public class Contract1 : SmartContract
    {
        public static bool Main(string operation, object[] args)
        {
            Storage.Put("Hello", "World");
            return true;
        }
    }
}
