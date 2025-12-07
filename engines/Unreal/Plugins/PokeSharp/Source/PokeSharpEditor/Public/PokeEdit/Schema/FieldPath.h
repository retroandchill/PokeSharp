// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSchema.h"
#include "Mcro/Range.h"
#include "Mcro/Range/Conversion.h"

namespace PokeEdit
{
    struct FPropertySegment
    {
        FName Name;

        explicit FPropertySegment(const FName InName) : Name(InName)
        {
        }

        static constexpr auto JsonSchema = TJsonObjectType(
            std::in_place_type<FPropertySegment>, std::make_tuple(TJsonField<&FPropertySegment::Name>(TEXT("name"))));
    };

    struct FListIndexSegment
    {
        int32 Index;

        explicit FListIndexSegment(const int32 InIndex) : Index(InIndex)
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FListIndexSegment>,
                            std::make_tuple(TJsonField<&FListIndexSegment::Index>(TEXT("index"))));
    };

    struct FDictionaryKeySegment
    {
        TSharedRef<FJsonValue> Key;

        explicit FDictionaryKeySegment(const TSharedRef<FJsonValue> &InIndex) : Key(InIndex)
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FDictionaryKeySegment>,
                            std::make_tuple(TJsonField<&FDictionaryKeySegment::Key>(TEXT("key"))));
    };

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

        static constexpr auto JsonSchema = TJsonObjectType(
            std::in_place_type<FFieldPath>, std::make_tuple(TJsonField<&FFieldPath::Segments>(TEXT("segments"))));
    };

    template <>
    struct TJsonUnionTraits<FFieldPathSegment>
    {
        static constexpr auto JsonSchema = TJsonUnionType(
            TJsonDiscriminator<&FFieldPathSegment::GetIndex>(),
            TJsonUnionKey<FPropertySegment, FFieldPathSegment::IndexOfType<FPropertySegment>()>(TEXT("Property")),
            TJsonUnionKey<FListIndexSegment, FFieldPathSegment::IndexOfType<FListIndexSegment>()>(TEXT("ListIndex")),
            TJsonUnionKey<FDictionaryKeySegment, FFieldPathSegment::IndexOfType<FDictionaryKeySegment>()>(
                TEXT("DictionaryKey")));
    };

    template POKESHARPEDITOR_API struct TJsonConverter<FPropertySegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListIndexSegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryKeySegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FFieldPathSegment>;
    template POKESHARPEDITOR_API struct TJsonConverter<FFieldPath>;
} // namespace PokeEdit