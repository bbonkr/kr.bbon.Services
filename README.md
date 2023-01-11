# kr.bbon.Services

[![](https://img.shields.io/nuget/v/kr.bbon.Services)](https://www.nuget.org/packages/kr.bbon.Services) [![](https://img.shields.io/nuget/dt/kr.bbon.Services)](https://www.nuget.org/packages/kr.bbon.Services) [![publish to nuget](https://github.com/bbonkr/kr.bbon.Services/actions/workflows/build-tag.yaml/badge.svg)](https://github.com/bbonkr/kr.bbon.Services/actions/workflows/build-tag.yaml)

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
