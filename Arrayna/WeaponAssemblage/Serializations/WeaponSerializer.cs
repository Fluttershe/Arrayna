using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage.Serializations
{
	[Serializable]
	public class SerializableWeaponBundle
	{
		public PreserializedWeapon[] weapons;
		public string[] partIDs;
	}

	[Serializable]
	public class PreserializedPart
	{
		public string prefabID;
		public int listIndex;
		public int[] containedParts;
	}

	[Serializable]
	public class PreserializedWeapon
	{
		public PreserializedPart[] containedParts;
	}

	/// <summary>
	/// Serialize and deserialize <see cref="IWeapon"/>
	/// </summary>
	public static class WeaponPreserializer
	{
		/// <summary>
		/// 预序列化武器
		/// </summary>
		/// <param name="weapon"></param>
		/// <returns></returns>
		public static PreserializedWeapon Preserializate(IWeapon weapon)
		{
			PreserializedWeapon sweapon = new PreserializedWeapon();
			List<PreserializedPart> sparts = new List<PreserializedPart>();

			PreserializeParts(weapon.RootPart, sparts);
			sweapon.containedParts = sparts.ToArray();

			return sweapon;
		}

		/// <summary>
		/// 预序列化部件
		/// </summary>
		/// <param name="part"></param>
		/// <param name="parts"></param>
		/// <returns></returns>
		static PreserializedPart PreserializeParts(IPart part, List<PreserializedPart> parts)
		{
			if (part == null) return new PreserializedPart();

			PreserializedPart spart = new PreserializedPart();

			var ports = part.Ports.ToArray();
			var portCount = part.PortCount;

			spart.listIndex = parts.Count;
			spart.prefabID = part.PrefabID;
			spart.containedParts = new int[portCount];
			parts.Add(spart);
			for (int i = 0; i < portCount; i++)
			{
				spart.containedParts[i] = -1;
				if (ports[i].AttachedPort != null)
				{
					var tpart = ports[i].AttachedPort.Part;
					spart.containedParts[i] = PreserializeParts(tpart, parts).listIndex;
				}
			}

			return spart;
		}

		/// <summary>
		/// 从预序列化格式中反序列化出武器
		/// </summary>
		/// <param name="weapon"></param>
		/// <returns></returns>
		public static MonoWeapon DeserializeWeapon(PreserializedWeapon weapon)
		{
			MonoWeapon mWeapon = new GameObject("New Weapon").AddComponent<BasicWeapon>();

			mWeapon.RootPart = DeserializeParts(weapon.containedParts[0], weapon);
			((MonoPart)mWeapon.RootPart).transform.SetParent(mWeapon.transform);
			((MonoPart)mWeapon.RootPart).transform.localPosition = Vector3.zero;
			mWeapon.CompileWeaponAttribute();
			mWeapon.RootPart.UpdateBelongingWeapon();

			return mWeapon;
		}

		/// <summary>
		/// 反序列化部件
		/// </summary>
		/// <param name="part"></param>
		/// <param name="weapon"></param>
		/// <returns></returns>
		static MonoPart DeserializeParts(PreserializedPart part, PreserializedWeapon weapon)
		{
			if (part.containedParts == null) return null;

			var portCount = part.containedParts.Length;
			var prefab = WAPrefabStore.GetPartPrefab(part.prefabID);
			var mPart = GameObject.Instantiate(prefab.gameObject).GetComponent<MonoPart>();
			var ports = mPart.Ports.ToArray();

			for (int i = 0; i < ports.Length; i ++)
			{
				if (part.containedParts[i] < 0) continue;
				var index = part.containedParts[i];

				// TODO: Remove Workspace dependence?
				Workspace.Workspace.InstallPart((MonoPort)ports[i], DeserializeParts(weapon.containedParts[index], weapon));
			}

			return mPart;
		}
	}
}
