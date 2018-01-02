using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace WeaponAssemblage
{
	public delegate void WeaponEvent(IWeapon weapon);

	/// <summary>
	/// 武器接口，代表一把完整的武器
	/// </summary>
	public interface IWeapon
	{
		/// <summary>
		/// 包含的部件类型
		/// </summary>
		MultiSelectablePartType ContainedPartType { get; }

		/// <summary>
		/// 武器的数值属性
		/// </summary>
		WeaponAttributes BaseValue { get; }

		/// <summary>
		/// 武器的倍值属性
		/// </summary>
		WeaponAttributes ModValue { get; }

		/// <summary>
		/// 武器的最终属性
		/// </summary>
		WeaponAttributes FinalValue { get; }

		/// <summary>
		/// 该武器是否完善可用
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// 该武器的核心部件
		/// </summary>
		IPart RootPart { get; set; }

		/// <summary>
		/// 计算武器的数值
		/// </summary>
		void CompileWeaponAttribute();

		/// <summary>
		/// 切换武器
		/// </summary>
		/// <param name="weapon"></param>
		void Change(IWeapon weapon);
		void PrimaryFireUp();
		void PrimaryFireDown();
		void SecondaryFireUp();
		void SecondaryFireDown();
		void TertiaryFireUp();
		void TertiaryFireDown();
		void Reload();

		/// <summary>
		/// 切换武器
		/// </summary>
		/// <param name="weapon"></param>
		event Action<IWeapon, IWeapon> OnChange;
		event WeaponEvent OnPrimaryFireUp;
		event WeaponEvent OnPrimaryFireDown;
		event WeaponEvent OnSecondaryFireUp;
		event WeaponEvent OnSecondaryFireDown;
		event WeaponEvent OnTertiaryFireUp;
		event WeaponEvent OnTertiaryFireDown;
		event WeaponEvent OnReload;
	}

	public abstract class MonoWeapon : MonoBehaviour, IWeapon
	{
		public abstract MultiSelectablePartType ContainedPartType { get; }
		public abstract bool IsCompleted { get; }
		public abstract IPart RootPart { get; set; }
		public abstract WeaponAttributes BaseValue { get; }
		public abstract WeaponAttributes ModValue { get; }
		public abstract WeaponAttributes FinalValue { get; }

		public virtual event Action<IWeapon, IWeapon> OnChange;
		public virtual event WeaponEvent OnPrimaryFireUp;
		public virtual event WeaponEvent OnPrimaryFireDown;
		public virtual event WeaponEvent OnSecondaryFireUp;
		public virtual event WeaponEvent OnSecondaryFireDown;
		public virtual event WeaponEvent OnTertiaryFireUp;
		public virtual event WeaponEvent OnTertiaryFireDown;
		public virtual event WeaponEvent OnReload;

		public abstract void CompileWeaponAttribute();

		public virtual void Change(IWeapon weapon)
		{
			OnChange?.Invoke(this, weapon);
		}

		public virtual void PrimaryFireDown()
		{
			OnPrimaryFireDown?.Invoke(this);
		}

		public virtual void PrimaryFireUp()
		{
			OnPrimaryFireUp?.Invoke(this);
		}

		public virtual void Reload()
		{
			OnReload?.Invoke(this);
		}

		public virtual void SecondaryFireDown()
		{
			OnSecondaryFireDown?.Invoke(this);
		}

		public virtual void SecondaryFireUp()
		{
			OnSecondaryFireUp?.Invoke(this);
		}

		public virtual void TertiaryFireDown()
		{
			OnTertiaryFireDown?.Invoke(this);
		}

		public virtual void TertiaryFireUp()
		{
			OnTertiaryFireUp?.Invoke(this);
		}
	}

	/// <summary>
	/// 代表一把完整的武器
	/// </summary>
	public class BasicWeapon : MonoWeapon
	{
		[SerializeField]
		protected MonoPart rootPart;
		public override IPart RootPart
		{
			get => rootPart;
			set => rootPart = (MonoPart)value;
		}

		[SerializeField]
		protected MultiSelectablePartType containedPartType = new MultiSelectablePartType();
		public override MultiSelectablePartType ContainedPartType => containedPartType;

		[SerializeField]
		protected WeaponAttributes baseValue = new WeaponAttributes();
		public override WeaponAttributes BaseValue => baseValue;

		[SerializeField]
		protected WeaponAttributes modValue = new WeaponAttributes();
		public override WeaponAttributes ModValue => modValue;

		[SerializeField]
		protected WeaponAttributes finalValue = new WeaponAttributes();
		public override WeaponAttributes FinalValue => finalValue;

		public override bool IsCompleted
		{
			get
			{
				return 
					ContainedPartType[PartType.Reciever]
				 && ContainedPartType[PartType.Barrel]
				 && ContainedPartType[PartType.Magazine]
				 && ContainedPartType[PartType.Bullet];
			}
		}

		/// <summary>
		/// 计算该武器的各项数值
		/// </summary>
		public override void CompileWeaponAttribute()
		{
			containedPartType.SetDefault();
			baseValue.SetDefault();
			modValue.SetDefault();
			finalValue.SetDefault();
			GetPartAttributes(baseValue, modValue, containedPartType, rootPart);
		}

		/// <summary>
		/// 获取一个部件及其子部件的值
		/// </summary>
		/// <param name="attr"></param>
		/// <param name="part"></param>
		protected void GetPartAttributes(WeaponAttributes bVal, WeaponAttributes mVal, MultiSelectablePartType type, IPart part)
		{
			if (bVal == null || mVal == null) throw new ArgumentNullException();
			if (part == null) return;

			bVal.Add(part.BaseValue);
			mVal.Add(part.ModValue);
			type[part.Type] = true;

			var ports = part.Ports;
			foreach (IPort p in ports)
			{
				if (p.AttachedPort == null) continue;
				GetPartAttributes(bVal, mVal, type, p.AttachedPort.Part);
			}
		}
	}
}
