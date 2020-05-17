using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointListing : MonoBehaviour
{
	public Image checkCompleteImage;
	public Text checkpointNameText;

	private TouchOrbit touchOrbit;
	private Transform checkpointTransform;

    void Start()
    {
		checkCompleteImage.enabled = false;
		touchOrbit = FindObjectOfType<TouchOrbit>();
    }

	public void SetListing(string checkpointName, Color checkpointColor, Transform cpTransform)
	{
		checkpointNameText.text = checkpointName;
		checkpointNameText.color = checkpointColor;
		checkpointTransform = cpTransform;
	}

	public void ClearCheckpoint()
	{
		checkCompleteImage.enabled = true;
	}

	public void TouchListing()
	{
		if (checkpointTransform != null)
		{
			touchOrbit.SetFocusTransform(checkpointTransform);
		}
	}
}
