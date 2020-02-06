using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
	public GameObject skillPanel;
	private List<Spacecraft> spacecraftList;

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

	public void SetAggressionStance(bool bHostile)
	{

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
