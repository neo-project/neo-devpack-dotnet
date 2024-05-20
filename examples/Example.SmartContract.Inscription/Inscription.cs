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

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace Inscription
{
    [DisplayName("SampleInscription")]
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractDescription("A sample inscription contract.")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class SampleInscription : SmartContract
    {
        /// <summary>
        /// Neo.SmartContract.Examples.Event for logging inscriptions
        /// </summary>
        [DisplayName("InscriptionAdded")]
        public static event Action<UInt160, string> InscriptionAdded;

        /// <summary>
        /// Method to store an inscription
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="inscription">Inscription</param>
        /// <exception cref="Exception">Failure when is not signed by the address</exception>
        public static void AddInscription(UInt160 address, string inscription)
        {
            if (!Runtime.CheckWitness(address))
                throw new Exception("Unauthorized: Caller is not the address owner");

            Storage.Put(Storage.CurrentContext, address, inscription);
            InscriptionAdded(address, inscription);
        }

        /// <summary>
        /// Method to read an inscription
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Inscription readed</returns>
        [Safe]
        public static string GetInscription(UInt160 address)
        {
            return Storage.Get(Storage.CurrentContext, address);
        }
    }
}
