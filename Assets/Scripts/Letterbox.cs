using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letterbox : MonoBehaviour
{
	public CanvasGroup barsCanvasGroup;
	public bool bPortraitEnabled = true;
	public bool bLandscapeEnabled = false;

	private bool bPortrait = false;
	private bool bVignetteEnabled = false;

    void Start()
    {
		//ReadScreenOrientation();
    }

    void Update()
    {
		ReadScreenOrientation();
		ProcessNables();
    }

	void ReadScreenOrientation()
	{
		if (Input.deviceOrientation == DeviceOrientation.Portrait)
		{
			bPortrait = true;
			return;
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
		{
			bPortrait = false;
			return;
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
		{
			bPortrait = false;
			return;
		}
		if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
		{
			bPortrait = true;
			return;
		}
	}

	void ProcessNables()
	{
		if (bPortrait)
		{
			if (bPortraitEnabled)
			{
				if (!bVignetteEnabled)
					SetVignetteEnabled(true);
			}
			else
			{
				if (bVignetteEnabled)
					SetVignetteEnabled(false);
			}
		}
		else
		{
			if (bLandscapeEnabled)
			{
				if (!bVignetteEnabled)
					SetVignetteEnabled(true);
			}
			else
			{
				if (bVignetteEnabled)
					SetVignetteEnabled(false);
			}
		}
	}

	void SetVignetteEnabled(bool value)
	{
		barsCanvasGroup.alpha = value ? 1f : 0f;
		bVignetteEnabled = value;
		Debug.Log("set vignette " + value + " " + Time.time.ToString("F1"));
	}
}
