// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "JsonSchema.h"

#define JSON_OBJECT_SCHEMA_BEGIN(TypeName)                                                                             \
    template <>                                                                                                        \
    struct PokeEdit::TJsonObjectTraits<TypeName>                                                                       \
    {                                                                                                                  \
        using Type = TypeName;                                                                                         \
        static constexpr auto JsonSchema = PokeEdit::TJsonObjectType( \
            std::in_place_type<TypeName>

#define JSON_OBJECT_SCHEMA_END                                                                                         \
        );                                                                                                             \
    }                                                                                                                  \
    ;

#define JSON_FIELD_INTERNAL(FieldName, Name, Required)                                                                 \
    , PokeEdit::TJsonField<&Type::FieldName>(TEXT(#FieldName), Name, Required)

#define JSON_FIELD_NAMED_OPTIONAL(FieldName, Name) JSON_FIELD_INTERNAL(FieldName, Name, false)

#define JSON_FIELD_NAMED_REQUIRED(FieldName, Name) JSON_FIELD_INTERNAL(FieldName, Name, true)

#define JSON_FIELD_OPTIONAL(FieldName)                                                                                 \
    JSON_FIELD_NAMED_OPTIONAL(FieldName, PokeEdit::ToCamelCase(TEXT(#FieldName)).ToStringView())

#define JSON_FIELD_REQUIRED(FieldName)                                                                                 \
    JSON_FIELD_NAMED_REQUIRED(FieldName, PokeEdit::ToCamelCase(TEXT(#FieldName)).ToStringView())

#define JSON_VARIANT_BEGIN(TypeName)                                                                                   \
    template <>                                                                                                        \
    struct PokeEdit::TJsonUnionTraits<TypeName>                                                                        \
    {                                                                                                                  \
        using Type = TypeName;                                                                                         \
        static constexpr auto JsonSchema = PokeEdit::TJsonUnionType( \
            PokeEdit::TJsonDiscriminator<&TypeName::GetIndex>()

#define JSON_VARIANT_END );                                                                                            \
    }                                                                                                                  \
    ;

#define JSON_VARIANT_TYPE(TypeName, Discriminator)                                                                     \
    , PokeEdit::TJsonUnionKey<TypeName, Type::IndexOfType<TypeName>()>(Discriminator)