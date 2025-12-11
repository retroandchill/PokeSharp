// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/FieldPath.h"
#include "Templates/Overload.h"

namespace PokeEdit
{
    template struct TJsonConverter<FPropertySegment>;
    template struct TJsonConverter<FListIndexSegment>;
    template struct TJsonConverter<FDictionaryKeySegment>;
    template struct TJsonConverter<FFieldPathSegment>;
    template struct TJsonConverter<FFieldPath>;

    FString FFieldPath::ToString() const
    {
        FString Result;
        for (int32 i = 0; i < Segments.Num(); i++)
        {
            Visit(UE::Overload([&Result](const FPropertySegment &Segment) { Result += Segment.Name.ToString(); },
                               [&Result](const FListIndexSegment &Segment)
                               { Result += FString::Printf(TEXT("[%d]"), Segment.Index); },
                               [&Result](const FDictionaryKeySegment &Segment)
                               { Result += FString::Printf(TEXT("{%s}"), *Segment.Key->AsString()); }),
                  Segments[i]);

            if (i < Segments.Num() - 1)
            {
                Result += TEXT(".");
            }
        }
        return Result;
    }
} // namespace PokeEdit
