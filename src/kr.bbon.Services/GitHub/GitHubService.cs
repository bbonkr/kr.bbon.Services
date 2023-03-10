using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using kr.bbon.Core.Exceptions;
using kr.bbon.Services.GitHub.Models;
using Microsoft.Extensions.Options;

namespace kr.bbon.Services.GitHub;

public class GitHubService
{
    public const string MEDIA_TYPE = "application/json";
    public const string BASE_URL = "https://api.github.com";
    public const string GITHUB_API_VERION = "2022-11-28";

    public GitHubService(
        IOptionsMonitor<GitHubOptions> gitHubOptionsAccessor)
    {
        gitHubOptions = gitHubOptionsAccessor.CurrentValue ?? throw new ArgumentException("Please check your application settings about GitHub");
        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public Task<IssueModel?> CreateIssueFromExceptionAsync(
        Exception ex,
        string? endpoint = null,
        IEnumerable<string>? labels = null,
        IEnumerable<string>? assignees = null,
        int? milestone = null,
        CancellationToken cancellationToken = default)
    {
        var title = GenerateIssueTitle(ex, endpoint);

        var body = GenerateIssueBody(ex);

        CreateIssueRequestModel requestModel = new()
        {
            Owner = gitHubOptions.Owner,
            Repo = gitHubOptions.Repo,
            Title = title,
            Labels = labels ?? Enumerable.Empty<string>(),
            Assignees = assignees ?? Enumerable.Empty<string>(),
            Body = body,
            Milestone = milestone?.ToString(),
        };

        return CreateOrReopenIssueAsync(
            requestModel,
            cancellationToken);
    }

    public Task<IssueModel?> CreateIssueFromApiExceptionAsync(
        ApiException ex,
        string? endpoint = null,
        IEnumerable<string>? labels = null,
        IEnumerable<string>? assignees = null,
        int? milestone = null,
        CancellationToken cancellationToken = default)
    {
        var title = GenerateIssueTitle(ex, endpoint);

        var body = GenerateIssueBody(ex);

        CreateIssueRequestModel requestModel = new()
        {
            Owner = gitHubOptions.Owner,
            Repo = gitHubOptions.Repo,
            Title = title,
            Labels = labels ?? Enumerable.Empty<string>(),
            Assignees = assignees ?? Enumerable.Empty<string>(),
            Body = body,
            Milestone = milestone?.ToString(),

        };

        return CreateOrReopenIssueAsync(
            requestModel,
            cancellationToken);
    }

    private async Task<IssueModel?> CreateOrReopenIssueAsync(
        CreateIssueRequestModel model,
        CancellationToken cancellationToken = default)
    {
        SearchIssueRequestModel searchModel = new()
        {
            Owner = model.Owner,
            Repo = model.Repo,
            Limit = 20,
            Page = 1,
            Q = $"{model.Title}",
        };

        IssueModel? issueItem = null;
        try
        {
            if (!gitHubOptions.CreateNewIssueAlways)
            {
                var searchResult = await SearchIssuesAsync(searchModel, cancellationToken);
                if (searchResult?.Items.Any() ?? false)
                {
                    var firstItem = searchResult.Items
                        .Where(x => x.State == IssueStates.Open)
                        .FirstOrDefault();

                    if (firstItem == null && gitHubOptions.ReopenIfClosedOneExists)
                    {
                        firstItem = searchResult.Items
                            .Where(x => x.State == IssueStates.Closed)
                            .FirstOrDefault();
                    }

                    if (firstItem != null)
                    {
                        issueItem = await GetIssueAsync(new GetIssueRequestModel
                        {
                            Owner = model.Owner,
                            Repo = model.Repo,
                            IssueNumber = firstItem.Number,
                        }, cancellationToken);
                    }
                }
            }
        }
        catch
        {
            // Not found
            issueItem = null;
        }

        if (gitHubOptions.ReopenIfClosedOneExists && issueItem != null && issueItem.State.Equals(IssueStates.Closed, StringComparison.OrdinalIgnoreCase))
        {
            // Reopen
            issueItem = await UpdateIssueAsync(new UpdateIssueRequestModel
            {
                Owner = model.Owner,
                Repo = model.Repo,
                IssueNumber = issueItem.Number,
                Title = issueItem.Title,
                Body = issueItem.Body,
                State = IssueStates.Open,
                Labels = issueItem.Labels.Select(label => label.Name),
                Assignees = issueItem.Assignees.Select(assignee => assignee.Login),
                Milestone = issueItem.Milestone == null ? null : issueItem.Milestone?.Number.ToString(),
                StateReason = IssueStateReasons.Reopened,
            }, cancellationToken);
        }

        if (issueItem != null && issueItem.State != IssueStates.Closed)
        {
            // Add comment
            await CreateIssueCommentAsync(new IssueCommentRequestModel
            {
                Body = model.Body,
                IssueNumber = issueItem.Number,
                Owner = model.Owner,
                Repo = model.Repo,
            }, cancellationToken);

            return issueItem;
        }

        // create issue
        return await CreateIssueAsync(model, cancellationToken);
    }

    public async Task<IssueModel?> CreateIssueAsync(CreateIssueRequestModel model, CancellationToken cancellationToken = default)
    {
        GuardOwnerRepo(model);

        var url = $"{BASE_URL}/repos/{model.Owner}/{model.Repo}/issues";

        var request = GetHttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(JsonSerializer.Serialize(model, jsonSerializerOptions), Encoding.UTF8, MEDIA_TYPE);

        var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var issueModel = JsonSerializer.Deserialize<IssueModel>(json ?? string.Empty, jsonSerializerOptions);

            return issueModel;
        }
        else
        {
            var gitHubError = JsonSerializer.Deserialize<GitHubError>(json ?? string.Empty, jsonSerializerOptions);
            if (gitHubError != null)
            {
                throw new GitHubException(response.StatusCode, gitHubError);
            }
            else
            {
                throw new Exception($"GitHub API fault. HTTP{response.StatusCode}:{response.ReasonPhrase}");
            }
        }
    }

    public async Task<IssueModel?> UpdateIssueAsync(UpdateIssueRequestModel model, CancellationToken cancellationToken = default)
    {
        GuardOwnerRepo(model);
        GuardIssueNumber(model);

        var url = $"{BASE_URL}/repos/{model.Owner}/{model.Repo}/issues/{model.IssueNumber}";

        var request = GetHttpRequestMessage(HttpMethod.Patch, url);

        request.Content = new StringContent(JsonSerializer.Serialize(model, jsonSerializerOptions), Encoding.UTF8, MEDIA_TYPE);

        var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var issueModel = JsonSerializer.Deserialize<IssueModel>(json ?? string.Empty, jsonSerializerOptions);

            return issueModel;
        }
        else
        {
            var gitHubError = JsonSerializer.Deserialize<GitHubError>(json ?? string.Empty, jsonSerializerOptions);
            if (gitHubError != null)
            {
                throw new GitHubException(response.StatusCode, gitHubError);
            }
            else
            {
                throw new Exception($"GitHub API fault. HTTP{response.StatusCode}:{response.ReasonPhrase}");
            }
        }
    }

    public async Task<CommentModel?> CreateIssueCommentAsync(IssueCommentRequestModel model, CancellationToken cancellationToken = default)
    {
        GuardOwnerRepo(model);

        var url = $"{BASE_URL}/repos/{model.Owner}/{model.Repo}/issues/{model.IssueNumber}/comments";

        var request = GetHttpRequestMessage(HttpMethod.Post, url);

        request.Content = new StringContent(JsonSerializer.Serialize(new { model.Body }, jsonSerializerOptions), Encoding.UTF8, MEDIA_TYPE);

        var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var issueModel = JsonSerializer.Deserialize<CommentModel>(json ?? string.Empty, jsonSerializerOptions);

            return issueModel;
        }
        else
        {
            var gitHubError = JsonSerializer.Deserialize<GitHubError>(json ?? string.Empty, jsonSerializerOptions);
            if (gitHubError != null)
            {
                throw new GitHubException(response.StatusCode, gitHubError);
            }
            else
            {
                throw new Exception($"GitHub API fault. HTTP{response.StatusCode}:{response.ReasonPhrase}");
            }
        }
    }

    public async Task<IEnumerable<IssueModel>?> GetIssuesAsync(GetIssuesRequestModel model, CancellationToken cancellationToken = default)
    {
        GuardOwnerRepo(model);

        var url = $"{BASE_URL}/repos/{model.Owner}/{model.Repo}/issues";
        var querystring = GenerateGetIssuesQuerystring(model);

        if (!string.IsNullOrWhiteSpace(querystring))
        {
            url = $"{url}?{querystring}";
        }

        var request = GetHttpRequestMessage(HttpMethod.Get, url);

        var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<IEnumerable<IssueModel>>(json ?? string.Empty, jsonSerializerOptions);

            return result;
        }
        else
        {
            var gitHubError = JsonSerializer.Deserialize<GitHubError>(json ?? string.Empty, jsonSerializerOptions);
            if (gitHubError != null)
            {
                throw new GitHubException(response.StatusCode, gitHubError);
            }
            else
            {
                throw new Exception($"GitHub API fault. HTTP{response.StatusCode}:{response.ReasonPhrase}");
            }
        }
    }

    public async Task<IssueModel?> GetIssueAsync(GetIssueRequestModel model, CancellationToken cancellationToken = default)
    {
        GuardOwnerRepo(model);
        GuardIssueNumber(model);

        var url = $"{BASE_URL}/repos/{model.Owner}/{model.Repo}/issues/{model.IssueNumber}";

        var request = GetHttpRequestMessage(HttpMethod.Get, url);
        var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<IssueModel>(json ?? string.Empty, jsonSerializerOptions);

            return result;
        }
        else
        {
            var gitHubError = JsonSerializer.Deserialize<GitHubError>(json ?? string.Empty, jsonSerializerOptions);
            if (gitHubError != null)
            {
                throw new GitHubException(response.StatusCode, gitHubError);
            }
            else
            {
                throw new Exception($"GitHub API fault. HTTP{response.StatusCode}:{response.ReasonPhrase}");
            }
        }
    }

    public async Task<SearchResultModel?> SearchIssuesAsync(SearchIssueRequestModel model, CancellationToken cancellationToken = default)
    {
        var url = $"{BASE_URL}/search/issues";
        var querystring = GenerateSearchIssuesQeurystring(model);

        if (!string.IsNullOrWhiteSpace(querystring))
        {
            url = $"{url}?{querystring}";
        }

        var request = GetHttpRequestMessage(HttpMethod.Get, url);

        var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<SearchResultModel>(json ?? string.Empty, jsonSerializerOptions);

            return result;
        }
        else
        {
            var gitHubError = JsonSerializer.Deserialize<GitHubError>(json ?? string.Empty, jsonSerializerOptions);
            if (gitHubError != null)
            {
                throw new GitHubException(response.StatusCode, gitHubError);
            }
            else
            {
                throw new Exception($"GitHub API fault. HTTP{response.StatusCode}:{response.ReasonPhrase}");
            }
        }
    }

    private string GenerateGetIssuesQuerystring(GetIssuesRequestModel model)
    {
        Dictionary<string, string> querystrings = new();

        if (!string.IsNullOrWhiteSpace(model.Milestone))
        {
            querystrings.Add("milestone", Uri.EscapeDataString(model.Milestone));
        }

        if (!string.IsNullOrWhiteSpace(model.State))
        {
            querystrings.Add("state", Uri.EscapeDataString(model.State));
        }

        if (!string.IsNullOrWhiteSpace(model.Assignee))
        {
            querystrings.Add("assignee", Uri.EscapeDataString(model.Assignee));
        }

        if (!string.IsNullOrWhiteSpace(model.Creator))
        {
            querystrings.Add("creator", Uri.EscapeDataString(model.Creator));
        }

        if (!string.IsNullOrWhiteSpace(model.Mentioned))
        {
            querystrings.Add("mentioned", Uri.EscapeDataString(model.Mentioned));
        }
        if (model.Labels.Any())
        {
            querystrings.Add("labels", string.Join(",", model.Labels.Select(label => Uri.EscapeDataString(label))));
        }
        if (!string.IsNullOrWhiteSpace(model.Sort))
        {
            querystrings.Add("sort", Uri.EscapeDataString(model.Sort));
        }
        if (!string.IsNullOrWhiteSpace(model.Direction))
        {
            querystrings.Add("direction", Uri.EscapeDataString(model.Direction));
        }
        if (model.Since.HasValue)
        {
            querystrings.Add("since", $"{model.Since.Value:yyyy-MM-ddTHH:mm:ssZ}");
        }

        if (model.Limit > 100)
        {
            model.Limit = 100;
        }

        querystrings.Add("per_page", model.Limit.ToString());
        querystrings.Add("page", model.Page.ToString());

        return string.Join("&", querystrings.Select(x => $"{x.Key}={x.Value}"));
    }

    private string GenerateSearchIssuesQeurystring(SearchIssueRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Q))
        {
            throw new ArgumentException("Q is required", nameof(model));
        }
        Dictionary<string, string> querystrings = new();

        List<string> qBuilder = new();
        Regex replaceRegEx = new Regex(@"\W");

        qBuilder.Add(string.Join("+", replaceRegEx.Replace(model.Q, " ").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => Uri.EscapeDataString(x))));
        qBuilder.Add(Uri.EscapeDataString("is:issue"));

        List<string> searchIn = new() { "title" };
        if (model.SearchInBody)
        {
            searchIn.Add("body");
        }
        if (model.SearchInComments)
        {
            searchIn.Add("comments");
        }

        qBuilder.Add(Uri.EscapeDataString($"in:{string.Join(",", searchIn)}"));

        if (!string.IsNullOrWhiteSpace(model.Owner) && !string.IsNullOrWhiteSpace(model.Repo))
        {
            qBuilder.Add(Uri.EscapeDataString($"repo:{model.Owner}/{model.Repo}"));
        }
        else if (!string.IsNullOrWhiteSpace(model.Owner))
        {
            qBuilder.Add(Uri.EscapeDataString($"user:{model.Owner}"));
        }

        if (!string.IsNullOrWhiteSpace(model.State))
        {
            qBuilder.Add(Uri.EscapeDataString($"is:{model.State}"));
        }

        querystrings.Add("q", string.Join("+", qBuilder));

        if (!string.IsNullOrWhiteSpace(model.Sort))
        {
            querystrings.Add("sort", Uri.EscapeDataString(model.Sort));
        }

        if (!string.IsNullOrWhiteSpace(model.Order))
        {
            querystrings.Add("order", Uri.EscapeDataString(model.Order));
        }

        if (model.Limit > 100)
        {
            model.Limit = 100;
        }

        querystrings.Add("per_page", model.Limit.ToString());
        querystrings.Add("page", model.Page.ToString());

        return string.Join("&", querystrings.Select(x => $"{x.Key}={x.Value}"));
    }

    protected virtual string GenerateIssueTitle(Exception ex, string? endpoint)
        => $"[Bug]: {endpoint ?? ex.Message}";

    protected virtual string GenerateIssueBody(Exception ex)
    => @$"## Description

### Exception

Message: 
`{ex.Message}`

Stack trace: 
```plaintext
{ex.StackTrace}
```
";


    protected virtual string GenerateIssueBody(ApiException ex)
        => @$"## Description

### Exception

Exception Message: 
`{ex.Message}`

Stack trace: 
```plaintext
{ex.StackTrace}

Errors:
Code: {ex.Error.Code}
Message: {ex.Error.Message}
References: {ex.Error.Reference}
{((ex.Error.InnerErrors?.Any() ?? false) ? "More information: " : "")}
{ex.Error.InnerErrors?.Select(e => $@"
> Code: {e.Code}
> Message: {e.Message}
> References: {e.Reference}
")}
```
";

    private void GuardOwnerRepo(IOwnerRepoModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Owner))
        {
            throw new ArgumentException("Owner is required", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(model.Repo))
        {
            throw new ArgumentException("Repo is required", nameof(model));
        }
    }

    private void GuardIssueNumber(IIssueNumberModel model)
    {
        if (model.IssueNumber < 1)
        {
            throw new ArgumentException("Issue number is invalid", nameof(model));
        }
    }

    private HttpRequestMessage GetHttpRequestMessage(HttpMethod httpMethod, string url)
    {
        HttpRequestMessage request = new(httpMethod, url);
        if (request.Headers.Contains("Accept"))
        {
            request.Headers.Remove("Accept");
        }
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("X-GitHub-Api-Version", GetGitHubApiVersion());
        request.Headers.Add("User-Agent", GetUserAgent());
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", gitHubOptions.AccessToken);

        return request;
    }

    protected virtual string GetUserAgent() => "kr.bbon.GitHubService";

    protected virtual string GetGitHubApiVersion() => GITHUB_API_VERION;

    private readonly GitHubOptions gitHubOptions;
    private readonly JsonSerializerOptions jsonSerializerOptions;
}
