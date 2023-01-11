namespace kr.bbon.Services.GitHub.Models;

public class GetIssueRequestModel : IOwnerRepoModel, IIssueNumberModel
{
    public string Owner { get; set; } = string.Empty;
    public string Repo { get; set; } = string.Empty;
    public long IssueNumber { get; set; }
}
