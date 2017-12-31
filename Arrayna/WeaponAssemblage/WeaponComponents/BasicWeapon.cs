using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage
{
	/// <summary>
	/// 武器接口，代表一把完整的武器
	/// </summary>
	public interface IWeapon
	{
	}
	
	/// <summary>
	/// 代表一把完整的武器
	/// </summary>
	public class BasicWeapon
	{
		/// <summary>
		/// 包含的部件类型
		/// </summary>
		public MultiSelectablePartType ContainedPart { get; private set; }

		/// <summary>
		/// 武器的基本数值属性
		/// </summary>
		public WeaponAttributes AttributeValues { get; private set; }

		/// <summary>
		/// 该武器是否完善可用
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				return 
					ContainedPart[PartType.Reciever]
				 && ContainedPart[PartType.Barrel]
				 && ContainedPart[PartType.Magazine]
				 && ContainedPart[PartType.Bullet];
			}
		}

		/// <summary>
		/// 武器的核心部件
		/// </summary>
		protected IPart rootPart;
		IPart RootPart => rootPart;

		/// <summary>
		/// 计算该武器的各项数值
		/// </summary>
		public void CompileWeapon()
		{
			ContainedPart.Clear();
			AttributeValues.Clear();
			GetPartAttributes(AttributeValues, rootPart);
		}

		/// <summary>
		/// 获取一个部件及其子部件的值
		/// </summary>
		/// <param name="attr"></param>
		/// <param name="part"></param>
		void GetPartAttributes(WeaponAttributes attr, IPart part)
		{
			if (attr == null) throw new ArgumentNullException();
			if (part == null) return;

			var ports = part.Ports;

			foreach (IPartPort p in ports)
			{
				ContainedPart[p.Part.Type] = true;
				GetPartAttributes(attr, p.Part);
				attr.Add(p.Part.Attributes);
			}
		}
	}
}
