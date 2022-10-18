# Windows Unity PCVR サンプル

RICOH Live Streaming Client SDK for Windows を使用した Windows Unity PCVR サンプルアプリケーションです。

サービスのご利用には、API利用規約への同意とアカウントの登録、ソフトウェア利用許諾書への同意が必要です。
詳細は下記Webサイトをご確認ください。

* サービスサイト: https://livestreaming.ricoh/
* ソフトウェア開発者向けサイト: https://api.livestreaming.ricoh/
* アカウント登録: https://livestreaming.ricoh/user_register.html
* ソフトウェア使用許諾契約書 : [Software License Agreement](../../SoftwareLicenseAgreement.txt)
* NOTICE: This package includes SDK and sample application(s) for "RICOH Live Streaming Service".
At this moment, we provide API license agreement / software license agreement only in Japanese.

## 事前準備
[HTC VIVE Pro](https://www.vive.com/jp/setup/) および [SteamVR](https://store.steampowered.com/app/250820/SteamVR/?l=japanese)をセットアップする

## ビルド方法

1. Unity Hub で [unity-app-pcvr](./) をリストに追加し起動する
1. Projectの `Assets > Senes` で Scene をダブルクリックする
1. Client ID, Secret, Room ID を取得する
1. 設定ファイルを作成する。
1. `File > Build Settings > Build` を選択し、任意のフォルダに `ClientSDKForWindows-UnitySample.exe` を作成する。

### 設定ファイル

* [Secrets.template.json](./Secrets.template.json) を [Assets/StreamingAssets/](./Assets/StreamingAssets) にコピーし、`Secrets.json` に名前を変更する。
* `Secrets.json` を編集する。
    * `client_id`、 `client_secret`、`room_id` は実際の値を入力する。
``` json
{
    "client_id" : "",
    "client_secret" : "",
    "room_id" : "xxxxxx",
    "video_bitrate" : 20000
}
```

## 起動方法
1. HTC VIVE Pro の各種デバイスの電源をONにする
1. SteamVRを起動する
1. ClientSDKForWindows-UnitySample.exe を実行する

## 操作方法
1. RoomIDを入力する（"Keyboard" ボタン押下で仮想キーボードの表示/非表示が切り替え可能）
1. ドロップダウンリストから使用するオーディオデバイス（マイク・スピーカー）を選択する
1. "Connect" ボタンを押下し接続する

コントローラの各部名称・配置は [VIVE コントローラについて](https://www.vive.com/jp/support/vive/category_howto/about-the-controllers.html) を参照。

| 操作                     | 機能                                                       |
| ------------------------ | ---------------------------------------------------------- |
| トリガー押下             | メニューの操作                                             |
| トラックパッド左側タッチ | Theta 360度映像左回転（Theta 360度映像の表示時のみ有効）   |
| トラックパッド右側タッチ | Theta 360度映像右回転（Theta 360度映像の表示時のみ有効）   |
| トラックパッド押下       | Theta 360度映像初期位置（Theta 360度映像の表示時のみ有効） |
| グリップボタン押下       | 表示映像切替（接続先が複数存在する場合のみ有効）           |
| メニューボタン押下       | メニュー表示・非表示切替                                   |

## 対応HMD
- HTC VIVE Pro

## 対応バージョン
- Unity : 2021.3.11f1
- windows-unity-sdk : v2.0.0

## 対応プラットフォーム
- Windows10 21H1 x86_64以降
