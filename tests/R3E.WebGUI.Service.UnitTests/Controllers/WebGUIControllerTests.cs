using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using R3E.WebGUI.Service.API.Controllers;
using R3E.WebGUI.Service.Core.Services;
using R3E.WebGUI.Service.Domain.Models;
using System.Text;
using Xunit;

namespace R3E.WebGUI.Service.UnitTests.Controllers;

public class WebGUIControllerTests
{
    private readonly Mock<IWebGUIService> _mockService;
    private readonly Mock<ILogger<WebGUIController>> _mockLogger;
    private readonly WebGUIController _controller;

    public WebGUIControllerTests()
    {
        _mockService = new Mock<IWebGUIService>();
        _mockLogger = new Mock<ILogger<WebGUIController>>();
        _controller = new WebGUIController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task DeployWebGUI_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new DeployWebGUIRequest
        {
            ContractAddress = "0x1234567890abcdef1234567890abcdef12345678",
            ContractName = "TestContract",
            Network = "testnet",
            DeployerAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
            Description = "Test WebGUI",
            WebGUIFiles = CreateMockFiles()
        };

        var expectedResult = new ContractWebGUI
        {
            Subdomain = "testcontract",
            ContractAddress = request.ContractAddress
        };

        _mockService.Setup(s => s.DeployWebGUIAsync(
            request.ContractAddress,
            request.ContractName,
            request.Network,
            request.WebGUIFiles,
            request.DeployerAddress,
            request.Description))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.DeployWebGUI(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value;
        
        response.Should().NotBeNull();
        var responseObj = response!.GetType();
        responseObj.GetProperty("success")!.GetValue(response).Should().Be(true);
        responseObj.GetProperty("subdomain")!.GetValue(response).Should().Be("testcontract");
        responseObj.GetProperty("url")!.GetValue(response).Should().Be("https://testcontract.r3e-gui.com");
    }

    [Fact]
    public async Task DeployWebGUI_NoFiles_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeployWebGUIRequest
        {
            ContractAddress = "0x1234567890abcdef1234567890abcdef12345678",
            ContractName = "TestContract",
            WebGUIFiles = new FormFileCollection() // Empty collection
        };

        // Act
        var result = await _controller.DeployWebGUI(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("No WebGUI files provided");
    }

    [Fact]
    public async Task DeployWebGUI_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new DeployWebGUIRequest
        {
            ContractAddress = "0x1234567890abcdef1234567890abcdef12345678",
            ContractName = "TestContract",
            WebGUIFiles = CreateMockFiles()
        };

        _mockService.Setup(s => s.DeployWebGUIAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<IFormFileCollection>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act
        var result = await _controller.DeployWebGUI(request);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task SearchByContract_ValidAddress_ReturnsResults()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var network = "testnet";
        var expectedResults = new List<ContractWebGUI>
        {
            new() { ContractAddress = contractAddress, Network = network }
        };

        _mockService.Setup(s => s.SearchByContractAddressAsync(contractAddress, network))
            .ReturnsAsync(expectedResults);

        // Act
        var result = await _controller.SearchByContract(contractAddress, network);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var results = okResult!.Value as IEnumerable<ContractWebGUI>;
        results.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchByContract_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        
        _mockService.Setup(s => s.SearchByContractAddressAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Search error"));

        // Act
        var result = await _controller.SearchByContract(contractAddress);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetWebGUIInfo_ExistingSubdomain_ReturnsOkResult()
    {
        // Arrange
        var subdomain = "testcontract";
        var webGUI = new ContractWebGUI
        {
            Subdomain = subdomain,
            ContractName = "TestContract"
        };

        _mockService.Setup(s => s.GetBySubdomainAsync(subdomain))
            .ReturnsAsync(webGUI);

        // Act
        var result = await _controller.GetWebGUIInfo(subdomain);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var returnedWebGUI = okResult!.Value as ContractWebGUI;
        returnedWebGUI!.Subdomain.Should().Be(subdomain);
    }

    [Fact]
    public async Task GetWebGUIInfo_NonExistentSubdomain_ReturnsNotFound()
    {
        // Arrange
        var subdomain = "nonexistent";
        _mockService.Setup(s => s.GetBySubdomainAsync(subdomain))
            .ReturnsAsync((ContractWebGUI?)null);

        // Act
        var result = await _controller.GetWebGUIInfo(subdomain);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ListWebGUIs_ValidParameters_ReturnsPagedResults()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var network = "testnet";
        var pagedResult = new PagedResult<ContractWebGUI>
        {
            Items = new List<ContractWebGUI> { new() { ContractName = "Test" } },
            TotalCount = 1,
            Page = page,
            PageSize = pageSize
        };

        _mockService.Setup(s => s.ListWebGUIsAsync(page, pageSize, network))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.ListWebGUIs(page, pageSize, network);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var returnedResult = okResult!.Value as PagedResult<ContractWebGUI>;
        returnedResult!.Items.Should().HaveCount(1);
        returnedResult.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task UpdateWebGUI_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var subdomain = "testcontract";
        var request = new UpdateWebGUIRequest
        {
            Description = "Updated description",
            WebGUIFiles = CreateMockFiles()
        };

        var updatedWebGUI = new ContractWebGUI
        {
            Subdomain = subdomain,
            Description = request.Description
        };

        _mockService.Setup(s => s.UpdateWebGUIAsync(subdomain, request.WebGUIFiles, request.Description))
            .ReturnsAsync(updatedWebGUI);

        // Act
        var result = await _controller.UpdateWebGUI(subdomain, request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value;
        
        response.Should().NotBeNull();
        var responseObj = response!.GetType();
        responseObj.GetProperty("success")!.GetValue(response).Should().Be(true);
    }

    private IFormFileCollection CreateMockFiles()
    {
        var files = new FormFileCollection();
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("index.html");
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("<html></html>")));
        
        files.Add(mockFile.Object);
        return files;
    }
}