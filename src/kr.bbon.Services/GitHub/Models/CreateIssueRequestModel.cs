using System.Text.Json.Serialization;

namespace kr.bbon.Services.GitHub.Models;

public class CreateIssueRequestModel : IOwnerRepoModel
{
    [JsonIgnore]
    public string Owner { get; set; } = string.Empty;
    [JsonIgnore]
    public string Repo { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public IEnumerable<string> Assignees { get; set; } = Enumerable.Empty<string>();

    public string? Milestone { get; set; }

    public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();
}
