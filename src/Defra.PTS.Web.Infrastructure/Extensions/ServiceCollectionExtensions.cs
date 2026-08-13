using Defra.PTS.Web.Infrastructure.Models;
using Defra.PTS.Web.Infrastructure.Services;
using Defra.PTS.Web.Infrastructure.Services.Interfaces;
using Defra.Trade.Address.V1.ApiClient.Api;
using Defra.Trade.Address.V1.ApiClient.Client;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Security.Authentication.Infrastructure;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Web.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAddressLookupService, AddressLookupService>();

        return services;
    }

    public static IServiceCollection AddApplicationApis(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AddressApiConfig>(configuration.GetSection("AddressApi"))
            .Configure<ApimInternalSettings>(configuration.GetSection(ApimInternalSettings.OptionsName));
        services
            .AddApimAuthentication(configuration.GetSection(ApimSettings.InternalApim));
        services.AddTransient<IPlacesApi>((provider) =>
            new PlacesApi(CreateApiClientConfigurationSettings(provider, configuration)));
        return services;
    }

    private static Configuration CreateApiClientConfigurationSettings(IServiceProvider provider, IConfiguration configuration)
    {
        var authService = provider.GetService<IAuthenticationService>();
        var apimInternalApisSettings = provider.GetRequiredService<IOptionsSnapshot<ApimInternalSettings>>().Value;
        var authToken = authService.GetAuthenticationHeaderAsync().Result.ToString();
        var config = new Configuration
        {
            BasePath = configuration.GetValue<string>("AddressApi:BaseUrl"),
            DefaultHeaders = new Dictionary<string, string>
            {
                { apimInternalApisSettings.AuthorizationHeaderName, authToken },
                { apimInternalApisSettings.SubscriptionKeyHeaderName, apimInternalApisSettings.SubscriptionKey}
            }
        };
        return config;
    }
}
