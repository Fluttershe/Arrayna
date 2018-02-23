using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage.WeaponComponents
{
	public class TestWeapon : BasicWeapon
	{
		[Header("武器的属性值为上方的Final Value，下方数值仅为调试观测用数值")]
		[SerializeField]
		bool firing;

		[SerializeField]
		float fireTime;

		[SerializeField]
		float dispersal;

		[SerializeField]
		float dispersalIncrement;

		[SerializeField]
		float dispersalDecreRate;
		
		[SerializeField]
		int numberOfAmmo;

		[SerializeField]
		float reloadTime;

		[SerializeField]
		int projectileNumber = 1;

		[SerializeField]
		Transform firePort;

		[SerializeField]
		Projectile projectilePrefab;

		private void Update()
		{
			if (fireTime > 0)
			{
				fireTime -= Time.deltaTime;
			}

			if (dispersal > 0)
			{
				dispersal -= Time.deltaTime * dispersalDecreRate * dispersal;
			}

			if (reloadTime > 0)
			{
				reloadTime -= Time.deltaTime;
			}

			UpdateDebugValue();

			if (!firing || fireTime > 0 || reloadTime > 0) return;

			if (numberOfAmmo <= 0)
			{
				Reload();
				return;
			}

			LaunchProjectile();
		}

		public override void PrimaryFireDown()
		{
			firing = true;
		}

		public override void PrimaryFireUp()
		{
			firing = false;
		}

		public override void Reload()
		{
			if (reloadTime > 0 || numberOfAmmo == FinalValue[WpnAttrType.Capacity]) return;

			numberOfAmmo = Mathf.RoundToInt(FinalValue[WpnAttrType.Capacity]);
			reloadTime = FinalValue[WpnAttrType.ReloadingSpeed];
		}

		/// <summary>
		/// Generate a dispersed firing direction
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="dispersal">0~100, 100 = -30~30 degree, 0 = 0 degree</param>
		/// <returns></returns>
		public Vector2 RandomDispersedRotation(Vector2 direction, float dispersal)
		{
			dispersal = Mathf.Clamp(dispersal, 0, 100) * 0.3f;

			dispersal = Random.Range(-dispersal, dispersal);

			return Quaternion.Euler(0, 0, dispersal) * direction;
		}

		protected virtual void LaunchProjectile()
		{
			var totalDisp = 100 - FinalValue[WpnAttrType.Accuracy] + dispersal;

			for (int i = 0; i < projectileNumber; i ++)
			{
				var projectile = Instantiate(projectilePrefab.gameObject, firePort.transform.position, firePort.transform.rotation).GetComponent<Projectile>();
				projectile.transform.up = RandomDispersedRotation(firePort.up, totalDisp);
				projectile.Speed = FinalValue[WpnAttrType.MuzzleVelocity];
			}

			dispersal += dispersalIncrement;
			fireTime += 1 / FinalValue[WpnAttrType.FireRate];
			numberOfAmmo--;
		}

		private void UpdateDebugValue()
		{
			dispersalIncrement = 100 - FinalValue[WpnAttrType.Stability];
			if (dispersalIncrement < 0) dispersalIncrement = 0;
			dispersalDecreRate = (FinalValue[WpnAttrType.Stability] + FinalValue[WpnAttrType.Weight]) * 0.01f;
		}

		private void OnDrawGizmos()
		{
			var totalDisp = (100 - FinalValue[WpnAttrType.Accuracy]) + dispersal;
			totalDisp = Mathf.Clamp(totalDisp, 0, 100) * 0.3f;
			Gizmos.DrawRay(firePort.position, Quaternion.Euler(0, 0, totalDisp) * firePort.transform.up);
			Gizmos.DrawRay(firePort.position, Quaternion.Euler(0, 0, -totalDisp) * firePort.transform.up);
		}
	}
}
