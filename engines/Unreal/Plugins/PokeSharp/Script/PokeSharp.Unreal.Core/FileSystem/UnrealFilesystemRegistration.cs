using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace PokeSharp.Unreal.Core.FileSystem;

public static class UnrealFilesystemRegistration
{
    [RegisterServices]
    public static void RegisterFilesystem(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, System.IO.Abstractions.FileSystem>();
    }
}
