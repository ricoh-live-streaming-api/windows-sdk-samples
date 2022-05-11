using com.ricoh.livestreaming.webrtc;
using System.Collections.Generic;

/// <summary>
/// オーディオ入力デバイス名を Dropdown に表示するためのクラス
/// </summary>
public class DropdownAudioInput : DropdownDeviceBase
{
    protected override List<DeviceInfo> GetItems()
    {
        return DeviceUtil.GetAudioInputDeviceList();
    }
}
