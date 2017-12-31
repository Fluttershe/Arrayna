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
		public BasicPart Part { get; private set; }
		public BasicPort RootPort { get; private set; }

		private void Awake()
		{
			Part = GetComponent<BasicPart>();
			RootPort = (BasicPort)Part?.RootPort;
			if (Part == null || RootPort == null)
			{
				Debug.LogWarning($"This GameObject {this.name} isn't a weapon part, this agent will be disabled.");
				this.enabled = false;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (Part.RootPort.AttachedPort != null)
				Part.RootPort.AttachedPort.DetachPart();

			Workspace.ReadyToConnect = Workspace.CheckForNearPort((BasicPort)Part.RootPort);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			throw new NotImplementedException();
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
			if (Workspace.HeldPart != this.Part) return;
		}
	}

	/// <summary>
	/// 代表武器部件组装成武器的地方，管理武器的组装过程
	/// </summary>
	public class Workspace : MonoBehaviour
	{
		static Workspace _instance;
		/// <summary>
		/// Workspace的唯一实例
		/// </summary>
		public static Workspace Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<Workspace>();
					if (_instance == null) {
						_instance = new GameObject("Workspace").AddComponent<Workspace>();
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// 当前部件的数量
		/// </summary>
		public static int PartCount => Instance.partCount;
		[SerializeField]
		int partCount;

		/// <summary>
		/// 当前正在操作的部件
		/// </summary>
		public static BasicPart HeldPart => Instance.heldPart;
		[SerializeField]
		BasicPart heldPart;

		/// <summary>
		/// 当前准备链接上的接口
		/// </summary>
		public static BasicPort ReadyToConnect
		{
			get => Instance.readyToConnect;
			set => Instance.readyToConnect = value;
		}
		[SerializeField]
		BasicPort readyToConnect;

		/// <summary>
		/// 多长距离内部件可以连接
		/// </summary>
		public static float LinkDistance => Instance.linkDistance;
		[SerializeField]
		float linkDistance = 1;

		List<PartAgent> partsInWorkspace = new List<PartAgent>();
		List<BasicPort> portsInWorkspace = new List<BasicPort>();

		public static void EnterWorkspace() { }
		public static void EnterWorkspace(IWeapon weapon) { }
		public static void ExitWorkspace() { }

		/// <summary>
		/// 给部件绘制高光
		/// </summary>
		/// <param name="part"></param>
		public static void DrawHighLight(BasicPart part) { }

		/// <summary>
		/// 给部件取消高光
		/// </summary>
		/// <param name="part"></param>
		public static void CancelHighLight(BasicPart part) { }

		/// <summary>
		/// 给两个接口间绘制连接线
		/// </summary>
		/// <param name="port1"></param>
		/// <param name="port2"></param>
		public static void DrawLine(BasicPort port1, BasicPort port2) { }

		/// <summary>
		/// 给两个接口间取消连接线
		/// </summary>
		/// <param name="port1"></param>
		/// <param name="port2"></param>
		public static void CancelLine(BasicPort port1, BasicPort port2) { }

		/// <summary>
		/// 查找附近是否有合适的接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public static BasicPort CheckForNearPort(BasicPort port)
		{
			return Instance._checkForNearPort(port);
		}

		BasicPort _checkForNearPort(BasicPort port)
		{
			var closest = -1;
			var min = Single.PositiveInfinity;
			for (int i = 0; i < portsInWorkspace.Count; i++)
			{
				if (!portsInWorkspace[i].CanAttachBy(port.Part)) continue;
				var dist = (port.transform.position - portsInWorkspace[i].transform.position).sqrMagnitude;
				if (dist < min)
				{
					min = dist;
					closest = i;
				}
			}

			if (min < LinkDistance)
				return portsInWorkspace[closest];
			return null;
		}

		/// <summary>
		/// 向Workspace内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPart(IPart part)
		{
			return Instance._addPart(part);
		}

		bool _addPart(IPart part)
		{
			if (part == null) throw new ArgumentNullException();
			BasicPart mPart = part as BasicPart;
			if (mPart == null)
			{
				Debug.LogWarning($"This part {part.PartName} isn't inherited from Monobehaviour.");
				return false;
			}

			var agent = mPart.gameObject.AddComponent<PartAgent>();
			partsInWorkspace.Add(agent);
			partCount++;

			return true;
		}

		/// <summary>
		/// 从Workspace移除一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool RemovePart(IPart part)
		{
			if (part == null) throw new ArgumentNullException();
			BasicPart mPart = part as BasicPart;
			PartAgent agent = mPart?.GetComponent<PartAgent>();
			return RemovePart(agent);
		}

		/// <summary>
		/// 从Workspace移除一个部件
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		public static bool RemovePart(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			return Instance._removePart(agent);
		}

		bool _removePart(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			if (!partsInWorkspace.Contains(agent))
			{
				Debug.LogWarning("该部件不存在于该Workspace内");
				return false;
			}

			partsInWorkspace.Remove(agent);
			Destroy(agent);
			return true;
		}
	}
}
