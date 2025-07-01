using Microsoft.Extensions.Logging;

namespace Neo.SmartContract.Deploy.Steps
{
    /// <summary>
    /// Base class for deployment steps
    /// </summary>
    public abstract class BaseDeploymentStep : IDeploymentStep
    {
        protected readonly ILogger Logger;

        protected BaseDeploymentStep(ILogger logger)
        {
            Logger = logger;
        }

        public abstract string Name { get; }
        public abstract int Order { get; }

        public abstract Task<bool> ExecuteAsync(DeploymentContext context);

        /// <summary>
        /// Log and execute a sub-step
        /// </summary>
        protected async Task<T> ExecuteSubStepAsync<T>(string stepName, Func<Task<T>> action)
        {
            Logger.LogInformation("[{Step}] {SubStep}", Name, stepName);
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[{Step}] Failed at {SubStep}", Name, stepName);
                throw;
            }
        }
    }
}