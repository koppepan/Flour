using System;
using UnityEngine;

namespace Flour.UI
{
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public SubLayerType LayerType { get; private set; }
		public virtual bool IgnoreBack { get { return false; } }

		Action<AbstractSubLayer> onDestroy;

		public void Initialize(SubLayerType type, Action<AbstractSubLayer> onDestroy)
		{
			LayerType = type;
			this.onDestroy = onDestroy;
		}

		public virtual void OnOpen() { }
		public virtual void OnBack() { }
		public virtual void OnChangeSiblingIndex(int index) { }

		public virtual void Close()
		{
			onDestroy?.Invoke(this);
		}

		public virtual void OnClose()
		{
			Destroy(gameObject);
		}

	}
}
