using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopScroll : MonoBehaviour
{
	[SerializeField]
	float space;

	[SerializeField]
	float panelWidth;

	[SerializeField, Range(0f,1f)]
	float damp;

	[SerializeField]
	float scrollSensitiveness;

	[SerializeField]
	GameObject panelPrefab;

	[SerializeField]
	List<Transform> panelList = new List<Transform>();

	List<Transform> dummyList = new List<Transform>();

	int CurrentPanelIndex = 0;
	
	float lastSpeed;
	float LeftBoundary;
	float RightBoundary;
	int minNumberofPanels;

	private void Start()
	{
		LeftBoundary = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
		RightBoundary = Camera.main.ViewportToWorldPoint(Vector3.one).x;
		minNumberofPanels = (int)((LeftBoundary - RightBoundary) / panelWidth) + 1;
	}

	private void Update()
	{
		var scrollDelta = (Input.mouseScrollDelta.x + Input.mouseScrollDelta.y) * scrollSensitiveness;
		var currentSpeed = scrollDelta * (1 - damp) + lastSpeed * damp;
		currentSpeed = Mathf.Abs(currentSpeed) < 0.01f ? 0 : currentSpeed;
		lastSpeed = currentSpeed;
		print(lastSpeed);
		MoveList(currentSpeed);
	}

	void MoveList(float speed)
	{
		var index = (CurrentPanelIndex - 2 + panelList.Count) % panelList.Count;
		for (int i = 0; i < 5; i ++)
		{
			panelList[index].localPosition += Vector3.right * speed;
			//panelList[index].localScale = Vector3.one * Mathf.Cos(panelList[index].localPosition.x / 400f);
			index++;
			index %= panelList.Count;
		}


	}

	void Clamp()
	{

	}

	void CreateDummy()
	{
		var dummy = Instantiate(panelPrefab);
		dummy.transform.position = transform.position;
		dummyList.Add(dummy.transform);
	}
}
