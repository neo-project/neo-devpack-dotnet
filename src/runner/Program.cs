using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Neo.BlockchainToolkit;
using Neo.BlockchainToolkit.Models;
using Neo.BlockchainToolkit.Persistence;
using Neo.BlockchainToolkit.SmartContract;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Newtonsoft.Json;
using Nito.Disposables;

namespace Neo.Test.Runner
{
    [Command("neo-test-runner", Description = "Neo N3 smart contract runner for unit testing", UsePagerForHelpText = false)]
    [VersionOption(ThisAssembly.AssemblyInformationalVersion)]
    class Program
    {
        static Task<int> Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<IConsole>(PhysicalConsole.Singleton)
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            return app.ExecuteAsync(args);
        }

        [Argument(0, Description = "Path to neo-invoke JSON file")]
        [Required]
        internal string NeoInvokeFile { get; set; } = string.Empty;

        [Option(Description = "Account that is invoking the contract")]
        internal string Account { get; set; } = string.Empty;

        [Option("-c|--checkpoint", Description = "Path to checkpoint file")]
        internal string CheckpointFile { get; set; } = string.Empty;

        [Option("-n|--nef-file")]
        internal string NefFile { get; set; } = string.Empty;

        [Option("-e|--express", Description = "Path to neo-express file")]
        internal string NeoExpressFile { get; set; } = string.Empty;

        [Option("-i|--iterator-count")]
        internal int MaxIteratorCount { get; set; } = 100;

        [Option(Description = "Contracts to include in storage results")]
        public string[] Storages { get; } = Array.Empty<string>();

        internal async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console, IFileSystem fileSystem)
        {
            try
            {
                DebugInfo? debugInfo = string.IsNullOrEmpty(NefFile)
                    ? null
                    : (await DebugInfo.LoadAsync(NefFile, fileSystem: fileSystem))
                        .Match<DebugInfo?>(di => di, _ => null);

                ExpressChain? chain = string.IsNullOrEmpty(NeoExpressFile)
                    ? null : fileSystem.LoadChain(NeoExpressFile);

                var signer = ParseSigner(chain);

                ICheckpointStore checkpoint = string.IsNullOrEmpty(CheckpointFile)
                    ? new NullCheckpointStore(chain)
                    : new CheckpointStore(CheckpointFile, chain);

                using var _ = checkpoint as IDisposable ?? NoopDisposable.Instance;
                using var store = new MemoryTrackingStore(checkpoint);
                store.EnsureLedgerInitialized(checkpoint.Settings);

                var tryGetContract = store.CreateTryGetContract();
                var storages = Storages
                    .Select(s => tryGetContract(s, out var hash) ? hash : null)
                    .Where(h => h != null)
                    .Cast<UInt160>()
                    .Distinct();

                var parser = chain != null
                    ? chain.CreateContractParameterParser(tryGetContract, fileSystem)
                    : checkpoint.Settings.CreateContractParameterParser(tryGetContract, fileSystem);

                var script = await parser.LoadInvocationScriptAsync(NeoInvokeFile);

                using var snapshot = new SnapshotCache(store);
                using var engine = new TestApplicationEngine(snapshot, checkpoint.Settings, signer);

                List<LogEventArgs> logEvents = new();
                engine.Log += (_, args) => logEvents.Add(args);
                List<NotifyEventArgs> notifyEvents = new();
                engine.Notify += (_, args) => notifyEvents.Add(args);

                engine.LoadScript(script);
                engine.Execute();

                await WriteResultsAsync(app.Out, engine, logEvents, notifyEvents, storages, debugInfo);

                return 0;
            }
            catch (Exception ex)
            {
                await app.Error.WriteLineAsync(ex.Message);
                return 1;
            }
        }

        UInt160 ParseSigner(ExpressChain? chain)
        {
            if (string.IsNullOrEmpty(Account))
            {
                return UInt160.Zero;
            }

            if (UInt160.TryParse(Account, out var signer))
            {
                return signer;
            }

            if (chain != null && chain.TryGetDefaultAccount(Account, out var account))
            {
                return account.ToScriptHash(chain.AddressVersion);
            }

            throw new ArgumentException($"couldn't parse \"{Account}\" as {nameof(Account)}", nameof(Account));
        }

        private async Task WriteResultsAsync(TextWriter textWriter, TestApplicationEngine engine,
            IReadOnlyList<LogEventArgs> logEvents, IReadOnlyList<NotifyEventArgs> notifyEvents,
            IEnumerable<UInt160> storages, DebugInfo? debugInfo)
        {
            // using Newtonsoft.Json instead of System.Json because CommandLineUtils doesn't provide
            // a mechanism to access stdout Stream, just the stdout TextWriter

            using var writer = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };

            await writer.WriteStartObjectAsync();

            await writer.WritePropertyNameAsync("state");
            await writer.WriteValueAsync($"{engine.State}"); ;
            await writer.WritePropertyNameAsync("exception");
            await ((engine.FaultException == null)
                ? writer.WriteNullAsync()
                : writer.WriteValueAsync(engine.FaultException.GetBaseException().Message));
            await writer.WritePropertyNameAsync("gasconsumed");
            await writer.WriteValueAsync($"{new BigDecimal((BigInteger)engine.GasConsumed, NativeContract.GAS.Decimals)}");

            await writer.WritePropertyNameAsync("logs");
            await writer.WriteStartArrayAsync();
            for (int i = 0; i < logEvents.Count; i++)
            {
                await writer.WriteLogAsync(logEvents[i]);
            }
            await writer.WriteEndArrayAsync();

            await writer.WritePropertyNameAsync("notifications");
            await writer.WriteStartArrayAsync();
            for (int i = 0; i < notifyEvents.Count; i++)
            {
                await writer.WriteNotificationAsync(notifyEvents[i], MaxIteratorCount);
            }
            await writer.WriteEndArrayAsync();

            await writer.WritePropertyNameAsync("stack");
            await writer.WriteStartArrayAsync();
            IReadOnlyList<VM.Types.StackItem> list = engine.ResultStack;
            for (int i = 0; i < list.Count; i++)
            {
                await writer.WriteStackItemAsync(list[i], MaxIteratorCount);
            }
            await writer.WriteEndArrayAsync();

            await writer.WritePropertyNameAsync("storages");
            await writer.WriteStartArrayAsync();
            foreach (var contractHash in storages)
            {
                await writer.WriteStorageAsync(engine.Snapshot, contractHash);
            }
            await writer.WriteEndArrayAsync();

            if (debugInfo != null)
            {
                var contract = engine.ExecutedScripts.Values
                    .Where(s => s.IsT0).Select(s => s.AsT0)
                    .SingleOrDefault(c => c.Script.Span.ToScriptHash() == debugInfo.ScriptHash);

                if (contract != null)
                {
                    var sequencePoints = debugInfo.Methods.SelectMany(m => m.SequencePoints).ToArray();
                    var hitMap = engine.GetHitMap(contract.Hash);


                    var branchMap = engine.GetBranchMap(contract.Hash);

                    await writer.WritePropertyNameAsync("code-coverage");
                    await writer.WriteStartObjectAsync();

                    await writer.WritePropertyNameAsync("contract-hash");
                    await writer.WriteValueAsync($"{contract.Hash}");
                    await writer.WritePropertyNameAsync("debug-info-hash");
                    await writer.WriteValueAsync($"{debugInfo.ScriptHash}");

                    await writer.WritePropertyNameAsync("hit-map");
                    await writer.WriteStartObjectAsync();
                    foreach (var sp in sequencePoints)
                    {
                        await writer.WritePropertyNameAsync($"{sp.Address}");
                        var hitCount = hitMap.TryGetValue(sp.Address, out var count) ? count : 0;
                        await writer.WriteValueAsync(hitCount);
                    }
                    await writer.WriteEndObjectAsync();

                    await writer.WritePropertyNameAsync("branch-map");
                    await writer.WriteStartObjectAsync();
                    foreach (var t in GetBranchingSequencePoints(contract.Script, sequencePoints))
                    {
                        await writer.WritePropertyNameAsync($"{t.sequencePoint.Address}");
                        var (branchCount, continueCount) = branchMap.TryGetValue(t.branchAddress, out var count) ? count : (0, 0);
                        await writer.WriteValueAsync($"{branchCount}-{continueCount}");
                    }
                    await writer.WriteEndObjectAsync();

                    await writer.WriteEndObjectAsync();
                }
            }

            await writer.WriteEndObjectAsync();

            static IEnumerable<(DebugInfo.SequencePoint sequencePoint, int branchAddress)> GetBranchingSequencePoints(Script script, IReadOnlyList<DebugInfo.SequencePoint> sequencePoints)
            {
                var branchInstructions = script.EnumerateInstructions()
                    .Where(t => t.instruction.IsBranchInstruction())
                    .GroupBy(t => GetSequencePoint(t.address, sequencePoints));

                foreach (var group in branchInstructions)
                {
                    yield return (group.Key, group.Max(g => g.address));
                }
            }

            static DebugInfo.SequencePoint GetSequencePoint(int instructionPointer, IReadOnlyList<DebugInfo.SequencePoint> sequencePoints)
            {
                if (sequencePoints.Count == 0)
                {
                    throw new ArgumentException($"{nameof(sequencePoints)} can't be empty", nameof(sequencePoints));
                }

                for (int i = sequencePoints.Count - 1; i >= 0; i--)
                {
                    if (instructionPointer >= sequencePoints[i].Address)
                        return sequencePoints[i];
                }

                return sequencePoints[0];
            }
        }
    }
}
