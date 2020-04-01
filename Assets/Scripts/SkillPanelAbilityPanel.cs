using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelAbilityPanel : MonoBehaviour
{
	public Image abilityImage;
	public Text abilityText;

	private SkillPanel skillPanel;
	private Button myButton;
	private Image backgroundImage;

	void Awake()
    {
		skillPanel = GetComponentInParent<SkillPanel>();
		myButton = GetComponent<Button>();
		backgroundImage = GetComponent<Image>();
    }

	public void SetAbility(Ability ability)
	{
		abilityImage.sprite = ability.abilitySprite;
		if (backgroundImage != null)
			backgroundImage.sprite = ability.abilitySprite;
		abilityText.text = ability.abilityName;
		ability.BondToButton(myButton);
	}

	public void Activate()
	{
		int siblingIndex = transform.GetSiblingIndex();
		if (skillPanel != null)
			skillPanel.ActivateAbility(siblingIndex);
	}
}
