using com.ricoh.livestreaming;
using com.ricoh.livestreaming.webrtc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteSample : BehaviorBase
{
    [SerializeField]
    private DropdownMuteType audioMuteDropdown = null;
    [SerializeField]
    private DropdownMuteType videoMuteDropdown = null;
    [SerializeField]
    private Text remoteAudioMuteStats = null;
    [SerializeField]
    private Text remoteVideoMuteStats = null;

    public override string ClientId => "";
    public override string ClientSecret => "";
    public override MuteType AudioMuteType => audioMuteDropdown.Type;
    public override MuteType VideoMuteType => videoMuteDropdown.Type;

    public void Start()
    {
        Logger.Debug("Start MuteSample.");

        InitializeClient(new ClientListener(this));
        InitializeDropdown();
    }

    /// <summary>
    /// Mute Dropdown Çèâä˙âªÇ∑ÇÈ
    /// </summary>
    private void InitializeDropdown()
    {
        audioMuteDropdown.Initialize(OnAudioMuteDropdownValueChanged);
        videoMuteDropdown.Initialize(OnVideomuteDropdownValueChanged);
    }

    private void OnAudioMuteDropdownValueChanged(MuteType muteType)
    {
        var track = GetLSTrack(MediaStreamTrack.TrackType.Audio);
        if (track != null)
        {
            client.ChangeMute(track, muteType);
        }
    }

    private void OnVideomuteDropdownValueChanged(MuteType muteType)
    {
        var track = GetLSTrack(MediaStreamTrack.TrackType.Video);
        if (track != null)
        {
            client.ChangeMute(track, muteType);
        }
    }

    private void SetShowingRemoteAudioMuteStats(string value)
    {
        UnityUIContext.Post(__ =>
        {
            remoteAudioMuteStats.text = string.IsNullOrEmpty(value) ? "" : $"Update Audio Track Mute : {value}";
        }, null);
    }

    private void SetShowingRemoteVideoMuteStats(string value)
    {
        UnityUIContext.Post(__ =>
        {
            remoteVideoMuteStats.text = string.IsNullOrEmpty(value) ? "" : $"Update Video Track Mute : {value}";
        }, null);
    }

    private void ClearRemoteMuteStats()
    {
        SetShowingRemoteAudioMuteStats("");
        SetShowingRemoteVideoMuteStats("");
    }

    protected override void ShowRemoteView(string connectionId, VideoTrack videoTrack)
    {
        base.ShowRemoteView(connectionId, videoTrack);

        ClearRemoteMuteStats();
    }

    protected override void Connect(string _)
    {
        base.Connect("WinUnityAPISamplesMute");
    }

    private class ClientListener : ClientListenerBase
    {
        private new readonly MuteSample app;

        public ClientListener(MuteSample app) : base(app)
        {
            this.app = app;
        }
        public override void OnClosing(LSClosingEvent lSClosingEvent)
        {
            base.OnClosing(lSClosingEvent);
            app.ClearRemoteMuteStats();
        }

        public override void OnRemoveRemoteConnection(LSRemoveRemoteConnectionEvent lSRemoveRemoteConnectionEvent)
        {
            base.OnRemoveRemoteConnection(lSRemoveRemoteConnectionEvent);

            if (app.RemoteConnections.IsEmpty)
            {
                app.ClearRemoteMuteStats();
            }
        }

        public override void OnUpdateMute(LSUpdateMuteEvent lSUpdateMuteEvent)
        {
            var connectionId = lSUpdateMuteEvent.ConnectionId;
            var mediaStreamTrack = lSUpdateMuteEvent.MediaStreamTrack;
            var muteType = lSUpdateMuteEvent.MuteType;

            base.OnUpdateMute(lSUpdateMuteEvent);

            if (app.RemoteVideoRender?.ConnectionId == connectionId)
            {
                if (mediaStreamTrack.Type == MediaStreamTrack.TrackType.Audio)
                {
                    app.SetShowingRemoteAudioMuteStats(muteType.ToString());
                }
                else if (mediaStreamTrack.Type == MediaStreamTrack.TrackType.Video)
                {
                    app.SetShowingRemoteVideoMuteStats(muteType.ToString());
                }
                else
                {
                    // nothing to do.
                }
            }
        }
    }
}
