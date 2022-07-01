using com.ricoh.livestreaming.unity;
using com.ricoh.livestreaming.webrtc;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class VideoRender : VideoRenderBase
{
    protected Vector2 originalSize;
    protected RawImage image;

    /// <summary>
    /// VideoRender を生成し映像描画を開始する
    /// </summary>
    /// <param name="context">Unity メインスレッドの SynchronizationContext</param>
    /// <param name="target">映像描画対象のオブジェクト</param>
    /// <param name="track">映像描画元の VideoTrack</param>
    /// <param name="connectionId">映像描画元の Connection ID</param>
    public VideoRender(SynchronizationContext context, GameObject target, VideoTrack track, string connectionId = null) : base(context, track, connectionId)
    {
        listener = new VideoTrackListener(this);

        this.context.Post(__ =>
        {
            unityRenderer = new UnityRenderer();
            originalSize = Utils.GetRectSize(target);
            image = target.GetComponentInChildren<RawImage>();
            image.texture = Utils.CreateTexture((int)originalSize.x, (int)originalSize.y);
            texture = image.texture;
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
                    Utils.DestroyTexture(videoRender.image.texture);
                    videoRender.image.texture = Utils.CreateTexture(width, height);
                    Utils.AdjustAspect(videoRender.image, videoRender.originalSize, width, height);
                    videoRender.texture = videoRender.image.texture;
                }
            }, null);
        }
    }
}
