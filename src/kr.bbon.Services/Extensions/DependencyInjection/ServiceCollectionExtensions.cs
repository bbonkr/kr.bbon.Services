using kr.bbon.Services.GitHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kr.bbon.Services.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register <see cref="GitHubService" /> to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceLifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddGitHubService(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        services.AddOptions<GitHubOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(GitHubOptions.Name).Bind(options);
            });


        services.Add(new ServiceDescriptor(typeof(GitHubService), typeof(GitHubService), serviceLifetime));

        return services;
    }
}
