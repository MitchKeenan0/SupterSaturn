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

    void Start()
    {
		myAbilityList = new List<Ability>();
		spacecraftList = new List<Spacecraft>();
    }

	void CreateAbilityPanel(Ability ability)
	{
		myAbilityList.Add(ability);
		GameObject abPanel = Instantiate(abilityPanelPrefab, abilityPanel.transform);
		SkillPanelAbilityPanel spap = abPanel.GetComponent<SkillPanelAbilityPanel>();
		if (spap != null)
		{
			spap.SetAbility(ability);
		}
	}

	public void InitAbilities(List<Spacecraft> spl)
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

	public void DeactivateAbility(int abilityIndex)
	{
		if (myAbilityList[abilityIndex] != null)
			myAbilityList[abilityIndex].CancelAbility();
	}

	public void CancelAll()
	{
		foreach (Ability ab in myAbilityList)
			ab.CancelAbility();
	}
}
