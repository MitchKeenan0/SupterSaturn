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
	private LineRenderer touchLineRenderer;
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
		touchLineRenderer = GetComponent<LineRenderer>();
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

			if (bLining && (Input.touchCount > 0))
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

					Vector3 navigationTarget = (Quaternion.Euler(cameraMain.transform.eulerAngles) * (onscreenDirection * 0.9f));
					navigationTarget += cameraMain.transform.forward * 500f;
					Vector3 navigationVector = navigationTarget + transform.position;
					directionVector = navigationVector;

					/// touch line UI
					touchLineRenderer.SetPosition(0, transform.position);
					touchLineRenderer.SetPosition(1, transform.position + (spacecraft.transform.forward * 10)); /// directionVector
					float lineDist = Vector3.Distance(transform.position, onscreenDirection);
					Color lineColor = new Color(1f, lineDist, 1f);
					float velocity = rb.velocity.magnitude * 0.01f;
					lineColor.g = Mathf.Clamp(lineColor.g - velocity, 0f, 1f);
					lineColor.b = Mathf.Clamp(lineColor.b - velocity, 0f, 1f);
					Color start = lineColor * 0.8f;
					start.a = 1f;
					touchLineRenderer.startColor = start;
					touchLineRenderer.endColor = lineColor;

					/// visualize
					autopilot.ManeuverRotationTo(directionVector);
				}
			}
		}
	}

	void StartTouch()
	{
		bStartTouchDelay = true;
		startTouchCoroutine = StartTouchDelay(0.1f);
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
			touchLineRenderer.enabled = true;
			navigationHud.SetActive(true);
			bStartTouchDelay = false;
			bInputting = true;

			autopilot.FireEngineBurn(99f, false);
			orbitController.SetBackgroundUpdateInterval(0.1f);
		}
	}

	void EndTouch()
	{
		bEndTouchDelay = true;
		endTouchCoroutine = EndTouchDelay();
		StartCoroutine(endTouchCoroutine);
	}

	private IEnumerator endTouchCoroutine;
	private IEnumerator EndTouchDelay()
	{
		bLining = false;
		touchLineRenderer.enabled = false;

		yield return new WaitForSeconds(0.1f);

		autopilot.ManeuverRotationTo(directionVector);
		autopilot.MainEngineShutdown();
		orbitController.SetBackgroundUpdateInterval(2f);

		bInputting = false;
		bEndTouchDelay = false;
		bUpdateNavigationTarget = true;
		///inputController.SetNavigationButtonMode(1);
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
	}

	public void SetInputting(bool value)
	{
		bInputting = value;
		
		if (bInputting)
		{
			StartTouch();
			
		}
		else
		{
			EndTouch();
		}
	}
}
