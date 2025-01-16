// Copyright (C) 2015-2025 The Neo Project.
//
// CoverageFormatBase.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public abstract class CoverageFormatBase
    {
        public abstract void WriteReport(Action<string, Action<Stream>> writeAttachement);

        /// <summary>
        /// Dump to format
        /// </summary>
        /// <returns>First entry as string</returns>
        public string Dump()
        {
            Dictionary<string, string> outputMap = new();

            void writeAttachment(string filename, Action<Stream> writestream)
            {
                using MemoryStream stream = new();
                writestream(stream);
                var text = Encoding.UTF8.GetString(stream.ToArray());
                outputMap.Add(filename, text);
            }

            WriteReport(writeAttachment);
            return outputMap.First().Value;
        }
    }
}
