using UniRx.Async;

namespace Example
{
	public class Footer
	{
		IInputBinder inputBinder;
		ILayerHandler layerHandler;

		FooterLayer footerLayer;
		FooterSubLayer currentLayer;

		public Footer(IInputBinder inputBinder, ILayerHandler layerHandler)
		{
			this.inputBinder = inputBinder;
			this.layerHandler = layerHandler;
		}

		public async UniTask Open()
		{
			footerLayer = await layerHandler.AddLayerAsync<FooterLayer>(LayerType.Front, SubLayerType.Footer);
			footerLayer.Setup(OpenSubLayer);
		}

		public void Close()
		{
			currentLayer?.Close();
			footerLayer.Close();
		}

		async void OpenSubLayer(SubLayerType subLayerType)
		{
			if (currentLayer?.Key == subLayerType)
			{
				return;
			}
			await ChangeSubLayer(subLayerType);
		}
		async void CloseSubLayer()
		{
			if (currentLayer == null)
			{
				return;
			}
			await ChangeSubLayer(SubLayerType.None);
		}

		async UniTask ChangeSubLayer(SubLayerType subLayerType)
		{
			if (inputBinder.Binded)
			{
				return;
			}
			inputBinder.Bind();

			var fade = await layerHandler.AddLayerAsync<FadeLayer>(LayerType.Middle, SubLayerType.Blackout);
			await fade.FadeIn();

			if (currentLayer != null)
			{
				await currentLayer.CloseWait();
			}
			currentLayer = null;
			if (subLayerType != SubLayerType.None)
			{
				currentLayer = await layerHandler.AddLayerAsync<FooterSubLayer>(LayerType.Back, subLayerType);
				currentLayer?.Setup(CloseSubLayer);
			}

			await fade.FadeOut();

			inputBinder.Unbind();
		}
	}
}

