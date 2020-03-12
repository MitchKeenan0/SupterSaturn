using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftController : MonoBehaviour
{
	public float sensitivity = 30f;
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
	private float burnDuration = 0f;

	private IEnumerator loadCoroutine;
	private IEnumerator updateCoroutine;

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
		loadCoroutine = LoadWait(0.5f);
		StartCoroutine(loadCoroutine);
    }
	
	IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		inputController.SetCameraTarget(spacecraft.transform);
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
					/// burn duration
					float lineLength = touchLineVector.magnitude;
					burnDuration = lineLength / 50f;
					orbitController.SetBurnDuration(burnDuration);

					/// orbit direction
					moveVector = Vector3.Lerp(moveVector, new Vector3(touchLineVector.x, touchLineVector.y, 0f), Time.deltaTime * sensitivity);
					orbitController.SetDirection(moveVector);

					/// circular size for auto-orbit
					float positionRange = spacecraft.transform.position.magnitude;
					orbitController.SetOrbitRange(positionRange);

					/// visualize
					///List<Vector3> routeList = new List<Vector3>();
					///routeList = orbitController.GetTrajectory(); /// GetPoints();
					///autopilot.SetRoute(routeList);
					autopilot.SetManeuverVector(orbitController.GetOrbitTarget());
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
			autopilot.SetManeuverVector(spacecraft.transform.forward);
			autopilot.FireEngineBurn(burnDuration);
			//List<Vector3> routeList = new List<Vector3>();
			//routeList = orbitController.GetTrajectory();
			//autopilot.SetRoute(routeList);
			//autopilot.EnableMoveCommand(true);
		}
		inputController.NavigationMode(false);
	}

	public void SetActive(bool value)
	{
		bActive = value;
	}
}
