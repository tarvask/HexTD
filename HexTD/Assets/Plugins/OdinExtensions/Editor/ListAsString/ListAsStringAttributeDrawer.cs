using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using NamedValue = Sirenix.OdinInspector.Editor.ValueResolvers.NamedValue;

// Based on ListItemSelector
// https://www.odininspector.com/community-tools/561/listitemselector-attribute-easily-select-items-in-lists
namespace Plugins.OdinExtensions.Editor.ListAsString
{
	[DrawerPriority(10)]
	public class ListAsStringAttributeDrawer : OdinAttributeDrawer<ListAsStringAttribute>
	{
		private static readonly Color SELECTED_COLOR = new Color(0.301f, 0.563f, 1f, 0.4f);
	
		private bool m_isListElement;
		private PropertyContext<InspectorProperty> m_globalSelectedProperty;
		private InspectorProperty m_selectedProperty;
		private PropertyContext<bool> m_drawingSelected;

		private ValueResolver<string> m_stringResolver;

		protected override void Initialize()
		{
			m_isListElement = Property.Parent != null && Property.Parent.ChildResolver is IOrderedCollectionResolver;

			var listProperty = !m_isListElement ? Property : Property.Parent;
		
			var baseMemberProperty = listProperty.FindParent(x => 
				x.Info.PropertyType == PropertyType.Value, true);

			m_globalSelectedProperty =
				baseMemberProperty.Context.GetGlobal("SelectedProperty" + baseMemberProperty.GetHashCode(),
					(InspectorProperty) null);

			m_drawingSelected =
				baseMemberProperty.Context.GetGlobal("DrawingSelected" + baseMemberProperty.GetHashCode(), 
					false);

			if (!m_drawingSelected.Value && Attribute.StringAction != null)
				m_stringResolver = ValueResolver.Get<string>(Property, Attribute.StringAction,
					new NamedValue("index", typeof(int), 0));
		}

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var eventType = Event.current.type;

			if (m_isListElement)
			{
				var isSelected = m_globalSelectedProperty.Value == Property;
			
				if (isSelected && m_drawingSelected.Value)
				{
					SirenixEditorGUI.BeginBox("Selected");
					CallNextDrawer(label);
					SirenixEditorGUI.EndBox();
				}
				else
				{
					Property.PushDraw();

					if (eventType != EventType.Layout)
					{
						var rect = GUIHelper.GetCurrentLayoutRect();

						switch (eventType)
						{
							case EventType.Repaint when isSelected:
								EditorGUI.DrawRect(rect, SELECTED_COLOR);
								break;
							case EventType.MouseDown when rect.Contains(Event.current.mousePosition):
							{
								if (Property != m_globalSelectedProperty.Value)
								{
									m_globalSelectedProperty.Value = Property;
								}

								break;
							}
						}
					}

					Property.AnimateVisibility = false;

					m_stringResolver?.Context.NamedValues.Set("index", Property.Index);

					var str = m_stringResolver?.GetValue();
				
					if (m_stringResolver != null && m_stringResolver.HasError)
						m_stringResolver.DrawError();

					// Copied from DisplayAsString
					if (str == null)
						str = Property.ValueEntry.WeakSmartValue?.ToString() ?? "Null";

					if (label == null)
						EditorGUILayout.LabelField(str, SirenixGUIStyles.MultiLineLabel, 
							GUILayoutOptions.MinWidth(0.0f));
					else
					{
						GUIContent content = GUIHelper.TempContent(str);

						GUI.Label(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(false,
								SirenixGUIStyles.MultiLineLabel.CalcHeight(content,
									Property.LastDrawnValueRect.width - GUIHelper.BetterLabelWidth),
								GUILayoutOptions.MinWidth(0.0f)), label), content,
							SirenixGUIStyles.MultiLineLabel);
					}

					Property.PopDraw();
				}
			}
			else
			{
				CallNextDrawer(label);

				if (Event.current.type != EventType.Layout)
				{
					var sel = m_globalSelectedProperty.Value;

					// Select
					if (sel != null && sel != m_selectedProperty)
					{
						m_selectedProperty = sel;
						Select();
					}
					// Deselect when destroyed or not expanded
					else if (!Property.State.Expanded || m_selectedProperty != null && 
					         (m_selectedProperty.Index < Property.Children.Count &&
					          m_selectedProperty != Property.Children[m_selectedProperty.Index] ||
					          m_selectedProperty.Index >= Property.Children.Count))
					{
						Select();
						m_selectedProperty = null;
						m_globalSelectedProperty.Value = null;
					}
				}

				if (Property.State.Expanded && m_globalSelectedProperty.Value != null)
				{
					m_drawingSelected.Value = true;
					m_globalSelectedProperty.Value.Draw(null);
					m_drawingSelected.Value = false;
				}
			}
		}

		private void Select()
		{
			GUIHelper.RequestRepaint();
		}
	}
}