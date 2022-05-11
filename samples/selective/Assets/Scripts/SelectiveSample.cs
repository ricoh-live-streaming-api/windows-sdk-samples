using com.ricoh.livestreaming;
using com.ricoh.livestreaming.webrtc;
using UnityEngine;
using UnityEngine.UI;

public class SelectiveSample : BehaviorBase
{
    [SerializeField]
    private Toggle videoReceiveToggle = null;

    public override string ClientId => "";
    public override string ClientSecret => "";

    public void Start()
    {
        Logger.Debug("Start SelectiveSample.");
        InitializeClient(new ClientListener(this));
    }

    public void OnVideoReceiveToggleChanged(bool isOn)
    {
        if (!RemoteConnections.IsEmpty)
        {
            client.ChangeMediaRequirements(
                RemoteVideoRender.ConnectionId,
                isOn ? VideoRequirement.Required : VideoRequirement.Unrequired);
        }
    }

    protected override void ShowRemoteView(string connectionId, VideoTrack videoTrack)
    {
        base.ShowRemoteView(connectionId, videoTrack);

        UnityUIContext.Post(__ =>
        {
            videoReceiveToggle.isOn = true;
        }, null);
    }

    private class ClientListener : ClientListenerBase
    {
        public ClientListener(SelectiveSample app) : base(app)
        {
        }
    }
}
