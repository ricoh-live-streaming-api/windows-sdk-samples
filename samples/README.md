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
1. SDK の配置  
[windows-unity-sdk](https://github.com/ricoh-live-streaming-api/windows-unity-sdk) の `unity_app\Assets` 配下にある `Plugins` と `StreamingAssets` フォルダを、各サンプルの `Assets` 配下に配置する。

1. Client ID, Secret の設定  
各サンプルの Script（ソースファイル）に Client ID, Secret を設定する。
    ``` C#
    public override string ClientId => "Exsample_ClientID_0123456789";
    public override string ClientSecret => "Exsample_ClientSecret_0123456789";
    ```

## ビルド方法

1. Unity Hub で任意のサンプルをリストに追加し起動する。
1. Project の `Assets > Scenes` でサンプルの Scene をダブルクリックする。
1. `File > Build Settings > Build` を選択し、任意のフォルダに `ClientSDKForWindows-UnitySample.exe` を作成する。

## サンプル一覧
サンプルの詳細は各リンク先を参照

| サンプル                 | 概要                                       | Scene           | Script        | Client SDK API                                                                                                                 |
| ------------------------ | ------------------------------------------ | --------------- | ------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| [meta](./meta/README.md) | Connection/Track Metadata 更新通知の送受信 | MetaSampleScene | MetaSample.cs | Client#UpdateMeta<br>Client#UpdateTrackMeta<br>IClientListener#OnUpdateRemoteTrack<br>IClientListener#OnUpdateRemoteConnection |
