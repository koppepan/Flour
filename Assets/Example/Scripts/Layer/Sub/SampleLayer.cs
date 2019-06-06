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
		protected override async UniTask OnClose()
		{
			Debug.Log(Key + " on close");
			await base.OnClose();
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
