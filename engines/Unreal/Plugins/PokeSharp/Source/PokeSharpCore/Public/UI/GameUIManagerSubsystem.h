// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Extensions/Subsystems/SubsystemCollectionBaseRef.h"
#include "Kismet/GameplayStatics.h"

#include "GameUIManagerSubsystem.generated.h"

class UGameUIPolicy;
/**
 *
 */
UCLASS(MinimalAPI, Abstract, Config = Game)
class UGameUIManagerSubsystem : public UGameInstanceSubsystem
{
    GENERATED_BODY()

  public:
    UGameUIManagerSubsystem() = default;

    POKESHARPCORE_API void Initialize(FSubsystemCollectionBase &Collection) override;
    POKESHARPCORE_API void Deinitialize() override;
    POKESHARPCORE_API bool ShouldCreateSubsystem(UObject *Outer) const override;

    UFUNCTION(meta = (ScriptMethod, WorldContext = WorldContextObject))
    static UGameUIManagerSubsystem *GetInstance(const UObject *WorldContextObject)
    {
        return UGameplayStatics::GetGameInstance(WorldContextObject)->GetSubsystem<UGameUIManagerSubsystem>();
    }

    UGameUIPolicy *GetCurrentPolicy()
    {
        return CurrentPolicy;
    }

    const UGameUIPolicy *GetCurrentPolicy() const
    {
        return CurrentPolicy;
    }

    UFUNCTION(BlueprintNativeEvent, Category = "UI Manager")
    POKESHARPCORE_API void NotifyPlayerAdded(ULocalPlayer *LocalPlayer);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Manager")
    POKESHARPCORE_API void NotifyPlayerRemoved(ULocalPlayer *LocalPlayer);

    UFUNCTION(BlueprintNativeEvent, Category = "UI Manager")
    POKESHARPCORE_API void NotifyPlayerDestroyed(ULocalPlayer *LocalPlayer);

  protected:
    UFUNCTION(BlueprintCallable, Category = "UI Manager")
    POKESHARPCORE_API void SwitchToPolicy(UGameUIPolicy *NewPolicy);

    UFUNCTION(BlueprintNativeEvent, meta = (ScriptName = "ShouldCreateSubsystem"), Category = "Managed Subsystems")
    bool K2_ShouldCreateSubsystem(UObject *Outer) const;

    UFUNCTION(BlueprintImplementableEvent, meta = (ScriptName = "Initialize"), Category = "Managed Subsystems")
    void K2_Initialize(FSubsystemCollectionBaseRef Collection);

    UFUNCTION(BlueprintImplementableEvent, meta = (ScriptName = "Deinitialize"), Category = "Managed Subsystems")
    void K2_Deinitialize();

  private:
    TWeakObjectPtr<ULocalPlayer> PrimaryPlayer;

    UPROPERTY(Transient, BlueprintReadOnly, Getter, Category = "UI Manager", meta = (AllowPrivateAccess = "true"))
    TObjectPtr<UGameUIPolicy> CurrentPolicy = nullptr;

    UPROPERTY(Config, EditAnywhere)
    TSoftClassPtr<UGameUIPolicy> DefaultUIPolicyClass;
};