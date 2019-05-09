using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace Flour.UI
{
	public enum Layer
	{
		Back = 10,
		Middle = 11,
		Front = 12,
	}

	public class LayerHandler
	{
		SubLayerSourceRepository repository;

		Dictionary<Layer, LayerStack> layerStacks = new Dictionary<Layer, LayerStack>();

		public LayerHandler(Transform canvasRoot, Vector2 referenceResolution, SubLayerSourceRepository repository)
		{
			this.repository = repository;

			foreach (var layer in Enum.GetValues(typeof(Layer)).Cast<Layer>())
			{
				var stack = new GameObject(layer.ToString(), typeof(LayerStack)).GetComponent<LayerStack>();
				stack.transform.SetParent(canvasRoot);
				stack.Initialize(layer, referenceResolution);

				layerStacks.Add(layer, stack);
			}
		}

		public async Task<T> AddAsync<T>(Layer layer, SubLayerType type) where T : AbstractSubLayer
		{
			var task = repository.LoadAsync<T>(type);
			await task;

			if (task.Result == null)
			{
				return null;
			}

			var stack = layerStacks[layer];

			var current = stack.Peek();
			var old = stack.FirstOrDefault(type);

			// 開こうとしたSubLayerが存在しないので新しく生成
			if (old == null)
			{
				current?.OnInactivate();

				var sub = GameObject.Instantiate(task.Result);
				sub.Initialize(type, Remove);
				sub.OnOpen();
				layerStacks[layer].Push(sub);
				return sub;
			}

			// 開こうとしたSubLayerがすでに存在していて一番前にある
			if (current == old)
			{
				return (T)current;
			}

			// 開こうとしたSubLayerがすでに存在していて一番前にはない
			stack.Push(old);
			old = stack.Pop();
			stack.Peek()?.OnInactivate();
			old?.OnActivate();
			stack.Push(old);
			return (T)old;
		}

		public void Remove(Layer layer)
		{
			layerStacks[layer].Pop()?.OnClose();
			layerStacks[layer].Peek()?.OnActivate();
		}
		public void Remove(AbstractSubLayer subLayer)
		{
			foreach (var stack in layerStacks)
			{
				stack.Value.Remove(subLayer);
			}
		}
	}
}
