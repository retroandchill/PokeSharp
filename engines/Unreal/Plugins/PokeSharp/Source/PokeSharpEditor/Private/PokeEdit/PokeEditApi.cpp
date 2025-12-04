// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/PokeEditClient.h"

namespace PokeEdit
{
    const TSharedRef<FJsonValue> NoBodyJsonValue = MakeShared<FJsonValueObject>(MakeShared<FJsonObject>());

    TValueOrError<TArray<FEditorTabOption>, FString> GetEditorTabs()
    {
        static FName RequestName = "GetEditorTabs";

        auto JsonResult = SendRequest(RequestName, NoBodyJsonValue);
        if (JsonResult.HasError())
        {
            return MakeError(JsonResult.StealError());
        }

        return DeserializeFromJson<TArray<FEditorTabOption>>(JsonResult.GetValue());
    }
} // namespace PokeEdit