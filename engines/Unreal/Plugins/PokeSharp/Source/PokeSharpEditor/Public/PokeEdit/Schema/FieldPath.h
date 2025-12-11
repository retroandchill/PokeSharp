// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Mcro/Range/Conversion.h"
#include "PokeEdit/Serialization/JsonMacros.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    struct FPropertySegment
    {
        FName Name;

        explicit FPropertySegment(const FName InName) : Name(InName)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FPropertySegment)
        JSON_FIELD_REQUIRED(Name)
    JSON_OBJECT_SCHEMA_END

    struct FListIndexSegment
    {
        int32 Index;

        explicit FListIndexSegment(const int32 InIndex) : Index(InIndex)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListIndexSegment)
        JSON_FIELD_REQUIRED(Index)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryKeySegment
    {
        TSharedRef<FJsonValue> Key;

        explicit FDictionaryKeySegment(const TSharedRef<FJsonValue> &InIndex) : Key(InIndex)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryKeySegment)
        JSON_FIELD_REQUIRED(Key)
    JSON_OBJECT_SCHEMA_END

    using FFieldPathSegment = TVariant<FPropertySegment, FListIndexSegment, FDictionaryKeySegment>;

    struct FFieldPath
    {
        TArray<FFieldPathSegment> Segments;

        explicit FFieldPath(TArray<FFieldPathSegment> InSegments) : Segments(MoveTemp(InSegments))
        {
        }

        template <ranges::input_range R>
            requires std::convertible_to<ranges::range_common_reference_t<R>, FFieldPathSegment>
        explicit FFieldPath(R &&Range)
            : Segments(Range |
                       ranges::views::transform([](auto &&Segment) { return FFieldPathSegment(MoveTemp(Segment)); }) |
                       Mcro::Range::RenderAs<TArray>())
        {
        }

        template <typename... T>
            requires((std::is_convertible_v<T, FPropertySegment> || std::is_convertible_v<T, FListIndexSegment> ||
                      std::is_convertible_v<T, FDictionaryKeySegment>) &&
                     ...)
        explicit FFieldPath(T &&...InSegments)
            : Segments({FFieldPathSegment(TInPlaceType<std::remove_cvref_t<T>>(), Forward<T>(InSegments))...})
        {
        }

        POKESHARPEDITOR_API FString ToString() const;
    };

    JSON_OBJECT_SCHEMA_BEGIN(FFieldPath)
        JSON_FIELD_REQUIRED(Segments)
    JSON_OBJECT_SCHEMA_END

    JSON_VARIANT_BEGIN(FFieldPathSegment)
        JSON_VARIANT_TYPE(FPropertySegment, TEXT("Property"))
        JSON_VARIANT_TYPE(FListIndexSegment, TEXT("ListIndex"))
        JSON_VARIANT_TYPE(FDictionaryKeySegment, TEXT("DictionaryKey"))
    JSON_VARIANT_END

    template POKESHARPEDITOR_API struct TJsonConverter<FPropertySegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListIndexSegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryKeySegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FFieldPathSegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FFieldPath>;
} // namespace PokeEdit