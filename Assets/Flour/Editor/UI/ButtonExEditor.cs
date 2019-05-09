using UnityEditor;
using UnityEditor.UI;

namespace Flour.UI
{
	[CustomEditor(typeof(ButtonEx), true)]
	[CanEditMultipleObjects]
	public class ButtonExEditor : ButtonEditor
	{
		SerializedProperty activateDoubleClickProperty;
		SerializedProperty activateHoldProperty;

		SerializedProperty onDoubleClickProperty;
		SerializedProperty onHoldProperty;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			activateDoubleClickProperty = serializedObject.FindProperty("activateDoubleClick");
			activateHoldProperty = serializedObject.FindProperty("activateHold");

			onDoubleClickProperty = serializedObject.FindProperty("doubleClick");
			onHoldProperty = serializedObject.FindProperty("hold");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			EditorGUILayout.PropertyField(activateDoubleClickProperty);
			if (activateDoubleClickProperty.boolValue)
			{
				EditorGUILayout.PropertyField(onDoubleClickProperty);
			}

			EditorGUILayout.PropertyField(activateHoldProperty);
			if (activateHoldProperty.boolValue)
			{
				EditorGUILayout.PropertyField(onHoldProperty);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
