// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSchema.h"

namespace PokeEdit
{
    /**
     *
     */
    struct FEditorTabOption
    {
        FName Id;
        FText Name;

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FEditorTabOption>,
                            std::make_tuple(TJsonField<&FEditorTabOption::Id>(TEXT("id")),
                                            TJsonField<&FEditorTabOption::Name>(TEXT("name"))));
    };

    template struct POKESHARPEDITOR_API TJsonConverter<FEditorTabOption>;
} // namespace PokeEdit