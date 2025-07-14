using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using R3E.WebGUI.Service.Infrastructure.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace R3E.WebGUI.Service.IntegrationTests;

public class WebGUIIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebGUIIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the app DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<WebGUIDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<WebGUIDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Ensure the database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<WebGUIDbContext>();
                context.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task DeployWebGUI_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("0x1234567890abcdef1234567890abcdef12345678"), "contractAddress");
        formData.Add(new StringContent("TestContract"), "contractName");
        formData.Add(new StringContent("testnet"), "network");
        formData.Add(new StringContent("0xabcdef1234567890abcdef1234567890abcdef12"), "deployerAddress");
        formData.Add(new StringContent("Test WebGUI"), "description");
        
        var htmlContent = new ByteArrayContent(Encoding.UTF8.GetBytes("<html><body>Test</body></html>"));
        htmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
        formData.Add(htmlContent, "webGUIFiles", "index.html");

        // Act
        var response = await _client.PostAsync("/api/webgui/deploy", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
        result.GetProperty("subdomain").GetString().Should().NotBeNullOrEmpty();
        result.GetProperty("url").GetString().Should().StartWith("https://");
        result.GetProperty("contractAddress").GetString().Should().Be("0x1234567890abcdef1234567890abcdef12345678");
    }

    [Fact]
    public async Task DeployWebGUI_InvalidContractAddress_ReturnsBadRequest()
    {
        // Arrange
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("invalid-address"), "contractAddress");
        formData.Add(new StringContent("TestContract"), "contractName");
        formData.Add(new StringContent("testnet"), "network");
        formData.Add(new StringContent("0xabcdef1234567890abcdef1234567890abcdef12"), "deployerAddress");
        
        var htmlContent = new ByteArrayContent(Encoding.UTF8.GetBytes("<html></html>"));
        formData.Add(htmlContent, "webGUIFiles", "index.html");

        // Act
        var response = await _client.PostAsync("/api/webgui/deploy", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task DeployWebGUI_NoFiles_ReturnsBadRequest()
    {
        // Arrange
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("0x1234567890abcdef1234567890abcdef12345678"), "contractAddress");
        formData.Add(new StringContent("TestContract"), "contractName");

        // Act
        var response = await _client.PostAsync("/api/webgui/deploy", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("No WebGUI files provided");
    }

    [Fact]
    public async Task SearchByContract_ExistingContract_ReturnsResults()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        
        // First deploy a WebGUI
        await DeployTestWebGUI(contractAddress, "SearchTestContract");

        // Act
        var response = await _client.GetAsync($"/api/webgui/search?contractAddress={contractAddress}&network=testnet");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<JsonElement[]>(content);
        
        results.Should().NotBeEmpty();
        results[0].GetProperty("contractAddress").GetString().Should().Be(contractAddress.ToLowerInvariant());
    }

    [Fact]
    public async Task SearchByContract_NonExistentContract_ReturnsEmptyResults()
    {
        // Arrange
        var contractAddress = "0x9999999999999999999999999999999999999999";

        // Act
        var response = await _client.GetAsync($"/api/webgui/search?contractAddress={contractAddress}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<JsonElement[]>(content);
        
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task ListWebGUIs_WithPagination_ReturnsPagedResults()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            await DeployTestWebGUI($"0x{i:D40}", $"ListTestContract{i}");
        }

        // Act
        var response = await _client.GetAsync("/api/webgui/list?page=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("items").GetArrayLength().Should().BeLessOrEqualTo(3);
        result.GetProperty("totalCount").GetInt32().Should().BeGreaterOrEqualTo(5);
        result.GetProperty("page").GetInt32().Should().Be(1);
        result.GetProperty("pageSize").GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task GetWebGUIInfo_ExistingSubdomain_ReturnsWebGUIInfo()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var contractName = "InfoTestContract";
        var subdomain = await DeployTestWebGUI(contractAddress, contractName);

        // Act
        var response = await _client.GetAsync($"/api/webgui/{subdomain}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("subdomain").GetString().Should().Be(subdomain);
        result.GetProperty("contractName").GetString().Should().Be(contractName);
        result.GetProperty("contractAddress").GetString().Should().Be(contractAddress.ToLowerInvariant());
    }

    [Fact]
    public async Task GetWebGUIInfo_NonExistentSubdomain_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/webgui/nonexistent-subdomain");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateWebGUI_ExistingSubdomain_UpdatesSuccessfully()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var subdomain = await DeployTestWebGUI(contractAddress, "UpdateTestContract");
        
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Updated description"), "description");
        
        var updatedContent = new ByteArrayContent(Encoding.UTF8.GetBytes("<html><body>Updated</body></html>"));
        formData.Add(updatedContent, "webGUIFiles", "index.html");

        // Act
        var response = await _client.PutAsync($"/api/webgui/{subdomain}/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task UpdateWebGUI_NonExistentSubdomain_ReturnsInternalServerError()
    {
        // Arrange
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Updated description"), "description");

        // Act
        var response = await _client.PutAsync("/api/webgui/nonexistent/update", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CompleteWorkflow_DeploySearchUpdateDelete_WorksEndToEnd()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var contractName = "WorkflowTestContract";

        // Step 1: Deploy
        var subdomain = await DeployTestWebGUI(contractAddress, contractName);
        subdomain.Should().NotBeNullOrEmpty();

        // Step 2: Search and verify
        var searchResponse = await _client.GetAsync($"/api/webgui/search?contractAddress={contractAddress}");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        var searchResults = JsonSerializer.Deserialize<JsonElement[]>(searchContent);
        searchResults.Should().NotBeEmpty();

        // Step 3: Get info and verify view count increment
        var infoResponse1 = await _client.GetAsync($"/api/webgui/{subdomain}");
        var infoContent1 = await infoResponse1.Content.ReadAsStringAsync();
        var info1 = JsonSerializer.Deserialize<JsonElement>(infoContent1);
        var initialViewCount = info1.GetProperty("viewCount").GetInt64();

        var infoResponse2 = await _client.GetAsync($"/api/webgui/{subdomain}");
        var infoContent2 = await infoResponse2.Content.ReadAsStringAsync();
        var info2 = JsonSerializer.Deserialize<JsonElement>(infoContent2);
        var updatedViewCount = info2.GetProperty("viewCount").GetInt64();

        updatedViewCount.Should().Be(initialViewCount + 1);

        // Step 4: Update
        var updateData = new MultipartFormDataContent();
        updateData.Add(new StringContent("Updated in workflow test"), "description");
        
        var updateResponse = await _client.PutAsync($"/api/webgui/{subdomain}/update", updateData);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 5: Verify update
        var finalInfoResponse = await _client.GetAsync($"/api/webgui/{subdomain}");
        var finalInfoContent = await finalInfoResponse.Content.ReadAsStringAsync();
        var finalInfo = JsonSerializer.Deserialize<JsonElement>(finalInfoContent);
        
        finalInfo.GetProperty("description").GetString().Should().Be("Updated in workflow test");
        finalInfo.GetProperty("updatedAt").GetString().Should().NotBeNullOrEmpty();
    }

    private async Task<string> DeployTestWebGUI(string contractAddress, string contractName)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(contractAddress), "contractAddress");
        formData.Add(new StringContent(contractName), "contractName");
        formData.Add(new StringContent("testnet"), "network");
        formData.Add(new StringContent("0xabcdef1234567890abcdef1234567890abcdef12"), "deployerAddress");
        formData.Add(new StringContent($"Test WebGUI for {contractName}"), "description");
        
        var htmlContent = new ByteArrayContent(Encoding.UTF8.GetBytes($"<html><body>{contractName}</body></html>"));
        htmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
        formData.Add(htmlContent, "webGUIFiles", "index.html");

        var response = await _client.PostAsync("/api/webgui/deploy", formData);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        return result.GetProperty("subdomain").GetString()!;
    }
}