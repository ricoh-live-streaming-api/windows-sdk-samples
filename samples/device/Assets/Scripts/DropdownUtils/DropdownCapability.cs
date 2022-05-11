using com.ricoh.livestreaming.webrtc;
using System;
using System.Collections.Generic;

/// <summary>
/// ビデオデバイスの解像度・FrameRate を Dropdown に表示するためのクラス
/// </summary>
public class DropdownCapability : DropdownClassBase<VideoCapturerDeviceCapability>
{
    /// <summary>
    /// デバイス名
    /// </summary>
    public string DeviceName { get; private set; } = "";

    /// <summary>
    /// Width
    /// </summary>
    public int Width { get; private set; } = 0;

    /// <summary>
    /// Height
    /// </summary>
    public int Height { get; private set; } = 0;

    /// <summary>
    /// FrameRate
    /// </summary>
    public int FrameRate { get; private set; } = 0;

    private DropdownVideoCapturer dropdownVideoCapturer;
    private Action<string, int, int, int> onSelectAction;
    private string SelectedDeviceName => dropdownVideoCapturer.DeviceName;

    /// <summary>
    /// <see cref="DropdownCapability"/> を初期化する
    /// </summary>
    /// <param name="dropdownVideoCapturer"><see cref="DropdownVideoCapturer"/></param>
    /// <param name="onSelectAction">CapabilityDropdown の解像度/FrameRate の変更時に実行するアクション</param>
    public void Initialize(DropdownVideoCapturer dropdownVideoCapturer, Action<string, int, int, int> onSelectAction = null)
    {
        Initialize(false);
        this.dropdownVideoCapturer = dropdownVideoCapturer;
        this.onSelectAction = onSelectAction;
    }

    protected override List<VideoCapturerDeviceCapability> GetItems()
    {
        return DeviceUtil.GetVideoCapturerDeviceCapabilities(dropdownVideoCapturer.DeviceName);
    }

    protected override string GetItemName(VideoCapturerDeviceCapability capability)
    {
        return capability.Width + " x " + capability.Height + " ( " + capability.FrameRate + "fps )";
    }

    protected override void SelectItem(int select)
    {
        if (!Exists)
        {
            return;
        }

        if (DeviceName == SelectedDeviceName &&
            Width == Items[select].Width &&
            Height == Items[select].Height &&
            FrameRate == Items[select].FrameRate)
        {
            return;
        }

        DeviceName = SelectedDeviceName;
        Width = Items[select].Width;
        Height = Items[select].Height;
        FrameRate = Items[select].FrameRate;

        onSelectAction?.Invoke(DeviceName, Width, Height, FrameRate);
    }
}
