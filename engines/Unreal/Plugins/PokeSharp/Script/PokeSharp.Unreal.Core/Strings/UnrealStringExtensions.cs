using System.Runtime.CompilerServices;
using PokeSharp.Core.Strings;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.Strings;

public static class UnrealStringExtensions
{
    public static Name ToPokeSharpName(this FName name)
    {
        // Name was designed to have an identical layout to FName, allowing us to BitCast between the two types.
        // However, that layout differs between editor and game builds, so for packaged builds when we have no
        // display index, we can simply BitCast to CaselessName and let the implicit conversion to Name handle
        // the rest.
#if WITH_EDITOR
        return Unsafe.BitCast<FName, Name>(name);
#else
        return Unsafe.BitCast<FName, CaselessName>(name);
#endif
    }

    public static FName ToUnrealName(this Name name)
    {
        // Name was designed to have an identical layout to FName, allowing us to BitCast between the two types.
        // However, that layout differs between editor and game builds, so for packaged builds when we have no
        // display index, we can simply BitCast to CaselessName and let the implicit conversion to Name handle
        // the rest.
#if WITH_EDITOR
        return Unsafe.BitCast<Name, FName>(name);
#else
        return Unsafe.BitCast<CaselessName, FName>(name);
#endif
    }

    public static FName ToUnrealName(this CaselessName name)
    {
        // Name was designed to have an identical layout to FName, allowing us to BitCast between the two types.
        // However, that layout differs between editor and game builds, so for packaged builds when we have no
        // display index, we can simply BitCast to CaselessName and let the implicit conversion to Name handle
        // the rest.
#if WITH_EDITOR
        return Unsafe.BitCast<Name, FName>(name);
#else
        return Unsafe.BitCast<CaselessName, FName>(name);
#endif
    }

    public static Text ToPokeSharpText(this FText text)
    {
        return new Text(ITextData.FromUnrealText(text));
    }

    public static FText ToUnrealText(this Text text)
    {
        if (text.IsEmpty)
            return FText.None;

        if (text.Data is UnrealTextData unrealTextData)
        {
            return unrealTextData.ToUnrealText();
        }

        return new FText(text.ToString());
    }
}
