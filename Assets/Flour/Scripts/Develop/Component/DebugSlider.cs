using UnityEngine;
using UnityEngine.UI;

namespace Flour.Develop
{
	class DebugSlider : MonoBehaviour, IContent<float>
	{
		[SerializeField]
		private Slider slider = default;
		[SerializeField]
		private InputField inputField = default;

		public void Setup(float value, float min, float max, System.Action<float> onChanged)
		{
			slider.minValue = min;
			slider.maxValue = max;
			slider.value = value;

			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(_ =>
			{
				inputField.text = slider.value.ToString("F1");
				onChanged?.Invoke(slider.value);
			});

			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(val => slider.value = Mathf.Clamp(float.Parse(val), slider.minValue, slider.maxValue));
		}

		public float GetValue()
		{
			return slider.value;
		}
	}
}
