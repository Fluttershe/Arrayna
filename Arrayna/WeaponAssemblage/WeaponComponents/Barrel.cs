using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage
{
	public class Barrel : BasicPart, IPrimaryFireHandler
	{
		public override PartType Type => PartType.Barrel;
		private GameObject bulletPrefab;
		protected GameObject BulletPrefab
		{
			get
			{
				if (bulletPrefab == null)
				{
					bulletPrefab = Weapon?.GetPartOfType<Bullet>()?.BulletPrefab;
				}

				return bulletPrefab;
			}
		}

		private bool firing;
		private float fireRate;
		private float fireTimer;

		[SerializeField]
		protected MonoPort FirePort;

		protected override void Awake()
		{
			base.Awake();
			foreach (MonoPort port in portList)
			{
				if (port.name == "FirePort")
				{
					FirePort = port;
					break;
				}
			}

			if (FirePort == null)
			{
				Debug.LogWarning($"该枪管 {PartName} 缺少 FirePort.");
			}
		}

		public void OnFireDown(IWeapon weapon)
		{
			fireRate = weapon.FinalValue[AttributeType.FireRate];
			firing = true;
		}

		public void OnFireUp(IWeapon weapon)
		{
			firing = false;
		}

		private void Update()
		{
			if (fireTimer > 0) fireTimer -= Time.deltaTime;
			if (!firing || fireTimer > 0) return;

			if (bulletPrefab == null)
				print("Cannot find bullet.");
			else
				Instantiate(BulletPrefab, FirePort.transform.position, FirePort.transform.rotation);
			fireTimer = 1/fireRate;
		}

		protected override void PartAttached(IPort callerport, IPort calleeport)
		{
			base.PartAttached(callerport, calleeport);
		}

		protected override void PartDetached(IPort callerport, IPort calleeport)
		{
			base.PartDetached(callerport, calleeport);
			bulletPrefab = null;
		}
	}
}
