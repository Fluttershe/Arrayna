using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage
{
	[Serializable]
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
		
		protected MonoPort FirePort;

		protected override void Awake()
		{
			base.Awake();

			if (asstPortList.Count <= 0)
			{
				Debug.LogError($"该枪管 {PartName} 需要至少一个辅助接口作为射弹口.");
				return;
			}
			
			foreach (MonoPort p in asstPortList)
			{
				if (p != null)
				{
					FirePort = p;
					break;
				}
			}
		}

		public void OnFireDown(IWeapon weapon)
		{
			fireRate = weapon.FinalValue[WpnAttrType.FireRate];
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

			if (BulletPrefab == null)
				print("Bullet is null.");
			else
				Instantiate(BulletPrefab, FirePort.transform.position, FirePort.transform.rotation);
			fireTimer = 1/fireRate;
		}

		protected override void PartDetached(IPort callerport, IPort calleeport)
		{
			base.PartDetached(callerport, calleeport);
			bulletPrefab = null;
		}
	}
}
