using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFleetHUD : MonoBehaviour
{
	public GameObject diagramPanel;
	public GameObject diagramPrefab;
	public GameObject highlightPrefab;

	private CameraController cameraController;
	private ObjectManager objectManager;
	private SpacecraftInformation selectedSpacecraftInformation;
	private List<Spacecraft> spacecraftList;
	private List<Spacecraft> teamList;
	private List<Spacecraft> enemyList;
	private List<Image> teamImageList;
	private List<Button> teamButtonList;

	void Start()
    {
		spacecraftList = new List<Spacecraft>();
		cameraController = FindObjectOfType<CameraController>();
		objectManager = FindObjectOfType<ObjectManager>();
		InitTeamFleet();
    }

	// Internal functions
	void InitTeamFleet()
	{
		teamList = new List<Spacecraft>();
		teamImageList = new List<Image>();
		teamButtonList = new List<Button>();

		// team
		spacecraftList = objectManager.GetSpacecraftList();
		enemyList = new List<Spacecraft>();
		foreach (Spacecraft sp in spacecraftList)
		{
			if ((sp != null) && (sp.GetAgent() != null))
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
			if (!img.gameObject.CompareTag("Selection"))
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
		// mouse selection.add

		// if double click
		//if (teamList[index] != null)
		//{
		//	cameraController.SetOrbitTarget(teamList[index].transform);
		//}
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
