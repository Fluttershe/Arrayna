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
	/// 基于枚举类型的集合类
	/// </summary>
	/// <typeparam name="E"></typeparam>
	/// TODO: Figure out how to make a serializable Dictionary
	[Serializable]
	public abstract class EnumBaseCollection<E, V> : EnumBaseCollection where E : struct, IConvertible where V : struct
	{
		[NonSerialized]
		protected Dictionary<E, V> dict = new Dictionary<E, V>();

		[SerializeField]
		protected List<E> keys = new List<E>();

		[SerializeField]
		protected List<V> values = new List<V>();

		public EnumBaseCollection()
		{
			if (!typeof(E).IsEnum) throw new ArgumentException($"{typeof(E)} 不是枚举类型！");
			var eValues = Enum.GetValues(typeof(E));
			int enumNum = eValues.Length;

			dict = new Dictionary<E, V>();

			for (int i = 0; i < enumNum; i++)
			{
				dict.Add((E)eValues.GetValue(i), default(V));
				keys.Add((E)eValues.GetValue(i));
				values.Add(default(V));
			}
		}

		public EnumBaseCollection(EnumBaseCollection<E, V> collection)
		{
			var length = collection.keys.Count;

			dict = new Dictionary<E, V>();

			for (int i = 0; i < length; i++)
			{
				dict.Add(collection.keys[i], collection.values[i]);
				keys.Add(collection.keys[i]);
				values.Add(collection.values[i]);
			}
		}

		/// <summary>
		/// 如果没有初始化过该对象，初始化它
		/// </summary>
		public override void Init()
		{
			if (dict != null) return;

			dict = new Dictionary<E, V>();
			for (int i = 0; i < keys.Count; i++)
			{
				dict[keys[i]] = values[i];
			}
		}

		/// <summary>
		/// 仅用于编辑器更新数值
		/// </summary>
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
