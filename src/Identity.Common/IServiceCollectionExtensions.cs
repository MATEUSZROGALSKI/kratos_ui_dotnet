using Identity.Common.Authentication;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Ory.Kratos.Client.Api;
using Ory.Kratos.Client.Client;

namespace Identity.Common;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddKratosIntegration(this IServiceCollection @this)
    {
        var configuration = new Configuration { BasePath = EnvironmentVariables.InternalUrl };
        @this.TryAddSingleton<ICourierApi>(provider => new CourierApi(configuration));
        @this.TryAddSingleton<IFrontendApi>(provider => new FrontendApi(configuration));
        @this.TryAddSingleton<IMetadataApi>(provider => new MetadataApi(configuration));
        @this.AddAuthentication(EnvironmentVariables.SchemaName)
            .AddScheme<KratosAuthenticationOptions, KratosAuthenticationHandler>(EnvironmentVariables.SchemaName, null);
        return @this;
    }
}