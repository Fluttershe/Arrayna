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
		public float Speed;

		[SerializeField]
		public float Damage;

		private void Start()
		{
            Invoke("SelfDsetroy", 5);
        }

        private void Update()
		{
			transform.Translate(0, Speed * Time.deltaTime, 0, Space.Self);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			Destroy(this.gameObject);
		}

		void SelfDsetroy()
		{
			Destroy(this.gameObject);
		}
	}
}
