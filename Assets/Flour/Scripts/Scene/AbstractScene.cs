using UnityEngine;
using UniRx.Async;

namespace Flour.Scene
{
	public abstract class AbstractScene : MonoBehaviour
	{
		public string SceneName { get; private set; }

		private IOperationBundler operationBundler;

		protected ILayerHandler LayerHandler { get { return operationBundler.LayerHandler; } }
		protected ISceneHandler SceneHandler { get { return operationBundler.SceneHandler; } }

		public void Setup(string sceneName, IOperationBundler operationBundler)
		{
			SceneName = sceneName;
			this.operationBundler = operationBundler;
		}

		public virtual async UniTask Load(params object[] param)
		{
			await UniTask.DelayFrame(1);
		}

		public virtual void Unload() { }
	}
}
