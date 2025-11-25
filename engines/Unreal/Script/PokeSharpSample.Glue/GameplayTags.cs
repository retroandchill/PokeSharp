using UnrealSharp.GameplayTags;

public static class GameplayTags
{
    public static readonly FGameplayTag Platform_Trait_PlayInEditor = new("Platform.Trait.PlayInEditor");
    public static readonly FGameplayTag Input_MouseAndKeyboard = new("Input.MouseAndKeyboard");
    public static readonly FGameplayTag Input_Gamepad = new("Input.Gamepad");
    public static readonly FGameplayTag Input_Touch = new("Input.Touch");
    public static readonly FGameplayTag InputMode_Game = new("InputMode.Game");
    public static readonly FGameplayTag InputMode_Menu = new("InputMode.Menu");
}
