﻿using System;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace UnityUtility.Editor
{
	public static class ExtensionMethods
	{
		// From TOMS...
		// TODO: Figure out those 'reflection' stuffs.
		static readonly char[] dotSplit = { '.' };
		static readonly Regex indexRegex = new Regex(@"data\[(\d+)\]", RegexOptions.Compiled);

		public static object GetObject(this SerializedProperty property)
		{
			object parentObject;
			return GetObject(property, out parentObject);
		}

		public static object GetObject(this SerializedProperty property, out object parentObject)
		{
			const BindingFlags bf =
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic;
			if (property == null)
				throw new ArgumentNullException();
			object obj = property.serializedObject.targetObject;
			parentObject = null;
			var pathTokens = property.propertyPath.Split(dotSplit);
			for (int i = 0; i < pathTokens.Length; i++)
			{
				parentObject = obj;
				if (obj == null)
					return null;
				var field = obj.GetType().GetField(pathTokens[i], bf);
				if (field == null)
				{
					if (pathTokens[i] != "Array")
					{
						Debug.LogError("Unable to find field " + pathTokens[i] +
									   ". Maybe it's private? (fix this)");
						return null;
					}
					var match = indexRegex.Match(pathTokens[++i]);
					if (!match.Success)
					{
						Debug.LogError("Regex was not a match: " + pathTokens[i]);
						return null;
					}
					var index = Int32.Parse(match.Groups[1].Value);
					var list = obj as IList;
					if (list == null || index < 0 || index >= list.Count)
					{
						Debug.LogError("Unable to index: " + obj);
						return null;
					}
					obj = list[index];
				}
				else
				{
					obj = field.GetValue(obj);
				}
			}
			return obj;
		}
	}
}
