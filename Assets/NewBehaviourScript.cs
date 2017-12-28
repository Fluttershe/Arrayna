using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using WeaponAssemblage;

public class NewBehaviourScript : MonoBehaviour {
	[SerializeField]
	public MultiSelectablePartType typ;

	[SerializeField]
	public PartType Ttypes;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (typ[PartType.Addon])
			if (typ[PartType.Barrel])
				if (typ[PartType.BarrelAddon])
					if (typ[PartType.Bullet])
						if (typ[PartType.Magazine])
							if (typ[PartType.Reciever])
								if (typ[PartType.Sight])
									if (typ[PartType.Stock]) print("Haza!");
	}
}
