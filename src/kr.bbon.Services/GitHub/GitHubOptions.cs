namespace kr.bbon.Services.GitHub;

public class GitHubOptions
{
    public const string Name = "GitHub";

    public string Owner { get; set; } = "";

    public string Repo { get; set; } = "";

    public string AccessToken { get; set; } = "";

    public bool CreateNewIssueAlways { get; set; } = false;

    public bool ReopenIfClosedOneExists { get; set; } = false;
}


