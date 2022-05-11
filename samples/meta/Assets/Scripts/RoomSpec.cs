﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class RoomSpec
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Type
    {
        [EnumMember(Value = "sfu")]
        Sfu,
        [EnumMember(Value = "p2p")]
        P2p,
        [EnumMember(Value = "p2p_turn")]
        P2pTurn
    }

    private readonly Type type;
    private readonly int? bitrateReservationMbps;

    public RoomSpec(Type type = Type.Sfu, int? bitrateReservationMbps = null)
    {
        this.type = type;
        this.bitrateReservationMbps = bitrateReservationMbps;
    }

    public Dictionary<string, object> GetSpec()
    {
        var dic = new Dictionary<string, object>
        {
            ["type"] = type
        };

        if (bitrateReservationMbps.HasValue)
        {
            dic.Add("media_control", new Dictionary<string, object>() { ["bitrate_reservation_mbps"] = bitrateReservationMbps });
        }

        return dic;
    }
}
