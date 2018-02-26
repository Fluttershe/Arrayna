using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;

namespace WeaponAssemblage
{
	/// <summary>
	/// Weapon Assemblage Prefab Store
	/// </summary>
	[CreateAssetMenu]
	public class WAPrefabStore : ScriptableObject
	{
		static WAPrefabStore _instance;
		static WAPrefabStore Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = (WAPrefabStore)Resources.Load("WAPrefabStore");
					_instance.Awake();
				}

				return _instance;
			}
		}
		Dictionary<string, MonoPart> PartDictionary;

		[SerializeField]
		List<MonoPart> PartPrefabs;

		private void Awake()
		{
			_instance = this;
			PartDictionary = new Dictionary<string, MonoPart>();
			foreach (MonoPart p in PartPrefabs)
			{
				PartDictionary.Add(p.PrefabID, p);
			}
		}

		public static MonoPart GetPartPrefab(string prefabID)
		{
			return Instance.PartDictionary[prefabID];
		}
	}
}
