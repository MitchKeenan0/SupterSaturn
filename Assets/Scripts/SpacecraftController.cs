using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftController : MonoBehaviour
{
	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private TouchLine touchLine;
	private InputController inputController;
	private Camera cameraMain;
	private Vector2 touchLineVector = Vector2.zero;
	private Vector3 moveVector = Vector3.zero;
	private bool bActive = false;
	private bool bLining = false;

    void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		if (!spacecraft && (transform.parent != null))
			spacecraft = transform.parent.GetComponentInChildren<Spacecraft>();
		autopilot = spacecraft.gameObject.GetComponent<Autopilot>();
		touchLine = FindObjectOfType<TouchLine>();
		inputController = FindObjectOfType<InputController>();
		cameraMain = Camera.main;
    }

    void Update()
    {
		if (bActive)
		{
			if (touchLine.IsTouching() && !bLining)
				StartTouch();
			else if (!touchLine.IsTouching() && bLining)
				EndTouch();

			if (bLining)
			{
				touchLineVector = touchLine.GetLine();
				if (touchLineVector.magnitude > 5f)
				{
					Debug.Log("touchLineVector: " + touchLineVector);
					moveVector = new Vector3(touchLineVector.x, touchLineVector.y, 0f);
					Vector3 orbitVector = Vector3.ProjectOnPlane(moveVector, transform.position.normalized);
					Vector3 aimVector = Vector3.ProjectOnPlane(moveVector, cameraMain.transform.forward.normalized);
					autopilot.SetMoveCommand(orbitVector + aimVector, false);
				}
			}
		}
	}

	void StartTouch()
	{
		moveVector = Vector3.zero;
		bLining = true;
	}

	void EndTouch()
	{
		bLining = false;
		if (moveVector != Vector3.zero)
			autopilot.EnableMoveCommand(true);
		inputController.NavigationMode();
	}

	public void SetActive(bool value)
	{
		Debug.Log("SpacecraftController set active " + value);
		bActive = value;
	}
}
