﻿using System;
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
		bool AttachPart(IPart part);

		/// <summary>
		/// 尝试移除该端口上的部件
		/// </summary>
		/// <returns>返回移除下来的部件</returns>
		IPart DetachPart();

		/// <summary>
		/// 尝试从端口上移除该部件
		/// </summary>
		/// <returns>返回移除下来的部件</returns>
		IPart DetachFrom();

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
	/// TODO: 写一个Editor来编辑接口
	public abstract class MonoPort : MonoBehaviour, IPort
	{
		public abstract MultiSelectablePartType SuitableType { get; }

		public abstract Vector3 Position { get; set; }
		public abstract IPart Part { get; set; }

		public abstract IPort AttachedPort { get; }

		public abstract event Action<IPort, IPort> OnPortAttached;
		public abstract event Action<IPort, IPort> OnPortDetached;

		public abstract bool AttachPart(IPart part);

		public abstract bool CanAttachBy(IPart part);

		public abstract IPart DetachFrom();

		public abstract IPart DetachPart();
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
		public override bool AttachPart(IPart part)
		{
			if (!CanAttachBy(part))
			{
				Debug.LogWarning($"该端口不兼容 {part.Type} 类型的部件！");
				return false;
			}

			if (AttachedPort != null && DetachPart() == null)
				return false;

			// Casting.. Dirty?
			attachedPort = (MonoPort)part.RootPort;

			onPortAttached?.Invoke(this, part.RootPort);

			return true;
		}

		/// <summary>
		/// 从连接接口上移除部件
		/// </summary>
		/// <returns></returns>
		public override IPart DetachPart()
		{
			var port = AttachedPort;
			attachedPort = null;
			onPortDetached?.Invoke(this, port);
			return port.Part;
		}

		/// <summary>
		/// 把自身从所连接的部件上移除
		/// </summary>
		/// <returns></returns>
		public override IPart DetachFrom()
		{
			return AttachedPort.DetachPart();
		}
	}
}
