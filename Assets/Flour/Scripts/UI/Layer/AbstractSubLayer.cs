using System;
using UnityEngine;

namespace Flour.UI
{
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public SubLayerType LayerType { get; private set; }

		Action<AbstractSubLayer> onDestroy;

		public void Initialize(SubLayerType type, Action<AbstractSubLayer> onDestroy)
		{
			LayerType = type;
			this.onDestroy = onDestroy;
		}
		private void OnDestroy()
		{
			onDestroy?.Invoke(this);
		}

		public abstract void OnOpen();
		public abstract void OnClose();
		public abstract void OnActivate();
		public abstract void OnInactivate();
		public abstract bool OnBackKey();
	}
}
