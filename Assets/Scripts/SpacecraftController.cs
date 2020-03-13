using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftController : MonoBehaviour
{
	public float sensitivity = 15f;
	public float moveLineScale = 10f;

	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private TouchLine touchLine;
	private InputController inputController;
	private OrbitController orbitController;
	private Camera cameraMain;
	private Vector2 touchLineVector = Vector2.zero;
	private Vector2 touchPosition = Vector2.zero;
	private Vector3 inputVector = Vector3.zero;
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
		inputController.Begin();
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
				touchPosition = touchLine.GetPosition();
				if (Mathf.Abs(touchLineVector.magnitude) > 0f)
				{
					/// burn duration
					float lineLength = touchLineVector.magnitude;
					burnDuration = lineLength / 10f;
					Debug.Log("burn duration " + burnDuration);
					orbitController.SetBurnDuration(burnDuration);

					/// orbit direction
					inputVector = Vector3.Lerp(inputVector, new Vector3(touchPosition.x, touchPosition.y, 0f), Time.deltaTime * sensitivity);
					orbitController.SetDirection(inputVector);

					/// circular size for auto-orbit
					float positionRange = spacecraft.transform.position.magnitude;
					orbitController.SetOrbitRange(positionRange);

					/// visualize
					autopilot.ManeuverRotationTo(orbitController.GetOrbitTarget() - autopilot.gameObject.GetComponent<Rigidbody>().velocity);
				}
			}
		}
	}

	public void StartTouch()
	{
		inputVector = Vector3.zero;
		bLining = true;
	}

	public void EndTouch()
	{
		bLining = false;
		autopilot.ManeuverRotationTo(orbitController.GetOrbitTarget() - autopilot.gameObject.GetComponent<Rigidbody>().velocity);
		autopilot.FireEngineBurn(burnDuration);
		inputController.NavigationMode(false);
	}


	public void SetActive(bool value)
	{
		bActive = value;
	}
}
