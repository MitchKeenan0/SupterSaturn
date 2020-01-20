using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetPanelSlot : MonoBehaviour
{
	public Image slotImage;
	public Text slotText;

    public void SetSlot(Sprite sp, string st)
	{
		slotImage.preserveAspect = true;

		if (sp != null)
			slotImage.sprite = sp;
		else
			slotImage.enabled = false;
		
		if (st != null)
			slotText.text = st;
		else
			slotText.text = "";
	}
}
