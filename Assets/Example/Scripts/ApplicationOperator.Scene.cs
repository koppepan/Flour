using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;

namespace Example
{
	sealed partial class ApplicationOperator
	{
		bool sceneLoading = false;
		readonly Queue<Tuple<SceneType, object[]>> sceneTransitionQueue = new Queue<Tuple<SceneType, object[]>>();

		public async UniTask LoadSceneAsync(SceneType sceneType, params object[] args)
		{
			// 同じSceneを連続でLoadしようとしてるので無視
			if (sceneTransitionQueue.Count > 0 && sceneTransitionQueue.Last().Item1 == sceneType) return;

			// 別のシーンがLoad中なのでQueueに詰めるだけ
			if (sceneLoading)
			{
				sceneTransitionQueue.Enqueue(Tuple.Create(sceneType, args));
				return;
			}

			sceneLoading = true;
			InputBinder.Bind();

			var fade = await AddLayerAsync<FadeLayer>(LayerType.System, SubLayerType.Blackout);
			await fade.FadeIn();

			async UniTask task()
			{
				if (sceneTransitionQueue.Count > 0) return;
				await ResourceCompress();
				if (sceneTransitionQueue.Count > 0) return;
				await fade.FadeOut();
				sceneLoading = false;
			}

			await layerHandler.RemoveLayer(LayerType.Scene);
			await sceneHandler.LoadAsync(sceneType.ToJpnName(), Tuple.Create<IOperationBundler, AssetHandler>(this, assetHandler), task, args);

			InputBinder.Unbind();

			if (sceneTransitionQueue.Count > 0)
			{
				sceneLoading = false;
				var next = sceneTransitionQueue.Dequeue();
				await LoadSceneAsync(next.Item1, next.Item2);
			}
		}
		public async UniTask AddSceneAsync(string sceneName, params object[] args)
		{
			InputBinder.Bind();

#if UNITY_EDITOR && USE_LOCAL_ASSET
			var scenePath = AssetHelper.GetStageScenePath(sceneName);
			await sceneHandler.AddAsyncInEditor(scenePath, Tuple.Create<IOperationBundler, AssetHandler>(this, assetHandler), args);
#else
			await sceneHandler.AddAsync(sceneName, Tuple.Create<IOperationBundler, AssetHandler>(this, assetHandler), args);
#endif
			InputBinder.Unbind();
		}

		public async UniTask UnloadSceneAsync(string sceneName)
		{
			InputBinder.Bind();
			await sceneHandler.UnloadAsync(sceneName);
			InputBinder.Unbind();
		}
	}
}
