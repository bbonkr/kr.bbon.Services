namespace kr.bbon.Services.GitHub.Models;

public interface IOwnerRepoModel
{
    string Owner { get; }

    string Repo { get; }
}