using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace UnityUtility
{
	/// <summary>
	/// 在整个游戏过程中都会保持存活的Object，可在上面添加Component
	/// </summary>
	public class GlobalObject : MonoBehaviour
	{
		/// <summary>
		/// 用于快速索引位于 GlobalObject 上的 Component
		/// </summary>
		[SerializeField]
		private SDictionary<Type, Component> _components = new SDictionary<Type, Component>();

		private static GlobalObject _instance;

		private static SDictionary<Type, Component> Components => Instance._components;

		/// <summary>
		/// GlobalObject 的实例
		/// </summary>
		public static GlobalObject Instance
		{
			get
			{
				if (_instance != null) return _instance;

				LoadState();
				if (_instance != null) return _instance;

				_instance = FindObjectOfType<GlobalObject>();
				if (_instance != null) return _instance;

				_instance = new GameObject("GlobalObject").AddComponent<GlobalObject>();
				return _instance;
			}
		}

		/// <summary>
		/// 向 GlobalObject 上添加一个Component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static T AddComponent<T>() where T : Component
		{
			var type = typeof(T);
			Component comp;

			if (Components.TryGetValue(type, out comp))
			{
				Debug.LogWarning($"GlobalObject already contained a component of type {type.ToString()}");
				return (T)comp;
			}

			comp = Instance.gameObject.AddComponent<T>();
			Components.Add(type, comp);
			return (T)comp;
		}

		/// <summary>
		/// 从 GlobalObject 上获取一个 Component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public new static T GetComponent<T>() where T : Component
		{
			var type = typeof(T);
			Component comp;

			// If cannot find one in components 
			if (!Components.TryGetValue(type, out comp))
			{
				comp = Instance.gameObject.GetComponent<T>();
				if (comp != null)
					Components.Add(type, comp);
			}

			return (T)comp;
		}

		/// <summary>
		/// 从 GlobalObject 上获取一个 Component, 如果不存在，则添加一个
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static T GetOrAddComponent<T>() where T : Component
		{
			var obj = GetComponent<T>();
			if (obj == null)
				obj = AddComponent<T>();
			return obj;
		}

		/// <summary>
		/// 从 GlobalObject 上移除一个 Component, 如果Component不存在，返回false
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool RemoveComponent<T>() where T : Component
		{
			var type = typeof(T);
			Component comp;
			if (!Components.TryGetValue(type, out comp)) return false;

			Destroy(comp);
			Components.Remove(type);
			
			return true;
		}

		/// <summary>
		/// 保存玩家数据到硬盘中
		/// </summary>
		private static void SaveState()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile = File.Create(Application.persistentDataPath + "/.gb");

			formatter.Serialize(saveFile, Instance);

			saveFile.Close();
		}

		/// <summary>
		/// 从硬盘中读取数据
		/// </summary>
		private static void LoadState()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile;

			try
			{
				saveFile = File.Open(Application.persistentDataPath + "/.gb", FileMode.Open);
			}
			catch (FileNotFoundException)
			{
				return;
			}

			try
			{
				_instance = (GlobalObject)formatter.Deserialize(saveFile);
			}
			catch (SerializationException)
			{
				Debug.LogWarning("读取到旧版本存档，旧版本存档将会被覆盖");
			}

			saveFile.Close();
		}

		private void Awake()
		{
			print("Awake");
			DontDestroyOnLoad(this.gameObject);
		}

		private void Start()
		{
			print("Start");
		}

		private void OnEnable()
		{
			print("OnEnable");
		}

		private void OnDisable()
		{
			print("OnDisable");
		}

		private void OnApplicationFocus(bool focus)
		{
			print("OnApplicationFocus: " + focus);
		}

		private void OnApplicationPause(bool pause)
		{
			print("OnApplicationPause: " + pause);
		}

		private void OnApplicationQuit()
		{
			SaveState();
			print("OnApplicationQuit");
		}
	}
}
