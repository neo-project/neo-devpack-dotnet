using System.ComponentModel;
using System.Numerics;
using Akka.Util.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Neo.Compiler;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using ExecutionContext = Neo.VM.ExecutionContext;

namespace Neo.SmartContract.TestEngine
{
    public class TestEngine : ApplicationEngine
    {
        public const long TestGas = 2000_00000000;

        private static readonly List<MetadataReference> references = new();

        public event EventHandler<ExecutionContext> OnPreExecuteTestCaseStandard;

        public NefFile Nef { get; private set; }
        public ContractManifest Manifest { get; private set; }
        public JObject DebugInfo { get; private set; }

        static TestEngine()
        {
            string coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            references.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")));
            references.Add(MetadataReference.CreateFromFile(typeof(string).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location));
            string folder = Path.GetFullPath("../../../../../src/Neo.SmartContract.Framework/");
            string obj = Path.Combine(folder, "obj");
            IEnumerable<SyntaxTree> st = Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.StartsWith(obj))
                .OrderBy(p => p)
                .Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p));
            CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary);
            CSharpCompilation cr = CSharpCompilation.Create(null, st, references, options);
            references.Add(cr.ToMetadataReference());
        }

        public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null, DataCache snapshot = null, Block persistingBlock = null, IDiagnostic diagnostic = null)
             : base(trigger, verificable, snapshot, persistingBlock, TestProtocolSettings.Default, TestGas, diagnostic)
        {
        }

        public CompilationContext AddEntryScript(params string[] files)
        {
            return AddEntryScript(true, files);
        }

        public CompilationContext AddEntryScript(bool debug = true, params string[] files)
        {
            CompilationContext context = CompilationContext.Compile(files, references, new Options
            {
                AddressVersion = ProtocolSettings.Default.AddressVersion,
                Debug = debug,
            });
            if (context.Success)
            {
                Nef = context.CreateExecutable();
                Manifest = context.CreateManifest();
                DebugInfo = context.CreateDebugInformation();
                Reset();
            }
            else
            {
                context.Diagnostics.ForEach(Console.Error.WriteLine);
            }
            return context;
        }

        public void Reset()
        {
            this.State = VMState.BREAK; // Required for allow to reuse the same TestEngine
            this.InvocationStack.Clear();
            while (this.ResultStack.Count > 0)
            {
                this.ResultStack.Pop();
            }
            if (Nef != null)
            {
                this.LoadScript(Nef.Script);
                // Mock contract
                var contextState = CurrentContext.GetState<ExecutionContextState>();
                contextState.Contract ??= new ContractState { Nef = Nef };
            }
        }

        private int GetMethodEntryOffset(string methodname)
        {
            if (Manifest is null) return -1;
            foreach (var method in Manifest.Abi.Methods)
            {
                if (method.Name == methodname)
                    return method.Offset;
            }
            return -1;
        }

        private int GetMethodReturnCount(string methodname)
        {
            if (Manifest is null) return -1;
            foreach (var method in Manifest.Abi.Methods)
            {
                if (method.Name == methodname)
                {
                    if (method.ReturnType == ContractParameterType.Void)
                        return 0;
                    else
                        return 1;
                }
            }
            return -1;
        }

        public EvaluationStack ExecuteTestCaseStandard(string methodname, params StackItem[] args)
        {
            return ExecuteTestCaseStandard(methodname, Nef, args);
        }

        public EvaluationStack ExecuteTestCaseStandard(string methodname, NefFile contract, params StackItem[] args)
        {
            var offset = GetMethodEntryOffset(methodname);
            if (offset == -1) throw new Exception("Can't find method : " + methodname);
            var rvcount = GetMethodReturnCount(methodname);
            if (rvcount == -1) throw new Exception("Can't find method return count : " + methodname);
            return ExecuteTestCaseStandard(offset, (ushort)rvcount, contract, args);
        }

        public EvaluationStack ExecuteTestCaseStandard(int offset, ushort rvcount, params StackItem[] args)
        {
            return ExecuteTestCaseStandard(offset, rvcount, Nef, args);
        }

        public EvaluationStack ExecuteTestCaseStandard(int offset, ushort rvcount, NefFile contract, params StackItem[] args)
        {
            var context = InvocationStack.Pop();
            context = CreateContext(context.Script, rvcount, offset);
            LoadContext(context);
            // Mock contract
            var contextState = CurrentContext.GetState<ExecutionContextState>();
            contextState.Contract ??= new ContractState()
            {
                Nef = contract,
                Manifest = Manifest
            };
            OnPreExecuteTestCaseStandard?.Invoke(this, context);
            for (var i = args.Length - 1; i >= 0; i--)
                this.Push(args[i]);
            var initializeOffset = GetMethodEntryOffset("_initialize");
            if (initializeOffset != -1)
            {
                LoadContext(CurrentContext.Clone(initializeOffset));
            }
            while (true)
            {
                var bfault = (this.State & VMState.FAULT) > 0;
                var bhalt = (this.State & VMState.HALT) > 0;
                if (bfault || bhalt) break;
                Console.WriteLine("op:[" +
                    this.CurrentContext.InstructionPointer.ToString("X04") +
                    "]" +
                this.CurrentContext.CurrentInstruction?.OpCode + " " + this.CurrentContext.EvaluationStack.Print());
                this.ExecuteNext();
            }
            if (this.State == VMState.FAULT && this.FaultException != null)
            {
                Console.WriteLine("Fault Exception:\n" + this.FaultException);
            }
            return this.ResultStack;
        }

        protected override void OnSysCall(InteropDescriptor descriptor)
        {
            Console.WriteLine("syscall:[" + descriptor.Name + "]");
            base.OnSysCall(descriptor);
        }
    }
}
