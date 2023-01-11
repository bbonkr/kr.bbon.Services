using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class PullRequestModel
{
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
    [JsonPropertyName("diff_url")]
    public string? DiffUrl { get; set; }
    [JsonPropertyName("patch_url")]
    public string? PatchUrl { get; set; }
}

