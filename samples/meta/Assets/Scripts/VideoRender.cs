using com.ricoh.livestreaming.unity;
using com.ricoh.livestreaming.webrtc;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class VideoRender : IDisposable
{
    /// <summary>
    /// 映像描画元の VideoTrack ID
    /// </summary>
    public string TrackId => track?.Id;

    /// <summary>
    /// 映像描画元の Connection ID
    /// </summary>
    public string ConnectionId { get; private set; }

    private static UnityRenderer unityRenderer;
    private readonly Vector2 originalSize;
    private readonly object lockObject = new object();
    private readonly SynchronizationContext context;
    private RawImage image;
    private VideoTrack track;
    private bool isDrawing;

    /// <summary>
    /// VideoRender を生成し映像描画を開始する<br/>
    /// </summary>
    /// <param name="context">Unity メインスレッドの SynchronizationContext</param>
    /// <param name="target">映像描画対象のオブジェクト</param>
    /// <param name="originalSize">映像描画対象のオブジェクトのサイズ</param>
    /// <param name="track">映像描画元の VideoTrack</param>
    /// <param name="connectionId">映像描画元の Connection ID</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/>が null の場合</exception>
    public VideoRender(SynchronizationContext context, GameObject target, Vector2 originalSize, VideoTrack track, string connectionId = null)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.originalSize = originalSize;
        ConnectionId = connectionId;

        this.context.Post(__ =>
        {
            unityRenderer = new UnityRenderer();
            image = target.GetComponentInChildren<RawImage>();
            image.texture = Utils.CreateTexture((int)originalSize.x, (int)originalSize.y);
            SetTrack(track);
        }, null);
    }

    /// <summary>
    /// 映像描画元の VideoTrack を設定し映像描画を開始する
    /// </summary>
    /// <param name="videoTrack">映像描画元の VideoTrack</param>
    private void SetTrack(VideoTrack videoTrack)
    {
        if (TrackId == videoTrack.Id)
        {
            return;
        }

        track?.RemoveSink();

        track = videoTrack;
        track.AddSink();
        track.SetEventListener(new VideoTrackListener(this));
        isDrawing = true;
    }

    /// <summary>
    /// 映像を描画する<br/>
    /// Unity の Update メソッド上で呼び出す必要がある
    /// </summary>
    public void Draw()
    {
        if (isDrawing)
        {
            lock (lockObject)
            {
                unityRenderer.RenderToTexture(image.texture, track);
            }
        }
    }

    /// <summary>
    /// 映像描画を停止する
    /// </summary>
    public void Dispose()
    {
        isDrawing = false;

        context.Post(__ =>
        {
            Utils.DestroyTexture(image.texture);
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
                }
            }, null);
        }
    }
}
