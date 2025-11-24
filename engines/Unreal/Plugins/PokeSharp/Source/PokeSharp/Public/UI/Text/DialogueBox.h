// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonRichTextBlock.h"
#include "CommonUserWidget.h"

#include "DialogueBox.generated.h"

class FRichTextLayoutMarshaller;
class FSlateTextLayout;
/**
 * A text block that exposes more information about text layout.
 */
UCLASS()
class UDialogueTextBlock : public UCommonRichTextBlock
{
    GENERATED_BODY()

  public:
    FORCEINLINE TSharedPtr<FSlateTextLayout> GetTextLayout() const
    {
        return TextLayout;
    }

    FORCEINLINE TSharedPtr<FRichTextLayoutMarshaller> GetTextMarshaller() const
    {
        return TextMarshaller;
    }

  protected:
    TSharedRef<SWidget> RebuildWidget() override;

  private:
    TSharedPtr<FSlateTextLayout> TextLayout;
    TSharedPtr<FRichTextLayoutMarshaller> TextMarshaller;
};

struct FDialogueTextSegment
{
    FString Text;
    FRunInfo RunInfo;
};

/**
 *
 */
UCLASS()
class POKESHARP_API UDialogueBox : public UCommonUserWidget
{
    GENERATED_BODY()
  public:
    UDialogueTextBlock *GetLineText() const
    {
        return LineText.Get();
    }

    float GetLetterPlayTime() const
    {
        return LetterPlayTime;
    }

    void SetLetterPlayTime(const float NewTime)
    {
        LetterPlayTime = NewTime;
    }

    float GetEndHoldTime() const
    {
        return EndHoldTime;
    }

    void SetEndHoldTime(const float NewTime)
    {
        EndHoldTime = NewTime;
    }

    UFUNCTION(BlueprintCallable, Category = "Dialogue Box")
    void PlayLine(const FText &InLine);

    UFUNCTION(BlueprintCallable, Category = "Dialogue Box")
    void GetCurrentLine(FText &OutLine) const
    {
        OutLine = CurrentLine;
    }

    UFUNCTION(BlueprintCallable, Category = "Dialogue Box")
    bool HasFinishedPlayingLine() const
    {
        return bHasFinishedPlaying;
    }

    UFUNCTION(BlueprintCallable, Category = "Dialogue Box")
    void SkipToLineEnd();

    FDelegateHandle BindToOnLineFinishedPlaying(FSimpleDelegate &&Delegate)
    {
        return OnLineFinishedPlayingDelegate.Add(MoveTemp(Delegate));
    }

    bool UnbindFromOnLineFinishedPlaying(const FDelegateHandle Handle)
    {
        return OnLineFinishedPlayingDelegate.Remove(Handle);
    }

  protected:
    UFUNCTION(BlueprintImplementableEvent, Category = "Dialogue Box")
    void OnPlayLetter();

    UFUNCTION(BlueprintImplementableEvent, Category = "Dialogue Box")
    void OnLineFinishedPlaying();

  private:
    void PlayNextLetter();

    void CalculateWrappedString();
    FString CalculateSegments();

    UPROPERTY(BlueprintReadOnly, Getter, meta = (BindWidget, AllowPrivateAccess = "true"))
    TObjectPtr<UDialogueTextBlock> LineText;

    // The amount of time between printing individual letters (for the "typewriter" effect).
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Getter, Setter, Category = "Dialogue Box",
              meta = (AllowPrivateAccess = "true"))
    float LetterPlayTime = 0.025f;

    // The amount of time to wait after finishing the line before actually marking it completed.
    // This helps prevent accidentally progressing dialogue on short lines.
    UPROPERTY(EditAnywhere, BlueprintReadWrite, Getter, Setter, Category = "Dialogue Box",
              meta = (AllowPrivateAccess = "true"))
    float EndHoldTime = 0.15f;

    UPROPERTY()
    FText CurrentLine;

    TArray<FDialogueTextSegment> Segments;

    // The section of the text that's already been printed out and won't ever change.
    // This lets us cache some of the work we've already done. We can't cache absolutely
    // everything as the last few characters of a string may change if they're related to
    // a named run that hasn't been completed yet.
    FString CachedSegmentText;
    int32 CachedLetterIndex = 0;

    int32 CurrentSegmentIndex = 0;
    int32 CurrentLetterIndex = 0;
    int32 MaxLetterIndex = 0;

    uint32 bHasFinishedPlaying : 1 = true;

    FTimerHandle LetterTimer;

    FSimpleMulticastDelegate OnLineFinishedPlayingDelegate;
};
