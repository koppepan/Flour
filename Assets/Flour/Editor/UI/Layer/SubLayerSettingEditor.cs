using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Flour.UI
{
	[CustomEditor(typeof(SubLayerSetting))]
	public class SubLayerSettingEditor : Editor
	{
		SerializedProperty exportPathProperty;
		ReorderableList settingsProperty;

		private void OnEnable()
		{
			exportPathProperty = serializedObject.FindProperty("exportPath");
			settingsProperty = CreateList("settings");
		}
		ReorderableList CreateList(string key)
		{
			var prop = serializedObject.FindProperty(key);
			var list = new ReorderableList(serializedObject, prop);
			list.elementHeight = EditorGUIUtility.singleLineHeight * 2.5f;
			list.drawHeaderCallback = (rect) =>
			{
				EditorGUI.LabelField(rect, key);
			};
			list.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				var single = EditorGUIUtility.singleLineHeight;
				var element = prop.GetArrayElementAtIndex(index);
				rect.height -= 4;
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, rect.width, single), element.FindPropertyRelative("typeName"));
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + single + 4, rect.width, single), element.FindPropertyRelative("srcPath"));
			};

			return list;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(exportPathProperty);

			EditorGUILayout.Space();
			if (GUILayout.Button("sort"))
			{
				var src = (SubLayerSetting)target;
				src.settings = src.settings.OrderBy(x => x.typeName).ToList();
			}
			if (GUILayout.Button("export"))
			{
				var src = (SubLayerSetting)target;
				EnumCreator.Create(src.exportPath, "Flour.UI", "", "SubLayerType", src.settings.Select(x => x.typeName).ToArray());
			}
			EditorGUILayout.Space();

			settingsProperty.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}
