# Windows Unity Sample

RICOH Live Streaming Client SDK for Windows を使用した Windows Unity サンプルアプリケーションです。

サービスのご利用には、API利用規約への同意とアカウントの登録、ソフトウェア利用許諾書への同意が必要です。  
詳細は下記Webサイトをご確認ください。

* サービスサイト: https://livestreaming.ricoh/
  * ソフトウェア開発者向けサイト: https://api.livestreaming.ricoh/
* ソフトウェア使用許諾契約書 : [Software License Agreement](../SoftwareLicenseAgreement.txt)

* NOTICE: This package includes SDK and sample application(s) for "RICOH Live Streaming Service".  
At this moment, we provide API license agreement / software license agreement only in Japanese.

## 事前準備
* Client ID, Secret の設定  
各サンプルの Script（ソースファイル）に Client ID, Secret を設定する。
    ``` C#
    public override string ClientId => "Exsample_ClientID_0123456789";
    public override string ClientSecret => "Exsample_ClientSecret_0123456789";
    ```

## ビルド方法

1. Unity Hub で任意のサンプルをリストに追加し起動する。
1. Project の `Assets > Scenes` でサンプルの Scene をダブルクリックする。
1. `File > Build Settings > Build` を選択し、任意のフォルダに `ClientSDKForWindows-UnitySample.exe` を作成する。

## ログ出力

下記2種類のログを出力する。  

| 種類            | 内容                                                                        | 出力設定                                                                          | 出力場所                                                                                 |
| --------------- | --------------------------------------------------------------------------- | --------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| SDK・アプリログ | SDKやアプリが出力する [log4net](https://logging.apache.org/log4net/) のログ | 各サンプル/Assets/StreamingAssets/Log4net.Config.xml                              | C:/Users/ユーザー名/AppData/LocalLow/RICOH/ClientSDKForWindows-UnitySample/logs/main.log |
| Unityログ       | Unityが出力するプレイヤーログ                                               | [Unityマニュアル](https://docs.unity3d.com/ja/2020.3/Manual/LogFiles.html) を参照 | C:/Users/ユーザー名/AppData/LocalLow/RICOH/ClientSDKForWindows-UnitySample/Player.log    |

## サンプル一覧
サンプルの詳細は各リンク先を参照

| サンプル                   | 概要                                                                    | Scene                | Script              | Client SDK API                                                                                                                              |
| -------------------------- | ----------------------------------------------------------------------- | -------------------- | ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| [device](./device)         | カメラ、マイク、スピーカーのデバイス検出と切り替え                      | DeviceSampleScene    | DeviceSample.cs     | Client#ReplaceMediaStreamTrack<br>Client#SetAudioInputDevice<br>Client#SetAudioOutputDevice<br>WindowProcedureHookListener#OnDevicesChanged |
| [domemaster](./domemaster) | RICOH THETA から受信した360度映像をドームマスター形式に変換して画面出力 | DomemasterScene      | DomemasterSample.cs |                                                                                                                                             |
| [meta](./meta)             | Connection/Track Metadata 更新通知の送受信                              | MetaSampleScene      | MetaSample.cs       | Client#UpdateMeta<br>Client#UpdateTrackMeta<br>IClientListener#OnUpdateRemoteTrack<br>IClientListener#OnUpdateRemoteConnection              |
| [mute](./mute)             | ミュート状態の変更とミュート状態更新通知の送受信                        | MetaSampleScene      | MuteSample.cs       | Client#ChangeMute<br>IClientListener#OnUpdateMute                                                                                           |
| [selective](./selective)   | 相手映像の選択受信設定                                                  | SelectiveSampleScene | SelectiveSample.cs  | Client#ChangeMediaRequirements                                                                                                              |
