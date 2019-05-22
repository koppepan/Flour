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
	}

	public sealed class LayerHandler
	{
		SubLayerSourceRepository[] repositories;

		LayerType[] layerOrder;
		Dictionary<LayerType, Layer> layers = new Dictionary<LayerType, Layer>();

		Vector2 safeAreaOffsetMin;
		Vector2 safeAreaOffsetMax;

		public LayerHandler(Transform canvasRoot, Vector2 referenceResolution, params SubLayerSourceRepository[] repositories)
		{
			this.repositories = repositories;

			var screenSize = new Vector2(Screen.width, Screen.height);
			var safeArea = SafeAreaSimulateData.GetSafeArea(screenSize, Screen.safeArea);

			safeAreaOffsetMin = new Vector2(safeArea.position.x + 2, safeArea.position.y + 2);
			safeAreaOffsetMax = new Vector2(-(screenSize.x - (safeArea.position.x + safeArea.width - 2)), -(screenSize.y - (safeArea.position.y + safeArea.height - 2)));

			var layerTypes = Enum.GetValues(typeof(LayerType)).Cast<LayerType>();
			foreach (var type in layerTypes)
			{
				var layer = new GameObject(type.ToString(), typeof(Layer)).GetComponent<Layer>();
				layer.transform.SetParent(canvasRoot);
				layer.Initialize(type, referenceResolution, safeArea, screenSize);

				layers.Add(type, layer);
			}

			layerOrder = layerTypes.Reverse().ToArray();
		}

		private void SafeAreaExpansion(RectTransform rect)
		{
			rect.offsetMin = -safeAreaOffsetMin;
			rect.offsetMax = -safeAreaOffsetMax;
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

		public async UniTask<AbstractSubLayer> AddAsync(LayerType layerType, SubLayerType subLayerType)
		{
			return await AddAsync<AbstractSubLayer>(layerType, subLayerType);
		}

		public async UniTask<T> AddAsync<T>(LayerType layerType, SubLayerType subLayerType) where T : AbstractSubLayer
		{
			var prefab = await LoadAsync<T>(subLayerType);

			if (prefab == null)
			{
				return null;
			}

			return (T)Add(layerType, subLayerType, prefab);
		}

		private AbstractSubLayer Add(LayerType layerType, SubLayerType subLayerType, AbstractSubLayer prefab)
		{
			var layer = layers[layerType];

			var current = layer.Stack.Peek();
			var old = layer.Stack.FirstOrDefault(subLayerType);

			// 開こうとしたSubLayerが存在しないので新しく生成
			if (old == null)
			{
				var sub = GameObject.Instantiate(prefab);
				sub.Initialize(subLayerType, SafeAreaExpansion, Remove);
				sub.OnOpen();
				layer.Stack.Push(sub);
				return sub;
			}

			// 開こうとしたSubLayerがすでに存在していて一番前にある
			if (current == old)
			{
				return current;
			}

			// 開こうとしたSubLayerがすでに存在していて一番前にはない
			layer.Stack.Push(old);
			return old;
		}

		public void Remove(LayerType layer)
		{
			layers[layer].Stack.Peek()?.Close();
		}
		void Remove(AbstractSubLayer subLayer)
		{
			foreach (var layer in layers.Values)
			{
				var sub = layer.Stack.FirstOrDefault(x => x == subLayer);
				if (sub == null)
				{
					continue;
				}

				if (layer.Stack.Peek() == subLayer)
				{
					layer.Stack.Pop().OnClose();
					return;
				}
				else
				{
					layer.Stack.Remove(subLayer);
					sub.OnClose();
				}
			}
		}
	}
}
