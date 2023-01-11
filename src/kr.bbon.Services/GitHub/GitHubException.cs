using System.Net;
using kr.bbon.Services.GitHub.Models;

namespace kr.bbon.Services.GitHub;

public class GitHubException : Exception
{
    public GitHubException(HttpStatusCode statusCode, GitHubError error) : base(error.Message)
    {
        StatusCode = statusCode;
        Error = error;
    }

    public HttpStatusCode StatusCode { get; private set; }

    public GitHubError Error { get; private set; }
}