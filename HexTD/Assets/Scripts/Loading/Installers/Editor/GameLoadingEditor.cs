using System;
using Loading.Installers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IdleCivilization.Client.Loading.Editor
{
	[CustomEditor(typeof(GameLoadingServiceInstaller))]
	public class GameLoadingEditor : UnityEditor.Editor
	{
		private ReorderableList _reorderableList;
		private SerializedProperty _property;


		private void OnEnable()
		{
			_property = serializedObject.FindProperty("loadingSteps");
			_reorderableList = new ReorderableList(serializedObject, _property, true, true, true, true);
			_reorderableList.drawHeaderCallback += OnDrawHeader;
			_reorderableList.drawElementCallback += OnDrawElement;
		}

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Loading Steps");
		}

		private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
				element,
				GUIContent.none
			);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			_reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}