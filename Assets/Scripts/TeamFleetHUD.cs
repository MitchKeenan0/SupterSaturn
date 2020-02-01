using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFleetHUD : MonoBehaviour
{
	public GameObject diagramPanel;
	public GameObject diagramPrefab;
	public Vector3 healthBarScale;

	private CameraController cameraController;
	private ObjectManager objectManager;
	private MouseSelection mouseSelection;
	private SpacecraftInformation selectedSpacecraftInformation;
	private List<Spacecraft> spacecraftList;
	private List<Spacecraft> teamList;
	private List<Spacecraft> enemyList;
	private List<TeamFleetDiagram> teamDiagramList;
	private List<Button> teamButtonList;
	private float timeOfLastButtonPress = 0f;

	private IEnumerator loadWaitCoroutine;
	private IEnumerator updateCoroutine;

	void Awake()
	{
		spacecraftList = new List<Spacecraft>();
		teamList = new List<Spacecraft>();
		enemyList = new List<Spacecraft>();
		teamDiagramList = new List<TeamFleetDiagram>();
		teamButtonList = new List<Button>();
	}

	void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
		objectManager = FindObjectOfType<ObjectManager>();
		mouseSelection = FindObjectOfType<MouseSelection>();

		loadWaitCoroutine = LoadWait(0.4f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitTeamFleet();
		updateCoroutine = UpdateTimer(0.1f);
		StartCoroutine(updateCoroutine);
	}

	private IEnumerator UpdateTimer(float intervalTime)
	{
		while(true)
		{
			yield return new WaitForSeconds(intervalTime);
			UpdateTeamFleetHud();
		}
	}

	void UpdateTeamFleetHud()
	{
		int numDiagrams = teamDiagramList.Count;
		if (numDiagrams > 0)
		{
			for(int i = 0; i < numDiagrams; i++)
			{
				if (i < teamList.Count)
				{
					Spacecraft sp = teamList[i];
					TeamFleetDiagram tfd = teamDiagramList[i];
					tfd.SetClass(sp.spacecraftName);
					tfd.SetName("");
					float speed = 100 * sp.GetComponent<Rigidbody>().velocity.magnitude;
					string speedText = speed.ToString("F1") + " Km/sec";
					tfd.SetSpeed(speedText);
					/*
						public Text classText;
						public Text nameText;
						public Text speedText;
						public Image gravityIcon;
						public Image violenceIcon; 
					 */
				}
			}
		}
	}

	// Internal functions
	void AddTeam(Spacecraft sp)
	{
		if (!teamList.Contains(sp))
			teamList.Add(sp);
	}

	void AddEnemy(Spacecraft sp)
	{
		if (!enemyList.Contains(sp))
			enemyList.Add(sp);
	}

	void InitTeamFleet()
	{
		// team
		spacecraftList = new List<Spacecraft>(objectManager.GetSpacecraftList());
		int numSpacecraft = spacecraftList.Count;
		Debug.Log("team fleet reading list " + numSpacecraft);

		for (int i = 0; i < numSpacecraft; i++)
		{
			Spacecraft sp = spacecraftList[i];
			if (sp != null)
			{
				if (sp.GetAgent().teamID == 0)
					AddTeam(sp);
				else
					AddEnemy(sp);
			}
		}

		// diagrams
		int numToCreate = teamList.Count;
		for (int i = 0; i < numToCreate; i++)
		{
			CreateSpacecraftDiagram();
		}

		// bond diagrams to team
		int numTeam = teamList.Count;
		for (int i = 0; i < numTeam; i++)
		{
			Spacecraft sp = teamList[i];
			if (i < teamDiagramList.Count)
			{
				Image img = teamDiagramList[i].GetComponentInChildren<Image>();
				if ((img != null) && (sp.craftDiagram != null))
				{
					img.sprite = sp.craftDiagram;
					img.gameObject.SetActive(true);
				}

				SetHealthBarValue(sp);
			}
		}

		// interactivity
		Button[] buttons = GetComponentsInChildren<Button>();
		foreach (Button b in buttons)
		{
			teamButtonList.Add(b);
		}

		// scale panel
		RectTransform rt = diagramPanel.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2((teamButtonList.Count * 150f), rt.sizeDelta.y);
	}

	GameObject CreateSpacecraftDiagram()
	{
		GameObject product = null;
		if (diagramPrefab != null)
		{
			product = Instantiate(diagramPrefab, diagramPanel.transform);
			product.transform.SetParent(diagramPanel.transform);
			TeamFleetDiagram diagram = product.GetComponent<TeamFleetDiagram>();
			teamDiagramList.Add(diagram);
			Button b = product.GetComponent<Button>();
			if ((b != null) && !teamButtonList.Contains(b))
				teamButtonList.Add(b);
			Debug.Log("Spawned diagram");
		}
		return product;
	}

	// External functions
	public List<Spacecraft> GetTeamList(int teamID)
	{
		List<Spacecraft> list = new List<Spacecraft>();
		foreach(Spacecraft sp in spacecraftList)
		{
			if (sp.GetAgent().teamID == teamID)
				list.Add(sp);
		}

		return list;
	}

	public List<Spacecraft> GetEnemyList()
	{
		return enemyList;
	}

	public void SetHealthBarValue(Spacecraft sp)
	{
		if (teamList == null)
			teamList = new List<Spacecraft>();
		if (sp != null)
		{
			AddTeam(sp);
			if (teamList.Contains(sp))
			{
				int teamIndex = teamList.IndexOf(sp);
				if (teamIndex < teamDiagramList.Count)
				{
					HealthBar healthBar = teamDiagramList[teamIndex].gameObject.GetComponentInChildren<HealthBar>();
					if (healthBar != null)
					{
						Health health = sp.GetComponent<Health>();
						healthBar.InitHeath(health.maxHealth, health.GetHealth());
						Debug.Log("fleet hud set health bar");
					}
				}
			}
		}
	}

	public void Highlight(Spacecraft sp)
	{
		if (teamList.Contains(sp))
		{
			if (sp.GetHUDIcon() != null)
			{
				Image img = sp.GetHUDIcon().GetComponent<Image>();
				if (img != null)
				{
					img.color = Color.green;
				}
			}
		}
	}

	public void Unhighlight(Spacecraft sp)
	{
		if (teamList.Contains(sp))
		{
			if (sp.GetHUDIcon() != null)
			{
				Image img = sp.GetHUDIcon().GetComponent<Image>();
				if (img != null)
				{
					img.color = Color.white;
				}
			}
		}
	}

	public void ButtonHovered(bool value, int index)
	{
		if (index < teamList.Count)
		{
			cameraController.SetMouseContext(value, teamList[index]);
		}
	}

	public void ButtonPressed(int index)
	{
		if (Time.time - timeOfLastButtonPress >= 1f)
			mouseSelection.UpdateSelection(teamList[index].GetComponent<Selectable>(), true);
		else
			cameraController.SetOrbitTarget(teamList[index].transform);
		timeOfLastButtonPress = Time.time;
	}

	public void SpacecraftDestroyed(Spacecraft sp)
	{
		if (teamList.Contains(sp))
		{
			int index = teamList.IndexOf(sp);
			if (index < teamDiagramList.Count)
			{
				Image img = teamDiagramList[index].gameObject.GetComponent<Image>();
				if (img != null)
				{
					Color newColor = Color.red;
					img.color = newColor;
				}
			}
		}
	}
}
