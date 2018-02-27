using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WeaponAssemblage.Workspace
{
	public class EnterTest : MonoBehaviour
	{
        void Start()
        {
            if (PlayerWeaponStorage.Instance.weapons.Count > 0)
            {
                Workspace.EnterWorkspace(PlayerWeaponStorage.Instance.weapons[0]);
            }
            else
            {
                Workspace.EnterWorkspace();
            }
        }

        public void exit()
        {
            Workspace.ExitWorkspace();
            SceneManager.LoadScene("TestPlay");
        }
    }
}
