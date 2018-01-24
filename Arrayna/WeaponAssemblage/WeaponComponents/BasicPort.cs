using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace WeaponAssemblage
{
	/// <summary>
	/// 部件接口的Interface，用于代表部件上的接口
	/// </summary>
	public interface IPort
	{
		/// <summary>
		/// 该接口可接纳的部件类型
		/// </summary>
		MultiSelectablePartType SuitableType { get; }

		/// <summary>
		/// 接口的位置，x和y代表位置，z代表角度[0~360)
		/// </summary>
		Vector3 Position { get; set; }

		/// <summary>
		/// 目前该端口所隶属的部件
		/// </summary>
		IPart Part { get; set; }

		/// <summary>
		/// 目前该端口所连接的端口
		/// </summary>
		IPort AttachedPort { get; }

		/// <summary>
		/// 判断一个部件是否可以接到该端口上
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		bool CanAttachBy(IPart part);

		/// <summary>
		/// 尝试连接部件到该端口上
		/// </summary>
		/// <param name="part">尝试连接的部件</param>
		/// <returns>连接成功即返回true，否则返回false</returns>
		bool Attach(IPart part);

		/// <summary>
		/// 尝试移除该端口上的部件
		/// </summary>
		/// <returns>返回移除下来的部件</returns>
		IPart Detach();

		/// <summary>
		/// 当有部件与该接口连接时触发该事件
		/// </summary>
		event Action<IPort, IPort> OnPortAttached;

		/// <summary>
		/// 当有部件与该接口解除时触发该事件
		/// </summary>
		event Action<IPort, IPort> OnPortDetached;
	}

	/// <summary>
	/// 继承了 <see cref="MonoBehaviour"/> 和 <see cref="IPort"/> 的抽象类
	/// </summary>
	public abstract class MonoPort : MonoBehaviour, IPort
	{
		public abstract MultiSelectablePartType SuitableType { get; }

		public abstract Vector3 Position { get; set; }
		public abstract IPart Part { get; set; }

		public abstract IPort AttachedPort { get; }

		public abstract event Action<IPort, IPort> OnPortAttached;
		public abstract event Action<IPort, IPort> OnPortDetached;

		public abstract bool Attach(IPart part);

		protected abstract bool AttachRoot(IPort port);

		public abstract bool CanAttachBy(IPart part);

		public abstract IPart Detach();
	}

	/// <summary>
	/// 基本接口类，对 <see cref="MonoPort"/> 的简单实现
	/// </summary>
	public class BasicPort : MonoPort
	{
		[SerializeField]
		protected MultiSelectablePartType suitableType;
		public override MultiSelectablePartType SuitableType => suitableType;

		[SerializeField]
		protected Vector3 position;
		public override Vector3 Position
		{
			get => position;
			set => position = value;
		}

		[SerializeField]
		protected MonoPart part;
		public override IPart Part
		{
			get => part;
			set => part = (MonoPart)value;
		}

		[SerializeField]
		protected MonoPort attachedPort;
		public override IPort AttachedPort => attachedPort;

		Action<IPort, IPort> onPortAttached;
		public override event Action<IPort, IPort> OnPortAttached
		{
			add => onPortAttached += value;
			remove => onPortAttached -= value;
		}

		Action<IPort, IPort> onPortDetached;
		public override event Action<IPort, IPort> OnPortDetached
		{
			add => onPortDetached += value;
			remove => onPortDetached -= value;
		}

		/// <summary>
		/// 该接口是否可以接受一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public override bool CanAttachBy(IPart part)
		{
			return suitableType[part.Type];
		}

		/// <summary>
		/// 接入部件到该端口上
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public override bool Attach(IPart part)
		{
			// 如果部件类型不合适
			if (!CanAttachBy(part))
			{
				Debug.LogWarning($"该端口不兼容 {part.Type} 类型的部件！");
				return false;
			}

			// 如果该接口接有部件且无法移除..
			if (AttachedPort != null && Detach() == null)
				return false;

			// Casting.. Dirty?
			attachedPort = (MonoPort)part.RootPort;
			((BasicPort)attachedPort).AttachRoot(this);

			// 调用连接事件
			onPortAttached?.Invoke(this, part.RootPort);

			return true;
		}

		/// <summary>
		/// 连接根接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		protected override bool AttachRoot(IPort port)
		{
			if (!this.part.RootPort.Equals(this))
			{
				Debug.Log($"This function should be called by root port only! \nthis part is:{part.PartName}\n The attach port is from {port.Part.PartName}");
				return false;
			}
			if (this.attachedPort != null) Debug.LogWarning($"This rootport isn't empty! this is part {this.part.PartName}");
			this.attachedPort = (MonoPort)port;
			return true;
		}

		/// <summary>
		/// 从连接接口上移除部件
		/// </summary>
		/// <returns></returns>
		public override IPart Detach()
		{
			// 暂存并清除连接接口
			var port = this.attachedPort;
			this.attachedPort = null;

			// 如果连接接口的接口未清理
			if (port.AttachedPort != null)
			{
				// 对连接接口调用一次解除
				port.Detach();
			}

			// 如果该接口不是根接口
			if (!Part.RootPort.Equals(this))
			{
				// 调用移除事件
				onPortDetached?.Invoke(this, port);
			}
			
			// 返回移除的部件
			return port.Part;
		}
	}
}
