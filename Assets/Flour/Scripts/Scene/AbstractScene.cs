using UnityEngine;
using UniRx.Async;

namespace Flour.Scene
{
	public abstract class AbstractScene : MonoBehaviour
	{
		public string SceneName { get; private set; }

		public virtual async UniTask Load(string sceneName, params object[] param)
		{
			SceneName = sceneName;
			await UniTask.DelayFrame(1);
		}

		public virtual void Unload() { }
	}
}
