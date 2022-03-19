using System;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;

namespace Neo.BuildTasks
{
    public class NeoExpressBatch : DotNetToolTask
    {
        const string PACKAGE_ID = "Neo.Express";
        const string COMMAND = "neoxp";

        protected override string Command => COMMAND;
        protected override string PackageId => PACKAGE_ID;

        [Required]
        public ITaskItem? BatchFile { get; set; }

        public ITaskItem? InputFile { get; set; }

        public bool Reset { get; set; }

        public ITaskItem? Checkpoint { get; set; }

        public bool Trace { get; set; }

        public bool StackTrace { get; set; }

        protected override string GetArguments()
        {
            if (BatchFile is null) throw new Exception("Missing BatchFile Property");

            var builder = new StringBuilder("batch ");
            builder.AppendFormat("\"{0}\"", BatchFile.ItemSpec);

            if (InputFile is not null)
            {
                builder.AppendFormat(" --input \"{0}\"", InputFile.ItemSpec);
            }

            if (Reset)
            {
                builder.Append(" --reset");
                if (Checkpoint is not null)
                {
                    builder.AppendFormat(":\"{0}\"", Checkpoint.ItemSpec);
                }
            }

            if (Trace) { builder.Append(" --trace"); }
            if (StackTrace) { builder.Append(" --stack-trace"); }

            return builder.ToString();
        }
    }
}
