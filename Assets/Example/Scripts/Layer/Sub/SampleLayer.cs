using UnityEngine;
using UniRx.Async;

namespace Example
{
	public class SampleLayer : FooterSubLayer
	{
		protected override void OnOpen()
		{
			Debug.Log(Key + " on open");
		}
		protected override async UniTask OnClose(bool force)
		{
			Debug.Log(Key + " on close");
			await base.OnClose(force);
		}
		protected override void OnBack()
		{
			Debug.Log(Key + " on back");
			base.OnBack();
		}

		protected override void OnChangeSiblingIndex(int index)
		{
			Debug.Log(Key + " index " + index);
			Interactable = index == 0;
		}
	}
}
