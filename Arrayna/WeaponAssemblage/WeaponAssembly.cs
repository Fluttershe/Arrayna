using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;

namespace WeaponAssemblage
{
	public class WeaponAssembly : MonoBehaviour
	{
		[SerializeField]
		private List<MonoWeapon> weapons = new List<MonoWeapon>();
		[SerializeField]
		private List<MonoPart> parts = new List<MonoPart>();

		public List<MonoWeapon> Weapons => weapons;
		public List<MonoPart> Parts => parts;
	}
}
