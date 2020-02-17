using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTelemetry : MonoBehaviour
{
	public GameObject pitchJoint;
	public float updateInterval = 0.1f;

	private Camera cameraMain;
	private IEnumerator updateCoroutine;

    void Start()
    {
		cameraMain = Camera.main;
		updateCoroutine = UpdateCameraTelemetry(updateInterval);
		StartCoroutine(updateCoroutine);
    }

	private IEnumerator UpdateCameraTelemetry(float updateInterval)
	{
		while (true)
		{
			yield return new WaitForSeconds(updateInterval);
			Quaternion pitchRotation = cameraMain.transform.rotation;
			pitchRotation.z = pitchRotation.x;
			pitchRotation.y = 0;
			pitchRotation.x = 0;
			pitchJoint.transform.rotation = pitchRotation;
		}
	}
}
