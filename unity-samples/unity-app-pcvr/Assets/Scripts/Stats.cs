using com.ricoh.livestreaming;
using System;
using System.Threading;

public class Stats : IDisposable
{
    private readonly Client client;
    private static readonly object statsLock = new object();
    private RTCStatsLogger statsLogger;
    private Timer statsOutpuTimer;

    public Stats(Client client, string logFilePath)
    {
        this.client = client;
        statsLogger = new RTCStatsLogger(Utils.CreateFilePath(logFilePath));
    }

    public void Start()
    {
        if (statsOutpuTimer == null)
        {
            statsOutpuTimer = new Timer(RtcStatLog, null, 500, 1000);
        }
    }

    public void Stop()
    {
        statsOutpuTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        statsOutpuTimer?.Dispose();
        statsOutpuTimer = null;
    }

    public void Dispose()
    {
        Stop();

        lock (statsLock)
        {
            statsLogger?.Dispose();
            statsLogger = null;
        }
    }

    private void RtcStatLog(object state)
    {
        lock (statsLock)
        {
            var statsReports = client.GetStats();
            foreach (var report in statsReports)
            {
                statsLogger.Log(report.Key, report.Value);
            }
        }
    }
}
