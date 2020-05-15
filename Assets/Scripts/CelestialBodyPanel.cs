using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CelestialBodyPanel : MonoBehaviour
{
	public Image panelImage;
	public Text panelText;

	private Transform celestialTransform;

	public void SetPanel(Sprite sp, string st, Transform ct)
	{
		if (sp != null)
			panelImage.sprite = sp;
		panelText.text = st;
		celestialTransform = ct;
	}

	public void TouchPanel()
	{
		FindObjectOfType<TouchOrbit>().SetFocusTransform(celestialTransform);
	}
}
