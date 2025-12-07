// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/FieldDefinition.h"
#include <array>

namespace PokeEdit
{
    constexpr std::array FieldKindLiterals = {std::make_pair(TEXT("Text"), EFieldKind::Text),
                                              std::make_pair(TEXT("Int8"), EFieldKind::Int8),
                                              std::make_pair(TEXT("Int16"), EFieldKind::Int16),
                                              std::make_pair(TEXT("Int32"), EFieldKind::Int32),
                                              std::make_pair(TEXT("Int64"), EFieldKind::Int64),
                                              std::make_pair(TEXT("UInt8"), EFieldKind::UInt8),
                                              std::make_pair(TEXT("UInt16"), EFieldKind::UInt16),
                                              std::make_pair(TEXT("UInt32"), EFieldKind::UInt32),
                                              std::make_pair(TEXT("UInt64"), EFieldKind::UInt64),
                                              std::make_pair(TEXT("Float"), EFieldKind::Float),
                                              std::make_pair(TEXT("Double"), EFieldKind::Double),
                                              std::make_pair(TEXT("Bool"), EFieldKind::Bool),
                                              std::make_pair(TEXT("Choice"), EFieldKind::Choice),
                                              std::make_pair(TEXT("Object"), EFieldKind::Object),
                                              std::make_pair(TEXT("List"), EFieldKind::List),
                                              std::make_pair(TEXT("Dictionary"), EFieldKind::Dictionary),
                                              std::make_pair(TEXT("Optional"), EFieldKind::Optional)};

    FString LexToString(EFieldKind Kind)
    {
        check(static_cast<uint8>(Kind) < FieldKindLiterals.size());
        return FieldKindLiterals[static_cast<uint8>(Kind)].first;
    }

    bool LexFromString(EFieldKind &OutKind, const FStringView Lex)
    {
        if (auto Result = LexFromString(Lex); Result.IsSet())
        {
            OutKind = Result.GetValue();
            return true;
        }

        return false;
    }

    TOptional<EFieldKind> LexFromString(const FStringView Lex)
    {
        for (const auto &[Literal, Kind] : FieldKindLiterals)
        {
            if (Lex.Equals(Literal, ESearchCase::IgnoreCase))
            {
                return Kind;
            }
        }

        return NullOpt;
    }

    template struct TJsonConverter<TSharedRef<FFieldDefinition>>;
    template struct TJsonConverter<FDictionaryFieldPair>;
    template struct TJsonConverter<TSharedRef<FBoolFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FTextFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<int8>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<int16>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<int32>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<int64>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<uint8>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<uint16>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<uint32>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<uint64>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<float>>>;
    template struct TJsonConverter<TSharedRef<TNumberFieldDefinition<double>>>;
    template struct TJsonConverter<TSharedRef<FChoiceFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FObjectFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FListFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FDictionaryFieldDefinition>>;
    template struct TJsonConverter<TSharedRef<FOptionalFieldDefinition>>;
} // namespace PokeEdit