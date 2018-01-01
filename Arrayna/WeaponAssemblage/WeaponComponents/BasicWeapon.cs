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
		/// <summary>
		/// 包含的部件类型
		/// </summary>
		MultiSelectablePartType ContainedPart { get; }

		/// <summary>
		/// 武器的基本数值属性
		/// </summary>
		WeaponAttributes AttributeValues { get; }

		/// <summary>
		/// 该武器是否完善可用
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// 该武器的核心部件
		/// </summary>
		IPart RootPart { get; }

		/// <summary>
		/// 计算武器的数值
		/// </summary>
		void CompileWeaponAttribute();
	}

	public abstract class MonoWeapon : MonoBehaviour, IWeapon
	{
		public abstract MultiSelectablePartType ContainedPart { get; }
		public abstract WeaponAttributes AttributeValues { get; }
		public abstract bool IsCompleted { get; }
		public abstract IPart RootPart { get; }

		public abstract void CompileWeaponAttribute();
	}

	/// <summary>
	/// 代表一把完整的武器
	/// </summary>
	public class BasicWeapon : MonoWeapon
	{
		[SerializeField]
		protected MultiSelectablePartType containedPart;
		public override MultiSelectablePartType ContainedPart => containedPart;
		
		[SerializeField]
		protected WeaponAttributes attributeValues;
		public override WeaponAttributes AttributeValues => attributeValues;
		
		public override bool IsCompleted
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
		protected MonoPart rootPart;
		public override IPart RootPart => rootPart;

		/// <summary>
		/// 计算该武器的各项数值
		/// </summary>
		public override void CompileWeaponAttribute()
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

			foreach (IPort p in ports)
			{
				ContainedPart[p.Part.Type] = true;
				GetPartAttributes(attr, p.Part);
				attr.Add(p.Part.Attributes);
			}
		}
	}
}
