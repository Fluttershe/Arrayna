using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUtility
{
	[Serializable]
	public abstract class EnumBaseCollection
	{
		public abstract void Init();
	}
	
	/// <summary>
	/// 基于枚举类型的集合类
	/// </summary>
	/// <typeparam name="E"></typeparam>
	/// TODO: Figure out how to make a serializable Dictionary
	[Serializable]
	public abstract class EnumBaseCollection<E, V> : EnumBaseCollection, ISerializationCallbackReceiver, IEnumerable<V> where E : struct, IConvertible where V : struct
	{
		[NonSerialized]
		protected Dictionary<E, V> dict = new Dictionary<E, V>();

		[SerializeField]
		protected List<E> keys = new List<E>();

		[SerializeField]
		protected List<V> vals = new List<V>();

		public Type type { get; private set; }

		public EnumBaseCollection()
		{
			if (!typeof(E).IsEnum) throw new ArgumentException($"{typeof(E)} 不是枚举类型！");
			type = typeof(E);
			var eValues = Enum.GetValues(type);
			int enumNum = eValues.Length;

			dict = new Dictionary<E, V>();

			for (int i = 0; i < enumNum; i++)
			{
				dict.Add((E)eValues.GetValue(i), default(V));
				keys.Add((E)eValues.GetValue(i));
				vals.Add(default(V));
			}
		}

		public EnumBaseCollection(EnumBaseCollection<E, V> collection)
		{
			var length = collection.keys.Count;

			dict = new Dictionary<E, V>();

			for (int i = 0; i < length; i++)
			{
				dict.Add(collection.keys[i], collection.vals[i]);
				keys.Add(collection.keys[i]);
				vals.Add(collection.vals[i]);
			}
		}

		public void Clear()
		{
			dict.Clear();
			keys.Clear();
			vals.Clear();
		}

		/// <summary>
		/// 如果没有初始化过该对象，初始化它
		/// </summary>
		public override void Init()
		{
			if (dict != null || dict.Count > 0) return;

			Debug.Log("Clear");
			Clear();
			var eValues = Enum.GetValues(typeof(E));
			int enumNum = eValues.Length;

			dict = new Dictionary<E, V>();

			for (int i = 0; i < enumNum; i++)
			{
				dict.Add((E)eValues.GetValue(i), default(V));
				keys.Add((E)eValues.GetValue(i));
				vals.Add(default(V));
			}
		}

		public IEnumerator<V> GetEnumerator()
		{
			return ((IEnumerable<V>)vals).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<V>)vals).GetEnumerator();
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

		////// Unity Serialization //////

		public void OnBeforeSerialize()
		{
			if (dict == null) dict = new Dictionary<E, V>();
			var values = Enum.GetValues(type);
			for (int i = 0; i < values.Length; i ++)
			{
				keys[i] = (E)values.GetValue(i);
				vals[i] = dict[keys[i]];
			}
		}

		public void OnAfterDeserialize()
		{
			if (dict == null) dict = new Dictionary<E, V>();
			for (int i = 0; i < keys.Count; i++)
			{
				dict[keys[i]] = vals[i];
			}
		}
	}
}
