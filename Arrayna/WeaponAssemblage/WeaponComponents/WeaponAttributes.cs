using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityUtility;

namespace WeaponAssemblage
{
	/// <summary>
	/// 武器的属性类型
	/// </summary>
	public enum AttributeType
	{
		/// <summary>
		/// 伤害
		/// </summary>
		Damage,
		/// <summary>
		/// 射速（发每分）
		/// </summary>
		FireRate,
		/// <summary>
		/// 弹容量
		/// </summary>
		Capacity,
		/// <summary>
		/// 重量
		/// </summary>
		Weight,
		/// <summary>
		/// 初速
		/// </summary>
		MuzzleVelocity,
		/// <summary>
		/// 重装速度
		/// </summary>
		ReloadingSpeed,
		/// <summary>
		/// 准度
		/// </summary>
		Accuracy,
		/// <summary>
		/// 稳定性
		/// </summary>
		Stability,
	}

	/// <summary>
	/// 武器属性的集合类
	/// </summary>
	[Serializable]
	public class WeaponAttributes : EnumBasedCollection<AttributeType, float>
	{
		public void Add(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] += wa[keys[i]];
			}
		}

		public void Add(float wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] += wa;
			}
		}

		public void Sub(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] -= wa[keys[i]];
			}
		}

		public void Sub(float wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] -= wa;
			}
		}

		public void Mul(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] *= wa[keys[i]];
			}
		}

		public void Mul(float wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] *= wa;
			}
		}

		public void Div(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] /= wa[keys[i]];
			}
		}

		public void Div(float wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] /= wa;
			}
		}
	}
}
