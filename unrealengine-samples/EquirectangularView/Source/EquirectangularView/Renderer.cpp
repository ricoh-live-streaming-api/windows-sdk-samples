// Copyright 2022 RICOH Company, Ltd. All rights reserved.

#include "Renderer.h"

// Sets default values
ARenderer::ARenderer()
{
    // Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
    PrimaryActorTick.bCanEverTick = true;

    _region.SrcX = 0;
    _region.DestX = 0;
    _region.Height = 0;
    _region.SrcY = 0;
    _region.DestY = 0;
    _region.Width = 0;
}

// Called when the game starts or when spawned
void ARenderer::BeginPlay()
{
    _renderer = NewObject<ULSByteArrayRenderer>(this);
     Super::BeginPlay();
}

// Called every frame
void ARenderer::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void ARenderer::Initialize(ULSVideoTrack* videoTrack, FString connectionId)
{
    _videoTrack = videoTrack->Get();
}

UTexture2D* ARenderer::SetFrameSize(int width, int height)
{
    UTexture2D* renderTexture = UTexture2D::CreateTransient(width, height, PF_R8G8B8A8);

    auto rawData = (BYTE*)renderTexture->GetPlatformData()->Mips[0].BulkData.Lock(LOCK_READ_WRITE);
    size_t rawDataSize = static_cast<size_t>(width) * 4 * height;
    FMemory::Memset(rawData, 0xFF, rawDataSize);
    renderTexture->GetPlatformData()->Mips[0].BulkData.Unlock();
    renderTexture->UpdateResource();

    if (_renderTexture)
    {
        _renderTexture->ReleaseResource();
        _renderTexture = nullptr;
    }

    _renderTexture = renderTexture;
    _width = width;
    _height = height;

    _region.Width = (uint32)width;
    _region.Height = (uint32)height;

    _imageBuffer.Init(0, width * 4 * height);
    _imageBufferPtr = (uint8*)_imageBuffer.GetData();

    return _renderTexture;
}

void ARenderer::Render()
{
    if (!_videoTrack)
    {
        return;
    }

    if (_width <= 0 || _height <= 0)
    {
        ULSLogger::Error(FString::Format(TEXT("Render size is invalid.(%d x %d)"), { _width, _height }));
        return;
    }

    if (!_renderTexture)
    {
        ULSLogger::Error(TEXT("Texture is not ready."));
        return;
    }

    _renderer->WritePixelData(_imageBufferPtr, _width, _height, _videoTrack);

    // 更新用のリージョンを最新化
    _update = _region;
    const FUpdateTextureRegion2D* region = &(_update);

    // テクスチャを更新（非同期）
    _renderTexture->UpdateTextureRegions(0, 1, region, 4 * _width, 4, _imageBufferPtr);
}
