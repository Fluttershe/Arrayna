using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace WeaponAssemblage.Workspace
{
	public class WeaponStatePanel : MonoBehaviour
	{
		[SerializeField]
		Text[] states;

		[SerializeField]
		Text reqParts;

		public void UpdateState(IWeapon weapon)
		{
			var finalValue = weapon.FinalValue;
			var modValue = weapon.ModValue;
			var cotainedPart = weapon.ContainedPartType;

			for (int i = 0; i < states.Length; i ++)
			{
				WpnAttrType type = (WpnAttrType)i;
				states[i].text = String.Format("{0:F2}", finalValue[type]);
				if (modValue[type] != 1)
				{
					states[i].text += String.Format(" ({0:F2})", modValue[type]);
				}
			}

			StringBuilder sb = new StringBuilder();
			if (!cotainedPart[PartType.Receiver]) sb.Append("\n    枪身");
			if (!cotainedPart[PartType.Barrel]) sb.Append("\n    枪管");
			if (!cotainedPart[PartType.Magazine]) sb.Append("\n    弹夹");
			if (!cotainedPart[PartType.Bullet]) sb.Append("\n    弹药");

			if (sb.Length == 0) reqParts.text = "";
			else reqParts.text = sb.Insert(0, "武器仍缺少：").ToString();
		}
	}
}
