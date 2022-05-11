using com.ricoh.livestreaming.webrtc;
using System.Collections.Generic;

/// <summary>
/// オーディオ出力デバイス名を Dropdown に表示するためのクラス
/// </summary>
public class DropdownAudioOutput : DropdownDeviceBase
{
    protected override List<DeviceInfo> GetItems()
    {
        return DeviceUtil.GetAudioOutputDeviceList();
    }
}
