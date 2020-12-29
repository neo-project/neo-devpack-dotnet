using System;
using System.IO;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;

namespace NeoTestHarness
{
    public partial class TestApplicationEngine
    {
        public class TestVerifiable : Neo.Network.P2P.Payloads.IVerifiable
        {
            readonly UInt160[] signers;

            public TestVerifiable(params UInt160[] signers)
            {
                this.signers = signers;
            }

            public Witness[] Witnesses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public int Size => throw new NotImplementedException();

            public void Deserialize(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            public void DeserializeUnsigned(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            public UInt160[] GetScriptHashesForVerifying(StoreView snapshot)
            {
                return signers;
            }

            public void Serialize(BinaryWriter writer)
            {
                throw new NotImplementedException();
            }

            public void SerializeUnsigned(BinaryWriter writer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
