using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTest : MonoBehaviour, IPointerEnterHandler {

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Pointer Entered!");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
