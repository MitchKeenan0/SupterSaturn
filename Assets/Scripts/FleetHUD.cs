using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FleetHUD : MonoBehaviour
{
	public GameObject fleetPanelPrefab;
	public Vector3 panelOffset = new Vector3(0f, 100f, 0f);

	private List<Fleet> fleetList;
	private List<FleetPanel> panelList;
	private LocationDisplay locationDisplay;
	private Camera cameraMain;
	private Fleet playerFleet;

	private bool bMouseOnUI = false;
	public void MouseOnUI(bool value) { bMouseOnUI = value; }

	public List<Fleet> GetFleetList() { return fleetList; }

	private IEnumerator loadWaitCoroutine;

	void Awake()
	{
		fleetList = new List<Fleet>();
		panelList = new List<FleetPanel>();
	}

	void Start()
    {
		cameraMain = Camera.main;
		locationDisplay = FindObjectOfType<LocationDisplay>();

		loadWaitCoroutine = LoadWait(0.2f);
		StartCoroutine(loadWaitCoroutine);

		Fleet[] allFleets = FindObjectsOfType<Fleet>();
		foreach (Fleet f in allFleets)
		{
			fleetList.Add(f);
			if (f.teamID == 0)
			{
				playerFleet = f;
				FleetController fc = playerFleet.gameObject.GetComponentInChildren<FleetController>();
				locationDisplay.SetPlayerFleetController(fc);
			}
		}
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire1") && !ClickedOnObject())
			locationDisplay.SetDisplayLocation(null);

		foreach (FleetPanel panel in panelList)
			UpdateScreenPosition(panel);

		if (locationDisplay.GetLocation() != null)
			locationDisplay.UpdateCircleRender();
	}

	void UpdateScreenPosition(FleetPanel panel)
	{
		if (panel.GetFleet() != null)
		{
			Vector3 fleetScreenPosition = cameraMain.WorldToScreenPoint(panel.GetFleet().transform.position);
			panel.transform.position = fleetScreenPosition + panelOffset;
		}
	}

	bool ClickedOnObject()
	{
		bool bClickedObject = false;

		if (bMouseOnUI)
			bClickedObject = true;

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
			bClickedObject = true;
		
		return bClickedObject;
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitPanels();
	}

	void InitPanels()
	{
		int numFleets = fleetList.Count;
		for (int i = 0; i < numFleets; i++)
		{
			FleetPanel panel = CreateFleetPanel();
			Fleet panelFleet = fleetList[i];
			panel.SetFleet(panelFleet);
		}
	}

	FleetPanel CreateFleetPanel()
	{
		GameObject panelObject = Instantiate(fleetPanelPrefab, transform.position, Quaternion.identity);
		panelObject.transform.SetParent(transform);
		FleetPanel panel = panelObject.GetComponent<FleetPanel>();
		panelList.Add(panel);
		return panel;
	}
}
