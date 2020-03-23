using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CelestialBodyPanel : MonoBehaviour
{
	public Image panelImage;
	public Text panelText;

	public void SetPanel(Sprite sp, string st)
	{
		if (sp != null)
			panelImage.sprite = sp;
		panelText.text = st;
	}
}
