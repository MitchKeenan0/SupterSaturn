﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
	public int teamID = 0;
	public bool bAttackPredictions = true;

	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private FireCoordinator fireCoordinator;
	private Scanner scanner;
	private TargetPredictionHUD predictionHud;
	private ObjectManager objectManager;
	private RaycastManager raycastManager;
	private TeamFleetHUD teamFleetHUD;
	private SelectionSquad selectedSquad;
	private ActionCommandHUD actionHud;
	private Transform targetTransform = null;
	private List<Spacecraft> spacecraftList;
	private List<SpacecraftInformation> targetList;
	private List<Prediction> predictionList;
	private bool bEnabled = false;
	private bool bPredictingTarget = false;
	private bool bAutoTarget = false;
	private IEnumerator targetDestroyedRestCoroutine;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		targetList = new List<SpacecraftInformation>();
		predictionList = new List<Prediction>();
		spacecraft = GetComponent<Spacecraft>();
		autopilot = GetComponent<Autopilot>();
		fireCoordinator = GetComponent<FireCoordinator>();
		scanner = GetComponentInChildren<Scanner>();
		predictionHud = FindObjectOfType<TargetPredictionHUD>();
		objectManager = FindObjectOfType<ObjectManager>();
		raycastManager = FindObjectOfType<RaycastManager>();
		teamFleetHUD = FindObjectOfType<TeamFleetHUD>();
		selectedSquad = FindObjectOfType<SelectionSquad>();
		actionHud = FindObjectOfType<ActionCommandHUD>();
	}

	void Start()
	{
		
	}

	void Update()
    {
		if (isActiveAndEnabled)
		{
			if (bEnabled && ((teamID != 0) || (targetTransform != null) || bAutoTarget))
				UpdateTarget();
		}
	}

	void UpdateTarget()
	{
		if (targetTransform != null)
		{
			Spacecraft targetSpacecraft = targetTransform.GetComponent<Spacecraft>();
			bool validSpacecraft = (targetSpacecraft != null) && (targetSpacecraft.IsAlive()) && (targetSpacecraft.GetMarks() > 0);
			bool validPrediction = !validSpacecraft && bAttackPredictions && (bPredictingTarget && (targetTransform.position != Vector3.zero));

			if (validSpacecraft || validPrediction)
			{
				if (LineOfSight(targetTransform.position, targetTransform) || !validSpacecraft)
				{
					if (fireCoordinator != null)
					{
						fireCoordinator.UpdateFireCoordinator();
					}
				}
			}
			else
			{
				AquireTarget();
			}
		}
		else
		{
			AquireTarget();
		}
	}

	void AquireTarget()
	{
		targetTransform = null;
		GameObject[] scannerTargets = scanner.GetTargets();
		if (scannerTargets.Length > 0)
		{
			GameObject ga = scannerTargets[0];
			if (ga != null)
			{
				if (LineOfSight(ga.transform.position, ga.transform))
				{
					SetTarget(ga.transform);
					NotifyTeamOfTarget(ga.transform);
					bPredictingTarget = false;
				}
			}
		}

		if ((targetTransform == null) && bAttackPredictions)
		{
			if (predictionList == null)
				predictionList = new List<Prediction>();
			if (predictionHud.GetPredictions() != null)
				predictionList = predictionHud.GetPredictions();
			if (predictionList != null)
			{
				int numPredictions = predictionList.Count;
				for (int i = 0; i < numPredictions; i++)
				{
					if (predictionList[i] != null)
					{
						Spacecraft predictionSpacecraft = predictionList[i].spacecraft;
						if ((predictionSpacecraft != null) && (predictionSpacecraft.GetAgent().teamID != teamID)
							&& (predictionList[i].position != Vector3.zero))
						{
							SetTarget(predictionList[i].targetTransform);
							bPredictingTarget = true;
							break;
						}
					}
				}
			}
		}
	}

	void SetTarget(Transform value)
	{
		targetTransform = value;
		if (value != null)
		{
			targetList.Add(value.gameObject.GetComponent<SpacecraftInformation>());
			if (teamID == 0)
				actionHud.SetTarget(value);
		}
		else
		{
			targetList.Clear();
		}
	}

	void NotifyTeamOfTarget(Transform value)
	{
		if ((teamFleetHUD != null) && (teamFleetHUD.GetTeamList(teamID) != null))
		{
			List<Spacecraft> teamSpacecraftList = teamFleetHUD.GetTeamList(teamID);
			foreach (Spacecraft sp in teamSpacecraftList)
			{
				if (sp != null)
					sp.GetAgent().SuggestTarget(value);
			}
		}
	}

	public void Scan()
	{
		if (!scanner)
			GetComponentInChildren<Scanner>();
		if (scanner != null)
		{
			if (!scanner.IsScanning())
				scanner.BeginScanning();
			else
				scanner.StopScanning();
		}
	}

	public bool LineOfSight(Vector3 target, Transform targetTrans)
	{
		if (this != null)
		{
			if (!raycastManager)
				raycastManager = FindObjectOfType<RaycastManager>();
			if (raycastManager != null)
			{
				bool result = false;
				RaycastHit hit = raycastManager.CustomRaycast(transform.position, target - transform.position);
				if (hit.transform == targetTrans)
				{
					result = true;
				}
				return result;
			}
			else { return false; }
		}
		else { return false; }
	}

	public void SetOffense(bool value)
	{
		bAutoTarget = value;
		bAttackPredictions = value;
	}

	public void Regroup()
	{
		if (!selectedSquad)
			selectedSquad = GetComponent<SelectionSquad>();
		Vector3 centerPoint = selectedSquad.GetCenterPoint();
		//SetMoveOrder(centerPoint, false, null);
		//EnableMoveCommand(true);
	}

	public string GetHeadline()
	{
		string headline = "";

		if (targetTransform != null)
			headline = "Targeting " + targetTransform.name;

		return headline;
	}

	public void SuggestTarget(Transform value)
	{
		SetTarget(value);
	}

	public Transform GetTargetTransform()
	{
		return targetTransform;
	}
	
	public void NotifyTargetDestroyed(Spacecraft sp)
	{
		fireCoordinator.StandDown();
		predictionHud.SetPrediction(sp, Vector3.zero, Vector3.zero, 0f);
		targetTransform = null;
		targetDestroyedRestCoroutine = TargetDestroyedRest(0.2f);
		StartCoroutine(targetDestroyedRestCoroutine);
	}

	private IEnumerator TargetDestroyedRest(float waitTime)
	{
		bool bWasEnabled = bEnabled;
		bEnabled = false;
		yield return new WaitForSeconds(waitTime);
		bEnabled = bWasEnabled;
	}

	public void SetEnabled(bool value)
	{
		bEnabled = value;
	}

	public void AgentSpacecraftDestroyed()
	{
		if (!spacecraft)
			spacecraft = GetComponent<Spacecraft>();
		if (spacecraft != null)
		{
			if (predictionHud != null)
				predictionHud.SetPrediction(spacecraft, Vector3.zero, Vector3.zero, 0f);
			teamFleetHUD = FindObjectOfType<TeamFleetHUD>();
			teamFleetHUD.SpacecraftDestroyed(spacecraft);
			objectManager = FindObjectOfType<ObjectManager>();
			objectManager.SpacecraftDestroyed(spacecraft);
		}
		if (scanner != null)
			scanner.StopScanning();
		this.enabled = false;
	}
}
