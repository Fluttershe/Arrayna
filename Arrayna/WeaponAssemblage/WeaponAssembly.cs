using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace WeaponAssemblage
{
	[Serializable]
	public class WeaponAssembly : MonoBehaviour
	{
		static WeaponAssembly _instance;
		public static WeaponAssembly Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = LoadState();
				}

				return _instance;
			}
		}

		[SerializeField]
		private List<MonoWeapon> weapons = new List<MonoWeapon>();
		[SerializeField]
		private List<MonoPart> parts = new List<MonoPart>();

		public List<MonoWeapon> Weapons => weapons;
		public List<MonoPart> Parts => parts;

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
		private static WeaponAssembly LoadState()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile;

			try
			{
				saveFile = File.Open(Application.persistentDataPath + "/.gb", FileMode.Open);
			}
			catch (FileNotFoundException)
			{
				Debug.Log("Didn't find the file.");
				return new WeaponAssembly();
			}

			WeaponAssembly ins = new WeaponAssembly();
			try
			{
				ins = (WeaponAssembly)formatter.Deserialize(saveFile);
			}
			catch (SerializationException)
			{
				Debug.LogWarning("读取到旧版本存档，旧版本存档将会被覆盖");
			}

			saveFile.Close();
			return ins;
		}
	}
}
