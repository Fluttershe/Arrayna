using UnityEngine;
using System.Collections;

namespace WeaponAssemblage.Workspace
{
    public class CreatWeaponInTheArrayna : MonoBehaviour
    {
        void Awake()
        {
                Instantiate(PlayerWeaponStorage.Instance.weapons[0],new Vector3(0,0,-0.1f), Quaternion.identity);
        }
    }
}
