// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Interop/PokeEditCallbacks.h"
#include "Serialization/JsonConverter.h"
#include "Requests/RequestPacking.h"
#include <bit>
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
    
    template <size_t N>
        requires (N <= std::numeric_limits<int32>::max())
    constexpr TConstArrayView<size_t> GetIndexView(const std::array<size_t, N>& Array)
    {
        if constexpr (N == 0)
        {
            return TConstArrayView<size_t>();
        }
        else
        {
            return TConstArrayView<size_t>(Array.data(), static_cast<int32>(N));
        }
    }
    
    template <typename Result = void, TPackable... Args>
        requires((TPackable<Result> || std::same_as<Result, void>) && sizeof...(Args) <= 8)
    std::expected<Result, FString> SendRequest(const FName ControllerName, const FName MethodName, const TRequestPayload<Args...>& InArgs)
    {
        constexpr auto Indices = TRequestPayloadIndices<Args...>::Value;
        
        auto IndexView = GetIndexView(Indices);
        
        if constexpr (std::same_as<Result, void>)
        {    
            return FPokeEditManager::Get()
                .SendRequest(ControllerName, MethodName, std::bit_cast<const uint8*>(&InArgs), IndexView, nullptr);
        }
        else
        {
            TPackedType<Result> PackedResult;
            return FPokeEditManager::Get().SendRequest(ControllerName, MethodName, std::bit_cast<const uint8*>(&InArgs), IndexView, std::bit_cast<uint8*>(&PackedResult))
                .and_then([&PackedResult]
                {
                    return UnpackResponse<Result>(PackedResult);
                });
        }
    }
    
    template <typename Result = void, TPackable... Args>
        requires((TJsonDeserializable<Result> || std::same_as<Result, void>) && sizeof...(Args) <= 8)
    std::expected<Result, FString> SendRequest(const FName ControllerName, const FName MethodName, Args&&... InArgs)
    {
        return PackPayload(Forward<Args>(InArgs)...)
            .and_then([&](const TRequestPayload<TPackedType<Args>...> &PackedArgs)
            {
                return SendRequest<Result>(ControllerName, MethodName, PackedArgs);
            });
    }
    

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
} // namespace PokeEdit
