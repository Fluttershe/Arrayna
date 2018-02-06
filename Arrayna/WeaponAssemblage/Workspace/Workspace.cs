using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponAssemblage.Workspace
{
	/// <summary>
	/// 代表武器部件组装成武器的地方，管理武器的组装过程
	/// </summary>
	public class Workspace : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// Backing field of <see cref="Instance"/>
		/// </summary>
		private static Workspace _instance;

		/// <summary>
		/// Backing field of <see cref="PointerInPartlist"/>
		/// </summary>
		private bool pointerInPartlist;

		/// <summary>
		/// For tests in editor only
		/// </summary>
		[SerializeField, Tooltip("For tests in editor only!")]
		private MonoPart[] PartToAdd = new MonoPart[0];

		/// <summary>
		/// Backing field of <see cref="PartCount"/>
		/// </summary>
		[SerializeField]
		private int partCount;

		[SerializeField]
		private Partlist partlist;

		[SerializeField]
		private MonoWeapon operatingWeapon;

		[SerializeField]
		private PartAgent heldPart;

		[SerializeField]
		private MonoPort readyToConnect;

		[SerializeField]
		private float linkDistance = 1;

		[SerializeField]
		private LinkFlash linkFlashPrefab;
		private LinkFlash linkFlashA;
		private LinkFlash linkFlashB;

		[SerializeField]
		private List<PartAgent> partsInWorkspace = new List<PartAgent>();
		
		[SerializeField]
		private List<PartAgent> partsInPartlist = new List<PartAgent>();

		[SerializeField]
		private List<MonoPort> portsInWorkspace = new List<MonoPort>();

		#endregion

		#region Public static properties

		/// <summary>
		/// Workspace的唯一实例
		/// </summary>
		public static Workspace Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<Workspace>();
					if (_instance == null)
					{
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
		/// 鼠标指针是否在 Partlist 内
		/// </summary>
		public static bool PointerInPartlist => Instance.pointerInPartlist;

		/// <summary>
		/// 当前部件的数量
		/// </summary>
		public static int PartCount => Instance.partCount;

		/// <summary>
		/// 当前正在操作的武器
		/// </summary>
		public static MonoWeapon OperatingWeapon
		{
			get => Instance.operatingWeapon;
			set => Instance.operatingWeapon = value;
		}

		/// <summary>
		/// 当前正在操作的部件
		/// </summary>
		public static PartAgent HeldPart
		{
			get => Instance.heldPart;
			set
			{
				Instance.heldPart = value;
				Instance.partlist.ShowDragnDrop(value != null);
			}
		}

		/// <summary>
		/// 当前准备链接上的接口
		/// </summary>
		public static MonoPort ReadyToConnect
		{
			get => Instance.readyToConnect;
			set => Instance.readyToConnect = value;
		}

		/// <summary>
		/// 多长距离内部件可以连接
		/// </summary>
		public static float LinkDistance => Instance.linkDistance;
		#endregion

		#region Public static methods

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

		/// <summary>
		/// 退出Workspace
		/// </summary>
		public static void ExitWorkspace()
		{
			Instance._ExitWorkspace();
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
		public static void DrawLink(MonoPort port1, MonoPort port2)
		{
			Instance.linkFlashA.transform.position = port1.transform.position;
			Instance.linkFlashB.transform.position = port2.transform.position;
			Instance.linkFlashA.transform.SetParent(port1.transform);
			Instance.linkFlashB.transform.SetParent(port2.transform);
			Instance.linkFlashA.gameObject.SetActive(true);
			Instance.linkFlashB.gameObject.SetActive(true);
		}

		/// <summary>
		/// 取消连接线
		/// </summary>
		/// <param name="port1"></param>
		/// <param name="port2"></param>
		public static void CancelLink()
		{
			Instance.linkFlashA.transform.SetParent(Instance.transform);
			Instance.linkFlashB.transform.SetParent(Instance.transform);
			Instance.linkFlashA.gameObject.SetActive(false);
			Instance.linkFlashB.gameObject.SetActive(false);
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

		/// <summary>
		/// 将一个部件连接到一个接口上
		/// </summary>
		/// <param name="masterPort"></param>
		/// <param name="slavePart"></param>
		/// <returns></returns>
		public static bool InstallPart(MonoPort masterPort, MonoPart slavePart)
		{
			if (!masterPort.Attach(slavePart)) return false;

			var slavePort = (MonoPort)slavePart.RootPort;

			slavePort.transform.parent = null;
			HeldPart.transform.parent = slavePort.transform;

			slavePort.transform.rotation = masterPort.transform.rotation;
			slavePort.transform.position = masterPort.transform.position;

			slavePart.transform.parent = ((MonoPart)masterPort.Part).transform;
			slavePort.transform.parent = slavePart.transform;
			Debug.Log($"Set {slavePart.name}'s parent to {slavePart.transform.parent.name}");
			return true;
		}

		/// <summary>
		/// 将一个部件挪开
		/// </summary>
		/// <param name="agent"></param>
		public static void Bump(PartAgent agent)
		{
			agent.transform.position += Vector3.left;
		}

		/// <summary>
		/// 向Workspace内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToWorkspace(MonoPart part)
		{
			if (part == null) throw new ArgumentNullException();
			var agent = part.GetComponent<PartAgent>();
			if (agent == null)
				agent = part.gameObject.AddComponent<PartAgent>();
			return Instance._addPartToWorkspace(agent);
		}

		/// <summary>
		/// 向Workspace内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToWorkspace(PartAgent agent)
		{
			return Instance._addPartToWorkspace(agent);
		}

		/// <summary>
		/// 从Workspace移除一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool RemovePartFromWorkspace(IPart part)
		{
			if (part == null) throw new ArgumentNullException();
			MonoPart mPart = part as MonoPart;
			PartAgent agent = mPart?.GetComponent<PartAgent>();
			return Instance._removePartFromWorkspace(agent);
		}

		/// <summary>
		/// 从Workspace移除一个部件
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		public static bool RemovePartFromWorkspace(PartAgent agent)
		{
			return Instance._removePartFromWorkspace(agent);
		}

		/// <summary>
		/// 向Partlist内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToPartlist(MonoPart part)
		{
			if (part == null) throw new ArgumentNullException();
			var agent = part.GetComponent<PartAgent>();
			if (agent == null)
				agent = part.gameObject.AddComponent<PartAgent>();

			return Instance._addPartToPartlist(agent);
		}

		/// <summary>
		/// 向Partlist内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToPartlist(PartAgent agent)
		{
			return Instance._addPartToPartlist(agent);
		}

		/// <summary>
		/// 从Partlist移除一个部件
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		public static bool RemovePartFromPartlist(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			return Instance._removePartFromPartlist(agent);
		}

		#endregion

		#region Unity methods

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

			if (linkFlashA == null)
				linkFlashA = Instantiate(linkFlashPrefab.gameObject, this.transform).GetComponent<LinkFlash>();

			if (linkFlashB == null)
				linkFlashB = Instantiate(linkFlashPrefab.gameObject, this.transform).GetComponent<LinkFlash>();

			linkFlashA.gameObject.SetActive(false);
			linkFlashB.gameObject.SetActive(false);
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

		#endregion

		#region Private methods

		private void EnterPartlist(bool enter)
		{
			pointerInPartlist = enter;
		}

		private void _EnterWorkSpace(IWeapon weapon)
		{
			if (partlist == null
			&& (partlist = FindObjectOfType<Partlist>()) == null)
			{
				Debug.LogError("Cannot find a Partlist! Workspace will not function!");
				return;
			}

			partlist.OnEnterOrExit += EnterPartlist;
			partlist.ShowDragnDrop(false);

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
				AddPartToPartlist(p);

			operatingWeapon.CompileWeaponAttribute();
		}

		private void _ExitWorkspace()
		{
			Instance.enabled = false;

			partlist.OnEnterOrExit -= EnterPartlist;

			// If the weapon isn't complete, send a warning.
			if (!operatingWeapon.IsCompleted)
			{
				Debug.LogWarning("This weapon isn't complete!");
			}

			// Destory every agent we created when adding parts
			var count = partsInWorkspace.Count;
			for (int i = 0; i < count; i++)
			{
				Destroy(partsInWorkspace[i]);
			}

			foreach (PartAgent p in partsInPartlist)
			{
				Destroy(p);
			}

			// Clear the lists
			partsInPartlist.Clear();
			partsInWorkspace.Clear();
			portsInWorkspace.Clear();
		}

		private MonoPort _checkForNearPort(MonoPort port)
		{
			var closest = -1;
			var min = Single.PositiveInfinity;
			for (int i = 0; i < portsInWorkspace.Count; i++)
			{
				if (!portsInWorkspace[i].CanAttachBy(port.Part)     // 如果部件类型不合适
				|| (portsInWorkspace[i].Part == port.Part)          // 如果是自己身上的接口
				|| (portsInWorkspace[i].AttachedPort != null))      // 如果这个接口已经接有部件
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

		private bool _addPartToPartlist(PartAgent part)
		{
			if (part == null) throw new ArgumentNullException();

			var agent = part.GetComponent<PartAgent>();
			if (agent == null)
				agent = part.gameObject.AddComponent<PartAgent>();
			if (partsInPartlist.Contains(agent)) return false;
			partsInPartlist.Add(agent);
			partlist.AddPart(agent);

			return true;
		}

		private bool _removePartFromPartlist(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			if (!partsInPartlist.Contains(agent)) return false;

			// Remove the part from list
			partsInPartlist.Remove(agent);
			partlist.TakePart(agent);
			return true;
		}

		private bool _addPartToWorkspace(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();

			if (partsInWorkspace.Contains((PartAgent)agent)) return false;
			partsInWorkspace.Add((PartAgent)agent);
			partCount++;

			// Add ports from the part.
			foreach (MonoPort mp in agent.Part.Ports)
			{
				portsInWorkspace.Add(mp);
			}

			// If the root part of operating weapon is still empty and
			// we're adding a reciever, set the reciever as the root part.
			if (operatingWeapon.RootPart == null && agent.Part.Type == PartType.Reciever)
			{
				operatingWeapon.RootPart = agent.Part;
				agent.transform.SetParent(operatingWeapon.transform);
				Debug.Log($"Part {agent.Part.PartName} is set as the root part of operating weapon.");
			}

			return true;
		}

		private bool _removePartFromWorkspace(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			if (!partsInWorkspace.Contains(agent)) return false;

			// Remove ports from the part.
			foreach (MonoPort mp in agent.Part.Ports)
			{
				portsInWorkspace.Remove(mp);
			}

			if (operatingWeapon?.RootPart?.Equals(agent.Part) == true)
			{
				operatingWeapon.RootPart = null;
			}

			// Remove the part from list
			partsInWorkspace.Remove(agent);
			return true;
		}

		#endregion
	}
}
