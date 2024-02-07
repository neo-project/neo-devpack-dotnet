using System;

namespace Neo.SmartContract.Analyzer.Sample
{
    [SupportedStandards("NEP-11")]
    public class Examples
    {

    }

    public class SupportedStandardsAttribute : Attribute
    {
        public SupportedStandardsAttribute(string nep)
        {
            throw new NotImplementedException();
        }
    }
}
