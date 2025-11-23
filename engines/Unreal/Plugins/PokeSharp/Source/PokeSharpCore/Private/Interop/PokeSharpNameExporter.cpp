// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/PokeSharpNameExporter.h"

void UPokeSharpNameExporter::GetOrAddEntry(const UTF16CHAR *Str, const int32 Length, const bool FindMode,
                                           uint32 &ComparisonIndex, uint32 &DisplayIndex, int32 &Number)
{
    const auto Name = FName(TStringView(Str, Length), FindMode ? FNAME_Find : FNAME_Add);
    ComparisonIndex = Name.GetComparisonIndex().ToUnstableInt();
    DisplayIndex = Name.GetDisplayIndex().ToUnstableInt();
    Number = Name.GetNumber();
}

bool UPokeSharpNameExporter::IsValid(const uint32 ComparisonIndex, const uint32 DisplayIndex)
{
    const auto Name = GetName(ComparisonIndex, DisplayIndex, 0);
    return Name.GetComparisonNameEntry() != nullptr && Name.GetDisplayNameEntry() != nullptr;
}

bool UPokeSharpNameExporter::EqualsBuffer(const uint32 ComparisonIndex, const uint32 DisplayIndex, const int32 Number,
                                          const UTF16CHAR *Str, const int32 Length)
{
    const auto Name = GetName(ComparisonIndex, DisplayIndex, Number);
    return Name == TStringView(Str, Length);
}

void UPokeSharpNameExporter::GetString(const uint32 ComparisonIndex, const uint32 DisplayIndex, const int32 Number,
                                       FString &OutString)
{
    OutString = GetName(ComparisonIndex, DisplayIndex, Number).ToString();
}

void UPokeSharpNameExporter::FromUnrealName(const FName Name, uint32 &ComparisonIndex, uint32 &DisplayIndex,
                                            int32 &Number)
{
    ComparisonIndex = Name.GetComparisonIndex().ToUnstableInt();
    DisplayIndex = Name.GetDisplayIndex().ToUnstableInt();
    Number = Name.GetNumber();
}

FName UPokeSharpNameExporter::GetName(const uint32 ComparisonIndex, const uint32 DisplayIndex, const int32 Number)
{
    return FName(FNameEntryId::FromUnstableInt(ComparisonIndex), FNameEntryId::FromUnstableInt(DisplayIndex), Number);
}
