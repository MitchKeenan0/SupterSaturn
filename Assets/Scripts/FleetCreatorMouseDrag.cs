using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetCreatorMouseDrag : MonoBehaviour
{
	public Transform mouseTransform;
	private FleetPanelSlot panelSlot;
	private bool bDragging = false;

    void Start()
    {
		panelSlot = GetComponent<FleetPanelSlot>();
	}

	void OnMouseDown()
	{
		panelSlot.transform.SetParent(mouseTransform);
		bDragging = true;
		Debug.Log("Dragging");
	}

	private void OnMouseUp()
	{
		if (bDragging)
		{
			panelSlot.transform.SetParent(transform.parent);
			bDragging = false;
		}
	}

	private void Update()
	{
		if (bDragging)
		{
			Vector3 screenMousePosition = Input.mousePosition;
			Vector3 panelDragPosition = screenMousePosition;
			panelDragPosition.x = panelDragPosition.z = 0f;
			panelSlot.transform.position = panelDragPosition;
			Debug.Log("Dragging");
		}
	}
}
