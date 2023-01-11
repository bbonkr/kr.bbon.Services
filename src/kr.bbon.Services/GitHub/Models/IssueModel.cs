using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class IssueModel
{
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("repository_url")]
    public string RepositoryUrl { get; set; } = string.Empty;
    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; } = string.Empty;
    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; } = string.Empty;
    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; } = string.Empty;
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
    public long Number { get; set; }
    public string State { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public GitHubUserModel? User { get; set; }
    public List<LabelModel> Labels { get; set; } = new();
    public GitHubUserModel? Assignee { get; set; }
    public List<GitHubUserModel> Assignees { get; set; } = new();
    public MilestoneModel? Milestone { get; set; }
    public bool Locked { get; set; }
    [JsonPropertyName("active_lock_reason")]
    public string ActiveLockReason { get; set; } = string.Empty;
    public long Comments { get; set; }
    [JsonPropertyName("pull_request")]
    public PullRequestModel? PullRequest { get; set; }
    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [JsonPropertyName("closed_by")]
    public GitHubUserModel? ClosedBy { get; set; }
    [JsonPropertyName("author_association")]
    public string AuthorAssociation { get; set; } = string.Empty;
    [JsonPropertyName("state_reason")]
    public string StateReason { get; set; } = string.Empty;
}

