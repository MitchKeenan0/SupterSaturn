using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Text statusText;
	public Sprite[] navigationButtonSprites; /// 0-none/idle 1-ignite 2-shutdown
	public Button navigationModeButton;
	public Button cameraModeButton;
	public GameObject throttlePanel;
	public Button stopButton;
	public Color standbyColor;
	public Color activeColor;

	private Game game;
	private Camera cameraMain;
	private CameraTargetFinder cameraTargetFinder;
	private TouchOrbit touchOrbit;
	private OrbitController orbitController;
	private Spacecraft spacecraft = null;
	private ContextHeader contextHeader;
	private SkillPanel skillPanel;
	private Image navButtonImage;
	private CanvasGroup throttleCanvasGroup;
	private bool bNavigationMode = false;
	private bool bStopped = false;
	private bool bThrottleEnabled = false;

	public bool IsStopped() { return bStopped; }

	void Awake()
	{
		touchOrbit = FindObjectOfType<TouchOrbit>();
	}

	void Start()
	{
		game = FindObjectOfType<Game>();
		cameraMain = Camera.main;
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		orbitController = FindObjectOfType<OrbitController>();
		contextHeader = FindObjectOfType<ContextHeader>();
		skillPanel = FindObjectOfType<SkillPanel>();
		statusText.text = "";
		navButtonImage = navigationModeButton.GetComponent<Image>();
		navButtonImage.preserveAspect = true;
		throttleCanvasGroup = throttlePanel.GetComponent<CanvasGroup>();

		ActivateButton(navigationModeButton, false, true);
	}

	public void Begin()
	{
		cameraTargetFinder.SetActive(true);
		touchOrbit.SetDistance(touchOrbit.distanceMax);
		EnableCameraControl(true);
	}

	public void DeactivateButtonIndex(int index, bool value, bool bFinished)
	{
		Button button = null;
		switch(index)
		{
			case 0: button = navigationModeButton;
				break;
			case 1: button = cameraModeButton;
				break;
			case 2: button = stopButton;
				break;
			default:
				break;
		}
		if (button != null)
			ActivateButton(button, value, bFinished);
	}

	public void ActivateButton(Button button, bool value, bool finished)
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
	}

	public void UpdateStatusText(string value)
	{
		statusText.text = value;
	}

	public void SetNavigationButtonMode(int navModeIndex)
	{
		navButtonImage.sprite = navigationButtonSprites[navModeIndex];
	}

	public void StopButton()
	{
		ActivateButton(stopButton, bThrottleEnabled, false);
	}

	public void AllStop(bool bActuallyThough)
	{
		Debug.Log("ic allstop");
		if (bActuallyThough)
			bStopped = true;
		if (game.GetSpacecraftList() != null)
		{
			if (bStopped)
			{
				spacecraft = game.GetSpacecraftList()[0];
				Autopilot autopilot = spacecraft.GetComponent<Autopilot>();
				autopilot.AllStop();
			}
		}

		UpdateStatusText("All stop");
		orbitController.KillOrbit();
		bool bActivated = bStopped;
	}

	public void NavigationMode(bool value)
	{
		bool bTurningOff = bNavigationMode;
		if (value == false)
			bTurningOff = true;

		bNavigationMode = value;
		//SetFineCameraInput(bNavigationMode);
		if (spacecraft == null)
			spacecraft = game.GetSpacecraftList()[0];
		if (spacecraft != null)
			spacecraft.GetComponentInChildren<SpacecraftController>().SetActive(bNavigationMode);

		if (bNavigationMode)
		{
			UpdateStatusText("Navigation mode");
			contextHeader.SetContextHeader("Navigation awaiting input");
		}
		else
		{
			UpdateStatusText("");
			contextHeader.SetContextHeader("");
		}

		ActivateButton(navigationModeButton, bNavigationMode, false); /// bTurningOff
	}

	public void EnableCameraControl(bool value)
	{
		if (!touchOrbit)
			touchOrbit = FindObjectOfType<TouchOrbit>();
		touchOrbit.SetActive(value);
	}

	public void SetCameraTarget(Transform target)
	{
		cameraTargetFinder.SetTarget(target);
	}

	public void SetNavigationInput(bool value)
	{
		spacecraft.GetComponentInChildren<SpacecraftController>().SetInputting(value);
	}

	public void NavigationButtonPressed()
	{
		NavigationMode(true);
	}

	public void NavigationButtonReleased()
	{
		SetNavigationInput(false);
		NavigationMode(false);
		SpacecraftController sc = spacecraft.GetComponentInChildren<SpacecraftController>();
		sc.CancelNavCommand();
		//AllStop(false);
	}

	public void StopButtonPressed()
	{
		
	}

	public void StopButtonReleased()
	{
		
	}
}
