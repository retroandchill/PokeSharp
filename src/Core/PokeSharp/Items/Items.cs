using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Items;

public static class Items
{
    public static bool CanRegisterItem(Name item)
    {
        return GameGlobal.ItemHandlers.HasUseInFieldHandler(item);
    }

    public static bool CanUseOnPokemon(Name item)
    {
        return GameGlobal.ItemHandlers.HasUseOnPokemon(item) || Item.Get(item).IsMachine;
    }
}
