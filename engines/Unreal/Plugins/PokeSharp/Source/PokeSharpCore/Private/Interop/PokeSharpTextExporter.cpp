// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/PokeSharpTextExporter.h"

void UPokeSharpTextExporter::FromSourceString(const UTF16CHAR *Buffer, const int32 Length, FText &OutText)
{
    OutText = FText::FromStringView(TStringView(Buffer, Length));
}

void UPokeSharpTextExporter::FromLocalized(const UTF16CHAR *NamespaceBuffer,
                                           const int32 NamespaceLength,
                                           const UTF16CHAR *KeyBuffer,
                                           const int32 KeyLength,
                                           const UTF16CHAR *StrBuffer,
                                           const int32 StrLength,
                                           FText &OutText)
{
    OutText = FText::AsLocalizable_Advanced(FTextKey(TStringView(NamespaceBuffer, NamespaceLength)),
                                            FTextKey(TStringView(KeyBuffer, KeyLength)),
                                            TStringView(StrBuffer, StrLength));
}

void UPokeSharpTextExporter::FromLocText(const TCHAR *StrBuffer, const int32 StrLength, FText &OutText)
{
    if (!FTextStringHelper::ReadFromBuffer(StrBuffer, OutText))
    {
        OutText = FText::FromStringView(FStringView(StrBuffer, StrLength));
    }
}

void UPokeSharpTextExporter::ToLocText(const FText &Text, FString &OutStr)
{
    FTextStringHelper::WriteToBuffer(OutStr, Text);
}

const FString *UPokeSharpTextExporter::GetSourceString(const FText &Text)
{
    return FTextInspector::GetSourceString(Text);
}

const FString *UPokeSharpTextExporter::GetDisplayString(const FText &Text)
{
    return &Text.ToString();
}

bool UPokeSharpTextExporter::GetNamespace(const FText &Text, FString &OutNamespace)
{
    if (auto Result = FTextInspector::GetNamespace(Text); Result.IsSet())
    {
        OutNamespace = MoveTemp(Result.GetValue());
        return true;
    }

    OutNamespace.Empty();
    return false;
}

bool UPokeSharpTextExporter::GetKey(const FText &Text, FString &OutKey)
{
    if (auto Result = FTextInspector::GetKey(Text); Result.IsSet())
    {
        OutKey = MoveTemp(Result.GetValue());
        return true;
    }

    OutKey.Empty();
    return false;
}

bool UPokeSharpTextExporter::IsCultureInvariant(const FText &Text)
{
    return Text.IsCultureInvariant();
}

bool UPokeSharpTextExporter::IsTransient(const FText &Text)
{
    return Text.IsTransient();
}

bool UPokeSharpTextExporter::ShouldGatherForLocalization(const FText &Text)
{
    return Text.ShouldGatherForLocalization();
}

bool UPokeSharpTextExporter::IsEmpty(const FText &Text)
{
    return Text.IsEmpty();
}

bool UPokeSharpTextExporter::IsWhitespace(const FText &Text)
{
    return Text.IsEmptyOrWhitespace();
}

void UPokeSharpTextExporter::Destroy(FText &Text)
{
    std::destroy_at(&Text);
}

void UPokeSharpTextExporter::AsDisplaySpan(const FText &Text, const TCHAR *&OutBuffer, int32 &OutLength)
{
    const auto &AsString = Text.ToString();
    OutBuffer = AsString.GetCharArray().GetData();
    OutLength = AsString.Len();
}
