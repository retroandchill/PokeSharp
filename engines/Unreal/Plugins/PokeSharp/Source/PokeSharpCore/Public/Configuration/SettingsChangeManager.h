// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSManagedDelegate.h"

#include "SettingsChangeManager.generated.h"

struct FSettingsChangeDelegateCallback : TSharedFromThis<FSettingsChangeDelegateCallback>
{
    UE_NONCOPYABLE(FSettingsChangeDelegateCallback)

    explicit FSettingsChangeDelegateCallback(const FGCHandle &Delegate) : Delegate(Delegate)
    {
    }

    ~FSettingsChangeDelegateCallback()
    {
        Delegate.Dispose();
    }

    void Invoke()
    {
        Delegate.Invoke(nullptr, false);
    }

  private:
    FCSManagedDelegate Delegate;
};

/**
 *
 */
UCLASS()
class POKESHARPCORE_API USettingsChangeManager : public UObject
{
    GENERATED_BODY()

  public:
    static void Initialize();
    static void Shutdown();

    static USettingsChangeManager &Get()
    {
        checkf(Instance != nullptr, TEXT("Setting Change Manager is not initialized")) return *Instance;
    }

    FGuid Bind(UDeveloperSettings *Settings, const FGCHandle &ManagedDelegate) noexcept;
    void Unbind(const FGuid &Guid) noexcept;

  private:
    static TObjectPtr<USettingsChangeManager> Instance;
    TMap<FGuid, TSharedRef<FSettingsChangeDelegateCallback>> Callbacks;
};
