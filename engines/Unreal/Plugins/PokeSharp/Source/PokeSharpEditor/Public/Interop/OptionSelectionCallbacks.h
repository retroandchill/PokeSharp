// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 * 
 */
struct FOptionSelectionCallbacks
{
    using FGetNamesList = void(__stdcall*)(FName, TArray<FName>&);
    
    FGetNamesList GetNamesList;
};

class FOptionSelectionManager
{
    FOptionSelectionManager() = default;
    ~FOptionSelectionManager() = default;
    
    public:
        UE_NONCOPYABLE(FOptionSelectionManager);
        static FOptionSelectionManager& Get();
    
    void SetCallbacks(FOptionSelectionCallbacks NewCallbacks);
    
    TArray<FName> GetNamesList(FName ClassName) const;
    
private:
        FOptionSelectionCallbacks Callbacks;
};
