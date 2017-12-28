﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;

namespace WeaponAssemblage
{
	/// <summary>
	/// 武器部件的类型
	/// </summary>
	public enum PartType
	{
		/// <summary>
		/// 枪身
		/// </summary>
		Reciever,
		/// <summary>
		/// 枪管
		/// </summary>
		Barrel,
		/// <summary>
		/// 枪管挂件
		/// </summary>
		BarrelAddon,
		/// <summary>
		/// 枪托
		/// </summary>
		Stock,
		/// <summary>
		/// 瞄具
		/// </summary>
		Sight,
		/// <summary>
		/// 挂件
		/// </summary>
		Addon,
		/// <summary>
		/// 弹夹
		/// </summary>
		Magazine,
		/// <summary>
		/// 子弹
		/// </summary>
		Bullet,
	}

	[Serializable]
	public class MultiSelectablePartType : EnumBaseCollection<PartType, bool> { }

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
		RPM,
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

	[Serializable]
	public class AttributeCollection : EnumBaseCollection<AttributeType, AttributeValue> { }

	/// <summary>
	/// 属性值，用于存放一个属性对应的数值
	/// </summary>
	public struct AttributeValue
	{
		/// <summary>
		/// 参数类型
		/// </summary>
		public AttributeType Type { get; set; }

		/// <summary>
		/// 基本值
		/// </summary>
		public float BaseValue { get; set; }

		/// <summary>
		/// 比例值
		/// </summary>
		public float MODValue { get; set; }
	}

	/// <summary>
	/// 基本的部件interface，用于部件本身
	/// </summary>
	public interface IPart
	{
		/// <summary>
		/// 部件的类型
		/// </summary>
		PartType Type { get; }

		/// <summary>
		/// 部件的主接口
		/// </summary>
		IPartPort RootPort { get; }

		/// <summary>
		/// 该武器部件的属性
		/// </summary>
		AttributeCollection Attributes { get; }

		/// <summary>
		/// 判断这个部件是否可以安装到一个端口上
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		bool CanAttachTo(IPartPort port);

		/// <summary>
		/// 添加一个接口到该部件上
		/// </summary>
		/// <param name="part">尝试连接的部件</param>
		/// <returns>连接成功即返回true，否则返回false</returns>
		bool AddPort(IPartPort port);

		/// <summary>
		/// 从该部件上移除一个接口
		/// </summary>
		/// <param name="part">尝试连接的部件</param>
		/// <returns>解除成功即返回true，否则返回false</returns>
		bool RemovePort(IPartPort port);

		/// <summary>
		/// 当有部件连接到该部件时触发该事件
		/// </summary>
		event Action<IPart, IPart> OnPartAttached;

		/// <summary>
		/// 当有部件与该部件解除时触发该事件
		/// </summary>
		event Action<IPart, IPart> OnPartDetached;
	}

	/// <summary>
	/// 部件接口的Interface，用于代表部件上的接口
	/// </summary>
	public interface IPartPort
	{
		/// <summary>
		/// 该接口可接纳的部件类型
		/// </summary>
		MultiSelectablePartType SuitableType { get; }

		/// <summary>
		/// 接口的位置
		/// </summary>
		Vector2 Position { get; }
		
		/// <summary>
		/// 目前该端口所隶属的部件
		/// </summary>
		IPart Part { get; }

		/// <summary>
		/// 目前该端口所连接的部件
		/// </summary>
		IPart AttachedPart { get; }

		/// <summary>
		/// 判断这个端口是否可以接纳一个部件上
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		bool CanAttachBy(IPart part);

		/// <summary>
		/// 尝试连接部件到该端口上
		/// </summary>
		/// <param name="part">尝试连接的部件</param>
		/// <returns>连接成功即返回true，否则返回false</returns>
		bool AttachPart(IPart part);

		/// <summary>
		/// 尝试移除该端口上的部件
		/// </summary>
		/// <returns>返回移除下来的部件</returns>
		IPart DetachPart();

		/// <summary>
		/// 当有部件与该接口连接时触发该事件
		/// </summary>
		event Action<IPartPort, IPartPort> OnPortAttached;

		/// <summary>
		/// 当有部件与该接口解除时触发该事件
		/// </summary>
		event Action<IPartPort, IPartPort> OnPortDetached;
	}

	public class BasicPort : MonoBehaviour, IPartPort
	{
		[SerializeField]
		protected MultiSelectablePartType suitableType;
		public MultiSelectablePartType SuitableType => suitableType;

		public Vector2 Position { get; private set; }

		public IPart Part { get; private set; }

		public IPart AttachedPart { get; private set; }

		public event Action<IPartPort, IPartPort> OnPortAttached;
		public event Action<IPartPort, IPartPort> OnPortDetached;

		public bool CanAttachBy(IPart part)
		{
			return suitableType[part.Type];
		}

		public bool AttachPart(IPart part)
		{
			throw new NotImplementedException();
		}

		public IPart DetachPart()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// 基本部件类
	/// </summary>
	public class BasicPart : MonoBehaviour, IPart
	{
		[SerializeField, Tooltip("部件的重量")]
		protected int weight;
		public int Weight => weight;

		[SerializeField, Tooltip("部件的类型")]
		protected PartType type;
		public PartType Type => type;

		[SerializeField, Tooltip("部件的根端口")]
		protected IPartPort rootPort;
		public IPartPort RootPort => rootPort;

		[SerializeField, Tooltip("部件的各项属性")]
		protected AttributeCollection attributes;
		public AttributeCollection Attributes => attributes;

		[SerializeField, Tooltip("部件的各个端口")]
		protected List<IPartPort> ports;

		Action<IPart, IPart> onPartAttached;
		event Action<IPart, IPart> IPart.OnPartAttached
		{
			add { onPartAttached += value; }
			remove { onPartAttached -= value; }
		}

		Action<IPart, IPart> onPartDetached;
		event Action<IPart, IPart> IPart.OnPartDetached
		{
			add { onPartDetached += value; }
			remove { onPartDetached -= value; }
		}

		public bool CanAttachTo(IPartPort port)
		{
			return port.CanAttachBy(this);
		}

		/// <summary>
		/// 为该部件添加一个接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool AddPort(IPartPort port)
		{
			if (port == null) throw new NullReferenceException();

			if (ports.Contains(port)) Debug.LogWarning($"{this.name} 已经包含此接口");

			// 挂接事件
			port.OnPortDetached += PartDetached;
			port.OnPortAttached += PartAttached;
			return true;
		}

		/// <summary>
		/// 为该部件移除一个接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool RemovePort(IPartPort port)
		{
			if (port == null) throw new NullReferenceException();

			if (!ports.Contains(port)) Debug.LogWarning($"{this.name} 并不包含此接口");

			// 取消事件
			port.OnPortDetached -= PartDetached;
			port.OnPortAttached -= PartAttached;
			return true;
		}

		/// <summary>
		/// 用于接收来自部件接口的连接事件
		/// </summary>
		/// <param name="callerport"></param>
		/// <param name="calleeport"></param>
		void PartAttached(IPartPort callerport, IPartPort calleeport)
		{
			if (!callerport.Part.Equals(this)) return;

			onPartAttached?.Invoke(this, calleeport.Part);
		}

		/// <summary>
		/// 用于接收来自部件接口的移除事件
		/// </summary>
		/// <param name="callerport"></param>
		/// <param name="calleeport"></param>
		void PartDetached(IPartPort callerport, IPartPort calleeport)
		{
			if (!callerport.Part.Equals(this)) return;

			onPartDetached?.Invoke(this, calleeport.Part);
		}
	}

	/// <summary>
	/// 代表一把完整的武器
	/// </summary>
	public class Weapon
	{
		/// <summary>
		/// 武器的核心部件
		/// </summary>
		protected IPart rootPart;
		IPart RootPart => rootPart;
	}
}
