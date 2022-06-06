using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Neo.BuildTasks
{
    public interface IProcessRunner
    {
        record struct Results(int ExitCode, IReadOnlyCollection<string> Output, IReadOnlyCollection<string> Error);
        Results Run(string command, string arguments, string? workingDirectory = null);
    }

    // https://github.com/jamesmanning/RunProcessAsTask
    class ProcessRunner : IProcessRunner
    {
        public IProcessRunner.Results Run(string command, string arguments, string? workingDirectory = null)
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

            return new IProcessRunner.Results(process.ExitCode, output, error);
        }
    }
}
