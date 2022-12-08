// Copyright 2022 RICOH Company, Ltd. All rights reserved.

#pragma once

#include "LiveStreaming_ClientSDK.h"
#include "LSLogger.h"
#include "LSByteArrayRenderer.h"
#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Renderer.generated.h"

UCLASS(BlueprintType, Blueprintable)
class EQUIRECTANGULARVIEW_API ARenderer : public AActor
{
    GENERATED_BODY()

public:
    // Sets default values for this actor's properties
    ARenderer();

protected:
    // Called when the game starts or when spawned
    virtual void BeginPlay() override;

public:
    // Called every frame
    virtual void Tick(float DeltaTime) override;

private:
    UPROPERTY()
    UTexture2D* _renderTexture = nullptr;
    
    UPROPERTY()
    ULSByteArrayRenderer* _renderer = nullptr;
    
    VideoTrack* _videoTrack = nullptr;
    TArray<uint8> _imageBuffer;
    uint8* _imageBufferPtr = nullptr;
    FUpdateTextureRegion2D _region;
    FUpdateTextureRegion2D _update;

    int _width = 0;
    int _height = 0;

protected:
    UFUNCTION(BlueprintCallable, Category = "SampleClient")
    void Render();

    UFUNCTION(BlueprintCallable, Category = "SampleClient")
    void Initialize(ULSVideoTrack* videoTrack, FString connectionId);

    UFUNCTION(BlueprintCallable, Category = "SampleClient")
    UTexture2D* SetFrameSize(int width, int height);
};
