using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationHud : MonoBehaviour
{
	public GameObject burnTargetPanel;
	public Text burnDurationText;

	private float burnDuration = 0f;
	private bool bActive = false;

    void Start()
    {
		burnTargetPanel.gameObject.SetActive(false);
    }

	public void SetActive(bool value)
	{
		burnTargetPanel.SetActive(value);
		bActive = value;
	}

	public void SetPosition(Vector3 pos)
	{
		burnTargetPanel.GetComponent<RectTransform>().position = pos;
	}

	public void SetBurnDuration(float value)
	{
		if (burnDuration != value)
		{
			burnDuration = value;
			burnDurationText.text = burnDuration.ToString("F1");
		}
	}
}
