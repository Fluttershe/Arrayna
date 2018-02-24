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
		Transform[] scrollViews;

		[SerializeField]
		Transform[] contents;

		[SerializeField]
		GameObject partPanel;

		[SerializeField]
		BoxCollider2D dragndropArea;
		
		[SerializeField]
		List<MonoPart> partList = new List<MonoPart>();

		[SerializeField]
		List<GameObject> partPanelList = new List<GameObject>();

		Dictionary<MonoPart, GameObject> partToPanel = new Dictionary<MonoPart, GameObject>();

		int currentView = 0;

		/// <summary>
		/// true for enter, false for exit
		/// </summary>
		public event Action<bool> OnEnterOrExit;

		/// <summary>
		/// The parts in this part list.
		/// </summary>
		public List<MonoPart> Parts => partList;

		private void Awake()
		{
			dragndropArea = GetComponent<BoxCollider2D>();
			SwitchScrollView(currentView);

			contents = new Transform[scrollViews.Length];
			for (int i = 0; i < scrollViews.Length; i ++)
			{
				contents[i] = scrollViews[i].Find("Viewport").Find("Content");
			}
		}

		private void Update()
		{
			var rectTrans = transform as RectTransform;
			dragndropArea.size = new Vector2(rectTrans.rect.width, rectTrans.rect.height);
			dragndropArea.offset = Vector2.left * (rectTrans.rect.width / 2);
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

		public void AddPart(MonoPart part)
		{
			if (part == null) throw new ArgumentNullException();
			if (partToPanel.ContainsKey(part))
			{
				Debug.Log($"Already have this part:{part.name}");
				return;
			}

			// BarrelAddon is categorized as Addon
			var index = (int)(part.Type == PartType.BarrelAddon? PartType.Addon: part.Type);
			
			print($"Adding part {part.PartName}.");
			var go = Instantiate(partPanel, contents[index]);
			part.transform.SetParent(go.transform);
			
			var pos = part.transform.localPosition;
			pos.x = pos.y = 0;
			part.transform.localPosition = pos;

			partToPanel.Add(part, go);
			partList.Add(part);
			partPanelList.Add(go);
		}

		public void TakePart(MonoPart part)
		{
			if (part == null) throw new ArgumentNullException();
			if (!partToPanel.ContainsKey(part))
			{
				Debug.Log($"Didn't have this part:{part.name}");
				return;
			}

			print($"Taking part {part.PartName}.");
			var go = partToPanel[part];
			part.transform.SetParent(null);
			partToPanel.Remove(part);
			partList.Remove(part);
			partPanelList.Remove(go);
			Destroy(go);
		}

		public void SwitchScrollView(int index)
		{
			for (int i = 0; i < contents.Length; i ++)
			{
				if (i == index)
					scrollViews[i].gameObject.SetActive(true);
				else if (contents[i].gameObject.activeSelf)
					scrollViews[i].gameObject.SetActive(false);
			}
		}
	}
}
