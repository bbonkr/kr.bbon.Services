namespace kr.bbon.Services.GitHub.Models;

/// <summary>
/// https://docs.github.com/en/rest/search?apiVersion=2022-11-28#search-issues-and-pull-requests
/// </summary>
public class SearchSorts : GetIssueSorts
{
    public const string Reactions = "reactions";
}

