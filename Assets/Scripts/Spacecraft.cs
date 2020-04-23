using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	public GameObject realBody;
	public GameObject bigGhostBody;

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
	private Vector3 lerpTorque = Vector3.zero;
	private Vector3 velocity = Vector3.zero;
	private Vector3 angularVelocity = Vector3.zero;
	private Vector3 sideJetVector = Vector3.zero;
	private GameObject hudIcon;
	private GameObject gridObject;
	private MeshRenderer[] meshRenderComponents;
	private ParticleSystem[] particleComponents;
	private TrailRenderer[] trailComponents;
	private int numMarked = 0;
	private float timeAtLastClick = 0f;
	private bool bThrottle = false;
	private float throttleVelocityTarget = 0f;

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
		if (GetComponentInChildren<GridVisualizer>())
			gridObject = GetComponentInChildren<GridVisualizer>().gameObject;
	}

	void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
		mouseSelection = FindObjectOfType<MouseSelection>();
		battleOutcome = FindObjectOfType<BattleOutcome>();

		angularVelocity = transform.eulerAngles;

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
				float kernScale = Mathf.Clamp(Vector3.Dot(transform.forward.normalized, turningVector.normalized) * 3f, 0.5f, 3f);
				angularVelocity = Vector3.Lerp(angularVelocity, turningVector, Time.deltaTime * turningPower * kernScale);
				transform.rotation = Quaternion.LookRotation(angularVelocity);
			}

			/// vectoring
			velocity = Vector3.zero;
			if (sideJetVector != Vector3.zero)
				velocity += sideJetVector;

			/// thrust
			if (!bThrottle)
			{
				if (mainEnginesVector != Vector3.zero)
					velocity += mainEnginesVector * mainEnginePower * Time.deltaTime;

				if (velocity != Vector3.zero)
					rb.AddForce(velocity);
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
		throttleCoroutine = ThrottleForDuration(1f);
		StartCoroutine(throttleCoroutine);
	}

	private IEnumerator throttleCoroutine;
	private IEnumerator ThrottleForDuration(float waitTime)
	{
		bThrottle = true;
		yield return new WaitForSeconds(waitTime);
		bThrottle = false;
	}

	public void Maneuver(Vector3 targetDirection)
	{
		turningVector = targetDirection;
		if (turningVector == Vector3.zero)
			turningVector = transform.forward;
	}

	public void SideJets(Vector3 driveVector)
	{
		sideJetVector = driveVector;
		if (driveVector == Vector3.zero)
			sideJetVector = Vector3.zero;
	}

	public void MainEngines(float driveDirection)
	{
		bThrottle = false;
		mainEnginesVector = transform.forward * driveDirection;
		var ps = thrustParticles.main;
		var em = thrustParticles.emission;
		if (Mathf.Abs(driveDirection) > 0.0f)
		{
			em.enabled = true;
			ps.startSize = Mathf.Abs(driveDirection);
		}
		else
		{
			em.enabled = false;
		}
	}

	public void Brake()
	{
		float brakingScalar = Mathf.Clamp(Mathf.Sqrt(rb.velocity.magnitude), 1f, 100f);
		Vector3 brakingVelocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, Time.fixedDeltaTime * (mainEnginePower * 0.1f) * brakingScalar);
		rb.velocity = brakingVelocity;
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
				Destroy(gameObject, 0.2f);
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
		bigGhostBody.GetComponent<Renderer>().enabled = value;
		if (bigGhostBody.transform.childCount > 0)
		{
			Renderer[] bigGhostRenders = bigGhostBody.GetComponentsInChildren<Renderer>();
			for(int i = 0; i < bigGhostRenders.Length; i++)
				bigGhostRenders[i].enabled = value;
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

	/// Collision and mouse touch
	private void OnMouseOver()
	{
		if (!spacecraftInformation)
			spacecraftInformation = GetComponent<SpacecraftInformation>();
		if (cameraController != null)
			cameraController.SetMouseContext(true, this);
	}

	private void OnMouseExit()
	{
		if (cameraController != null)
			cameraController.SetMouseContext(false, null);
	}

	private void OnMouseDown()
	{
		if (mouseSelection != null)
		{
			if ((Time.time - timeAtLastClick) >= 1f)
				mouseSelection.UpdateSelection(GetComponent<Selectable>(), true);
			else if (cameraController != null)
				cameraController.SetOrbitTarget(transform);
		}
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

	public void HotStart()
	{
		if (!rb)
			rb = GetComponent<Rigidbody>();
		transform.position = new Vector3(-500f, 500f, -500f);
		rb.velocity = (transform.forward * (mainEnginePower * mainEnginePower));
	}
}
