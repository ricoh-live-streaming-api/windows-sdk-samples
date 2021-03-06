using com.ricoh.livestreaming.webrtc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class MetaSample : BehaviorBase
{
    [SerializeField]
    private InputField connectionMeta = null;
    [SerializeField]
    private InputField audioTrackMeta = null;
    [SerializeField]
    private InputField videoTrackMeta = null;
    [SerializeField]
    private Text remoteConnectionMeta = null;
    [SerializeField]
    private Text remoteAudioTrackMeta = null;
    [SerializeField]
    private Text remoteVideoTrackMeta = null;

    public override string ClientId => "";
    public override string ClientSecret => "";
    public override Dictionary<string, object> ConnectionMetadata => JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionMeta.text);
    public override Dictionary<string, object> AudioTrackMetadata => JsonConvert.DeserializeObject<Dictionary<string, object>>(audioTrackMeta.text);
    public override Dictionary<string, object> VideoTrackMetadata => JsonConvert.DeserializeObject<Dictionary<string, object>>(videoTrackMeta.text);

    public void Start()
    {
        Logger.Debug("Start MetaSample.");

        connectionMeta.text = "{\"user_name\":\"Alice\"}";
        audioTrackMeta.text = "{\"device_name\":\"Center Mic\"}";
        videoTrackMeta.text = "{\"device_name\":\"Main Camera\"}";

        InitializeClient(new ClientListener(this));
    }

    public void OnUpdateConnectMetaButtonClick()
    {
        client.UpdateMeta(new ReadOnlyDictionary<string, object>(ConnectionMetadata));
    }

    public void OnUpdateAudioTrackMetaButtonClick()
    {
        client.UpdateTrackMeta(
            GetLSTrack(MediaStreamTrack.TrackType.Audio),
            new ReadOnlyDictionary<string, object>(AudioTrackMetadata));
    }

    public void OnUpdateVideoTrackMetaButtonClick()
    {
        client.UpdateTrackMeta(
            GetLSTrack(MediaStreamTrack.TrackType.Video),
            new ReadOnlyDictionary<string, object>(VideoTrackMetadata));
    }

    private void SetShowingRemoteConnectionMetadata(string value)
    {
        UnityUIContext.Post(__ =>
        {
            remoteConnectionMeta.text = string.IsNullOrEmpty(value) ? "" : $"Update Connection Metadata : {value}";
        }, null);
    }

    private void SetShowingRemoteAudioTrackMetadata(string value)
    {
        UnityUIContext.Post(__ =>
        {
            remoteAudioTrackMeta.text = string.IsNullOrEmpty(value) ? "" : $"Update Audio Track Metadata : {value}";
        }, null);
    }

    private void SetShowingRemoteVideoTrackMetadata(string value)
    {
        UnityUIContext.Post(__ =>
        {
            remoteVideoTrackMeta.text = string.IsNullOrEmpty(value) ? "" : $"Update Video Track Metadata : {value}";
        }, null);
    }

    private void ClearRemoteMetadata()
    {
        SetShowingRemoteConnectionMetadata("");
        SetShowingRemoteAudioTrackMetadata("");
        SetShowingRemoteVideoTrackMetadata("");
    }

    protected override void ShowRemoteView(string connectionId, VideoTrack videoTrack)
    {
        base.ShowRemoteView(connectionId, videoTrack);

        ClearRemoteMetadata();
    }

    private class ClientListener : ClientListenerBase
    {
        private new readonly MetaSample app;

        public ClientListener(MetaSample app) : base(app)
        {
            this.app = app;
        }

        public override void OnClosing()
        {
            base.OnClosing();
            app.ClearRemoteMetadata();
        }

        public override void OnRemoveRemoteConnection(string connectionId, Dictionary<string, object> metadata, List<MediaStreamTrack> mediaStreamTracks)
        {
            base.OnRemoveRemoteConnection(connectionId, metadata, mediaStreamTracks);

            if (app.RemoteConnections.IsEmpty)
            {
                app.ClearRemoteMetadata();
            }
        }

        public override void OnUpdateRemoteTrack(string connectionId, MediaStream stream, MediaStreamTrack mediaStreamTrack, Dictionary<string, object> metadata)
        {
            base.OnUpdateRemoteTrack(connectionId, stream, mediaStreamTrack, metadata);

            if (app.RemoteVideoRender?.ConnectionId == connectionId)
            {
                if (mediaStreamTrack.Type == MediaStreamTrack.TrackType.Audio)
                {
                    app.SetShowingRemoteAudioTrackMetadata(JsonConvert.SerializeObject(metadata));
                }
                else if (mediaStreamTrack.Type == MediaStreamTrack.TrackType.Video)
                {
                    app.SetShowingRemoteVideoTrackMetadata(JsonConvert.SerializeObject(metadata));
                }
                else
                {
                    // nothing to do.
                }
            }
        }

        public override void OnUpdateRemoteConnection(string connectionId, Dictionary<string, object> metadata)
        {
            base.OnUpdateRemoteConnection(connectionId, metadata);

            if (app.RemoteVideoRender?.ConnectionId == connectionId)
            {
                app.SetShowingRemoteConnectionMetadata(JsonConvert.SerializeObject(metadata));
            }
        }
    }
}
