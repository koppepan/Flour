using System;
using UnityEngine;
using UniRx.Async;

namespace Flour.Layer
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public SubLayerType SubLayer { get; private set; }
		public virtual bool IgnoreBack { get { return false; } }

		private CanvasGroup _canvasGroup;
		protected CanvasGroup CanvasGroup
		{
			get
			{
				if (_canvasGroup == null)
				{
					_canvasGroup = GetComponent<CanvasGroup>();
				}
				return _canvasGroup;
			}
		}

		private LayerType currentLayer;

		private Action<LayerType, AbstractSubLayer> moveFront;
		private Func<AbstractSubLayer, UniTask> onDestroy;

		private Action<LayerType, RectTransform> safeAreaExpansion;

		internal void SetConstParameter(
			LayerType layer,
			SubLayerType subLayer,
			Action<LayerType, AbstractSubLayer> moveFront,
			Func<AbstractSubLayer, UniTask> onDestroy,
			Action<LayerType, RectTransform> safeAreaExpansion)
		{
			currentLayer = layer;
			SubLayer = subLayer;

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
