using com.ricoh.livestreaming.webrtc;
using System;

/// <summary>
/// <see cref="DeviceInfo"/> のデバイス名を Dropdown に表示するための基本クラス
/// </summary>
public abstract class DropdownDeviceBase : DropdownClassBase<DeviceInfo>
{
    /// <summary>
    /// デバイス名
    /// </summary>
    public string DeviceName { get; private set; } = "";

    private Action<string> onSelectAction;

    /// <summary>
    /// Dropdown を初期化する
    /// </summary>
    /// <param name="onSelectAction">Dropdown を選択した際に実行するアクション</param>
    public void Initialize(Action<string> onSelectAction = null)
    {
        Initialize(true);
        this.onSelectAction = onSelectAction;
    }

    /// <summary>
    /// <see cref="DeviceInfo"/> のデバイス名を取得する
    /// </summary>
    /// <param name="deviceInfo"></param>
    /// <returns>デバイス名</returns>
    protected override string GetItemName(DeviceInfo deviceInfo)
    {
        return deviceInfo.DeviceName;
    }

    protected override void SelectItem(int select)
    {
        if (!Exists)
        {
            return;
        }

        string deviceName = GetItemName(Items[select]);

        if (DeviceName == deviceName)
        {
            return;
        }

        DeviceName = deviceName;
        onSelectAction?.Invoke(deviceName);
    }
}
