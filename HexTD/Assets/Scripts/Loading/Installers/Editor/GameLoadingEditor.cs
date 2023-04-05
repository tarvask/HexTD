using System;
using Loading.Installers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IdleCivilization.Client.Loading.Editor
{
	[CustomEditor(typeof(GameLoadingInstaller))]
	public class GameLoadingEditor : UnityEditor.Editor
	{
		private ReorderableList reorderableList;
		private SerializedProperty property;


		private void OnEnable()
		{
			property = serializedObject.FindProperty("loadingSteps");
			reorderableList = new ReorderableList(serializedObject, property, true, true, true, true);
			reorderableList.drawHeaderCallback += OnDrawHeader;
			reorderableList.drawElementCallback += OnDrawElement;
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Loading Steps");
		}

		private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
				element,
				GUIContent.none
			);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}