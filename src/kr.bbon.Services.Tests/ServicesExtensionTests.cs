

using kr.bbon.Services.Extensions.DependencyInjection;
using kr.bbon.Services.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kr.bbon.Services.Tests;

public class ServicesExtensionTests
{
    [Fact]
    public void SholdBeResolvedAsTransient()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(_ => configuration);
        services.AddGitHubService(ServiceLifetime.Transient);

        var provider = services.BuildServiceProvider();

        // Act
        var githubService = provider.GetService<GitHubService>();

        // Assert
        Assert.NotNull(githubService);
        Assert.IsType<GitHubService>(githubService);
    }

    [Fact]
    public void SholdBeResolvedAsScoped()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(_ => configuration);
        services.AddGitHubService(ServiceLifetime.Scoped);

        var provider = services.BuildServiceProvider();

        // Act
        var githubService = provider.GetService<GitHubService>();

        // Assert
        Assert.NotNull(githubService);
        Assert.IsType<GitHubService>(githubService);
    }

    [Fact]
    public void SholdBeResolvedAsSingleton()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(_ => configuration);
        services.AddGitHubService(ServiceLifetime.Singleton);

        var provider = services.BuildServiceProvider();

        // Act
        var githubService = provider.GetService<GitHubService>();

        // Assert
        Assert.NotNull(githubService);
        Assert.IsType<GitHubService>(githubService);
    }
}