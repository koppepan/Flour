using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AbstractSubLayer : Flour.Layer.AbstractSubLayer
{
	public SubLayerType SubLayer => (SubLayerType)SubLayerId;

	private CanvasGroup _canvasGroup;
	protected CanvasGroup CanvasGroup
	{
		get
		{
			if (_canvasGroup == null)
			{
				_canvasGroup = GetComponent<CanvasGroup>();
			}
			return _canvasGroup;
		}
	}
}
