using System;
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

	/// <summary>
	/// 可多选的部件类型类
	/// </summary>
	[Serializable]
	public class MultiSelectablePartType : EnumBaseCollection<PartType, bool> { }

	/// <summary>
	/// 基本的部件interface，用于部件本身
	/// </summary>
	public interface IPart
	{
		/// <summary>
		/// 部件的名称
		/// </summary>
		string PartName { get; }

		/// <summary>
		/// 部件的描述信息
		/// </summary>
		string Description { get; }

		/// <summary>
		/// 该部件所属的武器
		/// </summary>
		IWeapon Weapon { get; }

		/// <summary>
		/// 部件的类型
		/// </summary>
		PartType Type { get; }

		/// <summary>
		/// 部件的主接口
		/// </summary>
		IPartPort RootPort { get; }

		/// <summary>
		/// 部件的各个接口
		/// </summary>
		IEnumerable<IPartPort> Ports { get; }

		/// <summary>
		/// 该武器部件的属性
		/// </summary>
		WeaponAttributes Attributes { get; }

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
	/// 基本部件类
	/// </summary>
	public class BasicPart : MonoBehaviour, IPart
	{
		[SerializeField, Tooltip("部件的名称")]
		protected string partName;
		public string PartName => partName;

		[SerializeField, Tooltip("部件的描述信息")]
		protected string description;
		public string Description => description;

		[SerializeField, Tooltip("部件所属的武器")]
		protected IWeapon weapon;
		public IWeapon Weapon { get; }

		[SerializeField, Tooltip("部件的类型")]
		protected PartType type;
		public PartType Type => type;

		[SerializeField, Tooltip("部件的根端口")]
		protected IPartPort rootPort;
		public IPartPort RootPort => rootPort;

		[SerializeField, Tooltip("部件的各项属性")]
		protected WeaponAttributes attributes;
		public WeaponAttributes Attributes => attributes;

		[SerializeField, Tooltip("部件的各个端口")]
		protected List<IPartPort> ports = new List<IPartPort>();
		public IEnumerable<IPartPort> Ports => ports.AsEnumerable();

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

		/// <summary>
		/// 判断该部件是否可以连接到一个接口上
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public virtual bool CanAttachTo(IPartPort port)
		{
			return port.CanAttachBy(this);
		}

		/// <summary>
		/// 为该部件添加一个接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public virtual bool AddPort(IPartPort port)
		{
			if (port == null) throw new NullReferenceException();

			if (ports.Contains(port))
			{
				Debug.LogWarning($"{this.name} 已经包含此接口");
				return false;
			}

			ports.Add(port);
			port.Part = this;
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
		public virtual bool RemovePort(IPartPort port)
		{
			if (port == null) throw new NullReferenceException();

			if (!port.Part.Equals(this))
			{
				Debug.LogWarning($"{this.name} 并不包含此接口");
				return false;
			}

			ports.Remove(port);
			port.Part = null;
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
		protected virtual void PartAttached(IPartPort callerport, IPartPort calleeport)
		{
			if (!callerport.Part.Equals(this)) return;

			onPartAttached?.Invoke(this, calleeport.Part);
		}

		/// <summary>
		/// 用于接收来自部件接口的移除事件
		/// </summary>
		/// <param name="callerport"></param>
		/// <param name="calleeport"></param>
		protected virtual void PartDetached(IPartPort callerport, IPartPort calleeport)
		{
			if (!callerport.Part.Equals(this)) return;

			onPartDetached?.Invoke(this, calleeport.Part);
		}
	}
}
