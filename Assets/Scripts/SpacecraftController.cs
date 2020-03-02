using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftController : MonoBehaviour
{
	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private TouchLine touchLine;
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
		this.enabled = false;
    }

    void Update()
    {
		if (touchLine.IsTouching() && !bLining)
			StartTouch();
		else if (!touchLine.IsTouching() && bLining)
			EndTouch();

		if (bLining)
		{
			touchLineVector = touchLine.GetLine();
			moveVector = new Vector3(touchLineVector.x, touchLineVector.y, 0f);
			moveVector = Vector3.ProjectOnPlane(moveVector, transform.position.normalized);
			autopilot.SetMoveCommand(autopilot.transform.position + moveVector, false);
		}
	}

	void StartTouch()
	{
		bLining = true;
		autopilot.EnableMoveCommand(true);
	}

	void EndTouch()
	{
		bLining = false;
	}

	public void SetActive(bool value)
	{
		bActive = value;
		this.enabled = value;
	}
}
