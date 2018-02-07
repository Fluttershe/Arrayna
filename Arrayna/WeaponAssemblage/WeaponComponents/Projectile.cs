using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage
{
	public class Projectile : MonoBehaviour
	{
		[SerializeField]
		float speed = 1;

		private void Update()
		{
			transform.Translate(0, speed * Time.deltaTime, 0, Space.Self);
		}
	}
}
