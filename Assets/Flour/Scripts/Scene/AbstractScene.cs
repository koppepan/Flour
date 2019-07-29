using UniRx.Async;
using UnityEngine;

namespace Flour.Scene
{
	public abstract class AbstractScene<T> : MonoBehaviour
	{
		public string SceneName { get; internal set; }
		internal void SetParameterInternal(T param) => SetParameter(param);

		internal async UniTask LoadInternal(params object[] args) => await Load(args);
		internal void OpenInternal() => Open();
		internal async UniTask UnloadInternal() => await Unload();
		internal void OnBackInternal() => OnBack();
		internal void ApplicationPauseInternal(bool pause) => ApplicationPause(pause);


		protected abstract void SetParameter(T param);
		protected virtual async UniTask Load(params object[] args) => await UniTask.DelayFrame(1);
		protected virtual void Open() { }
		protected virtual async UniTask Unload() => await UniTask.DelayFrame(1);
		protected virtual void OnBack() { }
		protected virtual void ApplicationPause(bool pause) { }
	}
}
