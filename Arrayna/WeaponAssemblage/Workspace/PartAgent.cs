using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeaponAssemblage
{
	/// <summary>
	/// 用于附加到位于Workspace内的武器部件上，处理各种Workspace相关的part操作
	/// </summary>
	public class PartAgent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public MonoPart Part { get; private set; }
		public MonoPort RootPort { get; private set; }

		private Vector3 lastPointerPos;

		private void Awake()
		{
			Part = GetComponent<MonoPart>();
			RootPort = (MonoPort)Part?.RootPort;
			if (Part == null || RootPort == null)
			{
				Debug.LogWarning($"This GameObject {this.name} isn't a complete weapon part, this agent will be disabled.");
				this.enabled = false;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (Workspace.HeldPart != this) Workspace.HeldPart = this;
			if (RootPort.AttachedPort != null)
			{
				Debug.Log("Try to detach part...");
				MonoPart part = (MonoPart)RootPort.Detach();
				if (part == null)
					Debug.Log($"解除部件失败？{RootPort.AttachedPort?.Part?.PartName}");
				else
				{
					part.transform.parent = null;
					Debug.Log("解除成功");
					Workspace.OperatingWeapon.CompileWeaponAttribute();
				}
			}

			MovePart(eventData);

			Workspace.ReadyToConnect = Workspace.CheckForNearPort((MonoPort)Part.RootPort);
			if (Workspace.ReadyToConnect != null)
				Workspace.DrawLine(RootPort, Workspace.ReadyToConnect);
			else
				Workspace.CancelLine();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (Workspace.HeldPart != null)
			{
				Debug.Log($"Already holding {Workspace.HeldPart.name}!");
			}
			Workspace.HeldPart = this;
			lastPointerPos = Camera.main.ScreenToWorldPoint(eventData.position);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			Workspace.DrawHighLight(Part);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Workspace.CancelHighLight(Part);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (Workspace.HeldPart != this) return;

			Workspace.CancelLine();
			if (Workspace.ReadyToConnect != null)
			{
				if (!Workspace.CombineParts(Workspace.ReadyToConnect, Part))
				{
					Workspace.Bump(this);
				}
				else
				{
					Workspace.ReadyToConnect = null;
					Debug.Log("安装成功");
					Workspace.OperatingWeapon.CompileWeaponAttribute();
				}
			}

			Workspace.HeldPart = null;
		}

		void MovePart(PointerEventData eventData)
		{
			var currentPointerPos = Camera.main.ScreenToWorldPoint(eventData.position);

			this.transform.position += currentPointerPos - lastPointerPos;
			lastPointerPos = currentPointerPos;
		}
	}

}
