using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class GitHubError
{
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("documentation_url")]
    public string DocumentationUrl { get; set; } = string.Empty;
}