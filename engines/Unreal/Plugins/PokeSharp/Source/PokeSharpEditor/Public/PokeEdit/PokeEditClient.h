// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Serialization/JsonConverter.h"
#include <expected>

class FJsonValue;

/**
 *
 */
namespace PokeEdit
{
    POKESHARPEDITOR_API extern const TSharedRef<FJsonValue> NoBodyJsonValue;

    POKESHARPEDITOR_API std::expected<TSharedRef<FJsonValue>, FString> SendRequest(
        FName RequestName,
        const TSharedRef<FJsonValue> &Payload);

    template <typename Result = void, TJsonSerializable Payload>
        requires(TJsonDeserializable<Result> || std::same_as<Result, void>)
    std::expected<Result, FString> SendRequest(const FName RequestName, Payload &&PayloadValue)
    {
        const auto JsonRequest = SerializeToJson(Forward<Payload>(PayloadValue));

        if constexpr (std::same_as<Result, void>)
        {
            return SendRequest(RequestName, JsonRequest);
        }
        else
        {
            return SendRequest(RequestName, JsonRequest)
                .and_then([](const TSharedRef<FJsonValue> &Response) { return DeserializeFromJson<Result>(Response); });
        }
    }

    template <typename Result = void>
        requires(TJsonDeserializable<Result> || std::same_as<Result, void>)
    std::expected<Result, FString> SendRequest(const FName RequestName)
    {
        return SendRequest<Result>(RequestName, NoBodyJsonValue);
    }
} // namespace PokeEdit
