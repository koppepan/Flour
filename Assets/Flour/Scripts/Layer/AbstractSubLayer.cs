using System;
using UnityEngine;
using UniRx.Async;

namespace Flour.Layer
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class AbstractSubLayer<TKey> : MonoBehaviour where TKey : struct
	{
		public TKey Key { get; private set; }
		private LayerType currentLayer;

		private Action<LayerType, AbstractSubLayer<TKey>> moveFront;
		private Action<LayerType, RectTransform> safeAreaExpansion;
		private Func<AbstractSubLayer<TKey>, UniTask> onDestroy;

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

		internal void SetConstParameter(LayerType layerType, TKey key,
			Action<LayerType, AbstractSubLayer<TKey>> moveFront,
			Action<LayerType, RectTransform> safeAreaExpansion,
			Func<AbstractSubLayer<TKey>, UniTask> onDestroy
			)
		{
			currentLayer = layerType;
			Key = key;

			this.moveFront = moveFront;
			this.safeAreaExpansion = safeAreaExpansion;
			this.onDestroy = onDestroy;
		}

		public void MoveFront() => moveFront(currentLayer, this);
		public void Close() => onDestroy(this);

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

		protected void SafeAreaExpansion() => SafeAreaExpansion(GetComponent<RectTransform>());
		protected void SafeAreaExpansion(RectTransform rect) => safeAreaExpansion(currentLayer, rect);
	}
}
