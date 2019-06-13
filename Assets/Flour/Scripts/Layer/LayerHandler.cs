using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx.Async;

namespace Flour.Layer
{
	public sealed class LayerHandler<TLayerKey, TSubKey> where TLayerKey : struct where TSubKey : struct
	{
		SortedList<int, TLayerKey> layerOrder = new SortedList<int, TLayerKey>();
		readonly Dictionary<TLayerKey, Layer<TLayerKey, TSubKey>> layers = new Dictionary<TLayerKey, Layer<TLayerKey, TSubKey>>();

		readonly SafeAreaHandler<TLayerKey> safeAreaHandler;

		public LayerHandler()
		{
			Assert.IsTrue(typeof(TLayerKey).IsEnum, "TLayerKey can use only enum.");
			Assert.IsTrue(typeof(TSubKey).IsEnum, "TSubKey can use only enum.");

			safeAreaHandler = new SafeAreaHandler<TLayerKey>(new Vector2(Screen.width, Screen.height), Screen.safeArea);
		}

		public void AddLayer(TLayerKey layerType, int sortingOrder, Transform canvasRoot, Vector2 referenceResolution, bool safeArea)
		{
			if (safeArea)
			{
				safeAreaHandler.AddSafeLayer(layerType);
			}
			if (layers.ContainsKey(layerType))
			{
				throw new ArgumentException($"same key already exists. key => {layerType}");
			}

			var layer = new Layer<TLayerKey, TSubKey>(canvasRoot, layerType, sortingOrder, referenceResolution);
			if (safeArea)
			{
				safeAreaHandler.Reduction(layerType, layer.Parent);
			}

			layers.Add(layerType, layer);

			layerOrder.Add(sortingOrder, layerType);
		}

		public void AddLayer(TLayerKey layerType, int sortingOrder, Transform canvasRoot, Vector2 referenceResolution, RenderMode renderMode, Camera camera)
		{
			if (layers.ContainsKey(layerType))
			{
				throw new ArgumentException($"same key already exists. key => {layerType}");
			}
			var layer = new Layer<TLayerKey, TSubKey>(canvasRoot, layerType, sortingOrder, referenceResolution, renderMode, camera);
			layers.Add(layerType, layer);
		}

		public async UniTask RemoveLayer(TLayerKey layerType)
		{
			if (!layers.ContainsKey(layerType))
			{
				return;
			}
			await RemoveAll(layerType);
			layers.Remove(layerType);
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

		private Layer<TLayerKey, TSubKey> GetLayer(TLayerKey layerType)
		{
			if (!layers.ContainsKey(layerType))
			{
				throw new NullReferenceException($"not found {layerType} layer canvas.");
			}
			return layers[layerType];
		}

		public T GetFirst<T>(TLayerKey layerKey, TSubKey subKey) where T : AbstractSubLayer<TLayerKey, TSubKey>
		{
			var layer = GetLayer(layerKey);
			return (T)layer.List.FirstOrDefault(subKey);
		}

		public IEnumerable<T> Get<T>(TLayerKey layerType, TSubKey key) where T : AbstractSubLayer<TLayerKey, TSubKey>
		{
			var layer = GetLayer(layerType);
			return layer.List.Find(key).Select(x => (T)x);
		}

		public T Add<T>(TLayerKey layerType, TSubKey key, T prefab, bool overlap) where T : AbstractSubLayer<TLayerKey, TSubKey>
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

		void MoveFront(TLayerKey layerType, AbstractSubLayer<TLayerKey, TSubKey> subLayer)
		{
			var layer = GetLayer(layerType);

			if (layer.List.FindIndex(subLayer) == 0)
			{
				return;
			}

			layer.List.Remove(subLayer, false);
			layer.List.Add(subLayer);
		}

		public async UniTask<bool> Remove(TLayerKey layer, bool force = false)
		{
			var sub = GetLayer(layer).List.FirstOrDefault();
			if (sub != null)
			{
				await sub.CloseWait(force);
			}
			return sub != null;
		}
		public async UniTask RemoveAll(TLayerKey layer)
		{
			var l = GetLayer(layer).List.SubLayers;
			for (int i = l.Count - 1; i >= 0; i--)
			{
				await l[i].CloseWait(true);
			}
		}
		public async UniTask RemoveAll()
		{
			foreach (var layer in layerOrder.Values.Reverse())
			{
				await RemoveAll(layer);
			}
		}
		async UniTask Remove(AbstractSubLayer<TLayerKey, TSubKey> subLayer, bool force)
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

			await subLayer.OnCloseInternal(force);
			layer.List.Remove(subLayer, true);
		}
	}
}
