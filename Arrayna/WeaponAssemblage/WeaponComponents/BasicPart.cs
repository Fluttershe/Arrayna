using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
		Receiver,
		/// <summary>
		/// 枪管
		/// </summary>
		Barrel,
		/// <summary>
		/// 枪托
		/// </summary>
		Stock,
		/// <summary>
		/// 瞄具
		/// </summary>
		Sight,
		/// <summary>
		/// 弹夹
		/// </summary>
		Magazine,
		/// <summary>
		/// 子弹
		/// </summary>
		Bullet,
		/// <summary>
		/// 挂件
		/// </summary>
		Addon,
		/// <summary>
		/// 枪管挂件
		/// </summary>
		BarrelAddon,
	}

	/// <summary>
	/// 可多选的部件类型类
	/// </summary>
	[Serializable]
	public class MultiSelectablePartType : EnumBasedCollection<PartType, bool> { }

	/// <summary>
	/// 基本的部件interface，用于部件本身
	/// </summary>
	public interface IPart
	{
		/// <summary>
		/// 部件的 Prefab code
		/// </summary>
		string PrefabID { get; }

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
		/// 部件的各个辅助接口
		/// </summary>
		IEnumerable<IPort> AsstPorts { get; }

		/// <summary>
		/// 部件的接口数量(不包括root port)
		/// </summary>
		int PortCount { get; }

		/// <summary>
		/// 该武器部件的数值属性
		/// </summary>
		WeaponAttributes BaseValue { get; }

		/// <summary>
		/// 该武器部件的倍值属性
		/// </summary>
		WeaponAttributes ModValue { get; }

		/// <summary>
		/// 将该部件设为特定武器的rootpart
		/// </summary>
		/// <param name="weapon"></param>
		void SetAsRootPart(IWeapon weapon);

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
		/// 更新该部件所属的武器
		/// </summary>
		void UpdateBelongingWeapon();

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

		public abstract IWeapon Weapon { get; protected set; }

		public abstract PartType Type { get; }

		public abstract IPort RootPort { get; }

		public abstract IEnumerable<IPort> Ports { get; }
		public abstract IEnumerable<IPort> AsstPorts { get; }

		public abstract int PortCount { get; }

		public abstract WeaponAttributes BaseValue { get; }
		public abstract WeaponAttributes ModValue { get; }

		[SerializeField]
		string prefabID;
		public string PrefabID => prefabID;

		public abstract event Action<IPart, IPart> OnPartAttached;
		public abstract event Action<IPart, IPart> OnPartDetached;

		public abstract void SetAsRootPart(IWeapon weapon);
		public abstract bool AddPort(IPort port, bool isRootPart = false);

		public abstract bool CanAttachTo(IPort port);

		public abstract bool RemovePort(IPort port);

		public abstract void UpdateBelongingWeapon();

		public abstract void OnBeforeSerialize();

		public abstract void OnAfterDeserialize();
	}

	/// <summary>
	/// 基本部件类，对 <see cref="MonoPart"/> 的简单实现
	/// </summary>
	[Serializable]
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
		protected MonoWeapon weapon;
		public override IWeapon Weapon
		{
			get => weapon;
			protected set => weapon = (MonoWeapon)value;
		}

		[SerializeField, Tooltip("部件的数值属性")]
		protected WeaponAttributes baseValue;
		public override WeaponAttributes BaseValue => baseValue;
		
		[SerializeField, Tooltip("部件的倍值属性")]
		protected WeaponAttributes modValue;
		public override WeaponAttributes ModValue => modValue;

		[SerializeField, Tooltip("部件的各个端口")]
		protected List<MonoPort> monoPorts = new List<MonoPort>();
		
		[SerializeField, Tooltip("部件的辅助端口")]
		protected List<MonoPort> monoAsstPorts = new List<MonoPort>();

		protected List<IPort> portList = new List<IPort>();
		protected List<IPort> asstPortList = new List<IPort>();
		public override IEnumerable<IPort> Ports => portList.AsEnumerable();
		public override IEnumerable<IPort> AsstPorts => asstPortList.AsEnumerable();
		public override int PortCount => portList.Count;
		
		public override event Action<IPart, IPart> OnPartAttached;
		public override event Action<IPart, IPart> OnPartDetached;

		protected virtual void Awake()
		{
			// 如果根节点不存在
			if (rootPort == null)
			{
				// 报错并终止该MonoBehaviour
				Debug.LogError($"该部件 {this.name} 缺少RootPort！");
				this.enabled = false;
				return;
			}

			// 将连接和移除事件挂接在每个接口上
			foreach (IPort port in portList)
			{
				port.OnPortDetached += PartDetached;
				port.OnPortAttached += PartAttached;
			}
		}

		/// <summary>
		/// 判断该部件是否可以连接到一个接口上
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public override bool CanAttachTo(IPort port)
		{
			return port.CanBeAttachedBy(this);
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

			if (port.IsAssistantPort)
			{
				if (asstPortList.Contains(port))
				{
					Debug.LogWarning($"{this.name} 已经包含此接口");
					return false;
				}

				asstPortList.Add(port);
				port.Part = this;
				return true;
			}

			if (portList.Contains(port))
			{
				Debug.LogWarning($"{this.name} 已经包含此接口");
				return false;
			}
			
			portList.Add(port);
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
			if (port == null || port.Equals(this.rootPort)) return false;

			if (!port.Part.Equals(this))
			{
				Debug.LogWarning($"{this.name} 并不包含此接口");
				return false;
			}

			port.Part = null;

			if (port.IsAssistantPort)
			{
				asstPortList.Remove(port);
			}
			else
			{
				portList.Remove(port);

				// 取消事件
				port.OnPortDetached -= PartDetached;
				port.OnPortAttached -= PartAttached;
			}

			return true;
		}

		/// <summary>
		/// 更新所属武器
		/// </summary>
		public override void UpdateBelongingWeapon()
		{
			Debug.Log($"Update: {PartName}, RootPart: {weapon?.RootPart?.Equals(this)}, Root attached: {rootPort.AttachedPort}");

			// 如果该部件不是武器的根部件，将weapon设为null
			if ((weapon?.RootPart?.Equals(this)) != true)
				weapon = null;

			// 如果根接口所连部件不为null, 将所属武器设置为等同于根接口部件
			if (rootPort.AttachedPort != null)
				weapon = (MonoWeapon)rootPort.AttachedPort.Part.Weapon;

			// 更新所有连接到这个部件上的其它部件
			foreach(IPort p in portList)
			{
				if (p.AttachedPort == null) continue;
				p.AttachedPort.Part.UpdateBelongingWeapon();
			}
		}

		/// <summary>
		/// 用于接收来自部件接口的连接事件
		/// </summary>
		/// <param name="callerport"></param>
		/// <param name="calleeport"></param>
		protected virtual void PartAttached(IPort callerport, IPort calleeport)
		{
			Debug.Log($"Part attached: {callerport.Part.PartName} -> {calleeport.Part.PartName}");
			if (!callerport.Part.Equals(this)) return;
			OnPartAttached?.Invoke(this, calleeport.Part);
			((BasicPart)calleeport.Part).UpdateBelongingWeapon();
		}

		/// <summary>
		/// 用于接收来自部件接口的移除事件
		/// </summary>
		/// <param name="callerport"></param>
		/// <param name="calleeport"></param>
		protected virtual void PartDetached(IPort callerport, IPort calleeport)
		{
			Debug.Log($"Part detached: {callerport.Part.PartName} -> {calleeport.Part.PartName}");
			if (!callerport.Part.Equals(this)) return;
			OnPartDetached?.Invoke(this, calleeport.Part);
			((BasicPart)calleeport.Part).UpdateBelongingWeapon();
		}

		////// Unity Serialization //////
		
		public override void OnBeforeSerialize()
		{
			monoPorts.Clear();
			for (int i = 0; i < portList.Count; i ++)
			{
				monoPorts.Add((MonoPort)portList[i]);
			}

			monoAsstPorts.Clear();
			for (int i = 0; i < asstPortList.Count; i++)
			{
				monoAsstPorts.Add((MonoPort)asstPortList[i]);
			}
		}

		public override void OnAfterDeserialize()
		{
			portList.Clear();
			for (int i = 0; i < monoPorts.Count; i++)
			{
				portList.Add(monoPorts[i]);
			}

			asstPortList.Clear();
			for (int i = 0; i < monoAsstPorts.Count; i++)
			{
				asstPortList.Add(monoAsstPorts[i]);
			}
		}

		public override void SetAsRootPart(IWeapon weapon)
		{
			this.weapon = (MonoWeapon)weapon;
		}
	}
}
