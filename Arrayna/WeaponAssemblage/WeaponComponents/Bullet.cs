﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage
{
	public class Bullet: BasicPart
	{
		public override PartType Type => PartType.Bullet;
		[SerializeField]
		protected GameObject bulletPrefab;
		public GameObject BulletPrefab => bulletPrefab;
	}
}