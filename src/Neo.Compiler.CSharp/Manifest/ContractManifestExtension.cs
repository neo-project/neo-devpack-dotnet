// Copyright (C) 2015-2026 The Neo Project.
//
// ContractManifestExtension.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Manifest;
using scfx::Neo.SmartContract.Framework;
using ContractParameterType = Neo.SmartContract.ContractParameterType;

namespace Neo.Compiler
{
    internal static partial class ContractManifestExtensions
    {
        private static System.Collections.Generic.List<CompilationException>
            CheckNep29Compliant(this ContractManifest manifest)
        {
            var deployMethod = manifest.Abi.GetMethod("_deploy", 2);
            var deployValid = deployMethod is not null &&
                              deployMethod.ReturnType == ContractParameterType.Void &&
                              deployMethod.Parameters.Length == 2 &&
                              deployMethod.Parameters[0].Type == ContractParameterType.Any &&
                              deployMethod.Parameters[1].Type == ContractParameterType.Boolean;

            System.Collections.Generic.List<CompilationException> errors = [];
            if (!deployValid) errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete NEP standard {NepStandard.Nep29.ToStandard()} implementation: _deploy"));
            return errors;
        }

        private static System.Collections.Generic.List<CompilationException>
            CheckNep30Compliant(this ContractManifest manifest)
        {
            var verifyMethod = manifest.Abi.GetMethod("verify", -1);
            var verifyValid = verifyMethod is not null && verifyMethod.Safe &&
                                verifyMethod.ReturnType == ContractParameterType.Boolean;

            System.Collections.Generic.List<CompilationException> errors = [];
            if (!verifyValid) errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep30.ToStandard()} implementation: verify"));
            return errors;
        }

        private static System.Collections.Generic.List<CompilationException>
            CheckNep11PayableCompliant(this ContractManifest manifest)
        {
            var onNEP11PaymentMethod = manifest.Abi.GetMethod("onNEP11Payment", 4);
            // The tokenId is a NEP-11 token id (ByteString), which maps to the ByteArray
            // contract parameter type. See INEP26.OnNEP11Payment.
            var onNEP11PaymentValid = onNEP11PaymentMethod is { ReturnType: ContractParameterType.Void } &&
                                        onNEP11PaymentMethod.Parameters.Length == 4 &&
                                        onNEP11PaymentMethod.Parameters[0].Type == ContractParameterType.Hash160 &&
                                        onNEP11PaymentMethod.Parameters[1].Type == ContractParameterType.Integer &&
                                        onNEP11PaymentMethod.Parameters[2].Type == ContractParameterType.ByteArray &&
                                        onNEP11PaymentMethod.Parameters[3].Type == ContractParameterType.Any;

            System.Collections.Generic.List<CompilationException> errors = [];
            if (!onNEP11PaymentValid) errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete NEP standard {NepStandard.Nep26.ToStandard()} implementation: onNEP11Payment"));
            return errors;
        }

        private static System.Collections.Generic.List<CompilationException>
            CheckNep17PayableCompliant(this ContractManifest manifest)
        {
            var onNEP17PaymentMethod = manifest.Abi.GetMethod("onNEP17Payment", 3);
            var onNEP17PaymentValid = onNEP17PaymentMethod is { ReturnType: ContractParameterType.Void } &&
                                        onNEP17PaymentMethod.Parameters.Length == 3 &&
                                        onNEP17PaymentMethod.Parameters[0].Type == ContractParameterType.Hash160 &&
                                        onNEP17PaymentMethod.Parameters[1].Type == ContractParameterType.Integer &&
                                        onNEP17PaymentMethod.Parameters[2].Type == ContractParameterType.Any;

            System.Collections.Generic.List<CompilationException> errors = [];
            if (!onNEP17PaymentValid) errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete NEP standard {NepStandard.Nep27.ToStandard()} implementation: onNEP17Payment"));
            return errors;
        }

        /// <summary>
        /// Collects every NEP standard compliance violation for the standards the
        /// manifest declares in <c>supportedstandards</c>. A non-empty result means the
        /// contract advertises a standard it does not correctly implement and must fail
        /// compilation, otherwise wallets, explorers and exchanges would trust an
        /// inaccurate declaration.
        /// </summary>
        internal static System.Collections.Generic.List<CompilationException> GetStandardsViolations(this ContractManifest manifest)
        {
            System.Collections.Generic.List<CompilationException> errors = [];
            if (manifest.SupportedStandards.Contains(NepStandard.Nep11.ToStandard()))
                errors.AddRange(manifest.CheckNep11Compliant());

            if (manifest.SupportedStandards.Contains(NepStandard.Nep17.ToStandard()))
                errors.AddRange(manifest.CheckNep17Compliant());

            if (manifest.SupportedStandards.Contains(NepStandard.Nep24.ToStandard()))
                errors.AddRange(manifest.CheckNep24Compliant());

            if (manifest.SupportedStandards.Contains(NepStandard.Nep26.ToStandard()))
                errors.AddRange(manifest.CheckNep11PayableCompliant());

            if (manifest.SupportedStandards.Contains(NepStandard.Nep27.ToStandard()))
                errors.AddRange(manifest.CheckNep17PayableCompliant());

            if (manifest.SupportedStandards.Contains(NepStandard.Nep29.ToStandard()))
                errors.AddRange(manifest.CheckNep29Compliant());

            if (manifest.SupportedStandards.Contains(NepStandard.Nep30.ToStandard()))
                errors.AddRange(manifest.CheckNep30Compliant());

            return errors;
        }
    }
}
