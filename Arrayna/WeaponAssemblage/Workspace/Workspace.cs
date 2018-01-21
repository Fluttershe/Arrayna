using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponAssemblage
{
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

		/// <summary>
		/// Workspace是否在使用中
		/// </summary>
		public static bool Active
		{
			get { return Instance.enabled; }
		}

		/// <summary>
		/// What the tooltip says
		/// </summary>
		[SerializeField, Tooltip("For tests in editor only!")]
		MonoPart[] PartToAdd = new MonoPart[0];

		/// <summary>
		/// 当前部件的数量
		/// </summary>
		public static int PartCount => Instance.partCount;
		[SerializeField]
		int partCount;
		
		/// <summary>
		/// 当前正在操作的武器
		/// </summary>
		public static MonoWeapon OperatingWeapon
		{
			get => Instance.operatingWeapon;
			set => Instance.operatingWeapon = value;
		}
		[SerializeField]
		MonoWeapon operatingWeapon;

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

		[SerializeField]
		protected LineRenderer linkingLine;

		private void Awake()
		{
			if (_instance != null)
			{
				Debug.LogWarning("Multiple workspace!");
				this.enabled = false;
			}
			else
			{
				_instance = this;
			}
			linkingLine = this.gameObject.GetComponent<LineRenderer>();
		}

		private void Start()
		{
			EnterWorkspace();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				OperatingWeapon.PrimaryFireDown();
			}

			if (Input.GetKeyUp(KeyCode.P))
			{
				OperatingWeapon.PrimaryFireUp();
			}
		}

		/// <summary>
		/// 进入Workspace
		/// </summary>
		public static void EnterWorkspace()
		{
			Instance._EnterWorkSpace(null);
		}

		/// <summary>
		/// 进入Workspace
		/// </summary>
		public static void EnterWorkspace(IWeapon weapon)
		{
			Instance._EnterWorkSpace(weapon);
		}

		void _EnterWorkSpace(IWeapon weapon)
		{
			Instance.enabled = true;

			// If there's a weapon, add it in, otherwise, create one.
			operatingWeapon = (MonoWeapon)weapon;
			if (operatingWeapon == null)
			{
				operatingWeapon = new GameObject("Weapon").AddComponent<BasicWeapon>();
				operatingWeapon.enabled = false;
			}
			
			// Editor only
			foreach (MonoPart p in PartToAdd)
				AddPart(p);
			
			operatingWeapon.CompileWeaponAttribute();
		}

		/// <summary>
		/// 退出Workspace
		/// </summary>
		public static void ExitWorkspace()
		{
			Instance._ExitWorkspace();
		}

		void _ExitWorkspace()
		{
			Instance.enabled = false;

			// If the weapon isn't complete, send a warning.
			if (!operatingWeapon.IsCompleted)
			{
				Debug.LogWarning("This weapon isn't complete!");
			}

			// Destory every agent we created when adding parts
			var count = partsInWorkspace.Count;
			for (int i = 0; i < count; i ++)
			{
				Destroy(partsInWorkspace[i]);
			}

			// Clear the lists
			partsInWorkspace.Clear();
			portsInWorkspace.Clear();
		}

		/// <summary>
		/// 给部件绘制高光
		/// </summary>
		/// <param name="part"></param>
		public static void DrawHighLight(MonoPart part)
		{
			//print($"{part.name} High light!"); 
		}

		/// <summary>
		/// 给部件取消高光
		/// </summary>
		/// <param name="part"></param>
		public static void CancelHighLight(MonoPart part)
		{
			//print($"{part.name} no High light!"); 
		}

		/// <summary>
		/// 给两个接口间绘制连接线
		/// </summary>
		/// <param name="port1"></param>
		/// <param name="port2"></param>
		public static void DrawLine(MonoPort port1, MonoPort port2)
		{
			Instance.linkingLine.SetPosition(0, port1.transform.position);
			Instance.linkingLine.SetPosition(1, port2.transform.position);
		}

		/// <summary>
		/// 取消连接线
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
				if (!portsInWorkspace[i].CanAttachBy(port.Part)		// 如果部件类型不合适
				|| (portsInWorkspace[i].Part == port.Part)			// 如果是自己身上的接口
				|| (portsInWorkspace[i].AttachedPort != null))		// 如果这个接口已经接有部件
					continue; // 跳过

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
			if (!masterPort.Attach(slavePart)) return false;

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
		public static bool AddPart(MonoPart part)
		{
			return Instance._addPart(part);
		}

		bool _addPart(MonoPart part)
		{
			if (part == null) throw new ArgumentNullException();
			
			var agent = part.gameObject.AddComponent<PartAgent>();
			partsInWorkspace.Add(agent);
			partCount++;

			// Add ports from the part.
			foreach (MonoPort mp in part.Ports)
			{
				portsInWorkspace.Add(mp);
			}

			// If the root part of operating weapon is still empty and
			// we're adding a reciever, set the reciever as the root part.
			if (operatingWeapon.RootPart == null && part.Type == PartType.Reciever)
			{
				operatingWeapon.RootPart = part;
				part.transform.parent = operatingWeapon.transform;
				Debug.Log($"Part {part.PartName} is set as the root part of operating weapon.");
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

			// Remove ports from the part.
			foreach (MonoPort mp in agent.Part.Ports)
			{
				portsInWorkspace.Remove(mp);
			}

			// Remove the part from list and destroy 
			// the agent attached to it.
			partsInWorkspace.Remove(agent);
			Destroy(agent);
			return true;
		}
	}
}
