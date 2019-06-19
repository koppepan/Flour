using UnityEngine;
using UnityEngine.UI;

namespace Example
{
	[RequireComponent(typeof(InputField))]
	public class DebugInputField : MonoBehaviour, DebugDialog.IContent<string>
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
