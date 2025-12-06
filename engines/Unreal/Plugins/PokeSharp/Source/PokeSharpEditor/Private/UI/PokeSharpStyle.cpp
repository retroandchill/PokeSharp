// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/PokeSharpStyle.h"
#include "Framework/Application/SlateApplication.h"
#include "Interfaces/IPluginManager.h"
#include "Styling/SlateStyleMacros.h"
#include "Styling/SlateStyleRegistry.h"

TSharedPtr<FSlateStyleSet> FPokeSharpStyle::StyleInstance = nullptr;

#define RootToContentDir Style->RootToContentDir

void FPokeSharpStyle::Initialize()
{
    if (StyleInstance.IsValid())
        return;

    StyleInstance = Create();
    FSlateStyleRegistry::RegisterSlateStyle(*StyleInstance);
}

void FPokeSharpStyle::Shutdown()
{
    FSlateStyleRegistry::UnRegisterSlateStyle(*StyleInstance);
    ensure(StyleInstance.IsUnique());
    StyleInstance.Reset();
}

void FPokeSharpStyle::ReloadTextures()
{
    if (FSlateApplication::IsInitialized())
    {
        FSlateApplication::Get().GetRenderer()->ReloadTextureResources();
    }
}

FName FPokeSharpStyle::GetStyleSetName()
{
    static FName StyleSetName(TEXT("PokeSharpStyle"));
    return StyleSetName;
}

TSharedRef<FSlateStyleSet> FPokeSharpStyle::Create()
{
    const FVector2D Icon40x40(40.0f, 40.0f);

    auto Style = MakeShared<FSlateStyleSet>("PokeSharpStyle");
    Style->SetContentRoot(IPluginManager::Get().FindPlugin(UE_PLUGIN_NAME)->GetBaseDir() / TEXT("Resources"));
    Style->Set("PokeSharp.Toolbar", new IMAGE_BRUSH(TEXT("Icon_Default_40x"), Icon40x40));
    return Style;
}

#undef RootToContentDir