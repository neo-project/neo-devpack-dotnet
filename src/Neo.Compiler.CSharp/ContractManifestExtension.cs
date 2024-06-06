

// Copyright (C) 2015-2024 The Neo Project.
//
// ContractManifestExtensions.cs file belongs to neo-express project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using System;
using System.Linq;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using scfx::Neo.SmartContract.Framework;
using scfx::Neo.SmartContract.Framework.Attributes;
using ContractParameterType = Neo.SmartContract.ContractParameterType;

namespace Neo.Compiler
{
    internal static class ContractManifestExtensions
    {
        private static void CheckNep11Compliant(this ContractManifest manifest)
        {
            try
            {
                var symbolMethod = manifest.Abi.GetMethod("symbol", 0);
                var decimalsMethod = manifest.Abi.GetMethod("decimals", 0);
                var totalSupplyMethod = manifest.Abi.GetMethod("totalSupply", 0);
                var balanceOfMethod1 = manifest.Abi.GetMethod("balanceOf", 1);
                var balanceOfMethod2 = manifest.Abi.GetMethod("balanceOf", 2);
                var tokensOfMethod = manifest.Abi.GetMethod("tokensOf", 1);
                var ownerOfMethod = manifest.Abi.GetMethod("ownerOf", 1);
                var transferMethod1 = manifest.Abi.GetMethod("transfer", 3);
                var transferMethod2 = manifest.Abi.GetMethod("transfer", 5);

                var symbolValid = symbolMethod != null && symbolMethod.Safe == true &&
                                  symbolMethod.ReturnType == ContractParameterType.String;
                var decimalsValid = decimalsMethod != null && decimalsMethod.Safe == true &&
                                    decimalsMethod.ReturnType == ContractParameterType.Integer;
                var totalSupplyValid = totalSupplyMethod != null && totalSupplyMethod.Safe == true &&
                                       totalSupplyMethod.ReturnType == ContractParameterType.Integer;
                var balanceOfValid1 = balanceOfMethod1 != null && balanceOfMethod1.Safe == true &&
                                      balanceOfMethod1.ReturnType == ContractParameterType.Integer &&
                                      balanceOfMethod1.Parameters[0].Type == ContractParameterType.Hash160;
                var balanceOfValid2 = balanceOfMethod2?.Safe == true &&
                                      balanceOfMethod2?.ReturnType == ContractParameterType.Integer &&
                                      balanceOfMethod2?.Parameters[0].Type == ContractParameterType.Hash160 &&
                                      balanceOfMethod2?.Parameters[0].Type == ContractParameterType.ByteArray;
                var tokensOfValid = tokensOfMethod != null && tokensOfMethod.Safe == true &&
                                    tokensOfMethod.ReturnType == ContractParameterType.InteropInterface &&
                                    tokensOfMethod.Parameters[0].Type == ContractParameterType.Hash160;
                var ownerOfValid1 = ownerOfMethod != null && ownerOfMethod.Safe == true &&
                                    ownerOfMethod.ReturnType == ContractParameterType.Hash160 &&
                                    ownerOfMethod.Parameters[0].Type == ContractParameterType.ByteArray;
                var ownerOfValid2 = ownerOfMethod != null && ownerOfMethod.Safe == true &&
                                    ownerOfMethod.ReturnType == ContractParameterType.InteropInterface &&
                                    ownerOfMethod.Parameters[0].Type == ContractParameterType.ByteArray;
                var transferValid1 = transferMethod1 != null && transferMethod1.Safe == false &&
                                     transferMethod1.ReturnType == ContractParameterType.Boolean &&
                                     transferMethod1.Parameters[0].Type == ContractParameterType.Hash160 &&
                                     transferMethod1.Parameters[1].Type == ContractParameterType.ByteArray &&
                                     transferMethod1.Parameters[2].Type == ContractParameterType.Any;
                var transferValid2 = transferMethod2?.Safe == false &&
                                     transferMethod2?.ReturnType == ContractParameterType.Boolean &&
                                     transferMethod2?.Parameters[0].Type == ContractParameterType.Hash160 &&
                                     transferMethod2?.Parameters[1].Type == ContractParameterType.Hash160 &&
                                     transferMethod2?.Parameters[2].Type == ContractParameterType.Integer &&
                                     transferMethod2?.Parameters[3].Type == ContractParameterType.ByteArray &&
                                     transferMethod2?.Parameters[4].Type == ContractParameterType.Any;
                var transferEvent = manifest.Abi.Events.Any(a =>
                    a.Name == "Transfer" &&
                    a.Parameters.Length == 4 &&
                    a.Parameters[0].Type == ContractParameterType.Hash160 &&
                    a.Parameters[1].Type == ContractParameterType.Hash160 &&
                    a.Parameters[2].Type == ContractParameterType.Integer &&
                    a.Parameters[3].Type == ContractParameterType.ByteArray);

                if (!symbolValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: symbol");
                if (!decimalsValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: decimals");

                if (!totalSupplyValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: totalSupply");

                if (!balanceOfValid1 && !balanceOfValid2) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf");

                if (!tokensOfValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: tokensOf");

                if (!ownerOfValid1 && !ownerOfValid2) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf");

                if (!transferValid1 && !transferValid2) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation:transfer");

                if (!transferEvent) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: {nameof(transferEvent)}");
            }
            catch (Exception ex) when (ex is not CompilationException)
            {
                throw;
            }
            catch
            {
                throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: Unidentified issue.");
            }
        }

        private static void CheckNep24Compliant(this ContractManifest manifest)
        {
            try
            {
                var royaltyInfoMethod = manifest.Abi.GetMethod("royaltyInfo", 0);

                var royaltyInfoValid = royaltyInfoMethod != null && royaltyInfoMethod.Safe &&
                                  royaltyInfoMethod.ReturnType == ContractParameterType.Array &&
                                  royaltyInfoMethod.Parameters[0].Type == ContractParameterType.ByteArray &&
                                  royaltyInfoMethod.Parameters[1].Type == ContractParameterType.Hash160 &&
                                  royaltyInfoMethod.Parameters[2].Type == ContractParameterType.Integer;

                if (!royaltyInfoValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo");
            }
            catch (Exception ex) when (ex is not CompilationException)
            {
                throw;
            }
            catch
            {
                throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep24.ToStandard()} implementation: Unidentified issue.");
            }
        }

        private static void CheckNep17Compliant(this ContractManifest manifest)
        {
            try
            {
                var symbolMethod = manifest.Abi.GetMethod("symbol", 0);
                var decimalsMethod = manifest.Abi.GetMethod("decimals", 0);
                var totalSupplyMethod = manifest.Abi.GetMethod("totalSupply", 0);
                var balanceOfMethod = manifest.Abi.GetMethod("balanceOf", 1);
                var transferMethod = manifest.Abi.GetMethod("transfer", 4);

                var symbolValid = symbolMethod != null && symbolMethod.Safe &&
                                  symbolMethod.ReturnType == ContractParameterType.String;
                var decimalsValid = decimalsMethod != null && decimalsMethod.Safe &&
                                    decimalsMethod.ReturnType == ContractParameterType.Integer;
                var totalSupplyValid = totalSupplyMethod != null && totalSupplyMethod.Safe &&
                                       totalSupplyMethod.ReturnType == ContractParameterType.Integer;
                var balanceOfValid = balanceOfMethod != null && balanceOfMethod.Safe &&
                                     balanceOfMethod.ReturnType == ContractParameterType.Integer &&
                                     balanceOfMethod.Parameters[0].Type == ContractParameterType.Hash160;
                var transferValid = transferMethod != null && transferMethod.Safe == false &&
                                    transferMethod.ReturnType == ContractParameterType.Boolean &&
                                    transferMethod.Parameters[0].Type == ContractParameterType.Hash160 &&
                                    transferMethod.Parameters[1].Type == ContractParameterType.Hash160 &&
                                    transferMethod.Parameters[2].Type == ContractParameterType.Integer &&
                                    transferMethod.Parameters[3].Type == ContractParameterType.Any;
                var transferEvent = manifest.Abi.Events.Any(s =>
                    s.Name == "Transfer" &&
                    s.Parameters.Length == 3 &&
                    s.Parameters[0].Type == ContractParameterType.Hash160 &&
                    s.Parameters[1].Type == ContractParameterType.Hash160 &&
                    s.Parameters[2].Type == ContractParameterType.Integer);

                if (!symbolValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: symbol");
                if (!decimalsValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: decimals");
                if (!totalSupplyValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: totalSupply");
                if (!balanceOfValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: balanceOf");
                if (!transferValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer");
            }
            catch (Exception ex) when (ex is not CompilationException)
            {
                throw new CompilationException(DiagnosticId.IncorrectNEPStandard, $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: Unidentified issue.");
            }
        }

        private static void CheckNep11PayableCompliant(this ContractManifest manifest)
        {
            try
            {
                var onNEP11PaymentMethod = manifest.Abi.GetMethod("onNEP11Payment", 4);
                var onNEP11PaymentValid = onNEP11PaymentMethod is { ReturnType: ContractParameterType.Void } &&
                                          onNEP11PaymentMethod.Parameters[0].Type == ContractParameterType.Hash160 &&
                                          onNEP11PaymentMethod.Parameters[1].Type == ContractParameterType.Integer &&
                                          onNEP11PaymentMethod.Parameters[2].Type == ContractParameterType.String &&
                                          onNEP11PaymentMethod.Parameters[3].Type == ContractParameterType.Any;

                if (!onNEP11PaymentValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep11Payable.ToStandard()} implementation: onNEP11Payment");
            }
            catch (Exception ex) when (ex is not CompilationException)
            {
                throw new CompilationException(DiagnosticId.IncorrectNEPStandard, $"Incomplete NEP standard {NepStandard.Nep11Payable.ToStandard()} implementation: Unidentified issue.");
            }
        }

        private static void CheckNep17PayableCompliant(this ContractManifest manifest)
        {
            try
            {
                var onNEP17PaymentMethod = manifest.Abi.GetMethod("onNEP17Payment", 3);
                var onNEP17PaymentValid = onNEP17PaymentMethod is { ReturnType: ContractParameterType.Void } &&
                                          onNEP17PaymentMethod.Parameters[0].Type == ContractParameterType.Hash160 &&
                                          onNEP17PaymentMethod.Parameters[1].Type == ContractParameterType.Integer &&
                                          onNEP17PaymentMethod.Parameters[2].Type == ContractParameterType.Any;

                if (!onNEP17PaymentValid) throw new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17Payable.ToStandard()} implementation: onNEP17Payment");
            }
            catch (Exception ex) when (ex is not CompilationException)
            {
                throw new CompilationException(DiagnosticId.IncorrectNEPStandard, $"Incomplete NEP standard {NepStandard.Nep17Payable.ToStandard()} implementation: Unidentified issue.");
            }
        }

        internal static ContractManifest CheckStandards(this ContractManifest manifest)
        {
            if (manifest.SupportedStandards.Contains(NepStandard.Nep11.ToStandard()))
            {
                manifest.CheckNep11Compliant();
            }

            if (manifest.SupportedStandards.Contains(NepStandard.Nep17.ToStandard()))
            {
                manifest.CheckNep17Compliant();
            }

            if (manifest.SupportedStandards.Contains(NepStandard.Nep24.ToStandard()))
            {
                manifest.CheckNep24Compliant();
            }

            if (manifest.SupportedStandards.Contains(NepStandard.Nep11Payable.ToStandard()))
            {
                manifest.CheckNep11PayableCompliant();
            }

            if (manifest.SupportedStandards.Contains(NepStandard.Nep17Payable.ToStandard()))
            {
                manifest.CheckNep17PayableCompliant();
            }

            return manifest;
        }
    }
}
