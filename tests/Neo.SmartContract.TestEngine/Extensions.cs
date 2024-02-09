using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Array = System.Array;

namespace Neo.SmartContract.TestEngine
{
    public static class Extensions
    {
        public static void ContractAdd(this DataCache snapshot, ContractState contract)
        {
            var key = new KeyBuilder(-1, 8).Add(contract.Hash);
            snapshot.Add(key, new StorageItem(contract));
        }

        public static void DeployNativeContracts(this DataCache snapshot, Block? persistingBlock = null)
        {
            persistingBlock ??= new NeoSystem(TestProtocolSettings.Default, new MemoryStore()).GenesisBlock;

            var method = typeof(SmartContract.Native.ContractManagement).GetMethod("OnPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            engine.LoadScript(Array.Empty<byte>());
            method!.Invoke(SmartContract.Native.NativeContract.ContractManagement, new object[] { engine });
            engine.Snapshot.Commit();

            method = typeof(SmartContract.Native.LedgerContract).GetMethod("PostPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            engine.LoadScript(Array.Empty<byte>());
            method!.Invoke(SmartContract.Native.NativeContract.Ledger, new object[] { engine });
            engine.Snapshot.Commit();
        }

        public static List<string> GetFiles(string root, Type[] types)
        {
            if (types.GroupBy(t => t)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList().Any()) throw new ArgumentException("Duplicate types are not allowed.");

            var files = new List<string>();
            foreach (var type in types)
            {
                files.AddRange(FindClassFiles(root, type));
            }
            return files;
        }

        private static List<string> FindClassFiles(string directory, Type type)
        {
            var files = Directory.GetFiles(directory, "*.cs");
            var regex = new Regex("^" + Regex.Escape(type.Name) + ".*\\.cs$");
            var matchedFiles = new List<string>();
            foreach (var file in files)
            {
                if (regex.IsMatch(Path.GetFileName(file)))
                {
                    var code = File.ReadAllText(file);
                    var syntaxTree = CSharpSyntaxTree.ParseText(code);
                    var root = syntaxTree.GetRoot();

                    var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                    foreach (var classNode in classNodes)
                    {
                        if (classNode.Identifier.Text == type.Name)
                        {
                            matchedFiles.Add(file);
                            break;
                        }
                    }
                }
            }
            return matchedFiles;
        }
      
        public static bool TryGetString(this byte[] byteArray, out string? value)
        {
            try
            {
                value = Utility.StrictUTF8.GetString(byteArray);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}
