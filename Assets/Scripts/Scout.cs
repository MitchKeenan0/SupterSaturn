using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : MonoBehaviour
{
	public float scoutMoveSpeed = 5;
	public float updateInterval = 0.02f;
	public GameObject scoutPanel;
	public GameObject scoutObject;
	public GameObject objectPanel;
	public GameObject connectionPrefab;
	public Vector3 scoutPanelOffset = new Vector3(50f, 0f, 0f);

	private TurnManager turnManager;
	private FleetController fleetController;
	private CampaignLocation originLocation;
	private CampaignLocation scoutLocation;
	private LineRenderer lineRenderer;
	private Camera cameraMain;

	private IEnumerator movementCoroutine;
	private IEnumerator screenUpdateCoroutine;

	void Awake()
	{
		turnManager = FindObjectOfType<TurnManager>();
		fleetController = GetComponent<FleetController>();
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = false;
		cameraMain = Camera.main;
	}

	void Start()
    {
		scoutPanel.SetActive(false);
		scoutObject.SetActive(false);
		objectPanel.SetActive(false);
	}

	public void Standby(CampaignLocation origin, CampaignLocation location)
	{
		scoutPanel.SetActive(true);
		originLocation = origin;
		scoutLocation = location;
		Vector3 locationScreenPosition = cameraMain.WorldToScreenPoint(scoutLocation.transform.position);
		scoutPanel.transform.position = locationScreenPosition + scoutPanelOffset;

		lineRenderer.SetPosition(0, originLocation.transform.position);
		lineRenderer.SetPosition(1, scoutLocation.transform.position);
		lineRenderer.enabled = true;

		screenUpdateCoroutine = ScreenPosition(updateInterval);
		StartCoroutine(screenUpdateCoroutine);
	}

	public void ScoutLocation(CampaignLocation origin, CampaignLocation scout)
	{
		transform.SetParent(null);
		scoutObject.transform.localScale = Vector3.one;
		ScreenPosition(0.001f);
		originLocation = origin;
		scoutLocation = scout;

		scoutObject.transform.position = originLocation.transform.position;
		scoutObject.SetActive(true);
		objectPanel.SetActive(true);

		movementCoroutine = ScoutMove(updateInterval);
		StartCoroutine(movementCoroutine);
	}

	public void Undo()
	{
		scoutPanel.SetActive(false);
		StopAllCoroutines();
		scoutObject.SetActive(false);

		lineRenderer.enabled = false;
		originLocation = null;
		scoutLocation = null;
	}

	private IEnumerator ScreenPosition(float updateInterval)
	{
		while (true)
		{
			UpdateScreenPosition(updateInterval);
			yield return new WaitForSeconds(updateInterval);
		}
	}

	void UpdateScreenPosition(float delta)
	{
		if (scoutObject.activeInHierarchy)
		{
			Vector3 objectScreenPosition = cameraMain.WorldToScreenPoint(scoutObject.transform.position);
			objectPanel.transform.position = objectScreenPosition;
		}

		if (scoutPanel.activeInHierarchy)
		{
			Vector3 panelScreenPosition = cameraMain.WorldToScreenPoint(scoutLocation.transform.position);
			scoutPanel.transform.position = panelScreenPosition + scoutPanelOffset;
		}
	}

	private IEnumerator ScoutMove(float updateInterval)
	{
		while (true)
		{
			UpdateMove(updateInterval);
			yield return new WaitForSeconds(updateInterval);
		}
	}

	void UpdateMove(float delta)
	{
		Vector3 toMoveLocation = scoutLocation.transform.position - scoutObject.transform.position;
		if (toMoveLocation.magnitude > 0.1f)
		{
			Vector3 move = scoutObject.transform.position + (toMoveLocation.normalized * scoutMoveSpeed * delta);
			scoutObject.transform.position = move;
		}
		else
		{
			FinishMove();
		}
	}

	void FinishMove()
	{
		scoutLocation.GetScouted(originLocation, connectionPrefab);
		scoutPanel.SetActive(false);
		scoutObject.SetActive(false);
		objectPanel.SetActive(false);
		lineRenderer.enabled = false;

		fleetController.ScoutComplete();
		transform.SetParent(fleetController.transform);
		scoutObject.transform.localScale = Vector3.one;
		StopCoroutine(movementCoroutine);
		StopCoroutine(screenUpdateCoroutine);
		turnManager.BeginTurn(false);
	}
}
