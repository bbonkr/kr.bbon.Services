using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class MilestoneModel
{
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; } = string.Empty;
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    public long Number { get; set; }
    public string State { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GitHubUserModel? Creator { get; set; }

    [JsonPropertyName("open_issues")]
    public long OpenIssues { get; set; }
    [JsonPropertyName("closed_issues")]
    public long ClosedIssues { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }
    [JsonPropertyName("due_on")]
    public DateTime? DueOn { get; set; }
}

