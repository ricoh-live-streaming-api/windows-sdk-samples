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
    /// Connect ���̃r�b�g���[�g���͈͊O
    /// </summary>
    private const int invalidVideoMaxBitrateKBPSOnConnect = 43225;

    /// <summary>
    /// ChangeVideoSendBitrate ���̃r�b�g���[�g���͈͊O
    /// </summary>
    private const int invalidMaxBitrateKBPSOnChangeVideoSendBitrate = 45708;

    /// <summary>
    /// Connect ���ɐݒ肵���r�b�g���[�g
    /// </summary>
    private int maxBitrateKbpsConnected;

    public void Start()
    {
        Logger.Debug("Start BitrateSample.");

        InitializeClient(new ClientListener(this));
    }

    /// <summary>
    /// �r�b�g���[�g�ύX�{�^�������C�x���g
    /// </summary>
    public void OnVideoSendBitrateChangeButtonClick()
    {
        SetErrorNotification(false);
        client.ChangeVideoSendBitrate(MaxBitrateKbps);
    }

    /// <summary>
    /// �r�b�g���[�g�ύX�{�^���̗L��/�����؂�ւ�
    /// </summary>
    /// <param name="enabled">true : �L��, false : ����</param>
    private void SetVideoSendBitrateChangeButtonEnabled(bool enabled)
    {
        UnityUIContext.Post(__ =>
        {
            bitrateChangeButton.interactable = enabled;
        }, null);
    }

    /// <summary>
    /// �G���[�ʒm���Ƀ��b�Z�[�W��\������
    /// </summary>
    /// <param name="isError">�r�b�g���[�g���L���͈͓����ǂ���</param>
    private void SetErrorNotification(bool isError)
    {
        UnityUIContext.Post(__ =>
        {
            if (isError)
            {
                errorNotification.text = $"Bitrate ���ݒ�͈͊O�ł��B";
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
    /// �ݒ�\�ȃr�b�g���[�g�͈̔͂�UI�ɕ\������
    /// </summary>
    /// <param name="maxBitrate">�ݒ�\�ȃr�b�g���[�g�̍ő�l</param>
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

            // �ڑ����� Connect ���ɐݒ肵���r�b�g���[�g���ύX�\�ȏ���l�ƂȂ�
            app.SetAvailableMaxBitrate(app.maxBitrateKbpsConnected);

            lastErrorCode = 0;
        }

        public override void OnClosing(LSClosingEvent lSClosingEvent)
        {
            base.OnClosing(lSClosingEvent);
            app.SetVideoSendBitrateChangeButtonEnabled(false);

            if (lastErrorCode == invalidMaxBitrateKBPSOnChangeVideoSendBitrate)
            {
                // ChangeVideoSendBitrate ���s���̃G���[�\���͐ؒf���ɏ���
                // (Connect ���s���͑����ؒf����邽�߁A�G���[�\�����c��)
                app.SetErrorNotification(false);
            }

            // �ؒf���� 20000Kbps ���ݒ�\�ȃr�b�g���[�g�̏���l�ƂȂ�
            app.SetAvailableMaxBitrate(20000);
        }

        public override void OnError(SDKErrorEvent error)
        {
            base.OnError(error);

            lastErrorCode = error.Detail.Code;

            if (lastErrorCode == invalidVideoMaxBitrateKBPSOnConnect || lastErrorCode == invalidMaxBitrateKBPSOnChangeVideoSendBitrate)
            {
                // �r�b�g���[�g�͈͊O�G���[�\��
                app.SetErrorNotification(true);
            }
        }
    }
}
