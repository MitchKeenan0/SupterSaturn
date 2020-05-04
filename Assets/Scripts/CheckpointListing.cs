using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointListing : MonoBehaviour
{
	public Image checkCompleteImage;
	public Text checkpointNameText;

    void Start()
    {
		checkCompleteImage.enabled = false;
    }

	public void SetListing(string checkpointName)
	{
		checkpointNameText.text = checkpointName;
	}

	public void ClearCheckpoint()
	{
		checkCompleteImage.enabled = true;
	}
}
