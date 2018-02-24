using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WeaponAssemblage.Serializations;
using UnityEngine;
using UnityUtility;

namespace WeaponAssemblage
{
	public class PlayerWeaponStorage : MonoBehaviour
	{
		public List<MonoWeapon> weapons = new List<MonoWeapon>();
		public List<MonoPart> spareParts = new List<MonoPart>();

		private static PlayerWeaponStorage _instance;
		public static PlayerWeaponStorage Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GlobalObject.GetOrAddComponent<PlayerWeaponStorage>();
					LoadFromFile();
				}

				return _instance;
			}
		}

		public static void SaveToFile()
		{
			SerializableWeaponBundle bundle = new SerializableWeaponBundle();
			bundle.weapons = new PreserializedWeapon[Instance.weapons.Count];
			bundle.partIDs = new string[Instance.spareParts.Count];

			for (int i = 0; i < Instance.weapons.Count; i ++)
			{
				bundle.weapons[i] = WeaponPreserializer.Preserializate(Instance.weapons[i]);
			}

			for (int i = 0; i < Instance.spareParts.Count; i ++)
			{
				bundle.partIDs[i] = Instance.spareParts[i].PrefabID;
			}

			Save(bundle);
		}

		public static void LoadFromFile()
		{
			var bundle = Load();
			if (bundle == null)
				bundle = new SerializableWeaponBundle();

			Instance.weapons.Clear();
			for (int i = 0; i < bundle.weapons.Length; i ++)
			{
				Instance.weapons.Add(WeaponPreserializer.DeserializeWeapon(bundle.weapons[i]));
			}

			Instance.spareParts.Clear();
			for (int i = 0; i < bundle.partIDs.Length; i ++)
			{
				Instance.spareParts.Add(GameObject.Instantiate(WAPrefabStore.GetPartPrefab(bundle.partIDs[i]).gameObject).GetComponent<MonoPart>());
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
			SerializableWeaponBundle bundle = null;
			try
			{
				deserialized = formatter.Deserialize(saveFile);
				bundle = (SerializableWeaponBundle)deserialized;
			}
			catch (SerializationException)
			{
				if (deserialized == null)
				{
					Debug.LogWarning("无法打开存档");
				}
				else
				{
					Debug.LogWarning("读取到旧版本存档，尝试解析……");
					var weapons = (PreserializedWeapon[])deserialized.GetType().GetField("weapons").GetValue(deserialized);
					var partIDs = (string[])deserialized.GetType().GetField("partIDs").GetValue(deserialized);

					if (weapons == null || partIDs == null)
					{
						Debug.LogWarning("无法解析出武器与部件，文件可能已经损坏");
					}
					else
					{
						Debug.LogWarning("文件解析成功，读取武器和部件信息……");
						bundle.weapons = weapons;
						bundle.partIDs = partIDs;
					}
				}
			}

			saveFile.Close();
			return bundle;
		}
	}
}
