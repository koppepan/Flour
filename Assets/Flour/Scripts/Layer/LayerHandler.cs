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

	public sealed class LayerHandler
	{
		readonly SubLayerSourceRepository[] repositories;

		readonly LayerType[] layerOrder;
		readonly Dictionary<LayerType, Layer> layers = new Dictionary<LayerType, Layer>();

		readonly SafeAreaHandler safeAreaHandler;

		public LayerHandler(Transform canvasRoot, Vector2 referenceResolution, SubLayerSourceRepository[] repositories, LayerType[] safeAreaLayers)
		{
			this.repositories = repositories;

			safeAreaHandler = new SafeAreaHandler(new Vector2(Screen.width, Screen.height), Screen.safeArea, safeAreaLayers);

			var layerTypes = Enum.GetValues(typeof(LayerType)).Cast<LayerType>().Where(x => x != LayerType.Debug);
			foreach (var type in layerTypes)
			{
				layers.Add(type, CreateLayer(type, canvasRoot, referenceResolution, safeAreaLayers.Contains(type)));
			}

			layerOrder = layerTypes.Reverse().ToArray();
		}
		public void AddDebugLayer(Transform canvasRoot, Vector2 referenceResolution)
		{
			layers.Add(LayerType.Debug, CreateLayer(LayerType.Debug, canvasRoot, referenceResolution, false));
		}

		private Layer CreateLayer(LayerType layerType, Transform canvasRoot, Vector2 referenceResolution, bool safeArea)
		{
			var layer = new GameObject(layerType.ToString(), typeof(Layer)).GetComponent<Layer>();
			layer.transform.SetParent(canvasRoot);

			var reduction = safeArea ? safeAreaHandler.Reduction : (Action<LayerType, RectTransform>)null;
			layer.Initialize(layerType, referenceResolution, reduction);

			return layer;
		}

		public bool OnBack()
		{
			foreach (var layer in layerOrder)
			{
				if (layers[layer].OnBack())
				{
					return true;
				}
			}
			return false;
		}

		private async UniTask<T> LoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			for (int i = 0; i < repositories.Length; i++)
			{
				if (repositories[i].ContainsKey(type))
				{
					return await repositories[i].LoadAsync<T>(type);
				}
			}
			return null;
		}

		public async UniTask<T> AddAsync<T>(LayerType layerType, SubLayerType subLayerType, bool overlap) where T : AbstractSubLayer
		{
			if (!layers.ContainsKey(layerType))
			{
				Debug.LogError($"not found {layerType} layer canvas.");
				return null;
			}

			var layer = layers[layerType];
			var old = layer.List.FirstOrDefault(subLayerType);

			// 既に同じSubLayerが存在する
			if (!overlap && old != null)
			{
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

			var prefab = await LoadAsync<T>(subLayerType);

			if (prefab == null)
			{
				return null;
			}

			var sub = GameObject.Instantiate(prefab);
			sub.SetConstParameter(layerType, subLayerType, MoveFront, Remove, safeAreaHandler.Expansion);
			sub.OnOpenInternal();
			layer.List.Add(sub);
			return sub;
		}

		void MoveFront(LayerType layer, AbstractSubLayer subLayer)
		{
			layers[layer].List.Remove(subLayer, false);
			layers[layer].List.Add(subLayer);
		}

		public bool Remove(LayerType layer)
		{
			var sub = layers[layer].List.FirstOrDefault();
			sub?.Close();
			return sub != null;
		}
		public void RemoveAll()
		{
			for (int i = 0; i < layerOrder.Length; i++)
			{
				while (Remove(layerOrder[i])) { }
			}
		}
		async UniTask Remove(AbstractSubLayer subLayer)
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
