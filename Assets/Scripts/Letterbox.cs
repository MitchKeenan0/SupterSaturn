using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letterbox : MonoBehaviour
{
	public CanvasGroup barsCanvasGroup;
	public bool bPortraitEnabled = true;
	public bool bLandscapeEnabled = false;

	private bool bUpdating = false;
	private bool bPortrait = false;
	private bool bVignetteEnabled = false;

	private IEnumerator loadCoroutine;

    void Start()
    {
		loadCoroutine = LoadLetterbox(0.2f);
		StartCoroutine(loadCoroutine);
    }

    void Update()
    {
		if (bUpdating)
		{
			ReadScreenOrientation();
			ProcessNables();
		}
    }

	private IEnumerator LoadLetterbox(float duration)
	{
		yield return new WaitForSeconds(duration);
		ReadScreenOrientation();
		if (Screen.width > Screen.height)
			bPortrait = false;
		ProcessNables();
		bUpdating = true;
	}

	void ReadScreenOrientation()
	{
		if (Input.deviceOrientation == DeviceOrientation.Portrait)
		{
			bPortrait = true;
			return;
		}
		if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
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
				SetVignetteEnabled(false);
			}
		}
	}

	void SetVignetteEnabled(bool value)
	{
		barsCanvasGroup.alpha = value ? 1f : 0f;
		bVignetteEnabled = value;
	}
}
