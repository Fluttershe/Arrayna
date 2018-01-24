using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WeaponAssemblage
{
	public class PartPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		[SerializeField]
		List<MonoPart> AvailableParts;
		
		[SerializeField]
		MonoPart part;

		public void SetPart(MonoPart part)
		{
			this.part = part;
		}

		public void OnDrag(PointerEventData eventData)
		{
			print("Drag");
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Workspace.AddPart(part);
			part = null;
			print("Pointer down");
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			print("Pointer enter");
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			print("Pointer exit");
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			print("Pointer up");
		}
	}
}
