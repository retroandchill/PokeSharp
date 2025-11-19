using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;

namespace PokeSharp.Core.Serialization.MessagePack;

public static class MessagePackFormatterResolverRegistration
{
    [RegisterServices]
    public static void RegisterMessagePackFormatter(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(services => MessagePackSerializerOptions
            .Standard.WithResolver(CompositeResolver.Create(services
                .GetRequiredService<IEnumerable<IFormatterResolver>>()
                .Concat([TypelessContractlessStandardResolver.Instance])
                .ToArray())));
    }
}