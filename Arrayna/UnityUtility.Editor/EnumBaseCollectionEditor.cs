using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityUtility;

namespace UnityUtility.Editor
{
	[CustomPropertyDrawer(typeof(UnityUtility.EnumBaseCollection), true)]
	public class EnumBaseCollectionEditor : PropertyDrawer
	{
		protected SerializedProperty keys, values;
		protected int length;
		float[] keyHeights;
		float[] valueHeights;
		float[] elementHeights;
		string[] enumNames;
		const float lineHeight = 16f;
		const float indent = 8f;
		object[] dummyArg = new object[1];

		public override float GetPropertyHeight(
			SerializedProperty property, GUIContent label)
		{
			keys = property.FindPropertyRelative("keys");
			values = property.FindPropertyRelative("values");
			if (keys == null || values == null)
				return 16f;
			length = keys.arraySize;
			if (values.arraySize < length)
				length = values.arraySize;
			keyHeights = new float[length];
			valueHeights = new float[length];
			elementHeights = new float[length];
			enumNames = keys.enumDisplayNames;
			var totalHeight = 0f;
			int i;
			for (i = 0; i < length; i++)
			{
				var thisHeight = EditorGUI.GetPropertyHeight(
					keys.GetArrayElementAtIndex(i), new GUIContent(""), true);
				var valueHeight = EditorGUI.GetPropertyHeight(
					values.GetArrayElementAtIndex(i), new GUIContent(""), true);
				keyHeights[i] = thisHeight;
				valueHeights[i] = valueHeight;
				if (valueHeight > thisHeight)
					thisHeight = valueHeight;
				thisHeight += 2f;
				totalHeight += thisHeight;
				elementHeights[i] = thisHeight;
			}
			return lineHeight +
				(property.isExpanded ? totalHeight + 8f : 0f);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (keys == null || values == null)
			{
				EditorGUI.HelpBox(position, $"{property.type} 不是一个有效的 EnumBaseCollection.", MessageType.Error);
				return;
			}

			var left = position.xMin;
			var top = position.yMin;
			var width = position.width;

			EditorGUI.PropertyField(new Rect(left, top, width, lineHeight), property, false);
			top += lineHeight;

			if (property.isExpanded)
			{
				var k_width = width / 2;
				var totalIndent = (EditorGUI.indentLevel * indent);
				var k_left = left + totalIndent;
				var v_left = left + k_width;
				var v_width = k_width - totalIndent;

				v_width -= 4f;
				k_left += 2f;
				k_width -= 4f;
				v_left += 2f;
				top += 4f;

				int i;
				for (i = 0; i < length; i++)
				{
					var i_key = keys.GetArrayElementAtIndex(i);
					var i_value = values.GetArrayElementAtIndex(i);
					EditorGUI.LabelField(new Rect(k_left, top, k_width,
						keyHeights[i]), enumNames[i]);
					EditorGUI.PropertyField(new Rect(v_left, top, v_width,
						valueHeights[i]), i_value, GUIContent.none);
					top += elementHeights[i];
				}
				
				((EnumBaseCollection)property.GetObject()).EditorUpdate();
			}
		}
	}
}
