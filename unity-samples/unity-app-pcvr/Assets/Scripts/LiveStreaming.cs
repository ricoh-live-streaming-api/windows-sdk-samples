using com.ricoh.livestreaming;
using com.ricoh.livestreaming.unity;
using com.ricoh.livestreaming.webrtc;
using log4net;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LiveStreaming : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private GameObject roomIds;
    [SerializeField]
    private GameObject buttonConnect;
    [SerializeField]
    private GameObject equirectangularSphere;
    [SerializeField]
    private GameObject normalVideoPlane;
    [SerializeField]
    private GameObject alertDialog;
    [SerializeField]
    private Text alertDialogMessageText;
    [SerializeField]
    private Button alertDialogButton;
    [SerializeField]
    private GameObject keyboard;
    [SerializeField]
    private DropdownAudioInput audioInputDropdown;
    [SerializeField]
    private DropdownAudioOutput audioOutputDropdown;
    [SerializeField]
    private float sphereRotationThresholdX;
    [SerializeField]
    private float sphereRotationSpeed;

    private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private SynchronizationContext unityUIContext;
    private static object frameLock = new object();
    private static object lockObject = new object();
    private Client client;
    private VideoRenderManager videoRenderManager;
    private VideoTrack remoteVideoTrack;
    private UnityRenderer unityRenderer;
    private UserData userData;
    private string userDataFilePath;
    private string logFilePath;
    private Stats stats;
    private IntPtr hWnd;    // own window handle
    private WindowProcedureHook windowProcedureHook;
    private Secrets secrets;

    // Texture
    private Texture2D mainTex;
    private Texture2D texture;

    // default Quaternion of video spheres
    private Quaternion equirectangularSphereQuaternion;

    // Renderer
    private Renderer equirectangularRenderer;
    private Renderer normalVideoRenderer;
    private Renderer videoRenderer;

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpszClass, string lpszTitle);

    public void Awake()
    {
        userDataFilePath = Application.persistentDataPath + "/UserData.json";
        userData = UserDataSerializer.Load(userDataFilePath);
        logFilePath = Application.persistentDataPath + "/logs";
        unityUIContext = SynchronizationContext.Current;

        // Read configuration.
        try
        {
            secrets = Secrets.GetInstance();
        }
        catch (Exception e)
        {
            Logger.Error("Failed to read configuration.", e);

            SetKeyboardActive(false);
            SetMenuActive(false);

            ShowAlertDialog("ClientID/ClientSecretが設定されていません。\nSecrets.jsonを確認してください。", () =>
            {
                alertDialogButton.onClick.RemoveAllListeners();
                alertDialog.SetActive(false);
                Quit();
            });
        }

        // set previous value of RoomID
        // (get from Secrets when not saved)
        var roomId = roomIds.GetComponentInChildren<InputField>();
        roomId.text = (string.IsNullOrEmpty(userData.RoomID) ? secrets.RoomId : userData.RoomID);

        videoRenderManager = new VideoRenderManager(new VideoTrackListener(this))
        {
            OnChangeShowingTrack = OnChangeShowingTrack
        };
    }

    public void Start()
    {
        Logger.Debug("Start PCVR Sample.");

        // Get own window handle.
        hWnd = FindWindow(null, Application.productName);
        windowProcedureHook = new WindowProcedureHook(hWnd);
        windowProcedureHook.AddListener(new WindowProcedureHookListener(this));

        unityRenderer = new UnityRenderer();

        // video plane is not displayed at first
        equirectangularSphere.SetActive(false);
        normalVideoPlane.SetActive(false);
        equirectangularSphereQuaternion = equirectangularSphere.transform.localRotation;

        equirectangularRenderer = equirectangularSphere.GetComponent<Renderer>();
        normalVideoRenderer = normalVideoPlane.GetComponent<Renderer>();

        // setup Texture
        mainTex = new Texture2D(2, 2);
        mainTex.SetPixel(0, 0, Color.blue);
        mainTex.SetPixel(1, 1, Color.blue);
        mainTex.Apply();
        equirectangularRenderer.material.mainTexture = mainTex;
        normalVideoRenderer.material.mainTexture = mainTex;

        InitializeClient(new ClientListener(this));
        InitializeDevice();
        SetConfigWebrtcLog();
    }

    public void Update()
    {
        lock (frameLock)
        {
            if (remoteVideoTrack != null && videoRenderer != null)
            {
                unityRenderer.RenderToTexture(videoRenderer.material.mainTexture, remoteVideoTrack);
            }
        }
    }

    public void OnApplicationQuit()
    {
        Logger.Debug("OnApplicationQuit");

        UserDataSerializer.Save(userData, userDataFilePath);
        windowProcedureHook.Term();

        client.Disconnect();
        Task.Run(async () =>
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

        videoRenderManager.Clear();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            if (equirectangularSphere.activeSelf)
            {
                Vector2 inputMovement = context.ReadValue<Vector2>();

                if (Mathf.Abs(inputMovement.x) > sphereRotationThresholdX)
                {
                    equirectangularSphere.transform.Rotate(0, inputMovement.x * sphereRotationSpeed, 0);
                }
            }
        }
    }

    public void OnInitilizeRotation(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            if (equirectangularSphere.activeSelf)
            {
                equirectangularSphere.transform.localRotation = equirectangularSphereQuaternion;
            }
        }
    }

    public void OnToggleDisplay(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            if (videoRenderManager.GetNumOfRemoteTrack() > 1)
            {
                videoRenderManager.ToggleDisplay();
            }
        }
    }

    public void OnMenuActive(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            SetMenuActive(!menuCanvas.activeSelf);
        }
    }

    public void OnConnectButtonClick()
    {
        Logger.Debug("OnConnectButtonClick");

        if (client.GetState() == ConnectionState.Idle)
        {
            SetConnectButtonText("Connecting...");
            Connect();
        }
        else
        {
            SetConnectButtonText("Disconnecting...");
            Disconnect();
        }
    }

    public void OnKeyboardButtonClick()
    {
        SetKeyboardActive(!keyboard.activeSelf);
    }

    /// <summary>
    /// AudioInputDropdownの変更イベント
    /// </summary>
    /// <param name="deviceName">変更後のデバイス名</param>
    private void OnAudioInputDropdownValueChanged(string deviceName)
    {
        client.SetAudioInputDevice(deviceName);
    }

    /// <summary>
    /// AudioOutputDropdownの変更イベント
    /// </summary>
    /// <param name="deviceName">変更後のデバイス名</param>
    private void OnAudioOutputDropdownValueChanged(string deviceName)
    {
        client.SetAudioOutputDevice(deviceName);
    }

    private void Connect()
    {
        string roomId = roomIds.GetComponentInChildren<InputField>().text;

        Task.Run(() =>
        {
            lock (lockObject)
            {
                try
                {
                    bool isH264Supported = CodecUtil.IsH264Supported();
                    Logger.Debug("IsH264Supported = " + isH264Supported);

                    var roomSpec = new RoomSpec(RoomSpec.Type.Sfu);

                    var clientSecret = secrets.ClientSecret;

                    var accessToken = JwtAccessToken.CreateAccessToken(
                        clientSecret,
                        roomId,
                        roomSpec);

                    var connectionMeta = new Dictionary<string, object>()
                    {
                        { "example_connection_metadata", "connection" }
                    };

                    var audioMeta = new Dictionary<string, object>()
                    {
                        { "example_track_metadata", "audio" }
                    };

                    var constraints = new MediaStreamConstraints()
                        .SetAudio(audioInputDropdown.Exists | audioOutputDropdown.Exists);

                    var stream = client.GetUserMedia(constraints);

                    var tracks = new List<LSTrack>();
                    foreach (var track in stream.GetAudioTracks())
                    {
                        var trackOption = new LSTrackOption()
                            .SetMeta(audioMeta)
                            .SetMuteType(MuteType.Unmute);

                        tracks.Add(new LSTrack(track, stream, trackOption));
                    }

                    var option = new Option()
                        .SetLocalLSTracks(tracks)
                        .SetMeta(connectionMeta);

                    client.Connect(secrets.ClientId, accessToken, option);
                    userData.RoomID = roomId;
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to Connect.", e);
                    SetConnectButtonText("Connect");
                    SetRoomIdActive(true);
                }
            }
        });
    }

    private void Disconnect()
    {
        Task.Run(() =>
        {
            lock (lockObject)
            {
                try
                {
                    client.Disconnect();
                    remoteVideoTrack = null;
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to Disconnect.", e);
                    SetConnectButtonText("Connect");
                    SetRoomIdActive(true);
                }
            }
        });
    }

    /// <summary>
    /// <see cref="client"/>を新しく生成し、初期化処理を行う。
    /// </summary>
    /// <param name="listener">クライアントに設定するリスナー</param>
    private void InitializeClient(IClientListener listener)
    {
        client = new Client();
        client.SetEventListener(listener);
        SetDevice(client);
        stats = new Stats(client, logFilePath);
    }

    /// <summary>
    /// webrtcのログの設定を行う
    /// </summary>
    private void SetConfigWebrtcLog()
    {
        uint logSize = 10 * 1024 * 1024;
        WebrtcLog.Create(logFilePath, "webrtc_", logSize);
    }

    private void InitializeDevice()
    {
        // Input/Output device selector setting.
        audioInputDropdown.Initialize(OnAudioInputDropdownValueChanged);
        audioOutputDropdown.Initialize(OnAudioOutputDropdownValueChanged);
    }

    private void SetDevice(Client client)
    {
        client.SetAudioInputDevice(audioInputDropdown.DeviceName);
        client.SetAudioOutputDevice(audioOutputDropdown.DeviceName);
    }

    private void SetConnectButtonText(string buttonText)
    {
        unityUIContext.Post(__ =>
        {
            var button = buttonConnect.GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = buttonText;
            button.interactable = !buttonText.Contains("...");
        }, null);
    }

    private void SetMenuActive(bool isDisplay)
    {
        unityUIContext.Post(__ =>
        {
            menuCanvas.SetActive(isDisplay);
        }, null);
    }

    private void SetKeyboardActive(bool isDisplay)
    {
        unityUIContext.Post(__ =>
        {
            keyboard.SetActive(isDisplay);
        }, null);
    }

    private void SetRoomIdActive(bool isDisplay)
    {
        unityUIContext.Post(__ =>
        {
            roomIds.SetActive(isDisplay);
        }, null);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// メタデータからVideoFormatを取得する
    /// </summary>
    /// <param name="meta">メタデータ</param>
    /// <returns>ビデオフォーマット</returns>
    private VideoRenderManager.VideoFormat GetVideoFormat(Dictionary<string, object> meta)
    {
        try
        {
            if (meta.TryGetValue("isTheta", out var isTheta))
            {
                if (Convert.ToBoolean(isTheta))
                {
                    return VideoRenderManager.VideoFormat.Equi;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error("Failed to GetVideoFormat.", e);
        }

        return VideoRenderManager.VideoFormat.Normal;
    }

    private void OnChangeShowingTrack(VideoTrack videoTrack, VideoRenderManager.VideoFormat videoFormat)
    {
        Logger.Info($"OnChangeShowingTrack(id={videoTrack.Id} format={videoFormat})");

        lock (frameLock)
        {
            remoteVideoTrack = videoTrack;

            switch (videoFormat)
            {
                case VideoRenderManager.VideoFormat.Normal:
                    videoRenderer = normalVideoRenderer;
                    break;
                case VideoRenderManager.VideoFormat.Equi:
                    videoRenderer = equirectangularRenderer;
                    break;
                default:
                    Logger.Error($"Unknown VideoFormat. format={videoFormat}");
                    videoRenderer = null;
                    break;
            }
        }
    }

    /// <summary>
    /// AlertDialogを表示する
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="onClick">クリックイベントハンドラ</param>
    private void ShowAlertDialog(string message, UnityAction onClick)
    {
        alertDialog.SetActive(true);
        alertDialogMessageText.text = message;
        alertDialogButton.onClick.AddListener(onClick);
    }

    private class ClientListener : IClientListener
    {
        private readonly LiveStreaming app;

        public ClientListener(LiveStreaming app)
        {
            this.app = app;
        }

        public void OnConnecting()
        {
            Logger.Debug("OnConnecting");
        }

        public void OnOpen()
        {
            Logger.Debug("OnOpen");

            lock (lockObject)
            {
                app.SetConnectButtonText("Disconnect");
                app.SetRoomIdActive(false);
                app.SetKeyboardActive(false);
                app.stats.Start();
            }
        }

        public void OnClosing()
        {
            Logger.Debug("OnClosing");
            app.SetConnectButtonText("Disconnecting...");
        }

        public void OnClosed()
        {
            Logger.Debug("OnClosed()");

            lock (lockObject)
            {
                try
                {
                    app.stats.Dispose();
                    app.client?.Dispose();
                    app.client = null;
                    app.videoRenderManager.Clear();
                    app.InitializeClient(this);
                    app.SetConnectButtonText("Connect");
                    app.SetRoomIdActive(true);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to OnClosed.", e);
                }
            }
        }

        public void OnAddLocalTrack(MediaStreamTrack mediaStreamTrack, MediaStream stream)
        {
            Logger.Debug($"OnAddLocalTrack({mediaStreamTrack.Id})");
        }

        public void OnAddRemoteConnection(string connectionId, Dictionary<string, object> metadata)
        {
            Logger.Debug($"OnAddRemoteConnection(connectionId = {connectionId})");
        }

        public void OnUpdateRemoteConnection(string connectionId, Dictionary<string, object> metadata)
        {
            Logger.Debug($"OnUpdateRemoteConnection(connectionId = {connectionId})");
        }

        public void OnRemoveRemoteConnection(string connectionId, Dictionary<string, object> metadata, List<MediaStreamTrack> mediaStreamTracks)
        {
            Logger.Debug($"OnRemoveRemoteConnection(connectionId = {connectionId})");

            app.videoRenderManager.RemoveRemoteTrack(connectionId);
            if (app.videoRenderManager.GetNumOfRemoteTrack() == 0)
            {
                app.unityUIContext.Post(__ =>
                {
                    app.equirectangularSphere.SetActive(false);
                    app.normalVideoPlane.SetActive(false);
                }, null);
            }
        }

        public void OnAddRemoteTrack(string connectionId, MediaStream stream, MediaStreamTrack mediaStreamTrack, Dictionary<string, object> metadata, MuteType muteType)
        {
            Logger.Debug($"OnAddRemoteTrack(connectionId = {connectionId}, streamId = {stream.Id}, trackId = {mediaStreamTrack.Id}, muteType = {muteType})");

            if (mediaStreamTrack is VideoTrack videoTrack)
            {
                app.videoRenderManager.AddRemoteTrack(connectionId, videoTrack, app.GetVideoFormat(metadata));
            }
        }

        public void OnUpdateRemoteTrack(string connectionId, MediaStream stream, MediaStreamTrack mediaStreamTrack, Dictionary<string, object> metadata)
        {
            Logger.Debug($"OnUpdateRemoteTrack(connectionId = {connectionId}, streamId = {stream.Id}, trackId = {mediaStreamTrack.Id})");
        }

        public void OnUpdateMute(string connectionId, MediaStream stream, MediaStreamTrack mediaStreamTrack, MuteType muteType)
        {
            Logger.Debug($"OnUpdateMute(connectionId = {connectionId}, streamId = {stream.Id}, trackId = {mediaStreamTrack.Id}, muteType = {muteType})");
        }

        public void OnChangeStability(string connectionId, Stability stability)
        {
            Logger.Debug($"OnChangeStability(connectionId = {connectionId}, stability = {stability})");
        }

        public void OnError(SDKErrorEvent error)
        {
            Logger.Debug($"OnError({error.ToReportString()})");
        }
    }

    private class VideoTrackListener : VideoTrack.IListener
    {
        private readonly LiveStreaming app;

        public VideoTrackListener(LiveStreaming app)
        {
            this.app = app;
        }

        public void OnFrameSizeChanged(string id, int width, int height)
        {
            Logger.Info($"OnFrameSizeChanged(id={id} width={width} height={height})");

            app.unityUIContext.Post(__ =>
            {
                lock (frameLock)
                {
                    var videoFormat = app.videoRenderManager.GetCurrentShowingTrackVideoFormat();
                    app.equirectangularSphere.SetActive(videoFormat == VideoRenderManager.VideoFormat.Equi);
                    app.normalVideoPlane.SetActive(videoFormat == VideoRenderManager.VideoFormat.Normal);

                    if (app.texture != null)
                    {
                        MonoBehaviour.Destroy(app.texture);
                    }

                    Utils.AdjustAspect(app.normalVideoPlane, width, height);

                    app.texture = Utils.CreateTexture(width, height);
                    app.equirectangularRenderer.material.mainTexture = app.texture;
                    app.normalVideoRenderer.material.mainTexture = app.texture;
                }
            }, null);
        }
    }

    private class WindowProcedureHookListener : WindowProcedureHook.IListener
    {
        private readonly LiveStreaming app;

        public WindowProcedureHookListener(LiveStreaming app)
        {
            this.app = app;
        }

        public void OnDevicesChanged(WindowProcedureHook.DeviceType type)
        {
            Logger.Debug(string.Format("OnDevicesChanged(type ={0})", type));

            app.unityUIContext.Post(__ =>
            {
                switch (type)
                {
                    case WindowProcedureHook.DeviceType.Audio:
                        app.audioInputDropdown.Refresh();
                        app.audioOutputDropdown.Refresh();
                        break;
                    case WindowProcedureHook.DeviceType.VideoCapture:
                        // nothing to do.
                        break;
                    default:
                        // nothing to do.
                        break;
                }
            }, null);
        }
    }
}