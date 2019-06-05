using System;
using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	public class FooterLayer : AbstractSubLayer
	{
		public override bool IgnoreBack => true;

		[SerializeField]
		Button sample1Button = default;
		[SerializeField]
		Button sample2Button = default;
		[SerializeField]
		Button sample3Button = default;
		[SerializeField]
		Button sample4Button = default;

		Action<SubLayerType> onSelect;

		public void Setup(Action<SubLayerType> onSelect)
		{
			this.onSelect = onSelect;
		}

		protected override void OnOpen()
		{
			sample1Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample1));
			sample2Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample2));
			sample3Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample3));
			sample4Button.onClick.AddListener(() => OpenSubLayer(SubLayerType.Sample4));
		}

		void OpenSubLayer(SubLayerType type)
		{
			onSelect?.Invoke(type);
		}
	}
}
