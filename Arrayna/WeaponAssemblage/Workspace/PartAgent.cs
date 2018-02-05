using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeaponAssemblage.Workspace
{
	/// <summary>
	/// 用于附加到位于Workspace内的武器部件上，处理各种Workspace相关的part操作
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class PartAgent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		/// Agent 的工作模式
		/// </summary>
		abstract class WorkMode : IAgentWorkMode
		{
			public WorkMode(PartAgent _agent)
			{
				this.agent = _agent;
			}

			protected PartAgent agent;

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
						part.transform.parent = null;
						Debug.Log($"Set {part.name}'s parent to {part.transform.parent}");
						Debug.Log("解除成功");
						Workspace.OperatingWeapon.CompileWeaponAttribute();
					}
				}

				base.OnDrag(eventData);

				if (Workspace.PointerInPartlist)
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

				Workspace.RemovePartFromPartlist(agent);
				Workspace.AddPartToWorkspace(agent.Part);
			}
		}

		/// <summary>
		/// 在 Partlist 内的工作模式
		/// </summary>
		class PartListMode : WorkMode
		{
			public PartListMode(PartAgent _agent) : base(_agent) { }

			public override void OnPointerUp(PointerEventData eventData)
			{
				if (Workspace.HeldPart != agent) return;
				Workspace.HeldPart = null;
				Workspace.CancelLink();

				Workspace.RemovePartFromWorkspace(agent);
				Workspace.AddPartToPartlist(agent.Part);
			}

			public override void OnDrag(PointerEventData eventData)
			{
				base.OnDrag(eventData);
				
				if (!Workspace.PointerInPartlist)
				{
					print("Switch to workspace mode.");
					agent.currentMode = agent.workspaceMode;
				}
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

			workspaceMode = new WorkspaceMode(this);
			partlistMode = new PartListMode(this);
			currentMode = partlistMode;
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
	}
}
