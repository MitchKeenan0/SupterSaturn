using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
	public int teamID = 0;
	public float preferredEngagementDistance = 10f;

	private Spacecraft spacecraft;
	private Autopilot autopilot;
	private FireCoordinator fireCoordinator;
	private Scanner scanner;
	private ObjectManager objectManager;
	private RaycastManager raycastManager;
	private TeamFleetHUD teamFleetHUD;
	private Transform targetTransform;
	private Transform followTransform;
	private List<Spacecraft> spacecraftList;
	private List<SpacecraftInformation> targetList;
	private bool bEnabled = false;
	private IEnumerator targetDestroyedRestCoroutine;

    void Awake()
    {
		spacecraft = GetComponent<Spacecraft>();
		autopilot = GetComponent<Autopilot>();
		fireCoordinator = GetComponent<FireCoordinator>();
		scanner = GetComponentInChildren<Scanner>();
		objectManager = FindObjectOfType<ObjectManager>();
		raycastManager = FindObjectOfType<RaycastManager>();
		teamFleetHUD = FindObjectOfType<TeamFleetHUD>();
		spacecraftList = new List<Spacecraft>();
		targetList = new List<SpacecraftInformation>();
	}

	void Start()
	{
		SetMoveOrder(transform.position, null);
	}

	void Update()
    {
		UpdateTarget();
		autopilot.SpacecraftNavigationCommands();
    }

	bool LineOfSight(Vector3 target, Transform targetTrans)
	{
		bool result = false;
		RaycastHit hit = raycastManager.CustomRaycast(transform.position, target - transform.position);
		if (hit.transform == targetTrans)
		{
			result = true;
		}
		return result;
	}

	void AquireTarget()
	{
		GameObject[] scannerTargets = scanner.GetTargets();
		if (scannerTargets.Length > 0)
		{
			GameObject ga = scannerTargets[0];
			if (ga != null)
			{
				// tbd target selection
				if (LineOfSight(ga.transform.position, ga.transform))
				{
					SetTarget(ga.transform);
					NotifyTeamOfTarget(ga.transform);
				}
			}
		}
	}

	void UpdateTarget()
	{
		if (bEnabled)
		{
			if (targetTransform != null)
			{
				Spacecraft targetSpacecraft = targetTransform.GetComponent<Spacecraft>();
				if (targetSpacecraft.GetMarks() > 0)
				{
					if (LineOfSight(targetTransform.position, targetTransform))
						fireCoordinator.UpdateFireCoordinator();
				}
				else
				{
					targetTransform = null;
					AquireTarget();
				}
			}
			else
			{
				AquireTarget();
			}
		}
	}

	void SetTarget(Transform value)
	{
		targetTransform = value;
		autopilot.SetTarget(value);
		targetList.Add(value.gameObject.GetComponent<SpacecraftInformation>());
	}

	void NotifyTeamOfTarget(Transform value)
	{
		int targetTeamID = value.gameObject.GetComponent<Spacecraft>().GetAgent().teamID;
		List<Spacecraft> teamSpacecraftList = teamFleetHUD.GetTeamList(teamID);
		foreach(Spacecraft sp in teamSpacecraftList)
		{
			if ((sp != null) && (teamID != targetTeamID))
			{
				if (LineOfSight(sp.transform.position, sp.transform))
				{
					sp.GetAgent().TargetSuggestion(value);
				}
			}
		}
	}

	// External functions
	public void SetMoveOrder(Vector3 position, Transform follow)
	{
		autopilot.SetMoveCommand(position);
		if (follow != null)
		{
			followTransform = follow;
			autopilot.SetFollowTransform(followTransform);
		}
	}

	public string GetHeadline()
	{
		string headline = "";

		if (targetTransform != null)
			headline = "Targeting " + targetTransform.name;

		else if (spacecraft.GetHealthPercent() > 1f)
			headline = "Damaged";

		return headline;
	}

	public void TargetSuggestion(Transform value)
	{
		SetTarget(value);
	}

	public Transform GetTargetTransform()
	{
		return targetTransform;
	}
	
	public void NotifyTargetDestroyed()
	{
		targetTransform = null;
		autopilot.SetTarget(null);
		fireCoordinator.StandDown();
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
		bEnabled = true;
		spacecraft.SetRenderComponents(value);
	}

	public void AgentSpacecraftDestroyed()
	{
		teamFleetHUD.SpacecraftDestroyed(spacecraft);
		objectManager.SpacecraftDestroyed(spacecraft);
	}
}
