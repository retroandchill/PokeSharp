// Fill out your copyright notice in the Description page of Project Settings.

#include "Configuration/SettingsChangeManager.h"
#include "LogPokeSharpCore.h"

TObjectPtr<USettingsChangeManager> USettingsChangeManager::Instance;

void USettingsChangeManager::Initialize()
{
    checkf(Instance == nullptr, TEXT("USettingsChangeManager has already been initialized")) Instance =
        NewObject<USettingsChangeManager>(GetTransientPackage(), TEXT("SettingsChangeManager"),
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
    const auto &Binding = Callbacks.Emplace(Guid, MakeShared<FSettingsChangeDelegateCallback>(ManagedDelegate));

#if WITH_EDITOR
    Settings->OnSettingChanged().AddSPLambda(
        &Binding.Get(), [&Binding = Binding.Get()](UObject *, FPropertyChangedEvent &) { Binding.Invoke(); });
#endif

    return Guid;
}

void USettingsChangeManager::Unbind(const FGuid &Guid) noexcept
{
    Callbacks.Remove(Guid);
}