using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Async;

namespace Flour.UI
{
	public enum Layer
	{
		Back = 10,
		Middle = 11,
		Front = 12,
	}

	public interface ILayerHandler
	{
		UniTask<T> AddAsync<T>(Layer layer, SubLayerType subLayer) where T : AbstractSubLayer;
	}

	public class LayerHandler : ILayerHandler
	{
		SubLayerSourceRepository[] repositories;

		Layer[] layerOrder;
		Dictionary<Layer, LayerStack> layerStacks = new Dictionary<Layer, LayerStack>();

		public LayerHandler(Transform canvasRoot, Vector2 referenceResolution, params SubLayerSourceRepository[] repositories)
		{
			this.repositories = repositories;

			var layers = Enum.GetValues(typeof(Layer)).Cast<Layer>();
			foreach (var layer in layers)
			{
				var stack = new GameObject(layer.ToString(), typeof(LayerStack)).GetComponent<LayerStack>();
				stack.transform.SetParent(canvasRoot);
				stack.Initialize(layer, referenceResolution);

				layerStacks.Add(layer, stack);
			}

			layerOrder = layers.Reverse().ToArray();
		}

		public bool OnBack()
		{
			foreach (var layer in layerOrder)
			{
				if (layerStacks[layer].OnBack())
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

		public async UniTask<T> AddAsync<T>(Layer layer, SubLayerType type) where T : AbstractSubLayer
		{
			var prefab = await LoadAsync<T>(type);

			if (prefab == null)
			{
				return null;
			}

			return (T)Add(layer, type, prefab);
		}

		private AbstractSubLayer Add(Layer layer, SubLayerType type, AbstractSubLayer prefab)
		{
			var stack = layerStacks[layer];

			var current = stack.Peek();
			var old = stack.FirstOrDefault(type);

			// 開こうとしたSubLayerが存在しないので新しく生成
			if (old == null)
			{
				var sub = GameObject.Instantiate(prefab);
				sub.Initialize(type, Remove);
				sub.OnOpen();
				layerStacks[layer].Push(sub);
				return sub;
			}

			// 開こうとしたSubLayerがすでに存在していて一番前にある
			if (current == old)
			{
				return current;
			}

			// 開こうとしたSubLayerがすでに存在していて一番前にはない
			stack.Push(old);
			return old;
		}

		public void Remove(Layer layer)
		{
			layerStacks[layer].Peek()?.Close();
		}
		void Remove(AbstractSubLayer subLayer)
		{
			foreach (var stack in layerStacks)
			{
				var sub = stack.Value.Peek();
				if (sub != subLayer)
				{
					sub = null;
				}
				if (sub == null)
				{
					sub = stack.Value.FirstOrDefault(subLayer);
				}

				if (sub != null)
				{
					stack.Value.Remove(sub);
					sub.OnClose();
					return;
				}
			}
		}
	}
}
