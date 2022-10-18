# Device Sample

## 1. 機能
* カメラ、マイク、スピーカーを検出し、ドロップダウンリストに表示する。
* ドロップダウンリストから選択したカメラ、マイク、スピーカーを切り替える。
* 選択中のカメラで適用可能な解像度・Frame rate(fps)に切り替える。

## 2. 使用方法
下記ドロップダウンリストにて各種デバイスを切り替える。  
USB外部カメラ等の接続・切断を検知した場合、`WindowProcedureHookListener#OnDevicesChanged` イベントにてドロップダウンリストの更新を行う。

* Camera  
  自身の映像表示に使用するカメラデバイスを切り替える。  
  また、切り替えたデバイスで適用可能な解像度・Frame rate(fps) の一覧を表示する。

* Resolution / Frame rate  
  自身の映像表示に使用する解像度・Frame rate(fps) を切り替える。  
  切り替え時、新しいデバイス名、解像度、Frame rate を使用して Client SDK API の `GetUserMedia` メソッドを呼び出し VideoTrack を生成する。  
  また、生成した VideoTrack を使用して Client SDK API の `ReplaceMediaStreamTrack` メソッドを呼び出す。

* Mic  
  マイクを切り替える。  
  切り替え時、Client SDK API の `SetAudioInputDevice` メソッドを呼び出し、Client に新しいデバイス名を設定する。

* Speaker  
  スピーカーを切り替える。  
  切り替え時、Client SDK API の `SetAudioOutputDevice` メソッドを呼び出し、Client に新しいデバイス名を設定する。

## 3. 参考リンク
Client SDK API の詳細は下記を参照
* [RICOH Live Streaming Client SDK API 外部仕様](https://api.livestreaming.ricoh/document/ricoh-live-streaming-client-sdk-api-%e5%a4%96%e9%83%a8%e4%bb%95%e6%a7%98/)
* [RICOH Live Streaming Client SDK for Windows APIドキュメント](https://github.com/ricoh-live-streaming-api/windows-unity-sdk/tree/main/doc)

## 4. 対応バージョン
* Unity : 2021.3.11f1
* windows-unity-sdk : v2.0.0