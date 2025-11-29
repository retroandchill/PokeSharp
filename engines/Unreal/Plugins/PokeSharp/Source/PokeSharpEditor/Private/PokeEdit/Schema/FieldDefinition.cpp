// Fill out your copyright notice in the Description page of Project Settings.


#include "PokeEdit/Schema/FieldDefinition.h"
#include "Unreachable.h"


namespace PokeEdit
{
    FString LexToString(EFieldKind Kind)
    {
        constexpr std::array KindNames = {  TEXT("Text"),  TEXT("Int"),  TEXT("Float"),  TEXT("Bool"),  TEXT("Choice"),  TEXT("Object"),  TEXT("List"),  TEXT("Dictionary"), TEXT("Optional") };
        check(static_cast<uint8>(Kind) < KindNames.size());
        return KindNames[static_cast<uint8>(Kind)];
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
        constexpr std::array Literals = {
            std::make_pair(TEXT("Text"), EFieldKind::Text),
            std::make_pair(TEXT("Int"), EFieldKind::Int),
            std::make_pair(TEXT("Float"), EFieldKind::Float),
            std::make_pair(TEXT("Bool"), EFieldKind::Bool),
            std::make_pair(TEXT("Choice"), EFieldKind::Choice),
            std::make_pair(TEXT("Object"), EFieldKind::Object),
            std::make_pair(TEXT("List"), EFieldKind::List),
            std::make_pair(TEXT("Dictionary"), EFieldKind::Dictionary),
            std::make_pair(TEXT("Optional"), EFieldKind::Optional)
        };
        
        for (const auto &[Literal, Kind] : Literals)
        {
            if (Lex.Equals(Literal, ESearchCase::IgnoreCase))
            {
                return Kind;
            }
        }
        
        return NullOpt;
    }

    TValueOrError<TSharedRef<FFieldDefinition>, FString> TJsonConverter<TSharedRef<FFieldDefinition>>::Deserialize(
        const TSharedRef<FJsonValue> &Value)
    {
        TSharedPtr<FJsonObject> *JsonObject;
        if (!Value->TryGetObject(JsonObject))
        {
            return MakeError(FString::Format(TEXT("Value '{0}' is not an object"), { WriteAsString(Value) }));
        }

        const auto Kind = (*JsonObject)->TryGetField(TEXT("kind"));
        if (!Kind.IsValid())
        {
            return MakeError(FString::Format(TEXT("Field 'kind' is missing from object '{0}'"), { WriteAsString(Value) }));
        }
        
        auto KindValue = PokeEdit::Deserialize<EFieldKind>(Kind.ToSharedRef());
        return MakeError(TEXT("Not implemented"));
    }

    TSharedRef<FJsonValue> TJsonConverter<TSharedRef<FFieldDefinition>>::Serialize(TSharedRef<FFieldDefinition> Value)
    {
        check(false);
        UE::Unreachable();
    }
}