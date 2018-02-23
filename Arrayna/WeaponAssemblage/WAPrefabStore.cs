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
		static WAPrefabStore instance;
		Dictionary<string, MonoPart> PartDictionary;

		[SerializeField]
		List<MonoPart> PartPrefabs;

		private void OnEnable()
		{
			instance = this;
			PartDictionary = new SDictionary<string, MonoPart>();
			foreach (MonoPart p in PartPrefabs)
			{
				PartDictionary.Add(p.PrefabID, p);
			}
		}

		public static MonoPart GetPartPrefab(string prefabID)
		{
			return instance?.PartDictionary[prefabID];
		}
	}
}
