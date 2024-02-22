using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public interface ICoverageFormat
    {
        void WriteReport(Action<string, Action<Stream>> writeAttachement);
    }
}
