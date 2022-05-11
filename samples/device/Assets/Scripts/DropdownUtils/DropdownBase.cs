using UnityEngine.UI;

/// <summary>
/// Dropdown の基本クラス
/// </summary>
public abstract class DropdownBase : Dropdown
{
    /// <summary>
    /// Dropdown を初期化する
    /// </summary>
    /// <param name="isRefresh">初期化と同時に <see cref="Dropdown.options"/> の追加を行う場合は true</param>
    public void Initialize(bool isRefresh = true)
    {
        onValueChanged.AddListener(delegate
        {
            OnValueChangedInternal();
        });

        if (isRefresh)
        {
            Refresh();
        }
    }

    /// <summary>
    /// <see cref="Dropdown.options"/> の更新を行う
    /// </summary>
    public abstract void Refresh();

    /// <summary>
    /// オプションをクリックした際に実行されるイベント
    /// </summary>
    protected abstract void OnValueChangedInternal();
}
