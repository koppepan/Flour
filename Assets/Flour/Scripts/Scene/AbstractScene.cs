using UniRx.Async;
using UnityEngine;

namespace Flour.Scene
{
	public abstract class AbstractScene<T> : MonoBehaviour
	{
		public string SceneName { get; private set; }

		internal void SetName(string sceneName) => SceneName = sceneName;
		public abstract void SetParameter(T param);

		public virtual async UniTask Load(params object[] args)
		{
			await UniTask.DelayFrame(1);
		}
		public virtual void Open() { }

		public virtual async UniTask Unload()
		{
			await UniTask.DelayFrame(1);
		}

		public virtual void OnBack() { }

		public virtual void ApplicationPause(bool pause) { }
	}
}
