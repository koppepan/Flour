# Flour

### 思想

* **Singletonを使わずに親クラスで依存を解決しApplication全体を一元管理する**
* **MonoBehaviourをGameObjectが必要な部分以外では使用しない**

---
### Flour.Common

* 汎用的に使えそうな

### Flour.Scene

* SceneLoad/UnloadなどをUnityの機能に投げっぱなしにするのではなく自分で管理する

### Flour.Layer

* uGuiをprefabで直接管理するのではなくCanvasをLayer, 各UIをSubLayerとして管理する
* Backキー対応を一元管理する
* 暗転などの汎用的な機能をstaticにせずにSubLayerとして管理することで依存関係をシンプルにする

---

### Example

* FlourComponentを使用しApplicationManagerでApplicationを一元管理するSample
* Unityの機能に依存する処理をできるだけApplicationManagerで行い各Sceneに必要な情報をinterface経由で渡す

