using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace PokeSharp.Core.Serialization.Json;

public static class JsonSerializerOptionsRegistration
{
    [RegisterServices]
    public static void RegisterJsonSerializerOptions(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            var resolvers = provider.GetServices<IJsonTypeInfoResolver>();
            return new JsonSerializerOptions
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(
                    resolvers.Concat([new DefaultJsonTypeInfoResolver()]).ToArray()
                ),
            };
        });
        services.AddOptions<JsonSerializerOptions>();
    }
}
