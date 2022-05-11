using com.ricoh.livestreaming.webrtc;
using System;
using System.Collections.Generic;

/// <summary>
/// ビデオデバイス名を Dropdown に表示するためのクラス
/// </summary>
public class DropdownVideoCapturer : DropdownDeviceBase
{
    /// <summary>
    /// <see cref="DropdownVideoCapturer"/> および <see cref="DropdownCapability"/> を初期化する
    /// </summary>
    /// <param name="dropdownCapability"><see cref="DropdownCapability"/></param>
    /// <param name="onSelectAction">CapabilityDropdown の解像度/FrameRate の変更時に実行するアクション</param>
    public void Initialize(DropdownCapability dropdownCapability, Action<string, int, int, int> onSelectAction)
    {
        // 先に DropdownCapability を初期化するが、この時は表示内容を更新しない
        dropdownCapability.Initialize(this, onSelectAction);

        // DropdownVideoCapturer 初期化時に DropdownCapability の表示内容を更新する
        Initialize((_) => dropdownCapability.Refresh());
    }

    protected override List<DeviceInfo> GetItems()
    {
        return DeviceUtil.GetVideoCapturerDeviceList();
    }
}
