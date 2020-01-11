using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerMask : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private MouseSelection mouseSelection;
	private Button button;

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		button = GetComponent<Button>();
		//button.onClick.AddListener(TaskOnClick);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseSelection.SetEnabled(false);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseSelection.SetEnabled(true);
	}

	//void TaskOnClick()
	//{
		
	//}

}
