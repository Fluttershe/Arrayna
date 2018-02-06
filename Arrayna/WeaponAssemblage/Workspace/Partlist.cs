using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityUtility;

namespace WeaponAssemblage.Workspace
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class Partlist : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		Transform content;

		[SerializeField]
		GameObject partPanel;

		[SerializeField]
		BoxCollider2D dragndropArea;
		
		[SerializeField]
		List<PartAgent> partAgentList = new List<PartAgent>();

		[SerializeField]
		List<GameObject> partPanelList = new List<GameObject>();

		Dictionary<PartAgent, GameObject> agentToPanel = new Dictionary<PartAgent, GameObject>();

		/// <summary>
		/// true for enter, false for exit
		/// </summary>
		public event Action<bool> OnEnterOrExit;

		private void Awake()
		{
			if (dragndropArea == null)
			dragndropArea = GetComponent<BoxCollider2D>();
		}

		private void Update()
		{
			var rectTrans = transform as RectTransform;
			dragndropArea.size = rectTrans.sizeDelta;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnEnterOrExit?.Invoke(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnEnterOrExit?.Invoke(false);
		}

		public void ShowDragnDrop(bool show)
		{
			dragndropArea.enabled = show;
		}

		public void AddPart(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			if (agentToPanel.ContainsKey(agent))
			{
				Debug.Log($"Already have this part:{agent.name}");
				return;
			}

			print($"Adding part {agent.Part.PartName}.");
			var go = Instantiate(partPanel, content);
			agent.transform.SetParent(go.transform);
			
			var pos = agent.transform.localPosition;
			pos.x = pos.y = 0;
			agent.transform.localPosition = pos;

			agentToPanel.Add(agent, go);
			partAgentList.Add(agent);
			partPanelList.Add(go);
		}

		public void TakePart(PartAgent agent)
		{
			if (agent == null) throw new ArgumentNullException();
			if (!agentToPanel.ContainsKey(agent))
			{
				Debug.Log($"Didn't have this part:{agent.name}");
				return;
			}

			print($"Taking part {agent.Part.PartName}.");
			var go = agentToPanel[agent];
			agent.transform.SetParent(null);
			agentToPanel.Remove(agent);
			partAgentList.Remove(agent);
			partPanelList.Remove(go);
			Destroy(go);
		}
	}
}
