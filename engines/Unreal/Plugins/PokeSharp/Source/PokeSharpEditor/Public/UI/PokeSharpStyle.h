// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Styling/SlateStyle.h"

/**
 *
 */
class POKESHARPEDITOR_API FPokeSharpStyle
{
  public:
    static void Initialize();
    static void Shutdown();

    static void ReloadTextures();

    static const ISlateStyle &Get()
    {
        return *StyleInstance;
    }

    static FName GetStyleSetName();

  private:
    static TSharedRef<FSlateStyleSet> Create();
    static TSharedPtr<FSlateStyleSet> StyleInstance;
};
