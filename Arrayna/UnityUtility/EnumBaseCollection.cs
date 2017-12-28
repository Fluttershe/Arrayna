using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityUtility
{
	[Serializable]
	public abstract class EnumBaseCollection
	{
		public abstract void Init();
		public abstract void EditorUpdate();
	}
	
	/// <summary>
	/// 可多选的枚举类型
	/// </summary>
	/// <typeparam name="E"></typeparam>
	/// TODO: Figure out how to made a serializable Dictionary
	[Serializable]
	public abstract class EnumBaseCollection<E, V> : EnumBaseCollection where E : struct, IConvertible
	{
		[NonSerialized]
		protected Dictionary<E, V> dict;

		[SerializeField]
		protected List<E> keys = new List<E>();

		[SerializeField]
		protected List<V> values = new List<V>();

		public EnumBaseCollection()
		{
			if (!typeof(E).IsEnum) throw new ArgumentException($"{typeof(E)} 不是枚举类型！");
			var values = Enum.GetValues(typeof(E));
			int enumNum = values.Length;

			dict = new Dictionary<E, V>();

			for (int i = 0; i < enumNum; i++)
			{
				dict.Add((E)values.GetValue(i), default(V));
				keys.Add((E)values.GetValue(i));
				this.values.Add(default(V));
			}
		}

		public override void Init()
		{
			if (dict != null) return;

			dict = new Dictionary<E, V>();
			for (int i = 0; i < keys.Count; i++)
			{
				dict[keys[i]] = values[i];
			}
		}

		public override void EditorUpdate()
		{
			if (!Application.isEditor) return;
			for (int i = 0; i < keys.Count; i++)
			{
				dict[keys[i]] = values[i];
			}
		}

		public virtual V this[E key]
		{
			get
			{
				Init();
				return dict[key];
			}

			set
			{
				Init();
				dict[key] = value;
			}
		}
	}
}
