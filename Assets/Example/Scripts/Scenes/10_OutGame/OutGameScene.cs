﻿using UniRx.Async;

namespace Example
{
	public class OutGameScene : AbstractScene
	{
		Footer footer;
		public override async UniTask Load(params object[] args)
		{
			footer = new Footer(InputBinder, LayerHandler);
			await footer.Open();
		}
		public override async UniTask Unload()
		{
			await footer.CloseWait();
		}

		public override void OnBack()
		{
			SceneHandler.LoadSceneAsync(SceneType.Title);
		}
	}
}
