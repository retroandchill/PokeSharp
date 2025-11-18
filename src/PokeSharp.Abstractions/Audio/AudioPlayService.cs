using PokeSharp.Core;

namespace PokeSharp.Audio;

[AutoServiceShortcut]
public interface IAudioPlayService
{
    void PlayCursorSE();

    void PlayDecisionSE();

    void PlayCancelSE();

    void PlayBuzzerSE();

    void PlayCloseMenuSE();
}
