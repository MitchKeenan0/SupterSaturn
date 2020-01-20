using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionPanelCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Image cardImage;
	public Text cardLevelText;

	private FleetCreator fleetCreator;
	private Button button;

	void Start()
	{
		fleetCreator = GetComponentInParent<FleetCreator>();
		button = GetComponentInChildren<Button>();
		button.onClick.AddListener(TaskOnClick);
	}

    public void SetCard(Sprite spr, int lev)
	{
		cardImage.sprite = spr;
		cardImage.preserveAspect = true;

		if (lev > 1)
			cardLevelText.text = GetRomanNumeral(lev);
		else
			cardLevelText.text = "";
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		int siblingIndex = transform.GetSiblingIndex();
		//fleetCreator.
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		int siblingIndex = transform.GetSiblingIndex();
		//fleetCreator.
	}

	void TaskOnClick()
	{
		int siblingIndex = transform.GetSiblingIndex();
		fleetCreator.Select(siblingIndex);
	}

	string GetRomanNumeral(int value)
	{
		string numeral = "";
		switch(value)
		{
			case 1:
				numeral = "I"; break;
			case 2:
				numeral = "II"; break;
			case 3:
				numeral = "III"; break;
			case 4:
				numeral = "IV"; break;
			case 5:
				numeral = "V"; break;
			default: break;
		}
		return numeral;
	}
}
