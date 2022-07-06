using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities.ProjectCreation;
using Neo.BuildTasks;

namespace build_tasks
{
    static class Extensions
    {
        public static void RunThrow(this IProcessRunner @this, string command, string arguments, string? workingDirectory = null)
        {
            var result = @this.Run(command, arguments, workingDirectory);
            if (result.ExitCode != 0)
            {
                if (result.Error.Count == 1)
                {
                    throw new Exception(result.Error.Single());
                }
                else
                {
                    throw new AggregateException(result.Error.Select(e => new Exception(e)));
                }
            }
        }

        public static ProjectCreator ImportNeoBuildTools(this ProjectCreator @this)
        {
            var buildTasksPath = typeof(NeoCsc).Assembly.Location;
            var testBuildAssmblyDirectory = Path.GetDirectoryName(typeof(TestBuild).Assembly.Location)
                ?? throw new Exception("Couldn't get directory name of TestBuild assembly");
            var targetsPath = Path.Combine(testBuildAssmblyDirectory, "build", "Neo.BuildTasks.targets");

            return @this
                .Property("NeoBuildTasksAssembly", buildTasksPath)
                .Import(targetsPath);
        }
    }
}
