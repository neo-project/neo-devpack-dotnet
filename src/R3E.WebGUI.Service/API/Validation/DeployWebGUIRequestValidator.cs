using FluentValidation;
using R3E.WebGUI.Service.API.Controllers;
using System.Text.RegularExpressions;

namespace R3E.WebGUI.Service.API.Validation;

public class DeployWebGUIRequestValidator : AbstractValidator<DeployWebGUIRequest>
{
    private static readonly Regex ContractAddressRegex = new(@"^0x[a-fA-F0-9]{40}$", RegexOptions.Compiled);
    private static readonly string[] AllowedNetworks = { "testnet", "mainnet" };
    private static readonly string[] AllowedFileExtensions = { ".html", ".css", ".js", ".json", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf", ".eot" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private const int MaxTotalFiles = 100;

    public DeployWebGUIRequestValidator()
    {
        RuleFor(x => x.ContractAddress)
            .NotEmpty()
            .WithMessage("Contract address is required")
            .Must(BeValidContractAddress)
            .WithMessage("Contract address must be in format 0x followed by 40 hexadecimal characters");

        RuleFor(x => x.ContractName)
            .NotEmpty()
            .WithMessage("Contract name is required")
            .Length(1, 100)
            .WithMessage("Contract name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z0-9_\-\.]+$")
            .WithMessage("Contract name can only contain letters, numbers, hyphens, underscores, and dots");

        RuleFor(x => x.Network)
            .NotEmpty()
            .WithMessage("Network is required")
            .Must(BeValidNetwork)
            .WithMessage($"Network must be one of: {string.Join(", ", AllowedNetworks)}");

        RuleFor(x => x.DeployerAddress)
            .NotEmpty()
            .WithMessage("Deployer address is required")
            .Must(BeValidContractAddress)
            .WithMessage("Deployer address must be in format 0x followed by 40 hexadecimal characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.WebGUIFiles)
            .NotNull()
            .WithMessage("WebGUI files are required")
            .Must(HaveAtLeastOneFile)
            .WithMessage("At least one WebGUI file must be provided")
            .Must(NotExceedMaxFileCount)
            .WithMessage($"Cannot upload more than {MaxTotalFiles} files")
            .Must(HaveValidFileTypes)
            .WithMessage($"Only the following file types are allowed: {string.Join(", ", AllowedFileExtensions)}")
            .Must(NotExceedMaxFileSize)
            .WithMessage($"Individual files cannot exceed {MaxFileSize / (1024 * 1024)}MB")
            .Must(HaveIndexHtml)
            .WithMessage("At least one HTML file (preferably index.html) must be included");
    }

    private bool BeValidContractAddress(string address)
    {
        return !string.IsNullOrEmpty(address) && ContractAddressRegex.IsMatch(address);
    }

    private bool BeValidNetwork(string network)
    {
        return !string.IsNullOrEmpty(network) && AllowedNetworks.Contains(network.ToLowerInvariant());
    }

    private bool HaveAtLeastOneFile(IFormFileCollection? files)
    {
        return files != null && files.Count > 0 && files.Any(f => f.Length > 0);
    }

    private bool NotExceedMaxFileCount(IFormFileCollection? files)
    {
        return files == null || files.Count <= MaxTotalFiles;
    }

    private bool HaveValidFileTypes(IFormFileCollection? files)
    {
        if (files == null) return true;

        return files.All(file =>
        {
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && AllowedFileExtensions.Contains(extension);
        });
    }

    private bool NotExceedMaxFileSize(IFormFileCollection? files)
    {
        if (files == null) return true;

        return files.All(file => file.Length <= MaxFileSize);
    }

    private bool HaveIndexHtml(IFormFileCollection? files)
    {
        if (files == null) return false;

        return files.Any(file =>
        {
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return extension == ".html";
        });
    }
}