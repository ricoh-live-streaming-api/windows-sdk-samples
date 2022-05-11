using System;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// <see cref="UserData"/>のシリアライザ
/// </summary>
public static class UserDataSerializer
{
    /// <summary>
    /// JSONファイルパス
    /// </summary>
    public static string FilePath { get; private set; }

    /// <summary>
    /// Json形式のファイルを読み込み、<see cref="UserData"/>に変換する
    /// </summary>
    /// <param name="filePath">JSONファイルパス</param>
    /// <returns>変換結果</returns>
    public static UserData Load(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        if (File.Exists(filePath))
        {
            using (var streamReader = new StreamReader(filePath))
            {
                string data = streamReader.ReadToEnd();

                return JsonConvert.DeserializeObject<UserData>(data);
            }
        }
        else
        {
            return new UserData();
        }
    }

    /// <summary>
    /// 指定されたファイルに<see cref="UserData"/>をJSON形式で保存する。
    /// </summary>
    /// <param name="userData">保存するデータ</param>
    public static void Save(UserData userData)
    {
        string json = JsonConvert.SerializeObject(userData);
        using (var streamWriter = new StreamWriter(FilePath))
        {
            streamWriter.Write(json);
        }
    }
}