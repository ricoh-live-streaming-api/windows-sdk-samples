using com.ricoh.livestreaming.unity;
using com.ricoh.livestreaming.webrtc;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceSample : BehaviorBase
{
    [SerializeField]
    private DropdownVideoCapturer videoCapturerDropdown = null;
    [SerializeField]
    private DropdownCapability capabilityDropdown = null;
    [SerializeField]
    private DropdownAudioInput audioInputDropdown = null;
    [SerializeField]
    private DropdownAudioOutput audioOutputDropdown = null;

    public override string ClientId => "";
    public override string ClientSecret => "";
    public override bool IsAudioEnabled => audioInputDropdown.Exists | audioOutputDropdown.Exists;

    private IntPtr hWnd;    // own window handle
    private WindowProcedureHook windowProcedureHook;

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpszClass, string lpszTitle);

    public void Start()
    {
        Logger.Debug("Start DeviceSample.");

        // Get own window handle.
        hWnd = FindWindow(null, Application.productName);
        windowProcedureHook = new WindowProcedureHook(hWnd);
        windowProcedureHook.AddListener(new WindowProcedureHookListener(this));

        InitializeClient(new ClientListener(this));
        InitializeDropdown();
    }

    public override void OnApplicationQuit()
    {
        Logger.Debug("Quit DeviceSample.");
        windowProcedureHook.Term();
        TerminateClient();
    }

    protected override void SetDevice()
    {
        VideoCapturer = new VideoDeviceCapturer(
            capabilityDropdown.DeviceName,
            capabilityDropdown.Width,
            capabilityDropdown.Height,
            capabilityDropdown.FrameRate);

        if (IsAudioEnabled)
        {
            client.SetAudioInputDevice(audioInputDropdown.DeviceName);
            client.SetAudioOutputDevice(audioOutputDropdown.DeviceName);
        }
    }

    /// <summary>
    /// デバイス Dropdown を初期化する
    /// </summary>
    private void InitializeDropdown()
    {
        audioInputDropdown.Initialize((deviceName) => client.SetAudioInputDevice(deviceName));
        audioOutputDropdown.Initialize((deviceName) => client.SetAudioOutputDevice(deviceName));
        videoCapturerDropdown.Initialize(capabilityDropdown, OnVideoDropdownValueChanged);
    }

    /// <summary>
    /// CapabilityDropdown の変更イベント<br/>
    /// VideoDeviceCapturer を生成し <see cref="com.ricoh.livestreaming.Client.ReplaceMediaStreamTrack(com.ricoh.livestreaming.LSTrack, MediaStreamTrack)"/> を実施する
    /// </summary>
    /// <param name="deviceName">デバイス名</param>
    /// <param name="width">Capture width</param>
    /// <param name="height">Capture height</param>
    /// <param name="frameRate">Capture frame rate</param>
    private void OnVideoDropdownValueChanged(string deviceName, int width, int height, int frameRate)
    {
        if (!IsConnected)
        {
            return;
        }

        VideoCapturer?.Release();
        VideoCapturer = new VideoDeviceCapturer(deviceName, width, height, frameRate);

        var track = GetLSTrack(MediaStreamTrack.TrackType.Video);
        if (track == null)
        {
            return;
        }

        var constraints = new MediaStreamConstraints()
            .SetVideoCapturer(VideoCapturer);

        var stream = client.GetUserMedia(constraints);
        var videoTrack = stream.GetVideoTracks()[0];
        client.ReplaceMediaStreamTrack(track, videoTrack);
        LocalVideoRender.SetTrack(videoTrack);
    }

    private class ClientListener : ClientListenerBase
    {
        public ClientListener(DeviceSample app) : base(app)
        {
        }
    }

    private class WindowProcedureHookListener : WindowProcedureHook.IListener
    {
        private readonly DeviceSample app;

        public WindowProcedureHookListener(DeviceSample app)
        {
            this.app = app;
        }

        public void OnDevicesChanged(WindowProcedureHook.DeviceType type)
        {
            app.UnityUIContext.Post(__ =>
            {
                switch (type)
                {
                    case WindowProcedureHook.DeviceType.Audio:
                        app.audioInputDropdown.Refresh();
                        app.audioOutputDropdown.Refresh();
                        break;
                    case WindowProcedureHook.DeviceType.VideoCapture:
                        app.videoCapturerDropdown.Refresh();
                        break;
                    default:
                        // nothing to do.
                        break;
                }
            }, null);
        }
    }
}
