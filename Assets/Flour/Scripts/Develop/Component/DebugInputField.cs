using UnityEngine;
using UnityEngine.UI;

namespace Flour.Develop
{
	[RequireComponent(typeof(InputField))]
	class DebugInputField : MonoBehaviour, IContent<string>
	{
		public void Setup(System.Action<string> onEnter)
		{
			var input = GetComponent<InputField>();
			input.onEndEdit.RemoveAllListeners();
			input.onEndEdit.AddListener(text => onEnter?.Invoke(text));
		}
		public string GetValue()
		{
			return GetComponent<InputField>().text;
		}
	}
}
