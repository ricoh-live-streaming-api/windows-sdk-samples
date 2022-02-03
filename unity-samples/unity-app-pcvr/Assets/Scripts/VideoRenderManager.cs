using com.ricoh.livestreaming.webrtc;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

public class VideoRenderManager
{
    private readonly IDictionary<string, VideoTrackLocal> remoteTrackMap = new Dictionary<string, VideoTrackLocal>();
    private int remoteVideoShowIndex = 0;
    private static readonly object _lock = new object();
    private readonly VideoTrack.IListener listener;
    private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public Action<VideoTrack, VideoRenderManager.VideoFormat> OnChangeShowingTrack;

    public enum VideoFormat
    {
        Normal,
        Equi,
    }

    public VideoRenderManager(VideoTrack.IListener listener)
    {
        this.listener = listener;
    }

    /// <summary>
    /// Add remote track.
    /// </summary>
    /// <param name="connectionId">Connection ID</param>
    /// <param name="track">Video track</param>
    /// <param name="format">Video format</param>
    public void AddRemoteTrack(string connectionId, VideoTrack track, VideoFormat format)
    {
        Logger.Info($"AddRemoteTrack(connectionId={connectionId} format={format})");

        lock (_lock)
        {
            if (remoteTrackMap.ContainsKey(connectionId))
            {
                // Delete old track.
                RemoveRemoteTrack(connectionId);
            }

            VideoTrackLocal videoTrackLocal = new VideoTrackLocal(this, track, format);
            remoteTrackMap.Add(connectionId, videoTrackLocal);
            if (!IsShowing())
            {
                videoTrackLocal.Show();
                OnChangeShowingTrack?.Invoke(track, format);
            }
        }
    }

    /// <summary>
    /// Remove remote track.
    /// </summary>
    /// <param name="connectionId">Connection ID</param>
    public void RemoveRemoteTrack(string connectionId)
    {
        Logger.Info($"RemoveRemoteTrack(connectionId={connectionId})");
        lock (_lock)
        {
            bool isShowing = IsShowing(connectionId);
            remoteTrackMap.Remove(connectionId);
            if (isShowing && remoteTrackMap.Count >= 1)
            {
                var videoLocal = remoteTrackMap.First().Value;
                videoLocal.Show();
                remoteVideoShowIndex = 0;
                OnChangeShowingTrack?.Invoke(videoLocal.Track, videoLocal.Format);
            }
        }
    }

    /// <summary>
    /// Get number of retmote tracks.
    /// </summary>
    /// <returns>Number of remote tracks.</returns>
    public int GetNumOfRemoteTrack()
    {
        lock (_lock)
        {
            return remoteTrackMap.Count;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            remoteTrackMap.Clear();
            remoteVideoShowIndex = 0;
        }
    }

    /// <summary>
    /// Switch display participant by toggle.
    /// </summary>
    public void ToggleDisplay()
    {
        lock (_lock)
        {
            Hide();
            remoteVideoShowIndex++;

            if (remoteTrackMap.Count <= remoteVideoShowIndex)
            {
                KeyValuePair<string, VideoTrackLocal> next = remoteTrackMap.First();
                next.Value.Show();
                OnChangeShowingTrack?.Invoke(next.Value.Track, next.Value.Format);
                remoteVideoShowIndex = 0;
            }
            else
            {
                int i = 0;
                foreach (VideoTrackLocal track in remoteTrackMap.Values)
                {
                    if (i == remoteVideoShowIndex)
                    {
                        track.Show();
                        OnChangeShowingTrack?.Invoke(track.Track, track.Format);
                        break;
                    }

                    i++;
                }
            }
        }
    }

    /// <summary>
    /// 現在表示中のTrackのビデオフォーマットを取得する
    /// </summary>
    /// <returns>ビデオフォーマット</returns>
    /// <exception cref="Exception">表示中のTrackがみつからない場合にthrowする</exception>
    public VideoFormat GetCurrentShowingTrackVideoFormat()
    {
        lock (_lock)
        {
            var track = remoteTrackMap
                .Values
                .Cast<VideoTrackLocal>()
                .FirstOrDefault(t => t.IsShowing);
            if (track != null)
            {
                return track.Format;
            }

            throw new Exception("Not found showing track.");
        }
    }

    private void Hide()
    {
        foreach (VideoTrackLocal track in remoteTrackMap.Values)
        {
            if (track.IsShowing)
            {
                track.Hide();
                break;
            }
        }
    }

    private bool IsShowing(string id)
    {
        if (!remoteTrackMap.ContainsKey(id))
        {
            return false;
        }

        return remoteTrackMap[id].IsShowing;
    }

    private bool IsShowing()
    {
        foreach (VideoTrackLocal track in remoteTrackMap.Values)
        {
            if (track.IsShowing)
            {
                return true;
            }
        }

        return false;
    }

    private class VideoTrackLocal
    {
        private readonly VideoRenderManager manager;
        public VideoTrack Track { get; private set; }
        public string Id => Track.Id;
        public bool IsShowing { get; private set; }
        public VideoFormat Format { get; private set; }

        public VideoTrackLocal(VideoRenderManager manager, VideoTrack track, VideoFormat format)
        {
            this.manager = manager;
            Track = track;
            Format = format;
        }

        public void Show()
        {
            Logger.Debug($"Show({Id})");
            Track.AddSink();
            Track.SetEventListener(manager.listener);
            IsShowing = true;
        }

        public void Hide()
        {
            Logger.Debug($"Hide({Id})");
            Track.RemoveSink();
            IsShowing = false;
        }
    }
}