using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WeaponAssemblage.Workspace
{
	public class PartPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IInitializePotentialDragHandler, ICancelHandler, ISelectHandler, IDeselectHandler, IUpdateSelectedHandler, IMoveHandler, IScrollHandler
	{
		public void OnDrag(PointerEventData eventData)
		{
			print(name + "Drag");
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			print(name + "Pointer down");
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			print(name + "Pointer enter");
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			print(name + "Pointer exit");
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			print(name + "Pointer up");
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			print(name + "Pointer click");
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			print(name + "Begin drag");
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			print(name + "End drag");
		}

		public void OnDrop(PointerEventData eventData)
		{
			print(name + "Drop drag");
		}

		public void OnCancel(BaseEventData eventData)
		{
			print(name + "Cancel");
		}

		public void OnSelect(BaseEventData eventData)
		{
			print(name + "Select");
		}

		public void OnDeselect(BaseEventData eventData)
		{
			print(name + "Deselect");
		}

		public void OnUpdateSelected(BaseEventData eventData)
		{
			print(name + "Update Select");
		}

		public void OnMove(AxisEventData eventData)
		{
			print(name + "Move");
		}

		public void OnScroll(PointerEventData eventData)
		{
			print(name + "Scroll");
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			print(name + "Initialize Potential Drag");
		}
	}
}
