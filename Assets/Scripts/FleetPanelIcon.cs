using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetPanelIcon : MonoBehaviour
{
	private Image iconImage;
	private Spacecraft spacecraft;

    void Awake()
    {
		iconImage = GetComponent<Image>();
    }

	public void SetSpacecraft(Spacecraft sp)
	{
		spacecraft = sp;
		iconImage.sprite = sp.craftIcon;
	}
}
