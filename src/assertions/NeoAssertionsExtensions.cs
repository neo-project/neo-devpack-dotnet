using Neo.Ledger;
using Neo.SmartContract;
using Neo.VM.Types;

namespace Neo.Assertions
{
    public static class NeoAssertionsExtensions
    {
        public static StackItemAssertions Should(this StackItem item) => new StackItemAssertions(item);

        public static NotifyEventArgsAssertions Should(this NotifyEventArgs args) => new NotifyEventArgsAssertions(args);

        public static StorageItemAssertions Should(this StorageItem item) => new StorageItemAssertions(item);
    }
}
