using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using WeaponAssemblage;
using WeaponAssemblage.Serializations;

public class SerializationTest : MonoBehaviour {

	public bool StartTest;
	public MonoWeapon weapon;
	
	private void Update()
	{
		if (StartTest)
		{
			StartTest = false;
			var sweapon = WeaponPreserializer.Preserializate(weapon);
			print(JsonUtility.ToJson(sweapon));
			var mweapon = WeaponPreserializer.DeserializeWeapon(sweapon);
		}
	}
}
