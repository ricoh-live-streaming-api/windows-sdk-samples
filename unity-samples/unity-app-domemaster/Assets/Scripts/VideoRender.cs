using com.ricoh.livestreaming.unity;
using com.ricoh.livestreaming.webrtc;
using System.Threading;
using UnityEngine;

public class VideoRender : VideoRenderBase
{
    private Renderer renderer;

    public VideoRender(SynchronizationContext context, GameObject target, VideoTrack track, string connectionId = null) : base(context, track, connectionId)
    {
        listener = new VideoTrackListener(this);

        this.context.Post(__ =>
        {
            unityRenderer = new UnityRenderer();
            renderer = target.GetComponent<Renderer>();
            texture = Utils.CreateTexture(2, 2);
            Show();
        }, null);
    }

    private class VideoTrackListener : VideoTrack.IListener
    {
        private readonly VideoRender videoRender;

        public VideoTrackListener(VideoRender videoRender)
        {
            this.videoRender = videoRender;
        }

        public void OnFrameSizeChanged(string id, int width, int height)
        {
            videoRender.context.Post(__ =>
            {
                lock (videoRender.lockObject)
                {
                    Utils.DestroyTexture(videoRender.texture);
                    videoRender.texture = Utils.CreateTexture(width, height);
                    videoRender.renderer.material.mainTexture = videoRender.texture;
                }
            }, null);
        }
    }
}
