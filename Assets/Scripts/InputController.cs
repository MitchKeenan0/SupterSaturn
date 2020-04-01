using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Text statusText;
	public Button navigationModeButton;
	public Button cameraModeButton;
	public Button stopButton;
	public Button scanButton;
	public Color standbyColor;
	public Color activeColor;

	private MouseSelection mouseSelection;
	private Camera cameraMain;
	private CameraTargetFinder cameraTargetFinder;
	private TouchOrbit touchOrbit;
	private OrbitController orbitController;
	private float originalCameraScale = 1f;
	private bool bNavigationMode = false;
	private bool bFreeCameraMode = false;
	private bool bStopped = false;
	private bool bScanning = false;
	private int cameraMode = 1;
	private int numCameraModes = 0;
	private List<float> distanceModes;

	public bool IsStopped() { return bStopped; }

	void Awake()
	{
		distanceModes = new List<float>();
	}

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		cameraMain = Camera.main;
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		touchOrbit = FindObjectOfType<TouchOrbit>();
		orbitController = FindObjectOfType<OrbitController>();
		distanceModes.Add(touchOrbit.distanceMin);
		distanceModes.Add(touchOrbit.distanceMax);
		originalCameraScale = touchOrbit.GetInputScale();
		statusText.text = "";
		numCameraModes = distanceModes.Count;

		ActivateButton(navigationModeButton, false, true);
		ActivateButton(cameraModeButton, false, true);
		ActivateButton(stopButton, false, true);
		ActivateButton(scanButton, false, true);
	}

	void ActivateButton(Button button, bool value, bool finished)
	{
		if (finished)
			EventSystem.current.SetSelectedGameObject(null);

		Color color = standbyColor;
		if (value && !finished)
			color = activeColor;
		ColorBlock buttonColorBlock = button.colors;
		buttonColorBlock.normalColor = color;
		buttonColorBlock.highlightedColor = color;
		buttonColorBlock.pressedColor = Color.black;
		buttonColorBlock.selectedColor = color;
		button.colors = buttonColorBlock;

		if (!value)
			FallbackCheck();
	}

	public void UpdateStatusText(string value)
	{
		statusText.text = value;
	}

	void FallbackCheck()
	{
		if (!bNavigationMode && !bFreeCameraMode)
			EnableCameraControl(true);
	}

	void SetDiminishedCameraMode(bool value)
	{
		if (value)
			touchOrbit.SetInputScale(originalCameraScale * 0.2f);
		else
			touchOrbit.SetInputScale(originalCameraScale);
	}

	public void Begin()
	{
		AllStop(false);
		CameraMode();
		ActivateButton(stopButton, false, true);
		Debug.Log("begun");
	}

	public void AllStop(bool bActuallyThough)
	{
		bool bTurningOff = bStopped;
		bStopped = !bStopped;
		if (bActuallyThough)
			bStopped = true;
		bool bEngineShutdown = false;
		if (bStopped)
		{
			List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
			foreach (Spacecraft sp in selectedSpacecraft)
			{
				Autopilot autopilot = sp.GetComponent<Autopilot>();
				if (autopilot.IsEngineActive())
					bEngineShutdown = true;
				autopilot.AllStop();
			}
		}
		else
		{
			List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
			foreach (Spacecraft sp in selectedSpacecraft)
			{
				Autopilot autopilot = sp.GetComponent<Autopilot>();
				autopilot.ReleaseBrakes();
			}
		}

		if (bStopped)
		{
			if (bEngineShutdown)
				UpdateStatusText("Main engine shutdown");
			else
				UpdateStatusText("All stop");
		}
		else
		{
			UpdateStatusText("Intertial stabilizers off");
			//orbitController.SetUpdating(true);
		}

		if (bEngineShutdown)
		{
			bTurningOff = false;
			bStopped = false;
			orbitController.SetUpdating(false);
		}

		if (bStopped && !bEngineShutdown)
			orbitController.ClearTrajectory();

		bool bActivated = bStopped || bEngineShutdown;
		ActivateButton(stopButton, bActivated, bTurningOff);
	}

	public void NavigationMode(bool value)
	{
		bool bTurningOff = bNavigationMode;
		if (value == false)
			bTurningOff = true;
		if (bTurningOff)
			value = false;

		SetDiminishedCameraMode(!bTurningOff);
		bNavigationMode = value;
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
		{
			if (sp != null)
				sp.GetComponentInChildren<SpacecraftController>().SetActive(bNavigationMode);
		}

		if (bNavigationMode)
			UpdateStatusText("Navigation mode");
		else
			UpdateStatusText("");
		ActivateButton(navigationModeButton, bNavigationMode, bTurningOff);
	}

	public void CameraMode()
	{
		EnableCameraControl(true);

		if (numCameraModes > 0)
		{
			cameraTargetFinder.SetActive(true);
			float dist = distanceModes[cameraMode];
			touchOrbit.SetDistance(dist);
			cameraMain.focalLength = 1f / (Mathf.Sqrt(dist) * 0.1f) * 6f;
			cameraMode++;
			if (cameraMode >= numCameraModes)
				cameraMode = 0;
			UpdateStatusText("Camera range " + dist + " km");
		}
	}

	public void CameraPlanet()
	{

	}

	public void Scan()
	{
		bool bFinished = bScanning;
		bScanning = !bScanning;
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
		{
			if (sp.GetAgent() != null)
				sp.GetAgent().Scan();
			UpdateStatusText("Scanner " + (bScanning ? "on" : "off"));
		}
		ActivateButton(scanButton, bScanning, bFinished);
	}

	public void EnableCameraControl(bool value)
	{
		bFreeCameraMode = value;
		touchOrbit.SetActive(bFreeCameraMode);
		UpdateStatusText("Free camera " + (value ? ("on") : ("off")));
	}

	public void SetCameraTarget(Transform target)
	{
		cameraTargetFinder.SetTarget(target);
	}
}
