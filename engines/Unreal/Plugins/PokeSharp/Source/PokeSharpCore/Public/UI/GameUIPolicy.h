// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"

#include "GameUIPolicy.generated.h"

class UPrimaryGameLayout;
class UGameUIManagerSubsystem;

UENUM()
enum class ELocalMultiplayerInteractionMode : uint8
{
    // Fullscreen viewport for the primary player only, regardless of the other player's existence
    PrimaryOnly,

    // Fullscreen viewport for one player, but players can swap control over who's is displayed and who's is dormant
    SingleToggle,

    // Viewports displayed simultaneously for both players
    Simultaneous
};

USTRUCT()
struct FRootViewportLayoutInfo
{
    GENERATED_BODY()

    UPROPERTY(Transient)
    TObjectPtr<ULocalPlayer> LocalPlayer;

    UPROPERTY(Transient)
    TObjectPtr<UPrimaryGameLayout> RootLayout;

    UPROPERTY(Transient)
    bool bAddedToViewport = false;

    FRootViewportLayoutInfo() = default;
    FRootViewportLayoutInfo(ULocalPlayer *InLocalPlayer, UPrimaryGameLayout *InRootLayout, const bool bIsInViewport)
        : LocalPlayer(InLocalPlayer), RootLayout(InRootLayout), bAddedToViewport(bIsInViewport)
    {
    }

    bool operator==(const ULocalPlayer *OtherLocalPlayer) const
    {
        return LocalPlayer == OtherLocalPlayer;
    }
};

/**
 *
 */
UCLASS(MinimalAPI, Abstract, Blueprintable, Within = GameUIManagerSubsystem)
class UGameUIPolicy : public UObject
{
    GENERATED_BODY()

  public:
    UGameUIPolicy() = default;

    template <std::derived_from<UGameUIPolicy> PolicyClass = UGameUIPolicy>
    static PolicyClass *GetInstance(const UObject *WorldContextObject)
    {
        return Cast<PolicyClass>(GetInstance(WorldContextObject));
    }

    UFUNCTION(meta = (WorldContext = "WorldContextObject", ScriptMethod))
    static UGameUIPolicy *GetInstance(const UObject *WorldContextObject);

    POKESHARPCORE_API UWorld *GetWorld() const override;

    POKESHARPCORE_API UGameUIManagerSubsystem *GetOwner() const;

    POKESHARPCORE_API UPrimaryGameLayout *GetRootLayout(const ULocalPlayer *Player) const;

    POKESHARPCORE_API ELocalMultiplayerInteractionMode GetLocalMultiplayerInteractionMode() const
    {
        return LocalMultiplayerInteractionMode;
    }

    POKESHARPCORE_API void RequestPrimaryControl(UPrimaryGameLayout *Layout);

  protected:
    UFUNCTION(meta = (ScriptMethod))
    POKESHARPCORE_API void AddLayoutToViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(meta = (ScriptMethod))
    POKESHARPCORE_API void RemoveLayoutFromViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Policy")
    POKESHARPCORE_API void OnRootLayoutAddedToViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Policy")
    POKESHARPCORE_API void OnRootLayoutRemovedFromViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Policy")
    POKESHARPCORE_API void OnRootLayoutReleased(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(meta = (ScriptMethod))
    POKESHARPCORE_API void CreateLayoutWidget(ULocalPlayer *LocalPlayer);

    UFUNCTION(meta = (ScriptMethod))
    POKESHARPCORE_API TSubclassOf<UPrimaryGameLayout> GetLayoutWidgetClass() const;

  private:
    POKESHARPCORE_API void NotifyPlayerAdded(ULocalPlayer *LocalPlayer);
    POKESHARPCORE_API void NotifyPlayerRemoved(ULocalPlayer *LocalPlayer);
    POKESHARPCORE_API void NotifyPlayerDestroyed(ULocalPlayer *LocalPlayer);

    friend class UGameUIManagerSubsystem;

    ELocalMultiplayerInteractionMode LocalMultiplayerInteractionMode = ELocalMultiplayerInteractionMode::PrimaryOnly;

    UPROPERTY(EditAnywhere)
    TSoftClassPtr<UPrimaryGameLayout> LayoutClass;

    UPROPERTY(Transient)
    TArray<FRootViewportLayoutInfo> RootViewportLayouts;
};