using UnityEngine;
using UniRx.Async;

namespace Flour.Scene
{
	public abstract class AbstractScene<T> : MonoBehaviour
	{
		public string SceneName { get; private set; }

		protected T param;

		public void Setup(string sceneName, T param)
		{
			SceneName = sceneName;
			this.param = param;
		}

		public virtual async UniTask Load(params object[] param)
		{
			await UniTask.DelayFrame(1);
		}

		public virtual void Unload() { }

		public virtual void OnBack() { }
	}
}
