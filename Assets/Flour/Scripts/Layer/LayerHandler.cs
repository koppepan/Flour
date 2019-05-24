﻿using System;
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
		readonly SubLayerSourceRepository[] repositories;

		readonly LayerType[] layerOrder;
		readonly Dictionary<LayerType, Layer> layers = new Dictionary<LayerType, Layer>();

		readonly SafeAreaHandler safeAreaHandler;

		public LayerHandler(Transform canvasRoot, Vector2 referenceResolution, SubLayerSourceRepository[] repositories, LayerType[] safeAreaLayers)
		{
			this.repositories = repositories;

			safeAreaHandler = new SafeAreaHandler(new Vector2(Screen.width, Screen.height), Screen.safeArea, safeAreaLayers);

			var layerTypes = Enum.GetValues(typeof(LayerType)).Cast<LayerType>();
			foreach (var type in layerTypes)
			{
				var layer = new GameObject(type.ToString(), typeof(Layer)).GetComponent<Layer>();
				layer.transform.SetParent(canvasRoot);

				var reduction = safeAreaLayers.Contains(type) ? safeAreaHandler.Reduction : (Action<LayerType, RectTransform>)null;
				layer.Initialize(type, referenceResolution, reduction);

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

		public async UniTask<AbstractSubLayer> AddAsync(LayerType layerType, SubLayerType subLayerType)
		{
			return await AddAsync<AbstractSubLayer>(layerType, subLayerType);
		}

		public async UniTask<T> AddAsync<T>(LayerType layerType, SubLayerType subLayerType) where T : AbstractSubLayer
		{
			var layer = layers[layerType];
			var old = layer.Stack.FirstOrDefault(subLayerType);

			// 既に同じSubLayerが存在する
			if (old != null)
			{
				// すでに存在していて一番前にある
				if (layer.Stack.FindIndex(old) == 0)
				{
					return (T)old;
				}

				// 一番前に来るように詰めなおして返す
				layer.Stack.Remove(old);
				layer.Stack.Push(old);
				return (T)old;
			}

			var prefab = await LoadAsync<T>(subLayerType);

			if (prefab == null)
			{
				return null;
			}

			var sub = GameObject.Instantiate(prefab);
			sub.SetConstParameter(layerType, subLayerType, safeAreaHandler.Expansion, Remove);
			sub.OnOpen();
			layer.Stack.Push(sub);
			return sub;
		}

		public void Remove(LayerType layer)
		{
			layers[layer].Stack.Peek()?.Close();
		}
		void Remove(AbstractSubLayer subLayer)
		{
			foreach (var layer in layers.Values)
			{
				var index = layer.Stack.FindIndex(subLayer);
				if (index == -1)
				{
					continue;
				}

				if (index == 0)
				{
					layer.Stack.Pop().OnClose();
				}
				else
				{
					layer.Stack.Remove(subLayer);
					subLayer.OnClose();
				}
				return;
			}
		}
	}
}
