using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpacecraftController : MonoBehaviour
{
	public float sensitivity = 15f;
	public float moveLineScale = 10f;
	public bool bNPCControlled = false;

	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private Rigidbody rb;
	private TouchLine touchLine;
	private InputController inputController;
	private OrbitController orbitController;
	private Camera cameraMain;
	private NavigationHud navigationHud;
	private ThrottleHUD throttleHud;
	private Crew crew;
	private Vector2 touchLineVector = Vector2.zero;
	private Vector2 touchPosition = Vector2.zero;
	private Vector3 directionVector = Vector3.zero;
	private bool bActive = false;
	private bool bLining = false;
	private bool bThrottleControlActive = false;
	private bool bStartTouchDelay = false;
	private bool bEndTouchDelay = false;
	public bool bUpdating = false;
	private bool bInputting = false;
	private bool bUpdateNavigationTarget = false;
	private int teamID = -1;
	private float burnDuration = 0f;

	public Autopilot GetAutopilot() { return autopilot; }
	public Spacecraft GetSpacecraft() { return spacecraft; }
	public bool IsActive() { return bActive; }
	private IEnumerator loadCoroutine;

	void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		if (!spacecraft && (transform.parent != null))
			spacecraft = transform.parent.GetComponentInChildren<Spacecraft>();
		if (spacecraft.GetAgent() != null)
			teamID = spacecraft.GetAgent().teamID;
		autopilot = spacecraft.gameObject.GetComponent<Autopilot>();
		rb = spacecraft.gameObject.GetComponent<Rigidbody>();
		touchLine = FindObjectOfType<TouchLine>();
		inputController = FindObjectOfType<InputController>();
		orbitController = FindObjectOfType<OrbitController>();
		cameraMain = Camera.main;
		navigationHud = FindObjectOfType<NavigationHud>();
		throttleHud = FindObjectOfType<ThrottleHUD>();
		if ((throttleHud != null) && !bNPCControlled)
			throttleHud.SetSpacecraft(spacecraft);
		crew = FindObjectOfType<Crew>();
		if (crew != null)
			crew.ImbueSpacecraft(spacecraft);
		Selectable s = spacecraft.GetComponent<Selectable>();
		if ((s != null) && !bNPCControlled)
			s.isSelected = true;

		loadCoroutine = LoadWait(0.5f);
		StartCoroutine(loadCoroutine);
    }
	
	IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (!bNPCControlled)
		{
			inputController.SetCameraTarget(spacecraft.transform);
			inputController.Begin();
			bUpdating = true;
			Image img = spacecraft.GetHUDIcon().GetComponent<Image>();
			if (img != null)
				img.color = Color.green;
		}
	}

	void Update()
	{
		if (bUpdating)
		{
			UpdateThrottleControl();
			if (bInputting)
				UpdateSpacecraftController();
		}
		if (bUpdateNavigationTarget)
		{
			Vector3 targetScreenPosition = cameraMain.WorldToScreenPoint(directionVector);
			navigationHud.SetPosition(targetScreenPosition);
		}
	}

	void UpdateSpacecraftController()
    {
		if (bActive && (teamID == 0))
		{
			if (!bStartTouchDelay && touchLine.IsTouching() && !bLining)
				StartTouch();
			else if (!bEndTouchDelay && !touchLine.IsTouching() && bLining)
				EndTouch();

			if (bLining)
			{
				touchLineVector = touchLine.GetLine() * moveLineScale;
				touchPosition = touchLine.GetPosition();
				if (Mathf.Abs(touchLineVector.magnitude) > 0f)
				{
					/// hud target position
					navigationHud.SetPosition(touchPosition);

					/// normalized onscreen
					Camera cameraMain = Camera.main;
					//Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
					Vector3 onscreenDirection = transform.position; /// = new Vector3(touchPosition.x, touchPosition.y, 0f) - centerScreen;

					// create ray from the camera and passing through the touch position:
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
					// create a logical plane at this object's position
					// and perpendicular to world Y:
					Plane plane = new Plane(Vector3.up, Vector3.zero);
					float distance = 0; // this will return the distance from the camera
					if (plane.Raycast(ray, out distance))
					{ // if plane hit...
						Vector3 pos = ray.GetPoint(distance); // get the point
						onscreenDirection = pos; // pos has the position in the plane you've touched
					}

					/// burn duration
					burnDuration = Vector3.Distance(onscreenDirection, spacecraft.transform.position) / 600f;
					orbitController.SetBurnDuration(burnDuration);
					navigationHud.SetBurnDuration(burnDuration);

					//Vector3 navigationTarget = (Quaternion.Euler(cameraMain.transform.eulerAngles) * (onscreenDirection * 0.9f));
					//navigationTarget += cameraMain.transform.forward * 900f;
					Vector3 navigationVector = onscreenDirection;// + spacecraft.transform.position;
					navigationVector.y = 0f;
					directionVector = navigationVector;
					orbitController.SetDirection(directionVector);

					/// visualize
					autopilot.ManeuverRotationTo(directionVector);
				}
			}
		}
	}

	void UpdateThrottleControl()
	{
		if (rb.velocity.magnitude > 1f)
		{
			if (!bThrottleControlActive)
			{
				throttleHud.SetThrottleControlActive(true);
				bThrottleControlActive = true;
			}
		}
		else if (bThrottleControlActive)
		{
			throttleHud.SetThrottleControlActive(false);
			bThrottleControlActive = false;
		}
	} 

	void StartTouch()
	{
		Debug.Log("start touch");
		bStartTouchDelay = true;
		startTouchCoroutine = StartTouchDelay(0.5f);
		StartCoroutine(startTouchCoroutine);
	}

	private IEnumerator startTouchCoroutine;
	private IEnumerator StartTouchDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (bStartTouchDelay)
		{
			bUpdateNavigationTarget = false;
			directionVector = Vector3.zero;
			bLining = true;
			navigationHud.SetActive(true);
			bStartTouchDelay = false;
			bInputting = true;
			Debug.Log("bLining = true");
		}
		else
		{
			Debug.Log("cancelled");
		}
	}

	void EndTouch()
	{
		bEndTouchDelay = true;
		StartCoroutine("EndTouchDelay");
	}

	private IEnumerator endTouchCoroutine;
	private IEnumerator EndTouchDelay()
	{
		bLining = false;
		yield return new WaitForSeconds(0.2f);
		autopilot.ManeuverRotationTo(directionVector - autopilot.gameObject.GetComponent<Rigidbody>().velocity);
		bInputting = false;
		bEndTouchDelay = false;
		bUpdateNavigationTarget = true;
		inputController.SetNavigationButtonMode(1);
		inputController.DeactivateButtonIndex(0, false, false);
	}

	public void SetActive(bool value)
	{
		bActive = value;
		bInputting = value;
		if (!value)
			CancelNavCommand();
	}

	public void CancelNavCommand()
	{
		if (bStartTouchDelay)
		{
			StopCoroutine("StartTouchDelay");
			bStartTouchDelay = false;
		}
		if (bEndTouchDelay)
		{
			StopCoroutine("EndTouchDelay");
			bEndTouchDelay = false;
		}
		bLining = false;
		navigationHud.SetActive(false);
		Debug.Log("canceled nav command");
	}

	public void SetInputting(bool value)
	{
		bInputting = value;
	}
}
