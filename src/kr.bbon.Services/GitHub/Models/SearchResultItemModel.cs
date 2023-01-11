using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);


public class SearchResultItemModel
{
    [JsonPropertyName("url")]
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

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public long Number { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public GitHubUserModel? User { get; set; }

    [JsonPropertyName("labels")]
    public List<LabelModel> Labels { get; set; } = new List<LabelModel>();

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("assignee")]
    public GitHubUserModel? Assignee { get; set; }

    [JsonPropertyName("milestone")]
    public MilestoneModel? Milestone { get; set; }

    [JsonPropertyName("comments")]
    public long Comments { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }

    [JsonPropertyName("pull_request")]
    public PullRequestModel? PullRequest { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("author_association")]
    public string AuthorAssociation { get; set; } = string.Empty;

    [JsonPropertyName("state_reason")]
    public string StateReason { get; set; } = string.Empty;
}

