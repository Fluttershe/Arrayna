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
		Stabilization,
	}

	/// <summary>
	/// 武器属性的集合类
	/// </summary>
	[Serializable]
	public class WeaponAttributes : EnumBaseCollection<AttributeType, float>
	{
		public void Add(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] += wa[keys[i]];
			}
		}

		public void Sub(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] -= wa[keys[i]];
			}
		}

		public void Mul(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] *= wa[keys[i]];
			}
		}

		public void Div(WeaponAttributes wa)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				this[keys[i]] /= wa[keys[i]];
			}
		}
	}

	/// <summary>
	/// 属性值，用于存放一个属性对应的数值
	/// </summary>
	[Serializable]
	public struct AttributeValue
	{
		/// <summary>
		/// 基本值
		/// </summary>
		public float BaseValue;

		/// <summary>
		/// 比例值
		/// </summary>
		public float MODValue;

		public AttributeValue(AttributeValue av)
		{
			BaseValue = av.BaseValue;
			MODValue = av.MODValue;
		}

		public static AttributeValue operator +(AttributeValue av1, AttributeValue av2)
		{
			av1.BaseValue += av2.BaseValue;
			av1.MODValue += av2.MODValue;

			return av1;
		}

		public static AttributeValue operator -(AttributeValue av1, AttributeValue av2)
		{
			av1.BaseValue -= av2.BaseValue;
			av1.MODValue -= av2.MODValue;

			return av1;
		}
	}

}
