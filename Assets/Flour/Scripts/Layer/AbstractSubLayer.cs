﻿using System;
using UnityEngine;

namespace Flour.Layer
{
	public abstract class AbstractSubLayer : MonoBehaviour
	{
		public SubLayerType SubLayer { get; private set; }
		public virtual bool IgnoreBack { get { return false; } }

		Action<RectTransform> safeAreaExpansion;
		Action<AbstractSubLayer> onDestroy;

		internal void SetConstParameter(SubLayerType type, Action<RectTransform> safeAreaExpansion, Action<AbstractSubLayer> onDestroy)
		{
			SubLayer = type;

			this.safeAreaExpansion = safeAreaExpansion;
			this.onDestroy = onDestroy;
		}

		public virtual void Close() => onDestroy?.Invoke(this);

		public virtual void OnOpen() { }
		public virtual void OnClose() => Destroy(gameObject);

		public virtual void OnBack() { }
		public virtual void OnChangeSiblingIndex(int index) { }

		protected void SafeAreaExpansion() => safeAreaExpansion?.Invoke(GetComponent<RectTransform>());
		protected void SafeAreaExpansion(RectTransform rect) => safeAreaExpansion?.Invoke(rect);
	}
}
