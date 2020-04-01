using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public bool bUpdating = false;
	private int teamID = -1;
	private float burnDuration = 0f;

	public Autopilot GetAutopilot() { return autopilot; }

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
		}
	}

	void Update()
	{
		if (bUpdating)
		{
			UpdateSpacecraftController();
			UpdateThrottleControl();
		}
	}

	void UpdateSpacecraftController()
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
					/// hud target position
					navigationHud.SetPosition(touchPosition);

					/// normalized onscreen
					Camera cameraMain = Camera.main;
					Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
					Vector3 onscreenDirection = new Vector3(touchPosition.x, touchPosition.y, 0f) - centerScreen;

					/// burn duration
					burnDuration = Mathf.Clamp(onscreenDirection.magnitude / 100f, 1f, 5f);
					orbitController.SetBurnDuration(burnDuration);
					navigationHud.SetBurnDuration(burnDuration);

					Vector3 navigationTarget = (Quaternion.Euler(cameraMain.transform.eulerAngles) * (onscreenDirection * 0.3f));
					navigationTarget += cameraMain.transform.forward * 500f;
					Vector3 navigationVector = navigationTarget + spacecraft.transform.position;
					directionVector = navigationVector;
					orbitController.SetDirection(directionVector);

					/// circular size for auto-orbit
					float positionRange = spacecraft.transform.position.magnitude;
					orbitController.SetOrbitRange(positionRange);

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

	public void StartTouch()
	{
		orbitController.SetUpdating(false);
		directionVector = Vector3.zero;
		bLining = true;
		navigationHud.SetActive(true);
	}

	public void EndTouch()
	{
		bLining = false;
		autopilot.ManeuverRotationTo(directionVector - autopilot.gameObject.GetComponent<Rigidbody>().velocity);
		autopilot.FireEngineBurn(burnDuration, false);
		inputController.NavigationMode(false);
		navigationHud.SetActive(false);
		orbitController.SetUpdating(true);
	}

	public void SetActive(bool value)
	{
		bActive = value;
	}
}
