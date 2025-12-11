// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <array>

namespace PokeEdit
{
    consteval TCHAR ToLower(const TCHAR Ch)
    {
        if (Ch >= 'A' && Ch <= 'Z')
        {
            return Ch - ('A' - 'a');
        }

        return Ch;
    }

    template <int32 N>
    struct TStaticString
    {
        std::array<TCHAR, N> Data;

        constexpr TStaticString() = default;

        constexpr explicit TStaticString(const TCHAR (&Str)[N])
        {
            Data = Str;
        }

        constexpr const TCHAR *GetData() const
        {
            return Data.data();
        }
        static consteval std::size_t Size()
        {
            return N;
        }
        constexpr FStringView ToStringView() const
        {
            return FStringView(GetData());
        }
    };

    template <int32 N>
    consteval auto ToCamelCase(const TCHAR (&PropertyName)[N])
    {
        TStaticString<N> Result{};

        for (int32 i = 0; i < N; ++i)
        {
            if (i == 0)
            {
                Result.Data[i] = ToLower(PropertyName[i]);
            }
            else
            {
                Result.Data[i] = PropertyName[i];
            }
        }

        return Result;
    }

    template <auto Str>
    struct TStaticStringView
    {
        static constexpr FStringView Value = Str.ToStringView();
    };

    template <typename Predicate, typename Tuple, std::size_t Index = 0>
    consteval auto FilterTuple(Predicate Check, Tuple Target)
    {
        constexpr auto Size = std::tuple_size_v<decltype(Target())>;
        if constexpr (Index == Size)
        {
            return std::tuple{};
        }
        else
        {
            if constexpr (constexpr auto Value = std::get<Index>(Target()); Check(Value))
            {
                return std::tuple_cat(std::tuple(Value), FilterTuple<Predicate, Tuple, Index + 1>(Check, Target));
            }
            else
            {
                return FilterTuple<Predicate, Tuple, Index + 1>(Check, Target);
            }
        }
    }
} // namespace PokeEdit
