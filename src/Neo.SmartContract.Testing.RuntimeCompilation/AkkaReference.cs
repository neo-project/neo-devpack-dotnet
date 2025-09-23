using Akka.Actor;

namespace Neo.SmartContract.Testing.RuntimeCompilation;

/// <summary>
/// Provides a compile-time reference to Akka so the runtime host
/// copies the Akka assemblies when this package is referenced.
/// </summary>
internal static class AkkaReference
{
    // ReSharper disable once UnusedMember.Local -- purposefully unused.
    private static readonly System.Type ActorSystemType = typeof(ActorSystem);
}
