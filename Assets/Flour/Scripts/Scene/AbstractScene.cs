using UnityEngine;
using UniRx.Async;

namespace Flour.Scene
{
	public abstract class AbstractScene<T> : MonoBehaviour
	{
		public string SceneName { get; private set; }

		public void SetName(string sceneName) => SceneName = sceneName;
		public abstract void SetParameter(T param);

		public virtual async UniTask Load(params object[] args)
		{
			await UniTask.DelayFrame(1);
		}

		public virtual void Unload() { }

		public virtual void OnBack() { }
	}
}
