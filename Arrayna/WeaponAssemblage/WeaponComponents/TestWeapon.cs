using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage.WeaponComponents
{
	public class TestWeapon : BasicWeapon
	{
		[Header("武器的属性值为上方的Final Value")]
		[SerializeField]
		bool firing;
		
		[SerializeField]
		int projectileNumber = 1;
		[Header("上方的Firing为开火，projectileNumber为每次开火的射弹数量")]
		[Header("下方为调试观测用数值")]
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
		Transform firePort;

		[SerializeField]
		Projectile projectilePrefab;

		protected override void Update()
		{
			// 计算射击时间
			if (fireTime > 0)
			{
				fireTime -= Time.deltaTime;
			}

			// 计算散射程度
			if (dispersal > 0)
			{
				dispersal *= 1 - (Time.deltaTime * dispersalDecreRate);
			}

			// 计算装弹时间
			if (reloadTime > 0)
			{
				reloadTime -= Time.deltaTime;
			}

			// 更新一些调试用数值
			UpdateDebugValue();

			// 如果没有开火，或者还在开火间隔间，或者还在装弹，终止函数
			if (!firing || fireTime > 0 || reloadTime > 0) return;

			// 如果弹药已经打空，这一轮进行装弹
			if (numberOfAmmo <= 0)
			{
				Reload();
				return;
			}

			// 发射子弹
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

		/// <summary>
		/// 装弹
		/// </summary>
		public override void Reload()
		{
			if (reloadTime > 0 || numberOfAmmo == FinalValue[WpnAttrType.Capacity]) return;

			numberOfAmmo = Mathf.RoundToInt(FinalValue[WpnAttrType.Capacity]);
			reloadTime = FinalValue[WpnAttrType.ReloadingTime];
		}

		/// <summary>
		/// 根据散射生成一个弹药发射的方向
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
			// 计算出实际的散射值
			var totalDisp = 100 - FinalValue[WpnAttrType.Accuracy] + dispersal;

			// 发射子弹
			for (int i = 0; i < projectileNumber; i ++)
			{
				var projectile = Instantiate(projectilePrefab.gameObject, firePort.transform.position, firePort.transform.rotation).GetComponent<Projectile>();
				projectile.transform.up = RandomDispersedRotation(firePort.up, totalDisp);
				projectile.Speed = FinalValue[WpnAttrType.MuzzleVelocity];
				projectile.Damage = FinalValue[WpnAttrType.Damage];
			}

			// 增加散射
			dispersal += dispersalIncrement;
			// 限制散射在一定范围内
			dispersal = Mathf.Clamp(dispersal, 0, FinalValue[WpnAttrType.Accuracy]);
			// 开始计算发射间隔
			fireTime += 1 / FinalValue[WpnAttrType.FireRate];
			// 弹药数-1
			numberOfAmmo--;
		}

		private void UpdateDebugValue()
		{
			// 计算散射相关数值……
			dispersalIncrement = 10 - FinalValue[WpnAttrType.CriticalRate] * 0.1f;
			if (dispersalIncrement < 0) dispersalIncrement = 0;
			dispersalDecreRate = FinalValue[WpnAttrType.CriticalRate] * 0.025f;
		}

		private void OnDrawGizmos()
		{
			// 绘制两条线代表散射范围
			var totalDisp = (100 - FinalValue[WpnAttrType.Accuracy]) + dispersal;
			totalDisp = Mathf.Clamp(totalDisp, 0, 100) * 0.3f;
			Gizmos.DrawRay(firePort.position, Quaternion.Euler(0, 0, totalDisp) * firePort.transform.up);
			Gizmos.DrawRay(firePort.position, Quaternion.Euler(0, 0, -totalDisp) * firePort.transform.up);
		}
	}
}
