using com.ricoh.livestreaming;
using UnityEngine;
using UnityEngine.UI;

public class BitrateSample : BehaviorBase
{
    [SerializeField]
    private Text range = null;
    [SerializeField]
    private InputField bitrateEdit = null;
    [SerializeField]
    private Text errorNotification = null;
    [SerializeField]
    private Button bitrateChangeButton = null;

    public override string ClientId => "";
    public override string ClientSecret => "";
    public override int MaxBitrateKbps => int.TryParse(bitrateEdit.text, out int bitrate) ? bitrate : 0;

    /// <summary>
    /// Connect 時のビットレートが範囲外
    /// </summary>
    private const int invalidVideoMaxBitrateKBPSOnConnect = 43225;

    /// <summary>
    /// ChangeVideoSendBitrate 時のビットレートが範囲外
    /// </summary>
    private const int invalidMaxBitrateKBPSOnChangeVideoSendBitrate = 45708;

    /// <summary>
    /// Connect 時に設定したビットレート
    /// </summary>
    private int maxBitrateKbpsConnected;

    public void Start()
    {
        Logger.Debug("Start BitrateSample.");

        InitializeClient(new ClientListener(this));
    }

    /// <summary>
    /// ビットレート変更ボタン押下イベント
    /// </summary>
    public void OnVideoSendBitrateChangeButtonClick()
    {
        SetErrorNotification(false);
        client.ChangeVideoSendBitrate(MaxBitrateKbps);
    }

    /// <summary>
    /// ビットレート変更ボタンの有効/無効切り替え
    /// </summary>
    /// <param name="enabled">true : 有効, false : 無効</param>
    private void SetVideoSendBitrateChangeButtonEnabled(bool enabled)
    {
        UnityUIContext.Post(__ =>
        {
            bitrateChangeButton.interactable = enabled;
        }, null);
    }

    /// <summary>
    /// エラー通知欄にメッセージを表示する
    /// </summary>
    /// <param name="isError">ビットレートが有効範囲内かどうか</param>
    private void SetErrorNotification(bool isError)
    {
        UnityUIContext.Post(__ =>
        {
            if (isError)
            {
                errorNotification.text = $"Bitrate が設定範囲外です。";
                range.color = Color.yellow;
            }
            else
            {
                errorNotification.text = "";
                range.color = Color.white;
            }
        }, null);
    }

    /// <summary>
    /// 設定可能なビットレートの範囲をUIに表示する
    /// </summary>
    /// <param name="maxBitrate">設定可能なビットレートの最大値</param>
    private void SetAvailableMaxBitrate(int maxBitrate)
    {
        UnityUIContext.Post(__ =>
        {
            range.text = $"(100 - {maxBitrate}Kbps)";
        }, null);
    }

    protected override void Connect(string _)
    {
        base.Connect("WinUnityAPISamplesBitrate");
        maxBitrateKbpsConnected = MaxBitrateKbps;
    }

    private class ClientListener : ClientListenerBase
    {
        private new readonly BitrateSample app;
        private int lastErrorCode;

        public ClientListener(BitrateSample app) : base(app)
        {
            this.app = app;
        }

        public override void OnOpen(LSOpenEvent lSOpenEvent)
        {
            base.OnOpen(lSOpenEvent);
            app.SetErrorNotification(false);
            app.SetVideoSendBitrateChangeButtonEnabled(true);

            // 接続中は Connect 時に設定したビットレートが変更可能な上限値となる
            app.SetAvailableMaxBitrate(app.maxBitrateKbpsConnected);

            lastErrorCode = 0;
        }

        public override void OnClosing(LSClosingEvent lSClosingEvent)
        {
            base.OnClosing(lSClosingEvent);
            app.SetVideoSendBitrateChangeButtonEnabled(false);

            if (lastErrorCode == invalidMaxBitrateKBPSOnChangeVideoSendBitrate)
            {
                // ChangeVideoSendBitrate 失敗時のエラー表示は切断時に消す
                // (Connect 失敗時は即時切断されるため、エラー表示を残す)
                app.SetErrorNotification(false);
            }

            // 切断中は 20000Kbps が設定可能なビットレートの上限値となる
            app.SetAvailableMaxBitrate(20000);
        }

        public override void OnError(SDKErrorEvent error)
        {
            base.OnError(error);

            lastErrorCode = error.Detail.Code;

            if (lastErrorCode == invalidVideoMaxBitrateKBPSOnConnect || lastErrorCode == invalidMaxBitrateKBPSOnChangeVideoSendBitrate)
            {
                // ビットレート範囲外エラー表示
                app.SetErrorNotification(true);
            }
        }
    }
}
