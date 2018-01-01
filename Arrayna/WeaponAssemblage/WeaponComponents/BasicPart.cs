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
		IPort RootPort { get; }

		/// <summary>
		/// 部件的各个接口
		/// </summary>
		IEnumerable<IPort> Ports { get; }

		/// <summary>
		/// 部件的接口数量(不包括root port)
		/// </summary>
		int PortCount { get; }

		/// <summary>
		/// 该武器部件的属性
		/// </summary>
		WeaponAttributes Attributes { get; }

		/// <summary>
		/// 判断这个部件是否可以安装到一个端口上
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		bool CanAttachTo(IPort port);

		/// <summary>
		/// 添加一个接口到该部件上
		/// </summary>
		/// <param name="part">尝试连接的部件</param>
		/// <returns>连接成功即返回true，否则返回false</returns>
		bool AddPort(IPort port, bool isRootPart = false);

		/// <summary>
		/// 从该部件上移除一个接口
		/// </summary>
		/// <param name="part">尝试连接的部件</param>
		/// <returns>解除成功即返回true，否则返回false</returns>
		bool RemovePort(IPort port);

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
	/// 继承了 <see cref="MonoBehaviour"/> 和 <see cref="IPart"/> 的基本部件抽象类
	/// </summary>
	public abstract class MonoPart : MonoBehaviour, IPart, ISerializationCallbackReceiver
	{
		public abstract string PartName { get; }

		public abstract string Description { get; }

		public abstract IWeapon Weapon { get; }

		public abstract PartType Type { get; }

		public abstract IPort RootPort { get; }

		public abstract IEnumerable<IPort> Ports { get; }

		public abstract int PortCount { get; }

		public abstract WeaponAttributes Attributes { get; }

		public abstract event Action<IPart, IPart> OnPartAttached;
		public abstract event Action<IPart, IPart> OnPartDetached;

		public abstract void RecollectPorts();

		public abstract bool AddPort(IPort port, bool isRootPart = false);

		public abstract bool CanAttachTo(IPort port);

		public abstract bool RemovePort(IPort port);

		public abstract void OnBeforeSerialize();

		public abstract void OnAfterDeserialize();
	}

	/// <summary>
	/// 基本部件类，对 <see cref="MonoPart"/> 的简单实现
	/// </summary>
	public class BasicPart : MonoPart
	{
		[SerializeField, Tooltip("部件的名称")]
		protected string partName;
		public override string PartName => partName;

		[SerializeField, Tooltip("部件的描述信息")]
		protected string description;
		public override string Description => description;

		[SerializeField, Tooltip("部件的类型")]
		protected PartType type;
		public override PartType Type => type;

		[SerializeField, Tooltip("部件的根端口")]
		protected MonoPort rootPort;
		public override IPort RootPort => rootPort;

		[SerializeField, Tooltip("部件所属的武器")]
		protected IWeapon weapon;
		public override IWeapon Weapon => weapon;

		[SerializeField, Tooltip("部件的各项属性")]
		protected WeaponAttributes attributes;
		public override WeaponAttributes Attributes => attributes;

		[SerializeField, Tooltip("部件的各个端口")]
		protected List<MonoPort> monoports = new List<MonoPort>();
		protected List<IPort> ports = new List<IPort>();
		public override IEnumerable<IPort> Ports => ports.AsEnumerable<IPort>();
		public override int PortCount => ports.Count;

		Action<IPart, IPart> onPartAttached;
		public override event Action<IPart, IPart> OnPartAttached
		{
			add { onPartAttached += value; }
			remove { onPartAttached -= value; }
		}

		Action<IPart, IPart> onPartDetached;
		public override event Action<IPart, IPart> OnPartDetached
		{
			add { onPartDetached += value; }
			remove { onPartDetached -= value; }
		}

		protected void Awake()
		{
			RecollectPorts();
		}

		protected void Start()
		{
			// 如果根节点不存在
			if (rootPort == null)
			{
				// 报错并终止该MonoBehaviour
				Debug.LogError($"该部件 {this.name} 缺少RootPort！");
				this.enabled = false;
				return;
				// 创建一个新节点
				//var port = new GameObject("RootPort").AddComponent<BasicPort>();
				//port.transform.parent = this.transform;
				//rootPort = port;
				//rootPort.Part = this;
			}
		}

		/// <summary>
		/// 判断该部件是否可以连接到一个接口上
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public override bool CanAttachTo(IPort port)
		{
			return port.CanAttachBy(this);
		}

		/// <summary>
		/// 重新收集和同步接口，由于Unity无法序列化接口，所以需要另外一个可以序列化的接口列表来处理
		/// </summary>
		public override void RecollectPorts()
		{
			if (ports.Count <= monoports.Count)
			{
				ports.Clear();

				for (int i = 0; i < monoports.Count; i ++)
				{
					if (monoports[i].IsExists())
						ports.Add(monoports[i]);
					else
						monoports.RemoveAt(i);
				}
			}

			if (ports.Count > monoports.Count)
			{
				monoports.Clear();

				for (int i = 0; i < ports.Count; i++)
				{
					if (ports[i].IsExists())
						monoports.Add((MonoPort)ports[i]);
					else
						ports.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// 为该部件添加一个接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public override bool AddPort(IPort port, bool isRootPart = false)
		{
			if (port == null) return false;

			if (isRootPart)
			{
				if (rootPort == null)
				{
					// Casting.. Dirty?
					rootPort = (MonoPort)port;
					rootPort.Part = this;
					return true;
				}
				else
				{
					Debug.LogWarning("尝试重复添加rootPort！");
					return false;
				}
			}

			if (ports.Contains(port))
			{
				Debug.LogWarning($"{this.name} 已经包含此接口");
				return false;
			}

			monoports.Add(port as MonoPort);
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
		public override bool RemovePort(IPort port)
		{
			if (port == null) return false;

			if (!port.Part.Equals(this))
			{
				Debug.LogWarning($"{this.name} 并不包含此接口");
				return false;
			}

			monoports.Remove(port as MonoPort);
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
		protected virtual void PartAttached(IPort callerport, IPort calleeport)
		{
			if (!callerport.Part.Equals(this)) return;

			onPartAttached?.Invoke(this, calleeport.Part);
		}

		/// <summary>
		/// 用于接收来自部件接口的移除事件
		/// </summary>
		/// <param name="callerport"></param>
		/// <param name="calleeport"></param>
		protected virtual void PartDetached(IPort callerport, IPort calleeport)
		{
			if (!callerport.Part.Equals(this)) return;

			onPartDetached?.Invoke(this, calleeport.Part);
		}

		////// Unity Serialization //////
		
		public override void OnBeforeSerialize()
		{
			monoports.Clear();
			for (int i = 0; i < ports.Count; i ++)
			{
				monoports.Add((MonoPort)ports[i]);
				if (monoports[i] == null)
				{
					Debug.Log("端口异常，有非继承于MonoPort的端口");
				}
			}
		}

		public override void OnAfterDeserialize()
		{
			ports.Clear();
			for (int i = 0; i < monoports.Count; i++)
			{
				ports.Add(monoports[i]);
				if (ports[i] == null)
				{
					Debug.Log("端口异常，有端口为空");
				}
			}
		}
	}
}
