// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/PokeEditClient.h"
#include "PokeEdit/Schema/EditorLabelRequest.h"

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

    TValueOrError<TArray<FText>, FString> GetEntryLabels(const FName EditorId)
    {
        static FName RequestName = "GetEntryLabels";

        const auto JsonRequest = SerializeToJson(FEditorLabelRequest(EditorId));
        auto JsonResult = SendRequest(RequestName, JsonRequest);
        if (JsonResult.HasError())
        {
            return MakeError(JsonResult.StealError());
        }

        return DeserializeFromJson<TArray<FText>>(JsonResult.GetValue());
    }
} // namespace PokeEdit