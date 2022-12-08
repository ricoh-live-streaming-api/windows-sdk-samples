// Copyright 2022 RICOH Company, Ltd. All rights reserved.

#include "AccessToken.h"

using json = nlohmann::json;
using traits = jwt::traits::nlohmann_json;
using claim = jwt::basic_claim<traits>;

FString UAccessToken::Create(const FString& clintSecret, const FString& roomId, const FString& roomType)
{
    const auto nbf = std::chrono::system_clock::now() - std::chrono::minutes{ 30 };
    const auto exp = nbf + std::chrono::hours{ 1 };

    auto connection_id = FString("WinUESampleUEApp").Append(FGuid::NewGuid().ToString());

    json media_control = json::object();
    media_control.push_back({ "bitrate_reservation_mbps", 25 });

    json room_spec = json::object();
    room_spec.push_back({ "type", TCHAR_TO_ANSI(*roomType) });
    room_spec.push_back({ "media_control", media_control });

    auto builder = jwt::create<traits>()
        .set_type("JWT")
        .set_not_before(nbf)
        .set_expires_at(exp)
        .set_payload_claim("connection_id", TCHAR_TO_ANSI(*connection_id))
        .set_payload_claim("room_id", TCHAR_TO_ANSI(*roomId))
        .set_payload_claim("room_spec", claim(room_spec));

    auto token = builder.sign(jwt::algorithm::hs256{ TCHAR_TO_ANSI(*clintSecret) });

    return token.c_str();
}
