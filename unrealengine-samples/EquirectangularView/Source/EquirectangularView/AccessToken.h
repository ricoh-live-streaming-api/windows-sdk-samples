// Copyright 2022 RICOH Company, Ltd. All rights reserved.

#pragma once

#include "../ThirdParty/jwt-cpp/jwt_ue.h"
#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "AccessToken.generated.h"

UCLASS()
class EQUIRECTANGULARVIEW_API UAccessToken : public UBlueprintFunctionLibrary
{
    GENERATED_BODY()

public:
    UFUNCTION(BlueprintCallable, BlueprintPure, meta = (DisplayName = "Create Access Token"), Category = "SampleClient")
    static FString Create(const FString& clintSecret, const FString& roomId, const FString& roomType);
};
