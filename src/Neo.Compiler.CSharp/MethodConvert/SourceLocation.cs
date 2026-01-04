// Copyright (C) 2015-2026 The Neo Project.
//
// SourceLocation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Neo.Json;

namespace Neo.Compiler
{
    public class SourceLocation
    {
        public Location Location { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">Location</param>
        public SourceLocation(Location location)
        {
            Location = location;
        }

        private static string ToRangeString(LinePosition pos) => $"{pos.Line + 1}:{pos.Character + 1}";

        /// <summary>
        /// Convert to Json
        /// </summary>
        /// <param name="docIndex">Document index</param>
        /// <returns>Json object</returns>
        public JObject ToJson(int docIndex)
        {
            var source = new JObject();
            source["document"] = docIndex;
            source["location"] = GetRange();

            return source;
        }

        /// <summary>
        /// Get Range for NEP19
        /// </summary>
        /// <returns>Location's range</returns>
        public string GetRange()
        {
            var span = Location.GetLineSpan();
            return ToRangeString(span.StartLinePosition) + "-" + ToRangeString(span.EndLinePosition);
        }
    }
}
