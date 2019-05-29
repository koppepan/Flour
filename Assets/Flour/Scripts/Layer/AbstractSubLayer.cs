using System;
using UnityEngine;
using UniRx.Async;

namespace Flour.Layer
{
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public LayerType Layer { get; private set; }
		public SubLayerType SubLayer { get; private set; }
		public virtual bool IgnoreBack { get { return false; } }

		Action<LayerType, RectTransform> safeAreaExpansion;
		Func<AbstractSubLayer, UniTask> onDestroy;

		internal void SetConstParameter(LayerType layer, SubLayerType subLayer, Action<LayerType, RectTransform> safeAreaExpansion, Func<AbstractSubLayer, UniTask> onDestroy)
		{
			Layer = layer;
			SubLayer = subLayer;

			this.safeAreaExpansion = safeAreaExpansion;
			this.onDestroy = onDestroy;
		}

		public virtual void Close() => onDestroy?.Invoke(this);

		internal void OnOpenInternal() => OnOpen();
		internal async UniTask OnCloseInternal() => await OnClose();
		internal void OnBackInternal() => OnBack();
		internal void OnChangeSiblingIndexInternal(int index) => OnChangeSiblingIndex(index);

		protected virtual void OnOpen() { }
		protected virtual async UniTask OnClose()
		{
			await UniTask.DelayFrame(1);
			Destroy(gameObject);
		}
		protected virtual void OnBack() { }
		protected virtual void OnChangeSiblingIndex(int index) { }

		protected void SafeAreaExpansion() => safeAreaExpansion?.Invoke(Layer, GetComponent<RectTransform>());
		protected void SafeAreaExpansion(RectTransform rect) => safeAreaExpansion?.Invoke(Layer, rect);
	}
}
