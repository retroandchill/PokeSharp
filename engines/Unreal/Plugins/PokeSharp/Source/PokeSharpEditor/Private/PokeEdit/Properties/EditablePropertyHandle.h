// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PropertyHandle.h"

namespace PokeEdit
{
    /**
     * 
     */
    class POKESHARPEDITOR_API FEditablePropertyHandle : IPropertyHandle
    {
    public:
        bool IsValidHandle() const override;
        bool IsSamePropertyNode(TSharedPtr<IPropertyHandle> OtherHandle) const override;
        bool IsEditConst() const override;
        bool IsEditable() const override;
        bool IsExpanded() const override;
        void SetExpanded(bool bExpanded) override;
        const FFieldClass * GetPropertyClass() const override;
        FProperty * GetProperty() const override;
        FStringView GetPropertyPath() const override;
        TSharedPtr<FPropertyPath> CreateFPropertyPath() const override;
        int32 GetArrayIndex() const override;
        void RequestRebuildChildren() override;
        FProperty * GetMetaDataProperty() const override;
        bool HasMetaData(const FName &Key) const override;
        const FString & GetMetaData(const FName &Key) const override;
        bool GetBoolMetaData(const FName &Key) const override;
        int32 GetIntMetaData(const FName &Key) const override;
        float GetFloatMetaData(const FName &Key) const override;
        double GetDoubleMetaData(const FName &Key) const override;
        UClass * GetClassMetaData(const FName &Key) const override;
        void SetInstanceMetaData(const FName &Key, const FString &Value) override;
        const FString * GetInstanceMetaData(const FName &Key) const override;
        const TMap<FName, FString> * GetInstanceMetaDataMap() const override;
        FText GetToolTipText() const override;
        void SetToolTipText(const FText &ToolTip) override;
        bool HasDocumentation() override;
        FString GetDocumentationLink() override;
        FString GetDocumentationExcerptName() override;
        uint8 * GetValueBaseAddress(uint8 *Base) const override;
        FPropertyAccess::Result GetValueAsFormattedString(FString &OutValue,
            EPropertyPortFlags PortFlags = PPF_PropertyWindow) const override;
        FPropertyAccess::Result GetValueAsDisplayString(FString &OutValue,
            EPropertyPortFlags PortFlags = PPF_PropertyWindow) const override;
        FPropertyAccess::Result GetValueAsFormattedText(FText &OutValue) const override;
        FPropertyAccess::Result GetValueAsDisplayText(FText &OutValue) const override;
        FPropertyAccess::Result SetValueFromFormattedString(const FString &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        void SetOnPropertyValueChanged(const FSimpleDelegate &InOnPropertyValueChanged) override;
        void SetOnPropertyValueChangedWithData(
            const TDelegate<void(const FPropertyChangedEvent &)> &InOnPropertyValueChanged) override;
        void SetOnChildPropertyValueChanged(const FSimpleDelegate &InOnChildPropertyValueChanged) override;
        void SetOnChildPropertyValueChangedWithData(
            const TDelegate<void(const FPropertyChangedEvent &)> &InOnPropertyValueChanged) override;
        void SetOnPropertyValuePreChange(const FSimpleDelegate &InOnPropertyValuePreChange) override;
        void SetOnChildPropertyValuePreChange(const FSimpleDelegate &InOnChildPropertyValuePreChange) override;
        void SetOnPropertyResetToDefault(const FSimpleDelegate &InOnPropertyResetToDefault) override;
        FPropertyAccess::Result GetValue(float &OutValue) const override;
        FPropertyAccess::Result GetValue(double &OutValue) const override;
        FPropertyAccess::Result GetValue(bool &OutValue) const override;
        FPropertyAccess::Result GetValue(int8 &OutValue) const override;
        FPropertyAccess::Result GetValue(int16 &OutValue) const override;
        FPropertyAccess::Result GetValue(int32 &OutValue) const override;
        FPropertyAccess::Result GetValue(int64 &OutValue) const override;
        FPropertyAccess::Result GetValue(uint8 &OutValue) const override;
        FPropertyAccess::Result GetValue(uint16 &OutValue) const override;
        FPropertyAccess::Result GetValue(uint32 &OutValue) const override;
        FPropertyAccess::Result GetValue(uint64 &OutValue) const override;
        FPropertyAccess::Result GetValue(FString &OutValue) const override;
        FPropertyAccess::Result GetValue(FText &OutValue) const override;
        FPropertyAccess::Result GetValue(FName &OutValue) const override;
        FPropertyAccess::Result GetValue(FVector &OutValue) const override;
        FPropertyAccess::Result GetValue(FVector2D &OutValue) const override;
        FPropertyAccess::Result GetValue(FVector4 &OutValue) const override;
        FPropertyAccess::Result GetValue(FQuat &OutValue) const override;
        FPropertyAccess::Result GetValue(FRotator &OutValue) const override;
        FPropertyAccess::Result GetValue(UObject *&OutValue) const override;
        FPropertyAccess::Result GetValue(const UObject *&OutValue) const override;
        FPropertyAccess::Result GetValue(FAssetData &OutValue) const override;
        FPropertyAccess::Result GetValueData(void *&OutAddress) const override;
        FPropertyAccess::Result GetValue(FProperty *&OutValue) const override;
        FPropertyAccess::Result GetValue(const FProperty *&OutValue) const override;
        FPropertyAccess::Result SetValue(const float &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const double &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const bool &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const int8 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const int16 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const int32 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const int64 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const uint8 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const uint16 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const uint32 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const uint64 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FString &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FText &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FName &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FVector &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FVector2D &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FVector4 &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FQuat &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FRotator &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(UObject * const &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const UObject * const &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FAssetData &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const TCHAR *InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(FProperty * const &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result SetValue(const FProperty * const &InValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        void NotifyPreChange() override;
        void NotifyPostChange(EPropertyChangeType::Type ChangeType) override;
        void NotifyFinishedChangingProperties() override;
        FPropertyAccess::Result SetObjectValueFromSelection() override;
        int32 GetNumPerObjectValues() const override;
        FPropertyAccess::Result SetPerObjectValues(const TArray<FString> &PerObjectValues,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result GetPerObjectValues(TArray<FString> &OutPerObjectValues) const override;
        FPropertyAccess::Result SetPerObjectValue(const int32 ObjectIndex,
            const FString &ObjectValue,
            EPropertyValueSetFlags::Type Flags = EPropertyValueSetFlags::DefaultFlags) override;
        FPropertyAccess::Result GetPerObjectValue(const int32 ObjectIndex, FString &OutObjectValue) const override;
        int32 GetIndexInArray() const override;
        TSharedPtr<IPropertyHandle> GetChildHandle(FName ChildName, bool bRecurse = true) const override;
        TSharedPtr<IPropertyHandle> GetChildHandle(uint32 Index) const override;
        TSharedPtr<IPropertyHandle> GetParentHandle() const override;
        TSharedPtr<IPropertyHandle> GetKeyHandle() const override;
        FPropertyAccess::Result GetNumChildren(uint32 &OutNumChildren) const override;
        uint32 GetNumOuterObjects() const override;
        void GetOuterObjects(TArray<UObject *> &OuterObjects) const override;
        void GetOuterStructs(TArray<TSharedPtr<FStructOnScope>> &OutStructs) const override;
        const UClass * GetOuterBaseClass() const override;
        void ReplaceOuterObjects(const TArray<UObject *> &OuterObjects) override;
        void GetOuterPackages(TArray<UPackage *> &OuterPackages) const override;
        void EnumerateRawData(const EnumerateRawDataFuncRef &InRawDataCallback) override;
        void EnumerateConstRawData(const EnumerateConstRawDataFuncRef &InRawDataCallback) const override;
        void AccessRawData(TArray<void *> &RawData) override;
        void AccessRawData(TArray<const void *> &RawData) const override;
        TSharedPtr<IPropertyHandleArray> AsArray() override;
        TSharedPtr<IPropertyHandleSet> AsSet() override;
        TSharedPtr<IPropertyHandleMap> AsMap() override;
        TSharedPtr<IPropertyHandleOptional> AsOptional() override;
        TSharedPtr<IPropertyHandleStruct> AsStruct() override;
        FText GetPropertyDisplayName() const override;
        void SetPropertyDisplayName(FText InDisplayName) override;
        void ResetToDefault() override;
        bool DiffersFromDefault() const override;
        FText GetResetToDefaultLabel() const override;
        bool GeneratePossibleValues(TArray<TSharedPtr<FString>> &OutOptionStrings,
            TArray<FText> &OutToolTips,
            TArray<bool> &OutRestrictedItems) override;
        bool GeneratePossibleValues(TArray<FString> &OutOptionStrings,
            TArray<FText> &OutToolTips,
            TArray<bool> &OutRestrictedItems,
            TArray<FText> *OutDisplayNames) override;
        void MarkHiddenByCustomization() override;
        void MarkResetToDefaultCustomized(bool bCustomized = true) override;
        void ClearResetToDefaultCustomized() override;
        bool IsFavorite() const override;
        bool IsCustomized() const override;
        bool IsResetToDefaultCustomized() const override;
        FString GeneratePathToProperty() const override;
        TSharedRef<SWidget> CreatePropertyNameWidget(const FText &NameOverride,
            const FText &ToolTipOverride,
            bool bDisplayResetToDefault,
            bool bDisplayText = true,
            bool bDisplayThumbnail = true) const override;
        TSharedRef<SWidget> CreatePropertyNameWidget(const FText &NameOverride = FText::GetEmpty(),
            const FText &ToolTipOverride = FText::GetEmpty()) const override;
        TSharedRef<SWidget> CreatePropertyValueWidget(bool bDisplayDefaultPropertyButtons = true) const override;
        TSharedRef<SWidget> CreatePropertyValueWidgetWithCustomization(const IDetailsView *DetailsView) override;
        TSharedRef<SWidget> CreateDefaultPropertyButtonWidgets() const override;
        void CreateDefaultPropertyCopyPasteActions(FUIAction &OutCopyAction, FUIAction &OutPasteAction) const override;
        void AddRestriction(TSharedRef<const FPropertyRestriction> Restriction) override;
        bool IsRestricted(const FString &Value) const override;
        bool IsRestricted(const FString &Value, TArray<FText> &OutReasons) const override;
        bool GenerateRestrictionToolTip(const FString &Value, FText &OutTooltip) const override;
        bool IsDisabled(const FString &Value) const override;
        bool IsDisabled(const FString &Value, TArray<FText> &OutReasons) const override;
        bool IsHidden(const FString &Value) const override;
        bool IsHidden(const FString &Value, TArray<FText> &OutReasons) const override;
        void SetIgnoreValidation(bool bInIgnore) override;
        TArray<TSharedPtr<IPropertyHandle>> AddChildStructure(TSharedRef<FStructOnScope> ChildStructure) override;
        TArray<TSharedPtr<IPropertyHandle>>
        AddChildStructure(TSharedRef<IStructureDataProvider> ChildStructure) override;
        void RemoveChildren() override;
        bool CanResetToDefault() const override;
        void ExecuteCustomResetToDefault(const FResetToDefaultOverride &OnCustomResetToDefault) override;
        FName GetDefaultCategoryName() const override;
        FText GetDefaultCategoryText() const override;
        bool IsCategoryHandle() const override;

    };
}