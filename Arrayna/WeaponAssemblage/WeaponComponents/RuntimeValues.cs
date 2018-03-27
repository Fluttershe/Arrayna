using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeaponAssemblage
{
	[Serializable]
	public class RuntimeValues
	{
		public bool HoldingPrimaryFire;
		public bool HoldingSecondaryFire;
		public bool Drew;

		public float FireTime;
		public float ReloadTime;
		public int ShotAmmo;
	}
}
