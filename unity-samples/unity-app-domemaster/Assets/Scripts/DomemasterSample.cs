using com.ricoh.livestreaming;
using com.ricoh.livestreaming.webrtc;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class DomemasterSample : BehaviorBase
{
    public override string ClientId => "";
    public override string ClientSecret => "";
    private int BitrateReservationMbps => 25;

    public void Start()
    {
        Logger.Debug("Start Domemaster Sample.");
        InitializeClient(new ClientListener(this));
    }

    public override void Update()
    {
        RemoteVideoRender?.Draw();
    }

    public void OnApplicationFinishButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    protected override void Connect()
    {
        _ = Task.Run(() =>
        {
            lock (clientLockObject)
            {
                try
                {
                    SetConnectButtonText("Connecting...", false);

                    var accessToken = JwtAccessToken.CreateAccessToken(
                        ClientSecret,
                        RoomId,
                        new RoomSpec(RoomSpec.Type.Sfu, BitrateReservationMbps));

                    var videoCodec = CodecUtil.IsH264Supported()
                        ? SendingVideoOption.VideoCodecType.H264
                        : SendingVideoOption.VideoCodecType.Vp8;

                    var sendingVideoOption = new SendingVideoOption()
                        .SetCodec(videoCodec);

                    var option = new Option()
                        .SetSendingOption(new SendingOption(sendingVideoOption, false));

                    client.Connect(ClientId, accessToken, option);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to Connect.", e);
                    SetConnectButtonText("Connect", true);
                }
            }
        });
    }

    private class ClientListener : ClientListenerBase
    {
        public ClientListener(DomemasterSample app) : base(app)
        {
        }

        public override void OnAddLocalTrack(MediaStreamTrack mediaStreamTrack, MediaStream stream)
        {
            // LocalTrack の映像描画は省略
        }
    }
}