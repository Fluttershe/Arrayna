using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeaponAssemblage
{
	[Serializable]
	public class RuntimeValues
	{
		public bool HoldingFire;
		public bool Holstered;

		public float FireTime;
		public float Dispersal;
		public float DispersalIncrement;
		public float DispersalDecreRate;
		public float ReloadTime;
		public int ShotAmmo;
	}
}
