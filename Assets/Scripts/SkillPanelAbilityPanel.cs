using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelAbilityPanel : MonoBehaviour
{
	public Image abilityImage;
	public Text abilityText;
	public Button loopButton;

	private SkillPanel skillPanel;
	private Ability ability;
	private InputController inputController;
	private Button myButton;
	private Image backgroundImage;
	private Rotator loopRotator;
	private bool bLooping = false;

	void Awake()
    {
		skillPanel = GetComponentInParent<SkillPanel>();
		myButton = GetComponent<Button>();
		backgroundImage = GetComponent<Image>();
		inputController = FindObjectOfType<InputController>();
		inputController.ActivateButton(loopButton, false, true);
		loopRotator = loopButton.gameObject.GetComponent<Rotator>();
		loopRotator.speedMultiplier = 0f;
    }

	public void SetAbility(Ability ab)
	{
		ability = ab;
		abilityImage.sprite = ab.abilitySprite;
		if (backgroundImage != null)
			backgroundImage.sprite = ab.abilitySprite;
		abilityText.text = ab.abilityName;
		ab.BondToButton(myButton);
	}

	public void Activate()
	{
		int siblingIndex = transform.GetSiblingIndex();
		if (skillPanel != null)
			skillPanel.ActivateAbility(siblingIndex);
	}

	public void Loop()
	{
		bool bTurningOff = bLooping;
		bLooping = !bLooping;
		if (ability != null)
			ability.ToggleLooping();
		loopRotator.speedMultiplier = 1f;
		inputController.ActivateButton(loopButton, bLooping, bTurningOff);
	}
}
