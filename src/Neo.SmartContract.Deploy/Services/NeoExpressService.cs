using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo;
using Neo.SmartContract.Deploy.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Service for interacting with Neo Express for local development
    /// </summary>
    public interface INeoExpressService
    {
        /// <summary>
        /// Check if Neo Express is installed
        /// </summary>
        Task<bool> IsInstalledAsync();

        /// <summary>
        /// Check if Neo Express is running
        /// </summary>
        Task<bool> IsRunningAsync();

        /// <summary>
        /// Start Neo Express
        /// </summary>
        Task<bool> StartAsync();

        /// <summary>
        /// Stop Neo Express
        /// </summary>
        Task<bool> StopAsync();

        /// <summary>
        /// Create a new Neo Express instance
        /// </summary>
        Task<bool> CreateInstanceAsync(string name = "default");

        /// <summary>
        /// Reset the blockchain
        /// </summary>
        Task<bool> ResetAsync();

        /// <summary>
        /// Create a checkpoint
        /// </summary>
        Task<string> CreateCheckpointAsync(string name);

        /// <summary>
        /// Restore from checkpoint
        /// </summary>
        Task<bool> RestoreCheckpointAsync(string name);

        /// <summary>
        /// Transfer GAS to an account
        /// </summary>
        Task<bool> TransferGasAsync(string toAddress, decimal amount);

        /// <summary>
        /// Get account information
        /// </summary>
        Task<NeoExpressAccount?> GetAccountAsync(string name);

        /// <summary>
        /// Create a new wallet
        /// </summary>
        Task<string?> CreateWalletAsync(string name);
    }

    /// <summary>
    /// Implementation of Neo Express service
    /// </summary>
    public class NeoExpressService : INeoExpressService
    {
        private readonly ILogger<NeoExpressService> _logger;
        private readonly NeoExpressOptions _options;
        private Process? _expressProcess;

        public NeoExpressService(ILogger<NeoExpressService> logger, IOptions<NeoExpressOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<bool> IsInstalledAsync()
        {
            try
            {
                var result = await RunCommandAsync("--version");
                return result.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsRunningAsync()
        {
            try
            {
                // Try to connect to the RPC endpoint
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(2);
                
                var request = new {
                    jsonrpc = "2.0",
                    method = "getblockcount",
                    @params = Array.Empty<object>(),
                    id = 1
                };
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
                var response = await client.PostAsync(
                    _options.RpcUrl,
                    new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> StartAsync()
        {
            try
            {
                if (await IsRunningAsync())
                {
                    _logger.LogInformation("Neo Express is already running");
                    return true;
                }

                _logger.LogInformation("Starting Neo Express...");

                _expressProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "neoxp",
                        Arguments = $"run -i {_options.ConfigFile} -s {_options.SecondsPerBlock}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                _expressProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        _logger.LogDebug("[NeoExpress] {Output}", e.Data);
                };

                _expressProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        _logger.LogError("[NeoExpress] {Error}", e.Data);
                };

                _expressProcess.Start();
                _expressProcess.BeginOutputReadLine();
                _expressProcess.BeginErrorReadLine();

                // Wait for Neo Express to start
                var retries = 30;
                while (retries-- > 0 && !await IsRunningAsync())
                {
                    await Task.Delay(_options.RetryWaitMs);
                }

                if (await IsRunningAsync())
                {
                    _logger.LogInformation("Neo Express started successfully");
                    return true;
                }

                _logger.LogError("Failed to start Neo Express");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting Neo Express");
                return false;
            }
        }

        public async Task<bool> StopAsync()
        {
            try
            {
                if (_expressProcess != null && !_expressProcess.HasExited)
                {
                    _logger.LogInformation("Stopping Neo Express...");
                    _expressProcess.Kill();
                    await _expressProcess.WaitForExitAsync();
                    _expressProcess.Dispose();
                    _expressProcess = null;
                    _logger.LogInformation("Neo Express stopped");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Neo Express");
                return false;
            }
        }

        public async Task<bool> CreateInstanceAsync(string name = "default")
        {
            try
            {
                _logger.LogInformation("Creating Neo Express instance: {Name}", name);
                
                var result = await RunCommandAsync($"create -f {_options.ConfigFile}");
                
                if (result.ExitCode == 0)
                {
                    _logger.LogInformation("Neo Express instance created successfully");
                    
                    // Create genesis wallet
                    await CreateWalletAsync("genesis");
                    
                    return true;
                }

                _logger.LogError("Failed to create Neo Express instance: {Error}", result.Error);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Neo Express instance");
                return false;
            }
        }

        public async Task<bool> ResetAsync()
        {
            try
            {
                _logger.LogInformation("Resetting Neo Express blockchain...");
                
                await StopAsync();
                
                var result = await RunCommandAsync($"reset -i {_options.ConfigFile} -f");
                
                if (result.ExitCode == 0)
                {
                    _logger.LogInformation("Neo Express blockchain reset successfully");
                    return true;
                }

                _logger.LogError("Failed to reset blockchain: {Error}", result.Error);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting blockchain");
                return false;
            }
        }

        public async Task<string> CreateCheckpointAsync(string name)
        {
            try
            {
                _logger.LogInformation("Creating checkpoint: {Name}", name);
                
                var result = await RunCommandAsync($"checkpoint create {name} -i {_options.ConfigFile}");
                
                if (result.ExitCode == 0)
                {
                    _logger.LogInformation("Checkpoint created: {Name}", name);
                    return name;
                }

                throw new Exception($"Failed to create checkpoint: {result.Error}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkpoint");
                throw;
            }
        }

        public async Task<bool> RestoreCheckpointAsync(string name)
        {
            try
            {
                _logger.LogInformation("Restoring checkpoint: {Name}", name);
                
                await StopAsync();
                
                var result = await RunCommandAsync($"checkpoint restore {name} -i {_options.ConfigFile} -f");
                
                if (result.ExitCode == 0)
                {
                    _logger.LogInformation("Checkpoint restored: {Name}", name);
                    return true;
                }

                _logger.LogError("Failed to restore checkpoint: {Error}", result.Error);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring checkpoint");
                return false;
            }
        }

        public async Task<bool> TransferGasAsync(string toAddress, decimal amount)
        {
            try
            {
                _logger.LogInformation("Transferring {Amount} GAS to {Address}", amount, toAddress);
                
                var result = await RunCommandAsync(
                    $"transfer GAS {amount} genesis {toAddress} -i {_options.ConfigFile}"
                );
                
                if (result.ExitCode == 0)
                {
                    _logger.LogInformation("GAS transfer successful");
                    return true;
                }

                _logger.LogError("Failed to transfer GAS: {Error}", result.Error);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring GAS");
                return false;
            }
        }

        public async Task<NeoExpressAccount?> GetAccountAsync(string name)
        {
            try
            {
                var result = await RunCommandAsync($"wallet show {name} -i {_options.ConfigFile}");
                
                if (result.ExitCode == 0 && !string.IsNullOrEmpty(result.Output))
                {
                    // Parse the output to extract account information
                    var lines = result.Output.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.Contains("Address:"))
                        {
                            var address = line.Split(':')[1].Trim();
                            return new NeoExpressAccount
                            {
                                Name = name,
                                Address = address
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account information");
                return null;
            }
        }

        public async Task<string?> CreateWalletAsync(string name)
        {
            try
            {
                _logger.LogInformation("Creating wallet: {Name}", name);
                
                var result = await RunCommandAsync($"wallet create {name} -i {_options.ConfigFile}");
                
                if (result.ExitCode == 0)
                {
                    var account = await GetAccountAsync(name);
                    if (account != null)
                    {
                        _logger.LogInformation("Wallet created: {Name} ({Address})", name, account.Address);
                        return account.Address;
                    }
                }

                _logger.LogError("Failed to create wallet: {Error}", result.Error);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating wallet");
                return null;
            }
        }

        private async Task<CommandResult> RunCommandAsync(string arguments)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "neoxp",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();

            return new CommandResult
            {
                ExitCode = process.ExitCode,
                Output = output,
                Error = error
            };
        }

        private class CommandResult
        {
            public int ExitCode { get; set; }
            public string Output { get; set; } = string.Empty;
            public string Error { get; set; } = string.Empty;
        }
    }

    /// <summary>
    /// Neo Express account information
    /// </summary>
    public class NeoExpressAccount
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}