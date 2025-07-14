using System.CommandLine;
using System.Text.Json;
using R3E.WebGUI.Deploy.Services;

namespace R3E.WebGUI.Deploy;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var contractAddressOption = new Option<string>(
            "--contract-address",
            "The deployed contract address (0x...)") { IsRequired = true };

        var contractNameOption = new Option<string>(
            "--contract-name",
            "The name of the contract") { IsRequired = true };

        var networkOption = new Option<string>(
            "--network",
            () => "testnet",
            "The network where the contract is deployed (testnet, mainnet)");

        var deployerAddressOption = new Option<string>(
            "--deployer-address",
            "The address of the deployer") { IsRequired = true };

        var webguiPathOption = new Option<string>(
            "--webgui-path",
            "Path to the WebGUI files directory") { IsRequired = true };

        var serviceUrlOption = new Option<string>(
            "--service-url",
            () => "https://api.r3e-gui.com",
            "URL of the R3E WebGUI hosting service");

        var descriptionOption = new Option<string?>(
            "--description",
            "Description of the contract WebGUI");

        var rootCommand = new RootCommand("R3E WebGUI Deploy Tool - Deploy Neo contract WebGUIs to R3E hosting service")
        {
            contractAddressOption,
            contractNameOption,
            networkOption,
            deployerAddressOption,
            webguiPathOption,
            serviceUrlOption,
            descriptionOption
        };

        rootCommand.SetHandler(async (contractAddress, contractName, network, deployerAddress, webguiPath, serviceUrl, description) =>
        {
            var deployer = new WebGUIDeployer(serviceUrl);
            await deployer.DeployAsync(contractAddress, contractName, network, deployerAddress, webguiPath, description);
        }, contractAddressOption, contractNameOption, networkOption, deployerAddressOption, webguiPathOption, serviceUrlOption, descriptionOption);

        return await rootCommand.InvokeAsync(args);
    }
}