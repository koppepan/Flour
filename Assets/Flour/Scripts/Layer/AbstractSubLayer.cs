using System;
using UnityEngine;
using UniRx.Async;

namespace Flour.Layer
{
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public int SubLayerId { get; private set; }
		public virtual bool IgnoreBack { get { return false; } }

		private LayerType currentLayer;

		private Action<LayerType, AbstractSubLayer> moveFront;
		private Func<AbstractSubLayer, UniTask> onDestroy;

		private Action<LayerType, RectTransform> safeAreaExpansion;

		internal void SetConstParameter(
			LayerType layer,
			int subLayerId,
			Action<LayerType, AbstractSubLayer> moveFront,
			Func<AbstractSubLayer, UniTask> onDestroy,
			Action<LayerType, RectTransform> safeAreaExpansion)
		{
			currentLayer = layer;
			SubLayerId = subLayerId;

			this.moveFront = moveFront;
			this.onDestroy = onDestroy;

			this.safeAreaExpansion = safeAreaExpansion;
		}

		public void MoveFront() => moveFront?.Invoke(currentLayer, this);
		public void Close() => onDestroy?.Invoke(this);

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

		protected void SafeAreaExpansion() => safeAreaExpansion?.Invoke(currentLayer, GetComponent<RectTransform>());
		protected void SafeAreaExpansion(RectTransform rect) => safeAreaExpansion?.Invoke(currentLayer, rect);
	}
}
