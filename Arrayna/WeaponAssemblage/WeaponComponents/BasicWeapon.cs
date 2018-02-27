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
		/// 武器的运行数据
		/// </summary>
		RuntimeValues RuntimeValues { get; }

		/// <summary>
		/// 该武器是否完善可用
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// 该武器的核心部件
		/// </summary>
		IPart RootPart { get; set; }

		/// <summary>
		/// 该武器的各个部件
		/// </summary>
		IEnumerable<IPart> Parts { get; }

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
		void OnPrimaryFireDown(IWeapon weapon);
		void OnPrimaryReadyToFire(IWeapon weapon);
		void OnPrimaryFireUp(IWeapon weapon);
	}

	public interface ISecondaryFireHandler
	{
		void OnSecondaryFireDown(IWeapon weapon);
		void OnSecondaryReadyToFire(IWeapon weapon);
		void OnSecondaryFireUp(IWeapon weapon);
	}

	public interface IReloadHandler
	{
		void OnReload(IWeapon weapon);
		void OnReloadOver(IWeapon weapon);
	}

	/// <summary>
	/// 继承了 <see cref="MonoBehaviour"/> 和 <see cref="IWeapon"/> 的基本武器抽象类
	/// </summary>
	public abstract class MonoWeapon : MonoBehaviour, IWeapon
	{
		public abstract MultiSelectablePartType ContainedPartType { get; }
		public abstract bool IsCompleted { get; }
		public abstract IPart RootPart { get; set; }
		public abstract IEnumerable<IPart> Parts { get; }
		public abstract WeaponAttributes BaseValue { get; }
		public abstract WeaponAttributes ModValue { get; }
		public abstract WeaponAttributes FinalValue { get; }
		public abstract RuntimeValues RuntimeValues { get; }

		protected virtual event WeaponEvent OnDraw;
		protected virtual event WeaponEvent OnHolster;
		protected virtual event WeaponEvent OnPrimaryFireUp;
		protected virtual event WeaponEvent OnPrimaryReadyToFire;
		protected virtual event WeaponEvent OnPrimaryFireDown;
		protected virtual event WeaponEvent OnSecondaryFireUp;
		protected virtual event WeaponEvent OnSecondaryReadyToFire;
		protected virtual event WeaponEvent OnSecondaryFireDown;
		//protected virtual event WeaponEvent OnTertiaryFireUp;
		//protected virtual event WeaponEvent OnTertiaryFireDown;
		protected virtual event WeaponEvent OnReload;
		protected virtual event WeaponEvent OnReloadOver;

		/// <summary>
		/// 清空Weapon里的Event
		/// </summary>
		protected virtual void ClearEvents()
		{
			OnDraw = null;
			OnHolster = null;
			OnPrimaryFireDown = null;
			OnPrimaryReadyToFire = null;
			OnPrimaryFireUp = null;
			OnSecondaryFireDown = null;
			OnSecondaryReadyToFire = null;
			OnSecondaryFireUp = null;
			OnReload = null;
			OnReloadOver = null;
		}

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
			if (RuntimeValues.HoldingFire) return;
			RuntimeValues.HoldingFire = true;
			OnPrimaryFireDown?.Invoke(this);
			print("Fire Down");
		}

		public virtual void PrimaryFireUp()
		{
			if (!RuntimeValues.HoldingFire) return;
			RuntimeValues.HoldingFire = false;
			OnPrimaryFireUp?.Invoke(this);
			print("Fire Up");
		}

		public virtual void Reload()
		{
			if (RuntimeValues.ReloadTime > 0 || 
				RuntimeValues.ShotAmmo <= 0) return;
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

		protected virtual void Update()
		{
			if (RuntimeValues.ReloadTime > 0)
			{
				RuntimeValues.ReloadTime -= Time.deltaTime;
				if (RuntimeValues.ReloadTime <= 0)
				{
					OnReloadOver?.Invoke(this);
				}
			}
			if (RuntimeValues.FireTime > 0) RuntimeValues.FireTime -= Time.deltaTime;
			if (RuntimeValues.Dispersal > 0) RuntimeValues.Dispersal *= 1 - (Time.deltaTime * RuntimeValues.DispersalDecreRate);

			if (!RuntimeValues.HoldingFire || RuntimeValues.FireTime > 0 || RuntimeValues.ReloadTime > 0) return;

			// 如果弹药已经打空，这一轮进行装弹
			if (RuntimeValues.ShotAmmo >= FinalValue[WpnAttrType.Capacity])
			{
				Reload();
				return;
			}

			// 发射子弹
			OnPrimaryReadyToFire?.Invoke(this);
		}
	}

	/// <summary>
	/// 基本武器类，对 <see cref="MonoWeapon"/> 的简单实现
	/// </summary>
	public class BasicWeapon : MonoWeapon, ISerializationCallbackReceiver
	{
		#region Fields

		[SerializeField]
		protected MonoPart rootPart;

		[SerializeField]
		protected List<MonoPart> monoPartList = new List<MonoPart>();
		protected List<IPart> partList = new List<IPart>();
		
		[SerializeField]
		protected MultiSelectablePartType containedPartType = new MultiSelectablePartType();
		
		[SerializeField]
		protected WeaponAttributes baseValue = new WeaponAttributes();
		
		[SerializeField]
		protected WeaponAttributes modValue = new WeaponAttributes();
		
		[SerializeField]
		protected WeaponAttributes finalValue = new WeaponAttributes();
		
		[SerializeField]
		protected RuntimeValues rtValues = new RuntimeValues();

		#endregion

		#region Public properties

		public override bool IsCompleted
		{
			get
			{
				return
					ContainedPartType[PartType.Receiver]
				 && ContainedPartType[PartType.Barrel]
				 && ContainedPartType[PartType.Magazine]
				 && ContainedPartType[PartType.Bullet];
			}
		}

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

		public override IEnumerable<IPart> Parts => partList.AsEnumerable();

		public override WeaponAttributes BaseValue => baseValue;
		public override WeaponAttributes ModValue => modValue;
		public override WeaponAttributes FinalValue => finalValue;

		public override MultiSelectablePartType ContainedPartType => containedPartType;

		public override RuntimeValues RuntimeValues => rtValues;

		#endregion

		#region Public methods

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
			modValue.Add(1);
			finalValue.Mul(modValue);
			rtValues.DispersalIncrement = 10 - finalValue[WpnAttrType.Stability] * 0.1f;
			if (rtValues.DispersalIncrement < 0) rtValues.DispersalIncrement = 0;
			rtValues.DispersalDecreRate = FinalValue[WpnAttrType.Stability] * 0.025f;
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

		#endregion

		#region Protected & Private methods

		/// <summary>
		/// 获取一个部件及其子部件的值
		/// </summary>
		/// <param name="attr"></param>
		/// <param name="part"></param>
		protected virtual void GetPartAttributes(WeaponAttributes bVal, WeaponAttributes mVal, MultiSelectablePartType type, IPart part)
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
				OnPrimaryFireDown += primary.OnPrimaryFireDown;
				OnPrimaryReadyToFire += primary.OnPrimaryReadyToFire;
				OnPrimaryFireUp += primary.OnPrimaryFireUp;
			}

			// 如果该部件可处理武器次要攻击，加入
			var secondary = part as ISecondaryFireHandler;
			if (secondary != null)
			{
				OnSecondaryFireDown += secondary.OnSecondaryFireDown;
				OnSecondaryReadyToFire += secondary.OnSecondaryReadyToFire;
				OnSecondaryFireUp += secondary.OnSecondaryFireUp;
			}

			// 如果该部件可处理武器重装，加入
			var reload = part as IReloadHandler;
			if (reload != null)
			{
				OnReload += reload.OnReload;
				OnReloadOver += reload.OnReloadOver;
			}

			// 遍历该部件上的所有接口，如果接口上接有部件则进行处理
			var ports = part.Ports;
			foreach (IPort p in ports)
			{
				if (p.AttachedPort == null) continue;
				GetPartAttributes(bVal, mVal, type, p.AttachedPort.Part);
			}
		}

		#endregion
		
		#region Unity methods

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

		#endregion

		#region Serializaion

		public virtual void OnBeforeSerialize()
		{
			monoPartList.Clear();
			foreach (MonoPart p in partList)
			{
				monoPartList.Add(p);
			}
		}

		public virtual void OnAfterDeserialize()
		{
			partList.Clear();
			foreach (IPart p in monoPartList)
			{
				partList.Add(p);
			}
		}
		#endregion
	}
}
