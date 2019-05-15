using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Flour.UI
{
	public enum LayerType
	{
		Back = 10,
		Middle = 11,
		Front = 12,
	}

	public interface ILayerHandler
	{
		UniTask<T> AddAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer;
	}

	public class LayerHandler : ILayerHandler
	{
		SubLayerSourceRepository[] repositories;

		LayerType[] layerOrder;
		Dictionary<LayerType, Layer> layers = new Dictionary<LayerType, Layer>();

		public LayerHandler(Transform canvasRoot, Vector2 referenceResolution, params SubLayerSourceRepository[] repositories)
		{
			this.repositories = repositories;

			var screenSize = new Vector2(Screen.width, Screen.height);
			var safeArea = Screen.safeArea;

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

		public async UniTask<T> AddAsync<T>(LayerType layer, SubLayerType type) where T : AbstractSubLayer
		{
			var prefab = await LoadAsync<T>(type);

			if (prefab == null)
			{
				return null;
			}

			return (T)Add(layer, type, prefab);
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
				sub.Initialize(subLayerType, Remove);
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
