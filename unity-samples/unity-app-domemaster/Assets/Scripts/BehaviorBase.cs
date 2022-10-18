using com.ricoh.livestreaming;
using com.ricoh.livestreaming.webrtc;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class BehaviorBase : MonoBehaviour
{
    [SerializeField]
    private InputField roomIDEdit = null;
    [SerializeField]
    private GameObject connectButton = null;
    [SerializeField]
    private GameObject localRenderTarget = null;
    [SerializeField]
    private GameObject remoteRenderTarget = null;

    #region Connect で使用するパラメータ
    public virtual VideoCapturer VideoCapturer { get; protected set; }
    public virtual bool IsAudioEnabled { get; protected set; }
    public virtual string ClientId { get; protected set; }
    public virtual string ClientSecret { get; protected set; }
    public virtual string RoomId => roomIDEdit.text;
    public virtual Dictionary<string, object> ConnectionMetadata { get; protected set; } = new Dictionary<string, object>() { { "connection_metadata_sample", "connection_metadata_default" } };
    public virtual Dictionary<string, object> AudioTrackMetadata { get; protected set; } = new Dictionary<string, object>() { { "audio_track_metadata_sample", "audio_track_metadata_default" } };
    public virtual Dictionary<string, object> VideoTrackMetadata { get; protected set; } = new Dictionary<string, object>() { { "video_track_metadata_sample", "video_track_metadata_default" } };
    public virtual MuteType AudioMuteType { get; protected set; } = MuteType.Unmute;
    public virtual MuteType VideoMuteType { get; protected set; } = MuteType.Unmute;
    public virtual int MaxBitrateKbps => 500;   // priority:normal
    #endregion

    public bool IsConnected => client.GetState() != ConnectionState.Idle;

    protected static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected readonly object clientLockObject = new object();
    protected SynchronizationContext UnityUIContext;
    protected Client client;
    private readonly List<LSTrack> localLSTracks = new List<LSTrack>();

    protected VideoRender LocalVideoRender;
    protected VideoRender RemoteVideoRender;
    protected readonly ConcurrentDictionary<string, VideoTrack> RemoteConnections = new ConcurrentDictionary<string, VideoTrack>();

    public virtual void Awake()
    {
        UnityUIContext = SynchronizationContext.Current;
    }

    public virtual void Update()
    {
        // LocalTrack の映像表示
        LocalVideoRender?.Draw();

        // RemoteTrack の映像表示処理
        RemoteVideoRender?.Draw();
    }

    /// <summary>
    /// <see cref="Client"/>を新しく生成し、初期化処理を行う
    /// </summary>
    /// <param name="listener">クライアントに設定するリスナー</param>
    protected void InitializeClient(IClientListener listener)
    {
        client = new Client();
        client.SetEventListener(listener);
    }

    /// <summary>
    /// <see cref="VideoCapturer"/> 等の Connect に必要な各種デバイスを設定する<br/>
    /// <see cref="Connect"/> 内で呼び出される
    /// </summary>
    protected virtual void SetDevice()
    {
        // デフォルトでは取得したリストの先頭デバイスを使用する。
        // 任意のデバイスを設定したい場合は、本メソッドをオーバーライドする。

        var videoDevice = DeviceUtil.GetVideoCapturerDeviceList().FirstOrDefault();
        if (videoDevice != null)
        {
            var capability = DeviceUtil.GetVideoCapturerDeviceCapabilities(videoDevice.DeviceName).FirstOrDefault();
            VideoCapturer = new VideoDeviceCapturer(videoDevice.DeviceName, capability.Width, capability.Height, capability.FrameRate);
            Logger.Debug($"videoDevice={videoDevice.DeviceName}, width={capability.Width}, height={capability.Height}, frameRate={capability.FrameRate}");
        }

        var audioInputDevice = DeviceUtil.GetAudioInputDeviceList().FirstOrDefault();
        if (audioInputDevice != null)
        {
            client.SetAudioInputDevice(audioInputDevice.DeviceName);
            Logger.Debug($"audioInputDevice={audioInputDevice.DeviceName}");
            IsAudioEnabled = true;
        }

        var audioOutputDevice = DeviceUtil.GetAudioOutputDeviceList().FirstOrDefault();
        if (audioOutputDevice != null)
        {
            client.SetAudioOutputDevice(audioOutputDevice.DeviceName);
            Logger.Debug($"audioOutputDevice={audioOutputDevice.DeviceName}");
            IsAudioEnabled = true;
        }
    }

    /// <summary>
    /// <see cref="Client"/>の切断と破棄を行う。
    /// </summary>
    protected void TerminateClient()
    {
        if (client == null)
        {
            return;
        }

        lock (clientLockObject)
        {
            client.Disconnect();

            _ = Task.Run(async () =>
            {
                if (client.GetState() != ConnectionState.Idle)
                {
                    while (client.GetState() != ConnectionState.Closed)
                    {
                        await Task.Delay(15);
                    }
                }
            }).Wait(100);

            client.Dispose();
            client = null;
        }
    }

    /// <summary>
    /// Client接続処理
    /// </summary>
    protected virtual void Connect(string connectionId = "")
    {
        _ = Task.Run(() =>
        {
            lock (clientLockObject)
            {
                try
                {
                    SetConnectButtonText("Connecting...", false);

                    SetDevice();

                    var accessToken = JwtAccessToken.CreateAccessToken(ClientSecret, RoomId, connectionId, new RoomSpec());

                    var constraints = new MediaStreamConstraints()
                        .SetAudio(IsAudioEnabled);

                    if (VideoCapturer != null)
                    {
                        constraints.SetVideoCapturer(VideoCapturer);
                    }

                    var mediaStream = client.GetUserMedia(constraints);

                    foreach (var track in mediaStream.GetAudioTracks())
                    {
                        var trackOption = new LSTrackOption()
                            .SetMeta(AudioTrackMetadata)
                            .SetMuteType(AudioMuteType);

                        localLSTracks.Add(new LSTrack(track, mediaStream, trackOption));
                    }

                    foreach (var track in mediaStream.GetVideoTracks())
                    {
                        var trackOption = new LSTrackOption()
                            .SetMeta(VideoTrackMetadata)
                            .SetMuteType(VideoMuteType);

                        localLSTracks.Add(new LSTrack(track, mediaStream, trackOption));
                    }

                    var videoCodec = CodecUtil.IsH264Supported()
                        ? SendingVideoOption.VideoCodecType.H264
                        : SendingVideoOption.VideoCodecType.Vp8;

                    var sendingVideoOption = new SendingVideoOption()
                        .SetCodec(videoCodec)
                        .SetMaxBitrateKbps(MaxBitrateKbps);

                    var option = new Option()
                        .SetLocalLSTracks(localLSTracks)
                        .SetMeta(ConnectionMetadata)
                        .SetSendingOption(new SendingOption(sendingVideoOption));

                    client.Connect(ClientId, accessToken, option);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to Connect.", e);
                    SetConnectButtonText("Connect", true);

                    VideoCapturer?.Release();
                    localLSTracks.Clear();
                }
            }
        });
    }

    /// <summary>
    /// Client切断処理
    /// </summary>
    protected void Disconnect()
    {
        _ = Task.Run(() =>
        {
            lock (clientLockObject)
            {
                try
                {
                    SetConnectButtonText("Disconnecting...", false);
                    client.Disconnect();
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to Disconnect.", e);
                    SetConnectButtonText("Connect", true);
                }
            }
        });
    }

    /// <summary>
    /// アプリ終了時に呼び出されるUnityのコールバック
    /// </summary>
    public virtual void OnApplicationQuit()
    {
        Logger.Debug("Quit Sample.");
        TerminateClient();
    }

    /// <summary>
    /// Connectボタン押下イベント
    /// </summary>
    public void OnConnectButtonClick()
    {
        if (!IsConnected)
        {
            Connect();
        }
        else
        {
            Disconnect();
        }
    }

    /// <summary>
    /// Connect ボタンに表示する文字列を設定する
    /// </summary>
    /// <param name="buttonText">表示文字列</param>
    /// <param name="interactable">true : ボタン操作可, false : ボタン操作不可</param>
    protected void SetConnectButtonText(string buttonText, bool interactable)
    {
        UnityUIContext.Post(__ =>
        {
            var button = connectButton.GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = buttonText;
            button.interactable = interactable;
        }, null);
    }

    /// <summary>
    /// <see cref="localLSTracks"/> から指定する TrackType の LSTrack を取得する
    /// </summary>
    /// <param name="trackType"><see cref="MediaStreamTrack.TrackType"/></param>
    /// <returns>成功 : <see cref="LSTrack"/>, 失敗 : null</returns>
    protected LSTrack GetLSTrack(MediaStreamTrack.TrackType trackType)
    {
        return localLSTracks.Find(item => item.MediaStreamTrack.Type == trackType);
    }

    /// <summary>
    /// RemoteView の表示
    /// </summary>
    /// <param name="connectionId">映像描画元の Connection ID</param>
    /// <param name="videoTrack">映像描画元の VideoTrack</param>
    protected virtual void ShowRemoteView(string connectionId, VideoTrack videoTrack)
    {
        // 表示中の RemoteTrack の映像描画を停止する
        RemoteVideoRender?.Dispose();

        // 新たな RemoteTrack の映像描画を開始する
        RemoteVideoRender = new VideoRender(UnityUIContext, remoteRenderTarget, videoTrack, connectionId);
    }

    protected abstract class ClientListenerBase : IClientListener
    {
        protected readonly BehaviorBase app;

        public ClientListenerBase(BehaviorBase app)
        {
            this.app = app;
        }

        public virtual void OnAddLocalTrack(LSAddLocalTrackEvent lSAddLocalTrackEvent)
        {
            var mediaStreamTrack = lSAddLocalTrackEvent.MediaStreamTrack;
            var stream = lSAddLocalTrackEvent.Stream;

            Logger.Debug($"streamId={stream.Id}, trackType={mediaStreamTrack.Type}, trackId={mediaStreamTrack.Id}");

            if (mediaStreamTrack is VideoTrack videoTrack)
            {
                // LocalTrack の映像描画を開始する
                app.LocalVideoRender = new VideoRender(app.UnityUIContext, app.localRenderTarget, videoTrack);
            }
        }

        public virtual void OnAddRemoteConnection(LSAddRemoteConnectionEvent lSAddRemoteConnectionEvent)
        {
        }

        public virtual void OnAddRemoteTrack(LSAddRemoteTrackEvent lSAddRemoteTrackEvent)
        {
            var connectionId = lSAddRemoteTrackEvent.ConnectionId;
            var stream = lSAddRemoteTrackEvent.Stream;
            var mediaStreamTrack = lSAddRemoteTrackEvent.MediaStreamTrack;
            var metadata = lSAddRemoteTrackEvent.Metadata;
            var muteType = lSAddRemoteTrackEvent.MuteType;

            Logger.Debug($"connectionId={connectionId}, streamId={stream.Id}, trackType={mediaStreamTrack.Type}, trackId={mediaStreamTrack.Id}, muteType={muteType}");

            if (mediaStreamTrack is VideoTrack videoTrack)
            {
                // RemoteTrack の映像描画を開始する。
                // デフォルトでは最後に接続された RemoteTrack のみを描画している。
                // 複数の RemoteTrack を同時に描画する場合、"app.remoteRenderTarget" のインスタンスを RemoteTrack 数分用意する。

                app.RemoteConnections.AddOrUpdate(connectionId, videoTrack, (k, v) => videoTrack);
                app.ShowRemoteView(connectionId, videoTrack);
            }
        }

        public virtual void OnClosed(LSCloseEvent lSCloseEvent)
        {
            app.UnityUIContext.Post(__ =>
            {
                app.VideoCapturer?.Release();

                // 切断した場合、Client を再生成する
                app.TerminateClient();
                app.InitializeClient(this);
            }, null);

            app.localLSTracks.Clear();
            app.SetConnectButtonText("Connect", true);
        }

        public virtual void OnClosing(LSClosingEvent lSClosingEvent)
        {
            app.SetConnectButtonText("Disconnecting...", false);

            // 切断完了を待たずに映像描画を停止する
            app.LocalVideoRender?.Dispose();
            app.RemoteVideoRender?.Dispose();
            app.RemoteConnections.Clear();
        }

        public virtual void OnConnecting(LSConnectingEvent lSConnectingEvent)
        {
        }

        public virtual void OnError(SDKErrorEvent error)
        {
            Logger.Debug($"{error.ToReportString()}");
        }

        public virtual void OnOpen(LSOpenEvent lSOpenEvent)
        {
            app.SetConnectButtonText("Disconnect", true);
        }

        public virtual void OnRemoveRemoteConnection(LSRemoveRemoteConnectionEvent lSRemoveRemoteConnectionEvent)
        {
            var connectionId = lSRemoveRemoteConnectionEvent.ConnectionId;

            Logger.Debug($"connectionId={connectionId}");

            if (app.RemoteConnections.TryRemove(connectionId, out var _))
            {
                if (app.RemoteConnections.IsEmpty)
                {
                    // 接続相手が居なければ RemoteTrack の映像描画を停止する
                    app.RemoteVideoRender.Dispose();
                }
                else
                {
                    if (app.RemoteVideoRender.ConnectionId == connectionId)
                    {
                        // 表示可能な RemoteTrack が存在する場合は描画を再開する
                        var remoteConnection = app.RemoteConnections.LastOrDefault();
                        app.ShowRemoteView(remoteConnection.Key, remoteConnection.Value);
                    }
                }
            }
        }

        public virtual void OnUpdateRemoteTrack(LSUpdateRemoteTrackEvent lSUpdateRemoteTrackEvent)
        {
            var connectionId = lSUpdateRemoteTrackEvent.ConnectionId;
            var stream = lSUpdateRemoteTrackEvent.Stream;
            var mediaStreamTrack = lSUpdateRemoteTrackEvent.MediaStreamTrack;

            Logger.Debug($"connectionId={connectionId}, streamId={stream.Id}, trackType={mediaStreamTrack.Type}, trackId={mediaStreamTrack.Id}");
        }

        public virtual void OnUpdateMute(LSUpdateMuteEvent lSUpdateMuteEvent)
        {
            var connectionId = lSUpdateMuteEvent.ConnectionId;
            var stream = lSUpdateMuteEvent.Stream;
            var mediaStreamTrack = lSUpdateMuteEvent.MediaStreamTrack;
            var muteType = lSUpdateMuteEvent.MuteType;

            Logger.Debug($"connectionId={connectionId}, streamId={stream.Id}, trackType={mediaStreamTrack.Type}, trackId={mediaStreamTrack.Id}, muteType={muteType}");
        }

        public virtual void OnUpdateRemoteConnection(LSUpdateRemoteConnectionEvent lSUpdateRemoteConnectionEvent)
        {
            var connectionId = lSUpdateRemoteConnectionEvent.ConnectionId;

            Logger.Debug($"connectionId={connectionId}");
        }

        public virtual void OnChangeStability(LSChangeStabilityEvent lSChangeStabilityEvent)
        {
            var connectionId = lSChangeStabilityEvent.ConnectionId;
            var stability = lSChangeStabilityEvent.Stability;

            Logger.Debug($"connectionId={connectionId}, stability={stability}");
        }
    }
}