using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeaponAssemblage
{
	public class Magazine : BasicPart, IReloadHandler
	{
		public override PartType Type => PartType.Magazine;

		public void OnReload(IWeapon weapon)
		{
			weapon.RuntimeValues.ReloadTime = weapon.FinalValue[WpnAttrType.ReloadingTime];
		}

		public void OnReloadOver(IWeapon weapon)
		{
			weapon.RuntimeValues.NumberOfAmmo = (int)weapon.FinalValue[WpnAttrType.Capacity];
		}
	}
}
