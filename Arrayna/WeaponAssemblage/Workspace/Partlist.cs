using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeaponAssemblage.Workspace
{
	public class Partlist : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		/// true for enter, false for exit
		/// </summary>
		public event Action<bool> OnEnterOrExit;

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnEnterOrExit?.Invoke(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnEnterOrExit?.Invoke(false);
		}
	}
}
