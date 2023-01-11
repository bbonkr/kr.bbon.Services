using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class GitHubUserModel
{
    public string Login { get; set; } = string.Empty;
    public long Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;
    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
    [JsonPropertyName("followers_url")]
    public string FollowersUrl { get; set; } = string.Empty;
    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; } = string.Empty;
    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; } = string.Empty;
    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; } = string.Empty;
    [JsonPropertyName("subscriptions_url")]
    public string SubscriptionsUrl { get; set; } = string.Empty;
    [JsonPropertyName("organizations_url")]
    public string OrganizationsUrl { get; set; } = string.Empty;
    [JsonPropertyName("repos_url")]
    public string ReposUrl { get; set; } = string.Empty;
    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; } = string.Empty;
    [JsonPropertyName("received_events_url")]
    public string ReceivedEventsUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }
}

