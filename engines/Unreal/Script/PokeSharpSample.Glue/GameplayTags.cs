using UnrealSharp.GameplayTags;

public static class GameplayTags
{
	public static readonly FGameplayTag Platform_Trait_PlayInEditor = new("Platform.Trait.PlayInEditor");
	public static readonly FGameplayTag Input_MouseAndKeyboard = new("Input.MouseAndKeyboard");
	public static readonly FGameplayTag Input_Gamepad = new("Input.Gamepad");
	public static readonly FGameplayTag Input_Touch = new("Input.Touch");
	public static readonly FGameplayTag InputMode_Game = new("InputMode.Game");
	public static readonly FGameplayTag InputMode_Menu = new("InputMode.Menu");
	public static readonly FGameplayTag UI_Layer_Game = new("UI.Layer.Game");
	public static readonly FGameplayTag UI_Layer_GameMenu = new("UI.Layer.GameMenu");
	public static readonly FGameplayTag UI_Layer_Menu = new("UI.Layer.Menu");
	public static readonly FGameplayTag UI_Layer_Modal = new("UI.Layer.Modal");
}