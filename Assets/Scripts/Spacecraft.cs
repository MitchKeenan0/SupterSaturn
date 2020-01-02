using System.Collections;
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

	private Rigidbody rb;
	private Agent agent;
	private Health health;
	private SpacecraftInformation spacecraftInformation;
	private CameraController cameraController;
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

	public Agent GetAgent() { return agent; }

	public bool IsAlive() { return health.GetHealth() >= 1; }

	public int GetHealthPercent() { return health.GetHealth() / health.maxHealth; }

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
		gridObject = GetComponentInChildren<GridVisualizer>().gameObject;
	}

	void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
		SetAgentEnabled(bAgentStartsEnabled);
		turningVector = transform.position + transform.forward;
		turningRotation = transform.rotation;
		if (agent.teamID != 0)
			SetRenderComponents(false);
	}

	void FixedUpdate()
	{
		UpdateSpacecraftPhysics();
	}

	void UpdateSpacecraftPhysics()
	{
		if (IsAlive())
		{
			if (turningRotation.normalized != Quaternion.identity)
			{
				rb.MoveRotation(turningRotation);
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
		agent.SetEnabled(value);
	}

	public void MainEngines(float driveDirection)
	{
		mainEnginesVector = transform.forward * driveDirection * mainEnginePower;
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
			Quaternion toRotation = Quaternion.LookRotation(turningVector, Vector3.up);
			turningRotation = Quaternion.Lerp(transform.rotation, toRotation, turningPower * Time.deltaTime);
		}
	}

	public void SpacecraftDestroyed(Transform responsibleTransform)
	{
		agent.AgentSpacecraftDestroyed();
		agent.SetEnabled(false);

		Transform destroyedTransform = Instantiate(destroyedParticlesPrefab, transform.position, Quaternion.Euler(rb.velocity));
		Destroy(destroyedTransform.gameObject, 10f);

		if (responsibleTransform != null)
		{
			Agent killerAgent = responsibleTransform.GetComponent<Agent>();
			if (killerAgent != null)
			{
				killerAgent.NotifyTargetDestroyed(this);
			}
		}

		Destroy(gameObject, 0.15f);
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
			gridObject.SetActive(value);
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
		cameraController.SetOrbitTarget(transform);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Gravity>())
		{
			if (!health)
				Debug.Log("No health");
			if (!collision.gameObject.transform)
				Debug.Log("No transform");
			health.ModifyHealth(-99, collision.gameObject.transform);
		}
		else if (collision.gameObject.GetComponent<Spacecraft>())
		{
			health.ModifyHealth(-5, collision.gameObject.transform);
		}
	}
}
