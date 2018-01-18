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
		/// 获得特定类型的武器部件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T GetPartOfType<T>();

		/// <summary>
		/// 获得特定类型的武器部件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T[] GetPartsOfType<T>();

		/// <summary>
		/// 获得特定类型的武器部件
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IPart GetPartOfType(PartType type);

		/// <summary>
		/// 获得特定类型的所有武器部件
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IPart[] GetPartsOfType(PartType type);

		/// <summary>
		/// 计算武器的数值
		/// </summary>
		void CompileWeaponAttribute();

		/// <summary>
		/// 拔出武器
		/// </summary>
		void Draw();

		/// <summary>
		/// 收起武器
		/// </summary>
		void Holster();

		/// <summary>
		/// 一级开火（主要攻击）
		/// </summary>
		void PrimaryFireUp();

		/// <summary>
		/// 一级停火（主要攻击）
		/// </summary>
		void PrimaryFireDown();

		/// <summary>
		/// 二级开火（次要攻击）
		/// </summary>
		void SecondaryFireUp();

		/// <summary>
		/// 二级停火（次要攻击）
		/// </summary>
		void SecondaryFireDown();
		
		/// <summary>
		/// 重装武器弹药
		/// </summary>
		void Reload();
	}

	public interface IDrawHandler
	{
		void OnDraw(IWeapon weapon);
	}

	public interface IHolsterHandler
	{
		void OnHolster(IWeapon weapon);
	}

	public interface IPrimaryFireHandler
	{
		void OnFireDown(IWeapon weapon);
		void OnFireUp(IWeapon weapon);
	}

	public interface ISecondaryFireHandler
	{
		void OnFireDown(IWeapon weapon);
		void OnFireUp(IWeapon weapon);
	}

	public interface IReloadHandler
	{
		void OnReload(IWeapon weapon);
	}

	/// <summary>
	/// 继承了 <see cref="MonoBehaviour"/> 和 <see cref="IWeapon"/> 的基本武器抽象类
	/// </summary>
	public abstract class MonoWeapon : MonoBehaviour, IWeapon
	{
		public abstract MultiSelectablePartType ContainedPartType { get; }
		public abstract bool IsCompleted { get; }
		public abstract IPart RootPart { get; set; }
		public abstract WeaponAttributes BaseValue { get; }
		public abstract WeaponAttributes ModValue { get; }
		public abstract WeaponAttributes FinalValue { get; }

		protected virtual event WeaponEvent OnDraw;
		protected virtual event WeaponEvent OnHolster;
		protected virtual event WeaponEvent OnPrimaryFireUp;
		protected virtual event WeaponEvent OnPrimaryFireDown;
		protected virtual event WeaponEvent OnSecondaryFireUp;
		protected virtual event WeaponEvent OnSecondaryFireDown;
		//protected virtual event WeaponEvent OnTertiaryFireUp;
		//protected virtual event WeaponEvent OnTertiaryFireDown;
		protected virtual event WeaponEvent OnReload;

		/// <summary>
		/// 清空Weapon里的Event
		/// </summary>
		protected virtual void ClearEvents()
		{
			OnDraw = null;
			OnHolster = null;
			OnPrimaryFireDown = null;
			OnPrimaryFireUp = null;
			OnSecondaryFireDown = null;
			OnSecondaryFireUp = null;
			OnReload = null;
		}

		[SerializeField]
		protected List<MonoPart> partList = new List<MonoPart>();

		public abstract void CompileWeaponAttribute();

		public virtual void Draw()
		{
			OnDraw?.Invoke(this);
		}
		
		public abstract T GetPartOfType<T>();
		public abstract T[] GetPartsOfType<T>();

		public abstract IPart GetPartOfType(PartType type);
		public abstract IPart[] GetPartsOfType(PartType type);

		public virtual void Holster()
		{
			OnHolster?.Invoke(this);
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
	}

	/// <summary>
	/// 基本武器类，对 <see cref="MonoWeapon"/> 的简单实现
	/// </summary>
	public class BasicWeapon : MonoWeapon
	{
		[SerializeField]
		protected MonoPart rootPart;
		public override IPart RootPart
		{
			get => rootPart;
			set
			{
				rootPart = (MonoPart)value;
				if (rootPart == null) return;
				rootPart.SetAsRootPart(this);
			}
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

		protected virtual void OnEnable()
		{
			foreach (IPart p in partList)
			{
				((MonoPart)p).enabled = true;
			}
		}

		protected virtual void OnDisable()
		{
			foreach (IPart p in partList)
			{
				((MonoPart)p).enabled = false;
			}
		}

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
			// 清除该武器中的各项数值和事件记录
			containedPartType.SetDefault();
			baseValue.SetDefault();
			modValue.SetDefault();
			finalValue.SetDefault();
			partList.Clear();
			ClearEvents();

			// 获取值
			GetPartAttributes(baseValue, modValue, containedPartType, rootPart);
			
			// 计算武器的最终数值
			finalValue.Add(baseValue);
			finalValue.Mul(modValue);
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

			// 将该部件加入部件列表中
			partList.Add((MonoPart)part);

			// 将该部件的值加入总和中
			bVal.Add(part.BaseValue);
			mVal.Add(part.ModValue);

			// 记录该部件类型为该武器所拥有的部件类型
			type[part.Type] = true;

			// 如果该部件可处理武器拔出事件，加入
			var draw = part as IDrawHandler;
			if (draw != null) OnDraw += draw.OnDraw;

			// 如果该部件可处理武器收起事件，加入
			var holster = part as IHolsterHandler;
			if (holster != null) OnHolster += holster.OnHolster;

			// 如果该部件可处理武器主要攻击，加入
			var primary = part as IPrimaryFireHandler;
			if (primary != null)
			{
				OnPrimaryFireDown += primary.OnFireDown;
				OnPrimaryFireUp += primary.OnFireUp;
			}

			// 如果该部件可处理武器次要攻击，加入
			var secondary = part as ISecondaryFireHandler;
			if (secondary != null)
			{
				OnSecondaryFireDown += secondary.OnFireDown;
				OnSecondaryFireUp += secondary.OnFireUp;
			}

			// 如果该部件可处理武器重装，加入
			var reload = part as IReloadHandler;
			if (reload != null)
			{
				OnReload += reload.OnReload;
			}

			// 遍历该部件上的所有接口，如果接口上接有部件则进行处理
			var ports = part.Ports;
			foreach (IPort p in ports)
			{
				if (p.AttachedPort == null) continue;
				GetPartAttributes(bVal, mVal, type, p.AttachedPort.Part);
			}
		}

		public override IPart GetPartOfType(PartType type)
		{
			foreach (IPart p in partList)
			{
				if (p.Type == type) return p;
			}

			return null;
		}

		public override IPart[] GetPartsOfType(PartType type)
		{
			var list = new List<IPart>();

			foreach (IPart p in partList)
			{
				if (p.Type == type) list.Add(p);
			}

			return list.ToArray();
		}

		public override T GetPartOfType<T>()
		{
			foreach (IPart p in partList)
			{
				if (p is T) return (T)p;
			}

			return default(T);
		}

		public override T[] GetPartsOfType<T>()
		{
			var list = new List<T>();

			foreach (IPart p in partList)
			{
				if (p is T) list.Add((T)p);
			}

			return list.ToArray();
		}
	}
}
