using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysUnitSize : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		var children = GetComponentsInChildren<MonoBehaviour>();
		foreach (MonoBehaviour mb in children)
		{
			if (mb.transform.parent != transform) continue;
			SetGlobalScale(mb.transform, Vector3.one);
			mb.transform.localPosition = Vector3.zero;
			var tmp = mb.transform.position;
			tmp.z = 0;
			mb.transform.position = tmp;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void SetGlobalScale(Transform t, Vector3 scale)
	{
		var lossy = t.transform.lossyScale;
		var local = t.transform.localScale;
		local.x *= scale.x / lossy.x;
		local.y *= scale.y / lossy.y;
		local.z *= scale.z / lossy.z;
		t.transform.localScale = local;
	}
}
