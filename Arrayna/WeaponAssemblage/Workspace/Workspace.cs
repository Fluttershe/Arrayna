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
				Debug.LogWarning($"This GameObject {this.name} isn't a weapon part, this agent will be disabled.");
				this.enabled = false;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (Workspace.HeldPart != this) Workspace.HeldPart = this;
			if (Part.RootPort.AttachedPort != null)
				Part.RootPort.AttachedPort.DetachPart();

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

	/// <summary>
	/// 代表武器部件组装成武器的地方，管理武器的组装过程
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
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

		[SerializeField, Tooltip("For tests in editor only!")]
		MonoPart[] PartToAdd;

		/// <summary>
		/// 当前部件的数量
		/// </summary>
		public static int PartCount => Instance.partCount;
		[SerializeField]
		int partCount;

		/// <summary>
		/// 当前正在操作的部件
		/// </summary>
		public static PartAgent HeldPart
		{
			get => Instance.heldPart;
			set => Instance.heldPart = value;
		}
		[SerializeField]
		PartAgent heldPart;

		/// <summary>
		/// 当前准备链接上的接口
		/// </summary>
		public static MonoPort ReadyToConnect
		{
			get => Instance.readyToConnect;
			set => Instance.readyToConnect = value;
		}
		[SerializeField]
		MonoPort readyToConnect;

		/// <summary>
		/// 多长距离内部件可以连接
		/// </summary>
		public static float LinkDistance => Instance.linkDistance;
		[SerializeField]
		float linkDistance = 1;

		[SerializeField]
		List<PartAgent> partsInWorkspace = new List<PartAgent>();

		[SerializeField]
		List<MonoPort> portsInWorkspace = new List<MonoPort>();

		public LineRenderer linkingLine;

		private void Awake()
		{
			linkingLine = this.gameObject.GetComponent<LineRenderer>();
		}

		private void Start()
		{
			foreach (MonoPart p in PartToAdd)
				AddPart(p);
		}

		public static void EnterWorkspace() { }
		public static void EnterWorkspace(IWeapon weapon) { }
		public static void ExitWorkspace() { }

		/// <summary>
		/// 给部件绘制高光
		/// </summary>
		/// <param name="part"></param>
		public static void DrawHighLight(MonoPart part) { print($"{part.name} High light!"); }

		/// <summary>
		/// 给部件取消高光
		/// </summary>
		/// <param name="part"></param>
		public static void CancelHighLight(MonoPart part) { print($"{part.name} no High light!"); }

		/// <summary>
		/// 给两个接口间绘制连接线
		/// </summary>
		/// <param name="port1"></param>
		/// <param name="port2"></param>
		public static void DrawLine(MonoPort port1, MonoPort port2)
		{
			Instance.linkingLine.SetPosition(0, port1.transform.position);
			Instance.linkingLine.SetPosition(1, port2.transform.position);
			print($"{port1.name} nad {port2.name} linked!");
		}

		/// <summary>
		/// 给两个接口间取消连接线
		/// </summary>
		/// <param name="port1"></param>
		/// <param name="port2"></param>
		public static void CancelLine() {
			Instance.linkingLine.SetPosition(0, Vector3.zero);
			Instance.linkingLine.SetPosition(1, Vector3.zero);
			//print($"{port1.name} nad {port2.name} dislinked!");
		}

		/// <summary>
		/// 查找附近是否有合适的接口
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public static MonoPort CheckForNearPort(MonoPort port)
		{
			return Instance._checkForNearPort(port);
		}

		MonoPort _checkForNearPort(MonoPort port)
		{
			var closest = -1;
			var min = Single.PositiveInfinity;
			for (int i = 0; i < portsInWorkspace.Count; i++)
			{
				if (!portsInWorkspace[i].CanAttachBy(port.Part)) continue;
				var dist = ((Vector2)port.transform.position - (Vector2)portsInWorkspace[i].transform.position).sqrMagnitude;
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

		public static bool CombineParts(MonoPort masterPort, MonoPart slavePart)
		{
			if (!masterPort.AttachPart(slavePart)) return false;

			var slavePort = (MonoPort)slavePart.RootPort;

			slavePort.transform.parent = null;
			HeldPart.transform.parent = slavePort.transform;

			slavePort.transform.rotation = masterPort.transform.rotation;
			slavePort.transform.position = masterPort.transform.position;

			slavePart.transform.parent = ((MonoPart)masterPort.Part).transform;
			slavePort.transform.parent = slavePart.transform;
			return true;
		}

		public static void Bump(PartAgent agent)
		{
			agent.transform.position += Vector3.left;
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
			MonoPart mPart = part as MonoPart;
			if (mPart == null)
			{
				Debug.LogWarning($"This part {part.PartName} isn't inherited from Monobehaviour.");
				return false;
			}
			
			var agent = mPart.gameObject.AddComponent<PartAgent>();
			partsInWorkspace.Add(agent);
			partCount++;

			foreach (MonoPort mp in part.Ports)
			{
				portsInWorkspace.Add(mp);
			}

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
			MonoPart mPart = part as MonoPart;
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
