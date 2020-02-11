using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
	public GameObject skillPanel;
	public Image offenseImage;
	public Text offenseText;

	private List<Spacecraft> spacecraftList;
	private bool offense = false;

    void Start()
    {
		spacecraftList = new List<Spacecraft>();
		skillPanel.SetActive(false);
    }

	public void SetSpacecraft(List<Spacecraft> spl)
	{
		spacecraftList = spl;
		skillPanel.SetActive(spacecraftList.Count > 0);
	}

	public void StopSpacecraft()
	{
		foreach (Spacecraft sp in spacecraftList)
			sp.GetAgent().EnableMoveCommand(false);
	}

	public void SetOffense()
	{
		offense = !offense;
		if (offense)
		{
			offenseImage.color = Color.red;
			offenseText.color = Color.red;
			offenseText.text = "Auto-Target";
		}
		else
		{
			offenseImage.color = Color.white;
			offenseText.color = Color.white;
			offenseText.text = "Manual";
		}
		foreach (Spacecraft sp in spacecraftList)
			sp.GetAgent().SetOffense(offense);
	}

	public void Regroup()
	{
		foreach (Spacecraft sp in spacecraftList)
		{
			if ((sp.GetAgent() != null) && (sp.GetAgent().teamID == 0))
				sp.GetAgent().Regroup();
		}
	}
}
