using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.State;

[RegisterSingleton]
[AutoServiceShortcut]
public class GameTemp
{
    #region Flags requesting something to happen
    public bool MenuCalling { get; set; }

    public bool ReadMenuCalling { get; set; }

    public bool DebugCalling { get; set; }

    public bool InteractCalling { get; set; }

    public bool BattleAbort { get; set; }

    public bool TitleScreenCalling { get; set; }

    public int CommonEventId { get; set; }
    #endregion

    #region Flags indicating something is happening
    public bool InMenu { get; set; }

    public bool InStorage { get; set; }

    public bool InBattle { get; set; }

    public bool MessageWindowShowing { get; set; }

    public bool EndingSurf { get; set; }

    public bool InMiniUpdate { get; set; }

    #endregion

    #region Battle

    public string BattlebackName { get; set; } = "";

    public bool ForceSingleBattle { get; set; }

    #endregion

    #region Player transfers

    public bool PlayerTransferring { get; set; }

    public int PlayerNewMapId { get; set; }

    public int PlayerNewX { get; set; }

    public int PlayerNewY { get; set; }

    public int PlayerNewDirection { get; set; }

    public FlyDestination? FlyDestination { get; set; }

    #endregion

    #region Transitions

    public bool TransitionProcessing { get; set; }

    public string TransitionName { get; set; } = "";

    public bool FadeState { get; set; }

    #endregion

    #region Other

    public bool BegunNewGame { get; set; }

    public bool MenuBeep { get; set; }

    public string? MemorizedBgm { get; set; }

    public int MemorizedBgmPosition { get; set; }

    public int MenuLastChoice { get; set; }

    public Dictionary<Name, (int? BuyPrice, int? SellPrice)> MartPrices { get; set; } = new();

    public void ClearMartPrices() => MartPrices.Clear();

    #endregion

    #region Overworld Battle Starting

    public List<int> PartyCriticalHitsDealt { get; set; } = [];

    public List<int> PartyDirectDamageTaken { get; set; } = [];

    #endregion
}
