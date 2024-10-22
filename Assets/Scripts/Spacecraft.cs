﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spacecraft : MonoBehaviour
{
	public bool bAgentStartsEnabled = true;
	public float mainEnginePower = 1f;
	public float turningSpeed = 1f;
	public float maneuverPower = 0.6f;
	public float thrustModeDrag = 0.01f;
	public float thrustPowerCost = 2f;
	public string spacecraftName = "Spacecraft";
	public float warpInSpeed = 100f;
	public ParticleSystem thrustParticles;
	public Transform destroyedParticlesPrefab;
	public Sprite craftIcon;
	public Sprite craftDiagram;
	public float iconScale = 1f;
	public int numericID = 0;
	public GameObject realBody;
	public GameObject bigGhostBody;

	private Rigidbody rb;
	private Agent agent;
	private Health health;
	private SpacecraftInformation spacecraftInformation;
	private CameraController cameraController;
	private MouseSelection mouseSelection;
	private BattleOutcome battleOutcome;
	private Powerplant powerplant;

	private Vector3 mainEnginesVector = Vector3.zero;
	private Vector3 maneuverEnginesVector = Vector3.zero;
	private Vector3 turningVector = Vector3.zero;
	private Vector3 lerpTorque = Vector3.zero;
	private Vector3 thrustVelocity = Vector3.zero;
	private Vector3 angularVelocity = Vector3.zero;
	private Vector3 sideJetVector = Vector3.zero;
	private GameObject hudIcon;
	private GameObject gridObject;
	private MeshRenderer[] meshRenderComponents;
	private ParticleSystem[] particleComponents;
	private TrailRenderer[] trailComponents;
	private int numMarked = 0;
	private bool bThrottling = false;
	private float throttleVelocityTarget = 0f;
	private float originalThrustDrag = 0f;

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

	public Vector3 GetMainEngineVector() { return mainEnginesVector; }

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		agent = GetComponent<Agent>();
		health = GetComponent<Health>();
		spacecraftInformation = GetComponent<SpacecraftInformation>();
		meshRenderComponents = GetComponentsInChildren<MeshRenderer>();
		particleComponents = GetComponentsInChildren<ParticleSystem>();
		trailComponents = GetComponentsInChildren<TrailRenderer>();
		powerplant = GetComponentInChildren<Powerplant>();
		if (GetComponentInChildren<GridVisualizer>())
			gridObject = GetComponentInChildren<GridVisualizer>().gameObject;
	}

	void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
		mouseSelection = FindObjectOfType<MouseSelection>();
		battleOutcome = FindObjectOfType<BattleOutcome>();

		angularVelocity = transform.eulerAngles;
		originalThrustDrag = thrustModeDrag;

		SetAgentEnabled(bAgentStartsEnabled);
		//if (agent != null && (agent.teamID != 0))
		//	SetRenderComponents(false);

		if (thrustParticles != null)
		{
			var em = thrustParticles.emission;
			em.enabled = false;
		}
	}

	void SetAgentEnabled(bool value)
	{
		if (agent != null)
			agent.SetEnabled(value);
	}

	void FixedUpdate()
	{
		UpdateSpacecraftPhysics();
	}

	void UpdateSpacecraftPhysics()
	{
		if (IsAlive())
		{
			/// rotating
			if (turningVector != Vector3.zero)
			{
				angularVelocity = Vector3.MoveTowards(angularVelocity, turningVector, Time.deltaTime * turningSpeed);
				transform.rotation = Quaternion.LookRotation(angularVelocity);
			}

			/// vectoring
			thrustVelocity = Vector3.zero;
			//Vector3 rbVelocity = rb.velocity;
			//if (rbVelocity.magnitude <= 3)
			//	rbVelocity = transform.forward;
			//float dotToTarget = Vector3.Dot(rbVelocity.normalized, turningVector.normalized);
			//float vectoringPower = Mathf.Clamp(maneuverPower * dotToTarget, maneuverPower * 0.5f, maneuverPower);
			//rb.velocity *= vectoringPower;
			//if (sideJetVector != Vector3.zero)
			//	velocity += sideJetVector;

			/// dynamic drag
			if ((rb.drag != 0f) && (mainEnginesVector != Vector3.zero))
			{
				Vector3 myV = rb.velocity.normalized;
				Vector3 myF = transform.forward.normalized;
				float dotToTarget = Vector3.Dot(myV, myF);
				if (dotToTarget > 0.9f)
					rb.drag = 0f;
			}
			
			/// thrust
			if (!bThrottling)
			{
				if (mainEnginesVector != Vector3.zero)
					thrustVelocity += transform.forward * mainEnginePower * Time.deltaTime;

				if (thrustVelocity != Vector3.zero)
					rb.AddForce(thrustVelocity);
			}
			else
			{
				if (rb.velocity.magnitude > throttleVelocityTarget)
					Brake();
			}
		}
	}

	public void SetThrottle(float value)
	{
		throttleVelocityTarget = rb.velocity.magnitude * value;
		throttleCoroutine = ThrottleForDuration(1.6f);
		StartCoroutine(throttleCoroutine);
	}

	private IEnumerator throttleCoroutine;
	private IEnumerator ThrottleForDuration(float waitTime)
	{
		bThrottling = true;
		yield return new WaitForSeconds(waitTime);
		bThrottling = false;
	}

	public void Maneuver(Vector3 targetDirection)
	{
		turningVector = targetDirection;
		//if (turningVector == Vector3.zero)
		//	turningVector = transform.forward;
	}

	public void MainEngines(float driveDirection)
	{
		bThrottling = false;
		mainEnginesVector = transform.forward * driveDirection;
		var em = thrustParticles.emission;
		if (driveDirection > 0.0f)
		{
			em.enabled = true;
			powerplant.AddPowerDrawOffset(-thrustPowerCost);
		}
		else
		{
			em.enabled = false;
			powerplant.AddPowerDrawOffset(thrustPowerCost);
			rb.drag = 0f;
		}
	}

	public void Brake()
	{
		float brakingScalar = Mathf.Clamp(Mathf.Sqrt(rb.velocity.magnitude), 1f, 100f);
		Vector3 brakingVelocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, Time.fixedDeltaTime * (mainEnginePower * 0.1f) * brakingScalar);
		rb.velocity = brakingVelocity;
	}

	public void SetDragMode(bool value)
	{
		rb.drag = value ? thrustModeDrag : 0f;
	}

	public void SetDrag(float value)
	{
		thrustModeDrag = originalThrustDrag * value;
		SetDragMode(thrustModeDrag != 0f);
	}

	public void SpacecraftKnockedOut(Transform responsibleTransform)
	{
		if ((battleOutcome != null) && (health != null))
		{
			if (agent.teamID == 0)
			{
				FindObjectOfType<TouchOrbit>().SetActive(false);
				battleOutcome.AddLost(health.maxHealth);
			}
			else
			{
				battleOutcome.AddScore(health.maxHealth);
			}
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

			if (responsibleTransform.gameObject.GetComponent<Gravity>())
			{
				Transform destroyedEffects = Instantiate(destroyedParticlesPrefab, transform.position, transform.rotation);
				Destroy(destroyedEffects.gameObject, 5f);
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
		//if (numMarked > 0)
		//{
		//	SetRenderComponents(true);
		//}
		//else if (agent.teamID != 0)
		//{
		//	SetRenderComponents(false);
		//}
	}

	public void SetBigGhostMode(bool value)
	{
		if (realBody != null)
			realBody.GetComponent<Renderer>().enabled = !value;
		//bigGhostBody.GetComponent<Renderer>().enabled = value;
		//if (bigGhostBody.transform.childCount > 0)
		//{
		//	Renderer[] bigGhostRenders = bigGhostBody.GetComponentsInChildren<Renderer>();
		//	for(int i = 0; i < bigGhostRenders.Length; i++)
		//		bigGhostRenders[i].enabled = value;
		//}
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
		if (GetHUDIcon() != null)
		{
			Image img = GetHUDIcon().GetComponent<Image>();
			if (img != null)
				img.color = value ? Color.green : Color.grey;
		}
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

	public void PlayerStart()
	{
		if (!rb)
			rb = GetComponent<Rigidbody>();
		float X = 0f;
		float Y = -50f;
		float Z = -350f;
		transform.position = new Vector3(X, Y, Z);
		PowerHUD powerHud = FindObjectOfType<PowerHUD>();
		if (powerHud != null)
		{
			Powerplant plant = GetComponentInChildren<Powerplant>();
			if (plant != null)
				powerHud.SetPowerSource(plant);
		}
		gameObject.tag = "Player";
		WarpIn();
	}

	public void WarpIn()
	{
		gameObject.SetActive(true);
		Vector3 enterPosition = transform.position;
		transform.position = enterPosition;
		Vector3 enterVelocity = (Vector3.zero - transform.position).normalized * warpInSpeed;
		GetComponent<Rigidbody>().AddForce(enterVelocity, ForceMode.Impulse);
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

	private void OnTriggerEnter(Collider other)
	{
		ObjectiveTrigger objTrigger = other.gameObject.GetComponent<ObjectiveTrigger>();
		if ((objTrigger != null) && (!objTrigger.IsTriggered()) && (GetComponent<Agent>().teamID == 0))
		{
			other.gameObject.GetComponent<ObjectiveTrigger>().Trigger();
			InputController ic = FindObjectOfType<InputController>();
			if (ic != null)
			{
				if (!ic.IsStopped())
					ic.AllStop(true);
			}
			int randomScore = Random.Range(10, 50);
			battleOutcome.AddScore(randomScore);
			battleOutcome.BattleOver(true);
		}
	}
}
