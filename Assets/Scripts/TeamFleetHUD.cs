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
	private List<Image> teamImageList;
	private List<Button> teamButtonList;
	private float timeOfLastButtonPress = 0f;

	private IEnumerator loadWaitCoroutine;

	void Awake()
	{
		spacecraftList = new List<Spacecraft>();
		teamList = new List<Spacecraft>();
		enemyList = new List<Spacecraft>();
	}

	void Start()
    {
		cameraController = FindObjectOfType<CameraController>();
		objectManager = FindObjectOfType<ObjectManager>();
		mouseSelection = FindObjectOfType<MouseSelection>();

		loadWaitCoroutine = LoadWait(0.1f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitTeamFleet();
	}

	// Internal functions
	void InitTeamFleet()
	{
		teamImageList = new List<Image>();
		teamButtonList = new List<Button>();

		// team
		spacecraftList = objectManager.GetSpacecraftList();
		foreach (Spacecraft sp in spacecraftList)
		{
			if ((sp != null) && (sp.GetAgent() != null) && !sp.CompareTag("Orbiter"))
			{
				if (sp.GetAgent().teamID == 0)
					teamList.Add(sp);
				else
					enemyList.Add(sp);
			}
		}

		// diagrams
		Image[] images = GetComponentsInChildren<Image>();
		foreach (Image img in images)
		{
			if (!img.gameObject.CompareTag("Selection")
				&& !img.gameObject.CompareTag("Health"))
			{
				teamImageList.Add(img);
				img.gameObject.SetActive(false);
			}
		}
		if (teamImageList.Count < teamList.Count)
		{
			int numToCreate = teamList.Count - teamImageList.Count;
			for (int i = 0; i < numToCreate; i++)
			{
				CreateSpacecraftDiagram();
			}
		}

		// bond diagrams to team
		int numTeam = teamList.Count;
		for (int i = 0; i < numTeam; i++)
		{
			Spacecraft sp = teamList[i];
			if (i < teamImageList.Count)
			{
				Image img = teamImageList[i];
				img.sprite = sp.craftDiagram;
				img.gameObject.SetActive(true);
			}
		}

		// interactivity
		Button[] buttons = GetComponentsInChildren<Button>();
		foreach (Button b in buttons)
		{
			teamButtonList.Add(b);
		}

		GameObject panelObject = diagramPanel.GetComponent<HorizontalLayoutGroup>().gameObject;
		RectTransform rt = panelObject.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2((teamButtonList.Count * 100f), rt.sizeDelta.y);
	}

	GameObject CreateSpacecraftDiagram()
	{
		GameObject product = null;
		if (diagramPrefab != null)
		{
			product = Instantiate(diagramPrefab, diagramPanel.transform);
			teamImageList.Add(product.GetComponent<Image>());
			Button b = product.GetComponent<Button>();
			if ((b != null) && !teamButtonList.Contains(b))
				teamButtonList.Add(b);
			product.SetActive(false);
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

	public void SetHealthBarValue(Spacecraft sp, float value)
	{
		if (teamList.Contains(sp))
		{
			if (teamImageList[teamList.IndexOf(sp)] != null)
			{
				Image spDiagram = teamImageList[teamList.IndexOf(sp)];
				var healthBar = spDiagram.transform.Find("HealthBar");
				if ((healthBar != null) && healthBar.CompareTag("Health"))
				{
					Vector2 healthBarr = healthBarScale;
					healthBarr.x *= sp.GetHealthPercent();
					healthBar.GetComponent<Image>().rectTransform.sizeDelta = healthBarr;
				}
			}
		}
	}

	public void Highlight(Spacecraft sp)
	{
		if (teamList.Contains(sp))
		{
			Image img = sp.GetHUDIcon().GetComponent<Image>();
			if (img != null)
			{
				img.color = Color.green;
			}
		}
	}

	public void Unhighlight(Spacecraft sp)
	{
		if (teamList.Contains(sp))
		{
			Image img = sp.GetHUDIcon().GetComponent<Image>();
			if (img != null)
			{
				img.color = Color.white;
			}
		}
	}

	public void ButtonHovered(bool value, int index)
	{
		if (teamList[index] != null)
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
			Image img = teamImageList[index].gameObject.GetComponent<Image>();
			if (img != null)
			{
				Color newColor = Color.red;
				img.color = newColor;
			}
		}
	}
}
