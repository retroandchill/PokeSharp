// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include <array>

namespace PokeEdit
{
    template <typename... T>
    struct TRequestPayload;

    template <typename... T>
    struct TRequestPayloadIndices;

    template <>
    struct TRequestPayload<>
    {
    };

    template <typename T0>
    struct TRequestPayload<T0>
    {
        T0 Field0;
    };

    template <typename T0, typename T1>
    struct TRequestPayload<T0, T1>
    {
        T0 Field0;
        T1 Field1;
    };

    template <typename T0, typename T1, typename T2>
    struct TRequestPayload<T0, T1, T2>
    {
        T0 Field0;
        T1 Field1;
        T2 Field2;
    };

    template <typename T0, typename T1, typename T2, typename T3>
    struct TRequestPayload<T0, T1, T2, T3>
    {
        T0 Field0;
        T1 Field1;
        T2 Field2;
        T3 Field3;
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4>
    struct TRequestPayload<T0, T1, T2, T3, T4>
    {
        T0 Field0;
        T1 Field1;
        T2 Field2;
        T3 Field3;
        T4 Field4;
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4, typename T5>
    struct TRequestPayload<T0, T1, T2, T3, T4, T5>
    {
        T0 Field0;
        T1 Field1;
        T2 Field2;
        T3 Field3;
        T4 Field4;
        T5 Field5;
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
    struct TRequestPayload<T0, T1, T2, T3, T4, T5, T6>
    {
        T0 Field0;
        T1 Field1;
        T2 Field2;
        T3 Field3;
        T4 Field4;
        T5 Field5;
        T6 Field6;
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7>
    struct TRequestPayload<T0, T1, T2, T3, T4, T5, T6, T7>
    {
        T0 Field0;
        T1 Field1;
        T2 Field2;
        T3 Field3;
        T4 Field4;
        T5 Field5;
        T6 Field6;
        T7 Field7;
    };

    template <>
    struct TRequestPayloadIndices<>
    {
        constexpr static std::array<size_t, 0> Value = {};
    };

    template <typename T0>
    struct TRequestPayloadIndices<T0>
    {
        using FPayload = TRequestPayload<T0>;

        constexpr static std::array Value = {offsetof(FPayload, Field0)};
    };

    template <typename T0, typename T1>
    struct TRequestPayloadIndices<T0, T1>
    {
        using FPayload = TRequestPayload<T0, T1>;

        constexpr static std::array Value = {offsetof(FPayload, Field0), offsetof(FPayload, Field1)};
    };

    template <typename T0, typename T1, typename T2>
    struct TRequestPayloadIndices<T0, T1, T2>
    {
        using FPayload = TRequestPayload<T0, T1, T2>;

        constexpr static std::array Value = {offsetof(FPayload, Field0),
                                             offsetof(FPayload, Field1),
                                             offsetof(FPayload, Field2)};
    };

    template <typename T0, typename T1, typename T2, typename T3>
    struct TRequestPayloadIndices<T0, T1, T2, T3>
    {
        using FPayload = TRequestPayload<T0, T1, T2, T3>;

        constexpr static std::array Value = {offsetof(FPayload, Field0),
                                             offsetof(FPayload, Field1),
                                             offsetof(FPayload, Field2),
                                             offsetof(FPayload, Field3)};
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4>
    struct TRequestPayloadIndices<T0, T1, T2, T3, T4>
    {
        using FPayload = TRequestPayload<T0, T1, T2, T3, T4>;

        constexpr static std::array Value = {offsetof(FPayload, Field0),
                                             offsetof(FPayload, Field1),
                                             offsetof(FPayload, Field2),
                                             offsetof(FPayload, Field3),
                                             offsetof(FPayload, Field4)};
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4, typename T5>
    struct TRequestPayloadIndices<T0, T1, T2, T3, T4, T5>
    {
        using FPayload = TRequestPayload<T0, T1, T2, T3, T4, T5>;

        constexpr static std::array Value = {offsetof(FPayload, Field0),
                                             offsetof(FPayload, Field1),
                                             offsetof(FPayload, Field2),
                                             offsetof(FPayload, Field3),
                                             offsetof(FPayload, Field4),
                                             offsetof(FPayload, Field5)};
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
    struct TRequestPayloadIndices<T0, T1, T2, T3, T4, T5, T6>
    {
        using FPayload = TRequestPayload<T0, T1, T2, T3, T4, T5, T6>;

        constexpr static std::array Value = {offsetof(FPayload, Field0),
                                             offsetof(FPayload, Field1),
                                             offsetof(FPayload, Field2),
                                             offsetof(FPayload, Field3),
                                             offsetof(FPayload, Field4),
                                             offsetof(FPayload, Field5),
                                             offsetof(FPayload, Field6)};
    };

    template <typename T0, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7>
    struct TRequestPayloadIndices<T0, T1, T2, T3, T4, T5, T6, T7>
    {
        using FPayload = TRequestPayload<T0, T1, T2, T3, T4, T5, T6, T7>;

        constexpr static std::array Value = {offsetof(FPayload, Field0),
                                             offsetof(FPayload, Field1),
                                             offsetof(FPayload, Field2),
                                             offsetof(FPayload, Field3),
                                             offsetof(FPayload, Field4),
                                             offsetof(FPayload, Field5),
                                             offsetof(FPayload, Field6),
                                             offsetof(FPayload, Field7)};
    };
} // namespace PokeEdit