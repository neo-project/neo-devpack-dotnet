// Auto-generated plugin for SampleHelloWorld
// Contract Hash: 0xef061fe2c2f02e63f00159e99dfd90cbc54ae0d2

using System;
using Neo;
using Neo.ConsoleService;
using Neo.Plugins;

namespace Neo.Plugins.SampleHelloWorldPlugin
{
    public class SampleHelloWorldPlugin : Plugin
    {
        public override string Name => "SampleHelloWorldPlugin";
        public override string Description => "CLI plugin for interacting with SampleHelloWorld contract";

        private NeoSystem _system;
        private SampleHelloWorldCommands _samplehelloworldCommands;
        private SampleHelloWorldWrapper _contractWrapper;

        public SampleHelloWorldPlugin()
        {
        }

        protected override void Configure()
        {
            // Load configuration if needed
        }

        protected override void OnSystemLoaded(NeoSystem system)
        {
            _system = system;
            _contractWrapper = new SampleHelloWorldWrapper(system);
            _samplehelloworldCommands = new SampleHelloWorldCommands(_contractWrapper);

            // Register CLI commands
            ConsoleHelper.RegisterCommand("samplehelloworld", _samplehelloworldCommands.Handle);

            Log($"{Name}: CLI commands registered for SampleHelloWorld contract", LogLevel.Info);
        }

        public override void Dispose()
        {
            // Cleanup if needed
        }
    }
}
