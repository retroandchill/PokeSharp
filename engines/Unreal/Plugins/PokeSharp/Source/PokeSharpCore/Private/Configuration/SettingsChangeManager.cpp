// Fill out your copyright notice in the Description page of Project Settings.

#include "Configuration/SettingsChangeManager.h"
#include "Containers/Ticker.h"
#include "Engine/DeveloperSettings.h"
#include "LogPokeSharpCore.h"
#include "UObject/Package.h"

TObjectPtr<USettingsChangeManager> USettingsChangeManager::Instance;

void FDebouncedSettingsCallback::RequestExecution(const float DebounceSeconds)
{
    // Already scheduled – just mark dirty, no new ticker
    if (bPendingExecution)
    {
        return;
    }

    bPendingExecution = true;

    // Schedule a one–shot ticker
    TickerHandle =
        FTSTicker::GetCoreTicker().AddTicker(FTickerDelegate::CreateRaw(this, &FDebouncedSettingsCallback::OnTick),
                                             DebounceSeconds);
}

bool FDebouncedSettingsCallback::OnTick(float)
{
    // Clear pending state before invoking, in case callback triggers new changes
    bPendingExecution = false;
    TickerHandle.Reset();

    Callback->Invoke();
    // Returning false so this ticker is not called again
    return false;
}

void USettingsChangeManager::Initialize()
{
    checkf(Instance == nullptr, TEXT("USettingsChangeManager has already been initialized")) Instance =
        NewObject<USettingsChangeManager>(GetTransientPackage(),
                                          TEXT("SettingsChangeManager"),
                                          RF_Public | RF_MarkAsRootSet);
}

void USettingsChangeManager::Shutdown()
{
    if (Instance == nullptr)
    {
        UE_LOG(LogPokeSharpCore, Error, TEXT("USettingsChangeManager has not been initialized"));
        return;
    }

    Instance->Callbacks.Empty();
    Instance = nullptr;
}

FGuid USettingsChangeManager::Bind(UDeveloperSettings *Settings, const FGCHandle &ManagedDelegate) noexcept
{
    auto Guid = FGuid::NewGuid();
    auto Callback = MakeShared<FSettingsChangeDelegateCallback>(ManagedDelegate);

    constexpr float DebounceSeconds = 0.10f;
    const auto &DebouncedEntry = Callbacks.Emplace(Guid, MakeShared<FDebouncedSettingsCallback>(Callback));

#if WITH_EDITOR
    Settings->OnSettingChanged().AddLambda(
        [WeakBinding = DebouncedEntry.ToWeakPtr()](UObject *, FPropertyChangedEvent &)
        {
            const auto Binding = WeakBinding.Pin();
            if (Binding == nullptr)
                return;
            Binding->RequestExecution(DebounceSeconds);
        });
#endif

    return Guid;
}

void USettingsChangeManager::Unbind(const FGuid &Guid) noexcept
{
    Callbacks.Remove(Guid);
}