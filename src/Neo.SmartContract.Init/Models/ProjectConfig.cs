using System.Collections.Generic;

namespace Neo.SmartContract.Init.Models;

public class ProjectConfig
{
    public string Name { get; set; } = string.Empty;
    public string Template { get; set; } = "basic";
    public string OutputPath { get; set; } = ".";
    public string Author { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}

public class ProjectTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public Dictionary<string, string> Files { get; set; } = new();
}