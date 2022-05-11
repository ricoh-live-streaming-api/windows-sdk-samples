using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// クラスの任意のフィールド値を Dropdown に表示するための基本クラス
/// </summary>
public abstract class DropdownClassBase<T> : DropdownBase where T : class
{
    /// <summary>
    /// Dropdown に表示するクラスのリストが存在する場合は true
    /// </summary>
    public bool Exists => Items.Count > 0;

    /// <summary>
    /// Dropdown に表示するクラスのリスト
    /// </summary>
    protected List<T> Items { get; private set; }

    /// <summary>
    /// Dropdown に表示するクラスのリストを取得する
    /// </summary>
    /// <returns>クラスのリスト</returns>
    protected abstract List<T> GetItems();

    /// <summary>
    /// Dropdown でクラスを選択した場合の処理<br/>
    /// クラスの任意のフィールド値（例：デバイス名）の更新等を行う
    /// </summary>
    /// <param name="select"></param>
    protected abstract void SelectItem(int select);

    /// <summary>
    /// クラスの任意のフィールド値から Dropdown に表示するための文字を取得する
    /// </summary>
    /// <param name="item">Dropdown に表示するクラス</param>
    /// <returns>表示文字列</returns>
    protected abstract string GetItemName(T item);

    protected override void OnValueChangedInternal()
    {
        if (Items.Count > value)
        {
            SelectItem(value);
        }
    }

    public override async void Refresh()
    {
        Items = await Task.Run(() => GetItems());

        string selectedText = captionText.text;

        ClearOptions();

        if (Exists)
        {
            int select = 0;

            foreach (var item in Items.Select((value, index) => new { value, index }))
            {
                string itemName = GetItemName(item.value);
                options.Add(new OptionData(itemName));
                if (selectedText == itemName)
                {
                    select = item.index;
                }
            }

            SelectItem(select);
            captionText.text = options[select].text;
            value = select;
        }
    }
}