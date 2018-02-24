using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage.Workspace
{
	public class EnterTest : MonoBehaviour
	{
		public bool enter;
		public bool exit;

		void Update()
		{
			if (enter)
			{
				enter = false;
				if (PlayerWeaponStorage.Instance.weapons.Count > 0)
					Workspace.EnterWorkspace(PlayerWeaponStorage.Instance.weapons[0]);
				else
					Workspace.EnterWorkspace();
			}

			if (exit)
			{
				exit = false;
				Workspace.ExitWorkspace();
			}
		}
	}
}
