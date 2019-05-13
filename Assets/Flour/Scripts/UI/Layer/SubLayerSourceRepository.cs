using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Flour.UI
{
	public class SubLayerSourceRepository
	{
		Dictionary<SubLayerType, string> srcPaths;
		Dictionary<SubLayerType, AbstractSubLayer> srcCaches = new Dictionary<SubLayerType, AbstractSubLayer>();

		int maxCache;

		public SubLayerSourceRepository(Dictionary<SubLayerType, string> srcPaths, int maxCache)
		{
			this.srcPaths = srcPaths;
			this.maxCache = maxCache == 0 ? 1 : maxCache;
		}

		public async Task<T> LoadAsync<T>(SubLayerType type) where T : AbstractSubLayer
		{
			if (!srcPaths.ContainsKey(type))
			{
				Debug.LogWarning(type.ToString() + " : missing source path.");
				return null;
			}
			if (srcCaches.ContainsKey(type))
			{
				var cache = srcCaches[type];
				srcCaches.Remove(type);
				srcCaches.Add(type, cache);
				return (T)srcCaches[type];
			}

			var request = Resources.LoadAsync<GameObject>(srcPaths[type]);
			await request;

			if (request.asset == null)
			{
				Debug.LogWarning(type.ToString() + " : not found resource.");
				return null;
			}
			srcCaches.Add(type, ((GameObject)request.asset).GetComponent<AbstractSubLayer>());

			if (srcCaches.Count > maxCache)
			{
				var remove = srcCaches.First();
				srcCaches.Remove(remove.Key);

				Resources.UnloadAsset(remove.Value);
				remove = default;
			}

			return (T)srcCaches[type];
		}
	}

	static class ResourceRequestExtenion
	{
		// Resources.LoadAsyncの戻り値であるResourceRequestにGetAwaiter()を追加する
		public static TaskAwaiter<Object> GetAwaiter(this ResourceRequest resourceRequest)
		{
			var tcs = new TaskCompletionSource<Object>();
			resourceRequest.completed += operation =>
			{
				// ロードが終わった時点でTaskCompletionSource.TrySetResult
				tcs.TrySetResult(resourceRequest.asset);
			};

			// TaskCompletionSource.Task.GetAwaiter()を返す
			return tcs.Task.GetAwaiter();
		}
	}
}
