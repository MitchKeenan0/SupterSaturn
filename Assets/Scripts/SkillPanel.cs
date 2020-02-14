using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
	public GameObject abilityPanelPrefab;
	public GameObject abilityPanel;
	public GameObject skillPanel;
	public Image offenseImage;
	public Text offenseText;

	private List<Ability> myAbilityList;
	private List<Spacecraft> spacecraftList;
	private bool offense = false;

    void Start()
    {
		myAbilityList = new List<Ability>();
		spacecraftList = new List<Spacecraft>();
		skillPanel.SetActive(false);
    }

	void CreateAbilityPanel(Ability ability)
	{
		myAbilityList.Add(ability);
		GameObject abPanel = Instantiate(abilityPanelPrefab, abilityPanel.transform);
	}

	public void SetSpacecraftList(List<Spacecraft> spl)
	{
		spacecraftList = spl;
		skillPanel.SetActive(spacecraftList.Count > 0);

		// init abilities
		foreach (Spacecraft sp in spacecraftList)
		{
			Ability[] spAbilities = sp.GetComponentsInChildren<Ability>();
			int numAbilities = spAbilities.Length;
			if (spAbilities.Length > 0)
			{
				if (myAbilityList.Count > 0)
				{
					for(int i = 0; i < numAbilities; i++)
					{
						Ability ability = spAbilities[i];
						bool bNew = true;
						foreach(Ability a in myAbilityList)
						{
							if (a.GetType() == ability.GetType())
							{
								bNew = false;
								break;
							}
						}
						if (bNew)
							CreateAbilityPanel(ability);
					}
				}
				else
				{
					foreach (Ability ab in spAbilities)
						CreateAbilityPanel(ab);
				}
			}
		}
	}

	public void ActivateAbility(int abilityIndex)
	{
		if (myAbilityList[abilityIndex] != null)
			myAbilityList[abilityIndex].StartAbility();
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
