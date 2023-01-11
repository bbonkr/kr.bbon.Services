namespace kr.bbon.Services.GitHub.Models;

public class IssueCommentRequestModel : IOwnerRepoModel, IIssueNumberModel
{
    public string Owner { get; set; } = string.Empty;

    public string Repo { get; set; } = string.Empty;

    public long IssueNumber { get; set; }

    public string Body { get; set; } = string.Empty;
}