using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Components.UI
{
	[CustomEditor(typeof(ButtonEx), true)]
	[CanEditMultipleObjects]
	public class ButtonExEditor : ButtonEditor
	{
		SerializedProperty activateDoubleClickProperty;
		SerializedProperty onDoubleClickProperty;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			activateDoubleClickProperty = serializedObject.FindProperty("activateDoubleClick");
			onDoubleClickProperty = serializedObject.FindProperty("doubleClick");
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
			serializedObject.ApplyModifiedProperties();
		}
	}
}
