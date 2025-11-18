namespace PokeSharp.UI;

public interface IWindow : IDisposable, IAsyncDisposable
{
    void SetSkin(string skin);

    ValueTask SetSkinAsync(string skin, CancellationToken cancellationToken = default);
}
