﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spacecraft : MonoBehaviour
{
	public bool bAgentStartsEnabled = true;
	public float mainEnginePower = 1f;
	public float turningPower = 1f;
	public float maneuverPower = 0.6f;
	public string spacecraftName = "Spacecraft";
	public ParticleSystem thrustParticles;
	public Transform destroyedParticlesPrefab;
	public Sprite craftIcon;
	public Sprite craftDiagram;
	public float iconScale = 1f;
	public int numericID = 0;

	private Rigidbody rb;
	private Agent agent;
	private Health health;
	private SpacecraftInformation spacecraftInformation;
	private CameraController cameraController;
	private MouseSelection mouseSelection;
	private BattleOutcome battleOutcome;

	private Vector3 mainEnginesVector = Vector3.zero;
	private Vector3 maneuverEnginesVector = Vector3.zero;
	private Vector3 turningVector = Vector3.zero;
	private Quaternion turningRotation = Quaternion.identity;
	private GameObject hudIcon;
	private GameObject gridObject;
	private MeshRenderer[] meshRenderComponents;
	private ParticleSystem[] particleComponents;
	private TrailRenderer[] trailComponents;
	private int numMarked = 0;
	private float timeAtLastClick = 0f;

	public Agent GetAgent() { return agent; }

	public bool IsAlive() { return (health != null) && (health.GetHealth() >= 1); }

	public int GetHealth()
	{
		if (!health)
			health = GetComponent<Health>();
		return health.GetHealth();
	}
	public float GetHealthPercent() { return Mathf.Floor(health.GetHealth()) / Mathf.Floor(health.maxHealth); }

	public int GetMarks() { return numMarked; }

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		agent = GetComponent<Agent>();
		health = GetComponent<Health>();
		spacecraftInformation = GetComponent<SpacecraftInformation>();
		meshRenderComponents = GetComponentsInChildren<MeshRenderer>();
		particleComponents = GetComponentsInChildren<ParticleSystem>();
		trailComponents = GetComponentsInChildren<TrailRenderer>();
		if (GetComponentInChildren<GridVisualizer>())
			gridObject = GetComponentInChildren<GridVisualizer>().gameObject;
	}

	void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
		mouseSelection = FindObjectOfType<MouseSelection>();
		battleOutcome = FindObjectOfType<BattleOutcome>();

		SetAgentEnabled(bAgentStartsEnabled);
		turningVector = transform.position + transform.forward;
		turningRotation = transform.rotation;
		if (agent != null && (agent.teamID != 0))
			SetRenderComponents(false);
	}

	void FixedUpdate()
	{
		if (mainEnginePower != 0)
			UpdateSpacecraftPhysics();
	}

	void UpdateSpacecraftPhysics()
	{
		if (IsAlive())
		{
			if ((turningRotation.normalized != Quaternion.identity)
				&& (turningRotation.normalized != null))
			{
				Quaternion lerpRotation = Quaternion.Lerp(transform.rotation, turningRotation, Time.fixedDeltaTime * turningPower * 100f);
				rb.MoveRotation(lerpRotation);
				transform.rotation = rb.rotation;
			}

			Vector3 rbForceVector = Vector3.zero;
			if (mainEnginesVector != Vector3.zero)
			{
				rbForceVector += mainEnginesVector;
			}
			if (maneuverEnginesVector != Vector3.zero)
			{
				rbForceVector += maneuverEnginesVector;
			}
			if (rbForceVector != Vector3.zero)
				rb.AddForce(rbForceVector);
		}
	}

	void SetAgentEnabled(bool value)
	{
		if (agent != null)
			agent.SetEnabled(value);
	}

	public void MainEngines(float driveDirection)
	{
		mainEnginesVector = transform.forward * driveDirection * mainEnginePower;
		//Debug.DrawRay(transform.position, mainEnginesVector, Color.blue);
		var ps = thrustParticles.main;
		var em = thrustParticles.emission;
		if (Mathf.Abs(driveDirection) > 0.1f)
		{
			em.enabled = true;
			ps.startSize = Mathf.Abs(driveDirection) * 0.1f;
		}
		else
		{
			em.enabled = false;
		}
	}

	public void ManeuverEngines(Vector3 driveDirection)
	{
		maneuverEnginesVector = driveDirection * maneuverPower;
	}

	public void Maneuver(Vector3 targetPoint)
	{
		turningVector = (targetPoint - transform.position);
		if (turningVector != Vector3.zero)
		{
			float shoulderScale = Mathf.Clamp(1f / Vector3.Angle(turningVector.normalized, transform.forward), 3f, 1f);
			Quaternion toRotation = Quaternion.LookRotation(turningVector, Vector3.up);
			turningRotation = Quaternion.Lerp(transform.rotation, toRotation, turningPower * Time.deltaTime * shoulderScale);
		}
	}

	public void SpacecraftKnockedOut(Transform responsibleTransform)
	{
		if (agent.teamID == 0)
		{
			battleOutcome.AddLost(health.maxHealth);
		}
		else
		{
			battleOutcome.AddScore(health.maxHealth);
		}

		GridVisualizer grid = GetComponentInChildren<GridVisualizer>();
		if (grid != null)
		{
			Destroy(grid.gameObject);
		}

		if (responsibleTransform != null)
		{
			Agent killerAgent = responsibleTransform.GetComponent<Agent>();
			if (killerAgent != null)
			{
				killerAgent.NotifyTargetDestroyed(this);
			}
		}

		TargetPredictionHUD predictionHud = FindObjectOfType<TargetPredictionHUD>();
		predictionHud.SetPrediction(this, Vector3.zero, Vector3.zero, 0f);

		if (agent != null)
		{
			agent.AgentSpacecraftDestroyed();
			agent.SetEnabled(false);
		}
	}

	public void AddMarkValue(int value)
	{
		numMarked += value;
		if (numMarked > 0)
		{
			SetRenderComponents(true);
		}
		else if (agent.teamID != 0)
		{
			SetRenderComponents(false);
		}
	}

	public void SetRenderComponents(bool value)
	{
		for (int i = 0; i < meshRenderComponents.Length; i++)
		{
			meshRenderComponents[i].enabled = value;
		}
		for (int i = 0; i < particleComponents.Length; i++)
		{
			var em = particleComponents[i].emission;
			em.enabled = false;
		}
		for (int i = 0; i < trailComponents.Length; i++)
		{
			trailComponents[i].emitting = value;
		}

		if (gridObject != null)
		{
			LineRenderer[] lines = gridObject.GetComponentsInChildren<LineRenderer>();
			foreach (LineRenderer line in lines)
				line.enabled = value;
		}
	}

	public void SetHUDIcon(GameObject value)
	{
		hudIcon = value;
	}

	public void SelectionHighlight(bool value)
	{
		cameraController.Highlight(value, this);
	}

	public SpacecraftInformation GetSpacecraftInformation()
	{
		spacecraftInformation = GetComponent<SpacecraftInformation>();
		return spacecraftInformation;
	}

	public GameObject GetHUDIcon()
	{
		return hudIcon;
	}

	// Collision and mouse touch
	private void OnMouseOver()
	{
		if (!spacecraftInformation)
			spacecraftInformation = GetComponent<SpacecraftInformation>();
		cameraController.SetMouseContext(true, this);
	}

	private void OnMouseExit()
	{
		cameraController.SetMouseContext(false, null);
	}

	private void OnMouseDown()
	{
		if ((Time.time - timeAtLastClick) >= 1f)
			mouseSelection.UpdateSelection(GetComponent<Selectable>(), true);
		else
			cameraController.SetOrbitTarget(transform);
		timeAtLastClick = Time.time;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Gravity>())
		{
			health.ModifyHealth(-99, collision.gameObject.transform);
		}
		else if (collision.gameObject.GetComponent<Spacecraft>())
		{
			health.ModifyHealth(-5, collision.gameObject.transform);
		}
	}
}
