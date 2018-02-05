using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponAssemblage.Workspace
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class LinkFlash : MonoBehaviour
	{
		[SerializeField]
		float rate = 2.5f;

		bool up;
		Color color;
		SpriteRenderer sr;

		private void Awake()
		{
			sr = GetComponent<SpriteRenderer>();
			color = sr.color;
		}

		private void Update()
		{
			if (up)
				color.a += Time.deltaTime * rate;
			else if (!up)
				color.a -= Time.deltaTime * rate;

			if (color.a >= 1 || color.a <= 0) up = !up;
			sr.color = color;
		}

		private void OnEnable()
		{
			color.a = 0;
			up = true;
		}
	}
}
