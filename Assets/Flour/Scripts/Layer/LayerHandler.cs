using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Flour.Layer
{
	public enum LayerType
	{
		Back = 10,
		Middle = 11,
		Front = 12,
		System = 13,

		Debug = 100,
	}

	public sealed class LayerHandler<TKey> where TKey : struct
	{
		SortedList<int, LayerType> layerOrder = new SortedList<int, LayerType>();
		readonly Dictionary<LayerType, Layer<TKey>> layers = new Dictionary<LayerType, Layer<TKey>>();

		readonly SafeAreaHandler safeAreaHandler;

		public LayerHandler()
		{
			safeAreaHandler = new SafeAreaHandler(new Vector2(Screen.width, Screen.height), Screen.safeArea);
		}

		public void AddLayer(LayerType layerType, int sortingOrder, Transform canvasRoot, Vector2 referenceResolution, bool safeArea)
		{
			if (safeArea)
			{
				safeAreaHandler.AddSafeLayer(layerType);
			}
			if (layers.ContainsKey(layerType))
			{
				throw new ArgumentException($"same key already exists. key => {layerType}");
			}

			var reduction = safeArea ? safeAreaHandler.Reduction : (Action<LayerType, RectTransform>)null;
			var layer = new Layer<TKey>(canvasRoot, layerType, sortingOrder, referenceResolution, reduction);

			layers.Add(layerType, layer);

			layerOrder.Add(sortingOrder, layerType);
		}

		public bool OnBack()
		{
			foreach (var layer in layerOrder.Values.Reverse())
			{
				if (layers[layer].OnBack())
				{
					return true;
				}
			}
			return false;
		}

		private Layer<TKey> GetLayer(LayerType layerType)
		{
			if (!layers.ContainsKey(layerType))
			{
				throw new NullReferenceException($"not found {layerType} layer canvas.");
			}
			return layers[layerType];
		}

		public T Get<T>(LayerType layerType, TKey key) where T : AbstractSubLayer<TKey>
		{
			var layer = GetLayer(layerType);
			var old = layer.List.FirstOrDefault(key);

			if (old == null)
			{
				return null;
			}

			// 既に同じSubLayerが存在する
			// すでに存在していて一番前にある
			if (layer.List.FindIndex(old) == 0)
			{
				return (T)old;
			}

			// 一番前に来るように詰めなおして返す
			layer.List.Remove(old, false);
			layer.List.Add(old);
			return (T)old;
		}

		public T Add<T>(LayerType layerType, TKey key, T prefab, bool overlap) where T : AbstractSubLayer<TKey>
		{
			var layer = GetLayer(layerType);
			var old = layer.List.FirstOrDefault(key);

			if (!overlap && old != null)
			{
				throw new ArgumentException($"same key already exists. key => {key}");
			}

			var sub = GameObject.Instantiate(prefab);
			sub.SetConstParameter(layerType, key, MoveFront, safeAreaHandler.Expansion, Remove);
			sub.OnOpenInternal();
			layer.List.Add(sub);
			return sub;
		}

		void MoveFront(LayerType layerType, AbstractSubLayer<TKey> subLayer)
		{
			var layer = GetLayer(layerType);
			layer.List.Remove(subLayer, false);
			layer.List.Add(subLayer);
		}

		public bool Remove(LayerType layer)
		{
			var sub = GetLayer(layer).List.FirstOrDefault();
			sub?.Close();
			return sub != null;
		}
		public void RemoveAll()
		{
			foreach (var layer in layerOrder.Values.Reverse())
			{
				while (Remove(layer)) { }
			}
		}
		async UniTask Remove(AbstractSubLayer<TKey> subLayer)
		{
			if (subLayer == null)
			{
				return;
			}
			var layer = layers.Values.FirstOrDefault(x => x.List.FindIndex(subLayer) != -1);
			if (layer == null)
			{
				return;
			}

			await subLayer.OnCloseInternal();
			layer.List.Remove(subLayer, true);
		}
	}
}
