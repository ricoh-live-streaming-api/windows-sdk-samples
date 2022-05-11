using com.ricoh.livestreaming.unity;
using com.ricoh.livestreaming.webrtc;
using System;
using System.Threading;
using UnityEngine;

public class VideoRenderBase : IDisposable
{
    /// <summary>
    /// 映像描画元の VideoTrack ID
    /// </summary>
    public string TrackId => videoTrack.Id;

    /// <summary>
    /// 映像描画元の Connection ID
    /// </summary>
    public string ConnectionId { get; private set; }

    protected static UnityRenderer unityRenderer;
    protected readonly object lockObject = new object();
    protected readonly SynchronizationContext context;
    protected VideoTrack.IListener listener;
    protected VideoTrack videoTrack;
    protected Texture texture;
    private bool isDrawing;

    /// <summary>
    /// VideoRenderBase を生成する
    /// </summary>
    /// <param name="context">Unity メインスレッドの SynchronizationContext</param>
    /// <param name="track">映像描画元の VideoTrack</param>
    /// <param name="connectionId">映像描画元の Connection ID</param>
    public VideoRenderBase(SynchronizationContext context, VideoTrack track, string connectionId = null)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.videoTrack = track ?? throw new ArgumentNullException(nameof(track));
        ConnectionId = connectionId;
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
                if (texture != null)
                {
                    unityRenderer.RenderToTexture(texture, videoTrack);
                }
            }
        }
    }

    /// <summary>
    /// 映像描画元の VideoTrack を設定し映像描画を開始する
    /// </summary>
    /// <param name="videoTrack">映像描画元の VideoTrack</param>
    public void SetTrack(VideoTrack videoTrack)
    {
        if (TrackId == videoTrack.Id)
        {
            return;
        }

        Hide();
        this.videoTrack = videoTrack;
        Show();
    }

    /// <summary>
    /// 映像描画を停止し texture を破棄する
    /// </summary>
    public void Dispose()
    {
        Hide();

        context.Post(__ =>
        {
            Utils.DestroyTexture(texture);
        }, null);
    }

    /// <summary>
    /// 映像描画を開始する
    /// </summary>
    protected void Show()
    {
        if (!isDrawing)
        {
            videoTrack.AddSink();
            videoTrack.SetEventListener(listener);
            isDrawing = true;
        }
    }

    /// <summary>
    /// 映像描画を停止する
    /// </summary>
    protected void Hide()
    {
        if (isDrawing)
        {
            videoTrack.RemoveSink();
            isDrawing = false;
        }
    }
}
