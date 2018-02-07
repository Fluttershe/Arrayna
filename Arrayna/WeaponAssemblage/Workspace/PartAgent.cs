using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeaponAssemblage.Workspace
{
	public interface IPointerEventsHandler : IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
	{ }

	/// <summary>
	/// 用于附加到位于Workspace内的武器部件上，处理各种Workspace相关的part操作
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class PartAgent : MonoBehaviour, IPointerEventsHandler
	{
		/// <summary>
		/// Agent 的工作模式
		/// </summary>
		abstract class WorkMode : IPointerEventsHandler
		{
			protected PartAgent agent;

			public WorkMode(PartAgent _agent)
			{
				this.agent = _agent;
			}

			public virtual void OnDrag(PointerEventData eventData)
			{
				MovePart(eventData);
			}

			public virtual void OnPointerEnter(PointerEventData eventData)
			{
				Workspace.DrawHighLight(agent.Part);
			}

			public virtual void OnPointerExit(PointerEventData eventData)
			{
				Workspace.CancelHighLight(agent.Part);
			}

			public virtual void OnPointerDown(PointerEventData eventData)
			{
				if (Workspace.HeldPart != null)
				{
					Debug.Log($"Already holding {Workspace.HeldPart.name}!");
				}
				Workspace.HeldPart = agent;
				agent.lastPointerPos = Camera.main.ScreenToWorldPoint(eventData.position);
			}

			public virtual void OnPointerUp(PointerEventData eventData)
			{
				if(Workspace.HeldPart != agent) return;
				Workspace.HeldPart = null;
				Workspace.CancelLink();
			}


			public virtual void MovePart(PointerEventData eventData)
			{
				var currentPointerPos = Camera.main.ScreenToWorldPoint(eventData.position);

				agent.transform.position += currentPointerPos - agent.lastPointerPos;
				agent.lastPointerPos = currentPointerPos;
			}

			public virtual void OnPointerClick(PointerEventData eventData) { }
		}

		/// <summary>
		/// 在 Workspace 内的工作模式
		/// </summary>
		class WorkspaceMode : WorkMode
		{
			public WorkspaceMode(PartAgent _agent) : base(_agent) { }

			public override void OnDrag(PointerEventData eventData)
			{
				if (Workspace.HeldPart != agent) Workspace.HeldPart = agent;
				if (agent.RootPort.AttachedPort != null)
				{
					MonoPart part = (MonoPart)agent.RootPort.Detach();
					if (part == null)
						Debug.Log($"解除部件失败？{agent.RootPort.AttachedPort?.Part?.PartName}");
					else
					{
						agent.transform.parent = null;
						Debug.Log($"Set {agent.name}'s parent to {agent.transform.parent}");
						Debug.Log("解除成功");
						Workspace.OperatingWeapon.CompileWeaponAttribute();
					}
				}

				base.OnDrag(eventData);

				if (Workspace.IsPointerInPartlist)
				{
					print("Switch to partlist mode.");
					agent.currentMode = agent.partlistMode;
				}

				Workspace.ReadyToConnect = Workspace.CheckForNearPort((MonoPort)agent.Part.RootPort);
				if (Workspace.ReadyToConnect != null)
					Workspace.DrawLink(agent.RootPort, Workspace.ReadyToConnect);
				else
					Workspace.CancelLink();
			}

			public override void OnPointerUp(PointerEventData eventData)
			{
				if (Workspace.HeldPart != agent) return;
				Workspace.CancelLink();
				Workspace.RemovePartFromPartlist(agent);
				Workspace.AddPartToWorkspace(agent);

				if (Workspace.ReadyToConnect != null)
				{
					if (!Workspace.InstallPart(Workspace.ReadyToConnect, agent.Part))
					{
						Workspace.Bump(agent);
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
		}

		/// <summary>
		/// 在 Partlist 内的工作模式
		/// </summary>
		class PartListMode : WorkMode
		{
			public PartListMode(PartAgent _agent) : base(_agent) { }

			public override void OnDrag(PointerEventData eventData)
			{
				base.OnDrag(eventData);
				
				if (!Workspace.IsPointerInPartlist)
				{
					print("Switch to workspace mode.");
					agent.currentMode = agent.workspaceMode;
				}
			}

			public override void OnPointerUp(PointerEventData eventData)
			{
				var parts = GetAttachedPart(agent.Part);
				
				Workspace.RemovePartFromWorkspace(agent);
				Workspace.AddPartToPartlist(agent);

				foreach (MonoPart p in parts)
				{
					p.RootPort.Detach();
					var a = p.GetComponent<PartAgent>();
					Workspace.RemovePartFromWorkspace(a);
					Workspace.AddPartToPartlist(a);
				}
				Workspace.OperatingWeapon.CompileWeaponAttribute();

				ResetPosition();

				if (Workspace.HeldPart != agent) return;
				Workspace.HeldPart = null;
				Workspace.CancelLink();
			}

			private void ResetPosition()
			{
				var pos = agent.transform.localPosition;
				pos.x = pos.y = 0;
				agent.transform.localPosition = pos;
			}

			private IPart[] GetAttachedPart(IPart part)
			{
				List<IPart> parts = new List<IPart>();
				foreach (IPort p in part.Ports)
				{
					if (p.AttachedPort != null)
					{
						parts.Add(p.AttachedPort.Part);
					}
				}

				return parts.ToArray();
			}
		}

		/// <summary>
		/// Receiver 专有模式
		/// </summary>
		class ReceiverMode : WorkMode
		{
			public ReceiverMode(PartAgent _agent) : base(_agent) { }

			public override void OnDrag(PointerEventData eventData) { }
			public override void OnPointerUp(PointerEventData eventData)
			{
				base.OnPointerUp(eventData);
				
				if (!Workspace.IsPointerInPartlist || !IsHoveringOver(eventData.position)) return;

				// If this receiver is inside Partlist and being click..
				// Swap the receiver inside Workspace with this one
				print("Switch receiver.");
				var prevReceiver = Workspace.OperatingWeapon.RootPart;

				// Detach all the parts that attached to the receiver inside Workspace
				List<MonoPart> partsFromPrevReceiver = new List<MonoPart>();
				foreach (MonoPort p in prevReceiver.Ports)
				{
					if (p.AttachedPort == null) continue;
					partsFromPrevReceiver.Add((MonoPart)p.AttachedPort.Part);
					((MonoPart)p.AttachedPort.Part).transform.SetParent(null);
					p.Detach();
				}

				// Move the old receiver to Partlist
				Workspace.RemovePartFromWorkspace((MonoPart)prevReceiver);
				Workspace.AddPartToPartlist((MonoPart)prevReceiver);

				// Move the new one to Workspace
				Workspace.RemovePartFromPartlist(agent.Part);
				Workspace.AddPartToWorkspace(agent.Part);

				// Try to connect the parts from previous receiver to the new one
				foreach (MonoPart p in partsFromPrevReceiver)
				{
					var port = Workspace.CheckForNearPort((MonoPort)p.RootPort);
					if (port != null)
						Workspace.InstallPart(port, p);
				}
			}

			bool IsHoveringOver(Vector3 pointerPos)
			{
				var worldPointerPos = Camera.main.ScreenToWorldPoint(pointerPos);

				var hit = Physics2D.Raycast(worldPointerPos, Vector2.right, 0.001f);

				return (hit.transform?.GetComponent<PartAgent>() == agent) == true;
			}
		}

		public MonoPart Part { get; private set; }
		public MonoPort RootPort { get; private set; }

		private Vector3 lastPointerPos;

		WorkMode currentMode;
		WorkspaceMode workspaceMode;
		PartListMode partlistMode;

		private void Awake()
		{
			Part = GetComponent<MonoPart>();
			RootPort = (MonoPort)Part?.RootPort;
			if (Part == null || RootPort == null)
			{
				Debug.LogWarning($"This GameObject {this.name} isn't a complete weapon part, this agent will be disabled.");
				this.enabled = false;
			}

			if (Part.Type == PartType.Reciever)
			{
				currentMode = new ReceiverMode(this);
			}
			else
			{
				workspaceMode = new WorkspaceMode(this);
				partlistMode = new PartListMode(this);
				currentMode = partlistMode;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			currentMode.OnDrag(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			currentMode.OnPointerDown(eventData);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			currentMode.OnPointerEnter(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			currentMode.OnPointerExit(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			currentMode.OnPointerUp(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			currentMode.OnPointerClick(eventData);
		}
	}
}
