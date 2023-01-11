# kr.bbon.Services

## Overview

Implements frequently used service layers.

## Features

### GitHubService

Support to create GitHub issue.

settings.json

```json
{
  "GitHub": {
    "Owner": "bbonkr",
    "Repo": "kr.bbon.Services",
    "AccessToken": "<GitHub accesstoken ISSUE READ, WRITE>",
    "CreateNewIssueAlways": false,
    "ReopenIfClosedOneExists": false
  }
}
```

| Name                    | Required | Default | Description                                   |
| :---------------------- | :------: | :-----: | :-------------------------------------------- |
| Owner                   |    ✅    |   N/A   | Repository owner                              |
| Repo                    |    ✅    |   N/A   | Repository name                               |
| AccessToken             |    ✅    |   N/A   | Repository name                               |
| CreateNewIssueAlways    |          |  false  | Create new GitHub Issue always Whether or not |
| ReopenIfClosedOneExists |          |  false  | Reopen if closed issue exists Whether or not  |

You can change title, body content generation

```csharp
public class MyGitHubService : GitHubService
{
    public MyGitHubService(...) : base(...) { }

    protected override string GenerateIssueTitle(Exception ex, string? endpoint) => $"{ex.Message}";

    protected override string GenerateIssueBody(ApiException ex) => $"{ex.StackTrace}";
}
```
