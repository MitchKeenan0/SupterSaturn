using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftController : MonoBehaviour
{
	public float moveLineScale = 10f;

	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private TouchLine touchLine;
	private InputController inputController;
	private OrbitController orbitController;
	private Camera cameraMain;
	private Vector2 touchLineVector = Vector2.zero;
	private Vector3 moveVector = Vector3.zero;
	private bool bActive = false;
	private bool bLining = false;
	private int teamID = -1;

    void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		if (!spacecraft && (transform.parent != null))
			spacecraft = transform.parent.GetComponentInChildren<Spacecraft>();
		if (spacecraft.GetAgent() != null)
			teamID = spacecraft.GetAgent().teamID;
		autopilot = spacecraft.gameObject.GetComponent<Autopilot>();
		touchLine = FindObjectOfType<TouchLine>();
		inputController = FindObjectOfType<InputController>();
		orbitController = FindObjectOfType<OrbitController>();
		cameraMain = Camera.main;
    }

    void Update()
    {
		if (bActive && (teamID == 0))
		{
			if (touchLine.IsTouching() && !bLining)
				StartTouch();
			else if (!touchLine.IsTouching() && bLining)
				EndTouch();

			if (bLining)
			{
				touchLineVector = touchLine.GetLine() * moveLineScale;
				if (Mathf.Abs(touchLineVector.magnitude) > 0f)
				{
					/// orbit direction
					moveVector = new Vector3(touchLineVector.x, touchLineVector.y, 0f);
					orbitController.SetDirection(moveVector);

					/// circular size
					float positionRange = spacecraft.transform.position.magnitude;
					orbitController.SetOrbitRange(positionRange);

					/// visualize
					List<Vector3> routeList = new List<Vector3>();
					routeList = orbitController.GetPoints();
					autopilot.SetRoute(routeList);
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
		if (orbitController != null)
		{
			List<Vector3> routeList = new List<Vector3>();
			routeList = orbitController.GetPoints();
			autopilot.SetRoute(routeList);
			autopilot.EnableMoveCommand(true);
		}
		inputController.NavigationMode(false);
	}

	public void SetActive(bool value)
	{
		bActive = value;
	}
}
