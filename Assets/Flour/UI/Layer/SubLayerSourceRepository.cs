using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Flour.UI
{
	public class SubLayerSourceRepository
	{
		Dictionary<SubLayerType, string> srcPaths;
		Dictionary<SubLayerType, AbstractSubLayer> srcCaches = new Dictionary<SubLayerType, AbstractSubLayer>();

		public SubLayerSourceRepository(Dictionary<SubLayerType, string> srcPaths)
		{
			this.srcPaths = srcPaths;
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
				return (T)srcCaches[type];
			}

			var request = Resources.LoadAsync<GameObject>(srcPaths[type]);
			await request;

			if (request.asset == null)
			{
				Debug.LogWarning(type.ToString() + " : not found resource.");
				return null;
			}

			srcCaches[type] = ((GameObject)request.asset).GetComponent<AbstractSubLayer>();
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
