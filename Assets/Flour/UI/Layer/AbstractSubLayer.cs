using System;
using UnityEngine;

namespace Flour.UI
{
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public Guid Identify { get; private set; }
		public abstract SubLayerType LayerType { get; }

		public void Initialize(Guid guid)
		{
			Identify = guid;
		}

		public abstract void OnOpen();
		public abstract void OnClose();
		public abstract void OnActivate();
		public abstract void OnInactivate();
		public abstract bool OnBackKey();
	}
}
