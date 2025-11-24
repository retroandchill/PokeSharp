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

    POKESHARP_API UWorld *GetWorld() const override;

    POKESHARP_API UGameUIManagerSubsystem *GetOwner() const;

    POKESHARP_API UPrimaryGameLayout *GetRootLayout(const ULocalPlayer *Player) const;

    POKESHARP_API ELocalMultiplayerInteractionMode GetLocalMultiplayerInteractionMode() const
    {
        return LocalMultiplayerInteractionMode;
    }

    POKESHARP_API void RequestPrimaryControl(UPrimaryGameLayout *Layout);

  protected:
    UFUNCTION(meta = (ScriptMethod))
    POKESHARP_API void AddLayoutToViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(meta = (ScriptMethod))
    POKESHARP_API void RemoveLayoutFromViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Policy")
    POKESHARP_API void OnRootLayoutAddedToViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Policy")
    POKESHARP_API void OnRootLayoutRemovedFromViewport(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Policy")
    POKESHARP_API void OnRootLayoutReleased(ULocalPlayer *LocalPlayer, UPrimaryGameLayout *Layout);

    UFUNCTION(meta = (ScriptMethod))
    POKESHARP_API void CreateLayoutWidget(ULocalPlayer *LocalPlayer);

    UFUNCTION(meta = (ScriptMethod))
    POKESHARP_API TSubclassOf<UPrimaryGameLayout> GetLayoutWidgetClass() const;

  private:
    POKESHARP_API void NotifyPlayerAdded(ULocalPlayer *LocalPlayer);
    POKESHARP_API void NotifyPlayerRemoved(ULocalPlayer *LocalPlayer);
    POKESHARP_API void NotifyPlayerDestroyed(ULocalPlayer *LocalPlayer);

    friend class UGameUIManagerSubsystem;

    ELocalMultiplayerInteractionMode LocalMultiplayerInteractionMode = ELocalMultiplayerInteractionMode::PrimaryOnly;

    UPROPERTY(EditAnywhere)
    TSoftClassPtr<UPrimaryGameLayout> LayoutClass;

    UPROPERTY(Transient)
    TArray<FRootViewportLayoutInfo> RootViewportLayouts;
};