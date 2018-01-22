using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopScroll : MonoBehaviour
{
	[SerializeField]
	float horizontalSpace;

	[SerializeField]
	float verticalSpace;

	[SerializeField]
	float panelWidth;

	[SerializeField, Range(0f,1f)]
	float damp;

	[SerializeField]
	float scrollSensitiveness;

	[SerializeField]
	LoopScrollPanel panelPrefab;

	[SerializeField]
	List<LoopScrollPanel> panelList = new List<LoopScrollPanel>();

	List<LoopScrollPanel> dummyList = new List<LoopScrollPanel>();

	[Space,Header("Values for debug:")]
	[SerializeField]
	int LeftMostIndex = 0;

	[SerializeField]
	int firstDummyIndex = -1;

	[SerializeField]
	float lastSpeed;
	[SerializeField]
	float LeftBoundary;
	[SerializeField]
	float RightBoundary;
	[SerializeField]
	int minNumberofPanels;

	private void Start()
	{
		LeftBoundary = Camera.main.ViewportToWorldPoint(Vector3.zero).x / transform.lossyScale.x;
		RightBoundary = Camera.main.ViewportToWorldPoint(Vector3.one).x / transform.lossyScale.x;
		minNumberofPanels = (int)Mathf.Abs((LeftBoundary - RightBoundary) / (panelWidth + horizontalSpace)) + 2;
	}

	private void Update()
	{
		var scrollDelta = (Input.mouseScrollDelta.x + Input.mouseScrollDelta.y) * scrollSensitiveness;
		var currentSpeed = scrollDelta * (1 - damp) + lastSpeed * damp;
		currentSpeed = Mathf.Abs(currentSpeed) < 0.01f ? 0 : currentSpeed;
		lastSpeed = currentSpeed;
		FillListWithDummies();
		MoveList(currentSpeed);
	}

	void MoveList(float speed)
	{
		var index = LeftMostIndex;
		panelList[LeftMostIndex].transform.localPosition -= Vector3.right * speed;
		ArrangePanels();
	}

	int OutofBoundary(LoopScrollPanel panel)
	{
		if (panel.transform.localPosition.x + panelWidth / 2 < LeftBoundary) return 1;
		if (panel.transform.localPosition.x - panelWidth / 2 > RightBoundary) return -1;
		return 0;
	}

	void ArrangePanels()
	{
		var lx = panelList[LeftMostIndex].transform.localPosition.x;
		// 如果最左Panel左边有空隙，填充入Panel并重设LeftMostIndex
		while (lx > LeftBoundary + panelWidth/2)
		{
			var formerLeft = LeftMostIndex;
			LeftMostIndex--;
			LeftMostIndex += LeftMostIndex < 0 ? panelList.Count : 0;
			panelList[LeftMostIndex].transform.localPosition = 
				panelList[formerLeft].transform.localPosition - Vector3.right * (panelWidth + horizontalSpace);
			lx = panelList[LeftMostIndex].transform.localPosition.x;
		}

		// 如果最左Panel移出Scroll view，deactive并重设LeftMostIndex
		while (lx < LeftBoundary - panelWidth/2)
		{
			var formerLeft = LeftMostIndex;
			LeftMostIndex ++;
			LeftMostIndex %= panelList.Count;
			lx = panelList[LeftMostIndex].transform.localPosition.x;
		}

		var idx = LeftMostIndex;
		var nxt = idx + 1;
		nxt %= panelList.Count;
		for (int i = 1; i < minNumberofPanels; i ++)
		{
			panelList[nxt].transform.localPosition = 
				panelList[idx].transform.localPosition + Vector3.right * (panelWidth + horizontalSpace);

			idx++;
			nxt++;
			idx %= panelList.Count;
			nxt %= panelList.Count;
		}


		for (int i = 0; i < panelList.Count; i++)
		{
			if (OutofBoundary(panelList[i]) != 0)
			{
				panelList[i].gameObject.SetActive(false);
			}
			else
			{
				panelList[i].gameObject.SetActive(true);
			}
		}
	}

	void Clamp()
	{

	}

	void FillListWithDummies()
	{
		if (panelList.Count >= minNumberofPanels) return;

		for (int i = 0; i < panelList.Count; i++)
		{
			if (panelList[i].DummyMaster != null)
			{
				firstDummyIndex = i;
			}
		}

		if (firstDummyIndex == 0)
		{
			print("EmptyList!");
			return;
		}

		var numberOfRequiredDummies = minNumberofPanels - panelList.Count;
		
		for (int i = 0; i <= numberOfRequiredDummies; i++)
		{
			panelList.Add(CreateDummy());
		}

		var realPanelIndex = 0;
		var dummyPanelIndex = 0;
		while (panelList.Count < minNumberofPanels)
		{
			panelList[firstDummyIndex + dummyPanelIndex].SetDummyMaster(panelList[realPanelIndex]);
			realPanelIndex++;
			dummyPanelIndex++;
			realPanelIndex %= firstDummyIndex;
		}
	}

	LoopScrollPanel FindRealPanel(int startIndex)
	{
		for (int i = 0; i < panelList.Count; i++)
		{
			if (panelList[startIndex].DummyMaster == null)
				return panelList[startIndex];

			startIndex++;
			startIndex %= panelList.Count;
		}

		return null;
	}

	LoopScrollPanel CreateDummy()
	{
		var dummy = Instantiate(panelPrefab.gameObject).GetComponent<LoopScrollPanel>();
		dummy.transform.position = Vector3.right * (RightBoundary + panelWidth);
		dummy.transform.SetParent(transform);
		dummy.transform.localScale = Vector3.one;
		dummyList.Add(dummy);
		dummy.gameObject.SetActive(false);
		return dummy;
	}
}
