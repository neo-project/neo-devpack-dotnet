// Copyright (C) 2015-2024 The Neo Project.
//
// Inscription.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace Inscription
{
    [DisplayName("SampleInscription")]
    [ContractAuthor("core-dev", "core@neo.org")]
    [ContractDescription("A sample inscription contract.")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/examples/Example.SmartContract.Inscription")]
    [ContractPermission(Permission.WildCard, Method.WildCard)]
    public class SampleInscription : SmartContract
    {
        // Neo.SmartContract.Examples.Event for logging inscriptions
        [DisplayName("InscriptionAdded")]
        public static event Action<UInt160, string> InscriptionAdded;

        // Method to store an inscription
        public static void AddInscription(UInt160 address, string inscription)
        {
            if (!Runtime.CheckWitness(address))
                throw new Exception("Unauthorized: Caller is not the address owner");

            Storage.Put(Storage.CurrentContext, address, inscription);
            InscriptionAdded(address, inscription);
        }

        // Method to read an inscription
        [Safe]
        public static string GetInscription(UInt160 address)
        {
            return Storage.Get(Storage.CurrentContext, address);
        }

        [DisplayName("_deploy")]
        public static void OnDeployment(object data, bool update)
        {
            if (update)
            {
                // Add logic for fixing contract on update
                return;
            }
            // Add logic here for 1st time deployed
        }

        // TODO: Allow ONLY contract owner to call update
        public static bool Update(ByteString nefFile, string manifest)
        {
            ContractManagement.Update(nefFile, manifest);
            return true;
        }

        // TODO: Allow ONLY contract owner to call destroy
        public static bool Destroy()
        {
            ContractManagement.Destroy();
            return true;
        }
    }
}
