using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.BuildTasks
{
    // https://github.com/jamesmanning/RunProcessAsTask
    class ProcessRunner
    {
        public record struct Results(int ExitCode, IReadOnlyCollection<string> Output, IReadOnlyCollection<string> Error);

        public static Results Run(string command, string arguments, string? workingDirectory = null)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo(command, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? null : workingDirectory,
            };

            var process = new System.Diagnostics.Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            ConcurrentQueue<string> output = new();
            process.OutputDataReceived += (sender, args) => { if (args.Data != null) { output.Enqueue(args.Data); } };

            ConcurrentQueue<string> error = new();
            process.ErrorDataReceived += (sender, args) => { if (args.Data != null) { error.Enqueue(args.Data); } };

            ManualResetEvent completeEvent = new(false);

            process.Exited += (_, _) => completeEvent.Set();

            if (!process.Start()) throw new Exception("Process failed to start");
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            completeEvent.WaitOne();

            return new Results(process.ExitCode, output, error);
        }
    }
}
