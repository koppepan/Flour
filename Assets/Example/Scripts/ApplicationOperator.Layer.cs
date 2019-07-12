using UnityEngine;
using UniRx.Async;

namespace Example
{
	sealed partial class ApplicationOperator
	{
		readonly SubLayerSourceRepository[] subLayerRepositories;

		private async UniTask<T> SubLayerPrefabLoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			for (int i = 0; i < subLayerRepositories.Length; i++)
			{
				if (subLayerRepositories[i].ContainsKey(type))
				{
					return await subLayerRepositories[i].LoadAsync<T>(type);
				}
			}
			return null;
		}

		public void AddSceneLayer(Transform sceneObj, Camera camera)
		{
			layerHandler.AddLayer(LayerType.Scene, (int)LayerType.Scene, sceneObj, new Vector2(Screen.width, Screen.height), RenderMode.ScreenSpaceCamera, camera);
		}
		public async UniTask<T> AddSceneLayerAsync<T>(SubLayerType subLayer) where T : AbstractSubLayer
		{
			return await AddLayerAsync<T>(LayerType.Scene, subLayer, false);
		}

		private async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer, bool overlap) where T : AbstractSubLayer
		{
			InputBinder.Bind();

			T sub = !overlap ? layerHandler.GetFirst<T>(layer, subLayer) : null;

			if (sub != null)
			{
				sub.MoveFront();
			}
			else
			{
				var prefab = await SubLayerPrefabLoadAsync<T>(subLayer);
				sub = layerHandler.Add(layer, subLayer, prefab, overlap);
			}

			InputBinder.Unbind();

			return sub;
		}
		public async UniTask<T> AddLayerAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
		{
			return await AddLayerAsync<T>(layer, subLayer, false);
		}
		public async UniTask<T> AddLayerOverlappingAsync<T>(LayerType layer, SubLayerType subLayer) where T : AbstractSubLayer
		{
			return await AddLayerAsync<T>(layer, subLayer, true);
		}

	}
}
