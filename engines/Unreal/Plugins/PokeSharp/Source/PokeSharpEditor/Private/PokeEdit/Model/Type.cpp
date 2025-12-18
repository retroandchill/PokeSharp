#include "PokeEdit/Model/Type.h"
#include "PokeEdit/Properties/JsonStructHandleTemplate.h"
#include "PokeEdit/Serialization/JsonSchema.h"

JSON_OBJECT_SCHEMA_BEGIN(FType)
    JSON_FIELD_OPTIONAL(Id)
    JSON_FIELD_OPTIONAL(Name)
    JSON_FIELD_OPTIONAL(IconPosition)
    JSON_FIELD_OPTIONAL(IsSpecialType)
    JSON_FIELD_OPTIONAL(IsPseudoType)
    JSON_FIELD_OPTIONAL(Weaknesses)
    JSON_FIELD_OPTIONAL(Resistances)
    JSON_FIELD_OPTIONAL(Immunities)
    JSON_FIELD_OPTIONAL(Flags)
JSON_OBJECT_SCHEMA_END

TSharedRef<PokeEdit::FJsonStructHandle> FType::CreateJsonHandle(const FName Name, const int32 Index)
{
    return MakeShared<PokeEdit::TJsonStructHandle<FType>>(Name, Index);
}

DEFINE_JSON_CONVERTERS(FType);