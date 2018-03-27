using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

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
		/// Backing field of <see cref="IsPointerInPartlist"/>
		/// </summary>
		private bool pointerInPartlist;

		/// <summary>
		/// Backing field of <see cref="PartCount"/>
		/// </summary>
		[SerializeField]
		private int partCount;

		[SerializeField]
		private float linkDistance = 1;

		[SerializeField]
		private Partlist partlist;

		[Header("References")]
		[SerializeField]
		private CanvasGroup warningWindow;
		[SerializeField]
		private MonoWeapon operatingWeapon;

		[SerializeField]
		private PartAgent heldPart;

		[SerializeField]
		private MonoPort readyToConnect;
		
		[SerializeField]
		private WeaponStatePanel statePanel;

		[SerializeField]
		private LinkFlash linkFlashPrefab;
		private LinkFlash linkFlashA;
		private LinkFlash linkFlashB;

		[SerializeField]
		private List<MonoPart> partsInWorkspace = new List<MonoPart>();

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
				}

				if (_instance == null)
				{
					_instance = new GameObject("Workspace").AddComponent<Workspace>();
				}

				return _instance;
			}
		}

		/// <summary>
		/// Workspace是否在使用中
		/// </summary>
		public static bool Active { get; protected set; } = false;

		/// <summary>
		/// 鼠标指针是否在 Partlist 内
		/// </summary>
		public static bool IsPointerInPartlist => Instance.pointerInPartlist;

		/// <summary>
		/// 当前Workspace内部件的数量
		/// </summary>
		public static int PartCount => Instance.partsInWorkspace.Count;

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
		public static bool ExitWorkspace()
		{
			return Instance._ExitWorkspace();
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
			if (masterPort.AttachedPort != null)
			{
				var prevPart = (MonoPart)masterPort.Detach();
				prevPart.transform.SetParent(null);
				Bump(prevPart);
			}
			if (!masterPort.Attach(slavePart)) return false;

			var slavePort = (MonoPort)slavePart.RootPort;

			slavePort.transform.parent = null;
			slavePart.transform.parent = slavePort.transform;

			slavePort.transform.rotation = masterPort.transform.rotation;
			slavePort.transform.position = masterPort.transform.position;

			slavePart.transform.parent = ((MonoPart)masterPort.Part).transform;
			slavePort.transform.parent = slavePart.transform;
			Debug.Log($"Set {slavePart.name}'s parent to {slavePart.transform.parent.name}");
			return true;
		}

		/// <summary>
		/// 将一个部件挪开 (TBD)
		/// </summary>
		/// <param name="agent"></param>
		public static void Bump(PartAgent agent)
		{
			agent.transform.position += Vector3.up;
		}

		/// <summary>
		/// 将一个部件挪开 (TBD)
		/// </summary>
		/// <param name="agent"></param>
		public static void Bump(MonoPart part)
		{
			part.transform.position += Vector3.up;
		}

		/// <summary>
		/// 向Workspace内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToWorkspace(MonoPart part)
		{
			if (part == null) return false;

			var agent = part.GetOrAddComponent<PartAgent>();

			return Instance._addPartToWorkspace(part);
		}

		/// <summary>
		/// 向Workspace内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToWorkspace(PartAgent agent)
		{
			if (agent == null) return false;
			
			var part = agent.GetComponent<MonoPart>();

			return Instance._addPartToWorkspace(part);
		}

		/// <summary>
		/// 从Workspace移除一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool RemovePartFromWorkspace(MonoPart part)
		{
			return Instance._removePartFromWorkspace(part);
		}

		/// <summary>
		/// 从Workspace移除一个部件
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		public static bool RemovePartFromWorkspace(PartAgent agent)
		{
			if (agent == null) return false;

			var part = agent.GetComponent<MonoPart>();

			return Instance._removePartFromWorkspace(part);
		}

		/// <summary>
		/// 向Partlist内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToPartlist(MonoPart part)
		{
			if (part == null) return false;

			var agent = part.GetOrAddComponent<PartAgent>();

			return Instance._addPartToPartlist(part);
		}

		/// <summary>
		/// 向Partlist内添加一个部件
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AddPartToPartlist(PartAgent agent)
		{
			if (agent == null) return false;

			var part = agent.GetComponent<MonoPart>();

			return Instance._addPartToPartlist(part);
		}

		/// <summary>
		/// 从Partlist移除一个部件
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		public static bool RemovePartFromPartlist(MonoPart part)
		{
			return Instance._removePartFromPartlist(part);
		}

		/// <summary>
		/// 从Partlist移除一个部件
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		public static bool RemovePartFromPartlist(PartAgent agent)
		{
			if (agent == null) return false;

			var part = agent.GetComponent<MonoPart>();

			return Instance._removePartFromPartlist(part);
		}

		public static void UpdateWeaponStates()
		{
			OperatingWeapon.CompileWeaponAttribute();
			Instance.statePanel?.UpdateState(OperatingWeapon);
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

			if (linkFlashPrefab == null)
				linkFlashPrefab = ((GameObject)Resources.Load("ConnectingFlash")).GetComponent<LinkFlash>();

			if (linkFlashA == null)
				linkFlashA = Instantiate(linkFlashPrefab.gameObject, this.transform).GetComponent<LinkFlash>();

			if (linkFlashB == null)
				linkFlashB = Instantiate(linkFlashPrefab.gameObject, this.transform).GetComponent<LinkFlash>();

			CancelLink();
		}

		private void OnApplicationQuit()
		{
			ExitWorkspace();
		}

		#endregion

		#region Private methods

		private void EnterPartlist(bool enter)
		{
			pointerInPartlist = enter;
		}

		private void _EnterWorkSpace(IWeapon weapon)
		{
			if (Active) return;
			// Enable instance..
			Active = true;

			// Begin setting up partlist...
			if (partlist == null
			&& (partlist = FindObjectOfType<Partlist>()) == null)
			{
				Debug.LogError("Cannot find a Partlist! Workspace will not function!");
				return;
			}

			partlist.OnEnterOrExit += EnterPartlist;
			partlist.ShowDragnDrop(false);
			// Finish setting up partlist...

			// Begin setting up operatingWeapon...
			// If there's a weapon, add it in, otherwise, create one.
			operatingWeapon = (MonoWeapon)weapon;
			if (operatingWeapon == null)
			{
				operatingWeapon = new GameObject("Weapon").AddComponent<BasicWeapon>();
			}
			
			operatingWeapon.transform.position = transform.position;
			operatingWeapon.transform.SetParent(this.transform);
			foreach (MonoPart p in operatingWeapon.Parts)
				AddPartToWorkspace(p);

			var storage = PlayerWeaponStorage.Instance;
			if (!storage.weapons.Contains(operatingWeapon))
				storage.weapons.Add(operatingWeapon);
			// Finish setting up operatingWeapon...

			// Setup state panel
			statePanel = FindObjectOfType<WeaponStatePanel>();
			
			// Add parts from storage to Workspace
			foreach (MonoPart p in storage.spareParts)
				AddPartToPartlist(p);

			// If there is no receiver in Workspace
			if (operatingWeapon.RootPart == null)
			{
				// Find a receiver from partlist.
				MonoPart receiver = null;
				foreach (MonoPart p in partlist.Parts)
				{
					if (p.Type == PartType.Receiver)
					{
						receiver = p;
						break;
					}
				}

				if (receiver == null)
				{
					Debug.LogWarning("You have no receiver left!");
				}
				else // If we managed to find one, add it to Workspace
				{
					RemovePartFromPartlist(receiver);
					AddPartToWorkspace(receiver);
				}
			}

			UpdateWeaponStates();
		}

		private bool _ExitWorkspace()
		{
			if (!Active) return !Active;

			// If the weapon isn't complete, send a warning and return
			if (!operatingWeapon.IsCompleted)
			{
				Debug.LogWarning("This weapon isn't complete!");
				StopAllCoroutines();
				StartCoroutine(FlashWarningWindow());
				return !Active;
			}

			// Disable this instance
			Active = false;

			partlist.OnEnterOrExit -= EnterPartlist;

			// Despite the weapon could be incomplete, we store it anyway.
			PlayerWeaponStorage.ReturnWeapon(OperatingWeapon);

			// Collect other parts in workspace
			CollectAllUnconnectedParts();

			// Destory every agent we created when adding parts
			foreach (MonoPart p in partsInWorkspace)
			{
				FileDebug.WriteLine($"Destroying agent {p.PartName}");
				var agent = p.GetComponent<PartAgent>();
				if (agent != null)
					Destroy(agent);
			}

			var storage = PlayerWeaponStorage.Instance;
			storage.spareParts.Clear();
			while (partlist.Parts.Count > 0)
			{
				var p = partlist.Parts[0];
				FileDebug.WriteLine($"Returning{p.PartName}");
				storage.spareParts.Add(p);
				RemovePartFromPartlist(p);
				var agent = p.GetComponent<PartAgent>();
				if (agent != null)
					Destroy(agent);
				PlayerWeaponStorage.ReturnPart(p);
			}

			PlayerWeaponStorage.SaveToFile();

			// Clear the lists
			partsInWorkspace.Clear();
			portsInWorkspace.Clear();

			return !Active;
		}

		private MonoPort _checkForNearPort(MonoPort port)
		{
			var closest = -1;
			var min = Single.PositiveInfinity;
			for (int i = 0; i < portsInWorkspace.Count; i++)
			{
				if (!portsInWorkspace[i].CanBeAttachedBy(port.Part)     // 如果部件类型不合适
				|| (portsInWorkspace[i].Part == port.Part)          // 如果是自己身上的接口
				//|| (portsInWorkspace[i].AttachedPort != null)      // 如果这个接口已经接有部件
				) continue; // 跳过

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

		private bool _addPartToPartlist(MonoPart part)
		{
			if (partlist.Parts.Contains(part)) return false;

			// Add the part to partlist
			partlist.AddPart(part);

			return true;
		}

		private bool _removePartFromPartlist(MonoPart part)
		{
			if (!partlist.Parts.Contains(part)) return false;

			// Remove the part from partlist
			partlist.TakePart(part);
			return true;
		}

		private bool _addPartToWorkspace(MonoPart part)
		{
			if (partsInWorkspace.Contains(part)) return false;

			partsInWorkspace.Add(part);

			// Add ports from the part.
			foreach (MonoPort mp in part.Ports)
			{
				portsInWorkspace.Add(mp);
			}

			// If the root part of operating weapon is still empty and
			// we're adding a reciever, set the reciever as the root part.
			if (operatingWeapon.RootPart == null && part.Type == PartType.Receiver)
			{
				operatingWeapon.RootPart = part;
				part.transform.SetParent(operatingWeapon.transform);
				part.transform.position = this.transform.position;
				Debug.Log($"Part {part.PartName} is set as the root part of operating weapon.");
			}
			Debug.Log($"Add part {part.name} to Workspace.");
			return true;
		}

		private bool _removePartFromWorkspace(MonoPart part)
		{
			if (!partsInWorkspace.Contains(part)) return false;

			// Remove ports from the part.
			foreach (MonoPort mp in part.Ports)
			{
				portsInWorkspace.Remove(mp);
			}

			if (operatingWeapon?.RootPart?.Equals(part) == true)
			{
				Debug.Log("Removing root part");
				operatingWeapon.RootPart = null;
			}

			// Remove the part from list
			partsInWorkspace.Remove(part);
			return true;
		}

		private IEnumerator FlashWarningWindow()
		{
			warningWindow.alpha = 1;
			yield return new WaitForSeconds(2);

			float flashDuration = 1f;

			while (flashDuration > 0)
			{
				warningWindow.alpha = flashDuration;
				flashDuration -= Time.deltaTime;
				yield return null;
			}

			warningWindow.alpha = 0;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// 收集所有未连接到枪身的部件
		/// </summary>
		public void CollectAllUnconnectedParts()
		{
			for (int i = 0; i < partsInWorkspace.Count; i++)
			{
				var p = partsInWorkspace[i];
				while (p.RootPort.AttachedPort != null)
					p = (MonoPart)p.RootPort.AttachedPort.Part;

				if (!p.Equals(operatingWeapon.RootPart))
				{
					p = partsInWorkspace[i];
					p.RootPort.Detach();
					RemovePartFromWorkspace(p);
					AddPartToPartlist(p);
					i--;
				}
			}
		}

		/// <summary>
		/// 收集所有在场部件
		/// </summary>
		public void CollectAllParts()
		{
			for (int i = 0; partsInWorkspace.Count > 1; i ++)
			{
				var p = partsInWorkspace[i];
				if (p.Type == PartType.Receiver) continue;
				p.RootPort.Detach();
				_removePartFromWorkspace(p);
				_addPartToPartlist(p);
				i--;
			}

			UpdateWeaponStates();
		}
		#endregion
	}
}
