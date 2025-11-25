using UnrealSharp.Core;
using UnrealSharp.EnhancedInput;

// ReSharper disable once CheckNamespace
namespace UnrealSharp.PokeSharp;

public partial record struct FCommandData
{
    public FCommandData(FText text, UInputAction? inputAction = null)
    {
        Text = text;
        InputAction = inputAction;
    }

    public void Deconstruct(out FText text, out UInputAction? inputAction)
    {
        text = Text;
        inputAction = InputAction;
    }
}
