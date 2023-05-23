# Windows Unity Domemaster サンプル

RICOH Live Streaming Client SDK for Windows を使用したサンプルアプリケーションです。
RICOH THETA から受信した360度映像をドームマスター形式に変換して画面出力します。

サービスのご利用には、API利用規約への同意とアカウントの登録、ソフトウェア利用許諾書への同意が必要です。
詳細は下記Webサイトをご確認ください。

* サービスサイト: https://livestreaming.ricoh/
* ソフトウェア開発者向けサイト: https://api.livestreaming.ricoh/
* アカウント登録: https://console.livestreaming.mw.smart-integration.ricoh.com/login/register
* ソフトウェア使用許諾契約書 : [Software License Agreement](../../SoftwareLicenseAgreement.txt)
* NOTICE: This package includes SDK and sample application(s) for "RICOH Live Streaming Service".
At this moment, we provide API license agreement / software license agreement only in Japanese.

## 使用方法

### カメラ操作方法

| 操作                          | カメラ動作                                    |
| ----------------------------- | --------------------------------------------- |
| キーボード W                  | 前移動                                        |
| キーボード A                  | 左移動                                        |
| キーボード S                  | 後移動                                        |
| キーボード D                  | 右移動                                        |
| キーボード E                  | 上移動                                        |
| キーボード Q                  | 下移動                                        |
| キーボード ESC                | Position および Rotation の X/Y/Z を 0 に設定 |
| キーボード SPACE              | UI表示/非表示切り替え                         |
| マウス右ボタン + 上下ドラッグ | 垂直方向（チルト）                            |
| マウス右ボタン + 左右ドラッグ | 水平方向（パン）                              |

### UI操作
| 名称                   | 機能                                                     | 備考               |
| ---------------------- | -------------------------------------------------------- | ------------------ |
| Reset(Position) ボタン | Position の X/Y/Z を 0 に設定                            |                    |
| Reset(Rotation) ボタン | Rotation の X/Y/Z を 0 に設定                            |                    |
| Save ボタン            | Position および Rotation の X/Y/Z をファイルに書き込む   | JSON形式で保存(*1) |
| Load ボタン            | Position および Rotation の X/Y/Z をファイルから読み込む | JSON形式で保存(*1) |

(*1) 保存場所 : C:/Users/ユーザー名/AppData/LocalLow/RICOH/ClientSDKForWindows-UnitySample/UserData.json

### カメラパラメータ
カメラの操作と連動して各パラメータの X/Y/Z が変化する。  
また、各パラメータ X/Y/Z の値を入力することでカメラの操作が可能。

| パラメータ |        X         |       Y        |        Z         |      範囲      |
| :--------: | :--------------: | :------------: | :--------------: | :------------: |
|  Position  |     左右位置     |    上下位置    |     前後位置     | -50 ～ 50 程度 |
|  Rotation  | 垂直角（チルト） | 水平角（パン） | 回転角（ロール） |    0 ～ 359    |

## 対応バージョン
* Unity : 2021.3.11f1
* windows-unity-sdk : v2.0.0