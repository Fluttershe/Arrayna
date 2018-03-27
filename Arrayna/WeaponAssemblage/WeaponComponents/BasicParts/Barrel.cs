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
		protected Bullet bullet;
		protected Bullet Bullet
		{
			get
			{
				if (bullet == null)
				{
					bullet = Weapon?.GetPartOfType<Bullet>();
				}

				return bullet;
			}
		}

		protected RuntimeValues runtimeValues;
		protected WeaponAttributes weaponAttributes;
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

		public void OnPrimaryFireDown(IWeapon weapon)
		{
			print("Received: down");
			runtimeValues = weapon.RuntimeValues;
			weaponAttributes = weapon.FinalValue;
		}

		public void OnPrimaryReadyToFire(IWeapon weapon)
		{
			print("Ready to fire");
			if (Bullet == null)
			{
				print("Bullet is null.");
				return;
			}

			var clampedAccuracy = Mathf.Clamp(weaponAttributes[WpnAttrType.Accuracy], 0, 100);
			var totalDisp = 100 - clampedAccuracy;
			for (int i = 0; i < Bullet.ProjectileNumber; i++)
			{
				var projectile = Instantiate(Bullet.ProjectilePrefab, FirePort.transform.position, FirePort.transform.rotation).GetComponent<Projectile>();
				projectile.transform.position += Vector3.forward;
				projectile.transform.up = RandomDispersedDirection(FirePort.transform.up, totalDisp);
				projectile.Speed = weaponAttributes[WpnAttrType.MuzzleVelocity];
				projectile.Damage = weaponAttributes[WpnAttrType.Damage];
				projectile.CriticalRate = weaponAttributes[WpnAttrType.CriticalRate];
			}

			// TODO: remove direct changes to runtimeValues
			runtimeValues.FireTime = 1 / weaponAttributes[WpnAttrType.FireRate];
			runtimeValues.ShotAmmo++;
		}

		public void OnPrimaryFireUp(IWeapon weapon)
		{
			print("Received: up");
			runtimeValues = null;
			weaponAttributes = null;
		}

		protected override void PartDetached(IPort callerport, IPort calleeport)
		{
			base.PartDetached(callerport, calleeport);
			bullet = null;
		}

		/// <summary>
		/// 根据散射生成一个弹药发射的方向
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="dispersal">0~100, 100 = -30~30 degree, 0 = 0 degree</param>
		/// <returns></returns>
		protected virtual Vector2 RandomDispersedDirection(Vector2 direction, float dispersal)
		{
			dispersal = Mathf.Clamp(dispersal, 0, 100) * 0.45f;

			dispersal = UnityEngine.Random.Range(-dispersal, dispersal);

			return Quaternion.Euler(0, 0, dispersal) * direction;
		}
	}
}
