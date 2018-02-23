using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WeaponAssemblage.Serializations;
using UnityEngine;

namespace WeaponAssemblage
{
	public class PlayerWeaponStorage : MonoBehaviour
	{
		[SerializeField]
		List<MonoWeapon> _weapons;
		[SerializeField]
		List<MonoPart> _parts;

		[SerializeField]
		bool test;

		private void Start()
		{
			weapons = _weapons;
			parts = _parts;
		}

		private void Update()
		{
			if (test)
			{
				test = false;
				SaveToFile();
				LoadFromFile();
			}
		}

		public static  List<MonoWeapon> weapons = new List<MonoWeapon>();
		public static  List<MonoPart> parts = new List<MonoPart>();

		public static void SaveToFile()
		{
			SerializableWeaponBundle bundle = new SerializableWeaponBundle();
			bundle.weapons = new PreserializedWeapon[weapons.Count];
			bundle.partIDs = new string[parts.Count];

			for (int i = 0; i < weapons.Count; i ++)
			{
				bundle.weapons[i] = WeaponPreserializer.Preserializate(weapons[i]);
			}

			for (int i = 0; i < parts.Count; i ++)
			{
				bundle.partIDs[i] = parts[i].PrefabID;
			}

			Save(bundle);
		}

		public static void LoadFromFile()
		{
			var bundle = Load();
			weapons.Clear();
			for (int i = 0; i < bundle.weapons.Length; i ++)
			{
				weapons.Add(WeaponPreserializer.DeserializeWeapon(bundle.weapons[i]));
			}

			parts.Clear();
			for (int i = 0; i < bundle.partIDs.Length; i ++)
			{
				parts.Add(GameObject.Instantiate(WAPrefabStore.GetPartPrefab(bundle.partIDs[i]).gameObject).GetComponent<MonoPart>());
			}
		}

		/// <summary>
		/// 保存玩家数据到硬盘中
		/// </summary>
		private static void Save(SerializableWeaponBundle bundle)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile = File.Create(Application.persistentDataPath + "/.wps.bdl");

			formatter.Serialize(saveFile, bundle);

			saveFile.Close();
		}

		/// <summary>
		/// 从硬盘中读取数据
		/// </summary>
		private static SerializableWeaponBundle Load()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream saveFile;

			try
			{
				saveFile = File.Open(Application.persistentDataPath + "/.wps.bdl", FileMode.Open);
			}
			catch (FileNotFoundException)
			{
				return null;
			}

			object deserialized = null;
			SerializableWeaponBundle bundle = new SerializableWeaponBundle();
			try
			{
				deserialized = formatter.Deserialize(saveFile);
				bundle = (SerializableWeaponBundle)deserialized;
			}
			catch (SerializationException)
			{
				if (deserialized != null)
				{
					var test = (PreserializedWeapon[])deserialized.GetType().GetField("weapons", System.Reflection.BindingFlags.GetField).GetValue(deserialized);
				}
				Debug.LogWarning("读取到旧版本存档，旧版本存档将会被覆盖");
			}

			saveFile.Close();
			return bundle;
		}
	}
}
