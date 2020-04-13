using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelAbilityPanel : MonoBehaviour
{
	public Image abilityImage;
	public Text abilityText;
	public Button loopButton;
	public RectTransform cooldownPanel;
	public RectTransform cooldownBar;
	public RectTransform chargePanel;
	public RectTransform chargeBar;
	public Text chargeText;
	public float cooldownUpdateInterval = 0.3f;

	private SkillPanel skillPanel;
	private Ability ability;
	private InputController inputController;
	private Button myButton;
	private Image backgroundImage;
	private Rotator loopRotator;
	private CanvasGroup cooldownCanvasGroup;
	private CanvasGroup chargeCanvasGroup;
	private bool bLooping = false;
	private bool bCooldownFinished = false;
	private float cooldownBarMaxSize = 0f;
	private float cooldownBarHeight = 1f;
	private float chargeBarMaxSize = 0f;
	private float chargeBarHeight = 1f;
	private IEnumerator cooldownCoroutine;

	void Awake()
    {
		skillPanel = GetComponentInParent<SkillPanel>();
		myButton = GetComponent<Button>();
		backgroundImage = GetComponent<Image>();
		inputController = FindObjectOfType<InputController>();
		inputController.ActivateButton(loopButton, false, true);
		loopRotator = loopButton.gameObject.GetComponent<Rotator>();
		loopRotator.speedMultiplier = 0f;

		cooldownCanvasGroup = cooldownPanel.GetComponent<CanvasGroup>();
		cooldownCanvasGroup.alpha = 0f;
		cooldownBarMaxSize = cooldownPanel.sizeDelta.x;
		cooldownBarHeight = cooldownBar.sizeDelta.y;
		cooldownBar.sizeDelta = new Vector2(0f, cooldownBarHeight);

		chargeCanvasGroup = chargePanel.GetComponent<CanvasGroup>();
		chargeCanvasGroup.alpha = 0f;
		chargeBarMaxSize = chargePanel.sizeDelta.x;
		chargeBarHeight = chargeBar.sizeDelta.y;
		chargeBar.sizeDelta = new Vector2(0f, cooldownBarHeight);
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

	public void BeginCooldown()
	{
		cooldownCoroutine = Cooldown(cooldownUpdateInterval);
		StartCoroutine(cooldownCoroutine);
	}

	public void SetUIChargePercent(float percent)
	{
		float percentium = Mathf.Clamp(percent, 0f, 1f);
		float chargeBarSizeX = Mathf.Clamp((chargeBarMaxSize * percent), 0f, chargeBarMaxSize);
		chargeBar.sizeDelta = new Vector2(chargeBarSizeX, chargeBarHeight);
		chargeCanvasGroup.alpha = (percent > 0f) ? 1f : 0f;
		chargeText.text = (percent * 100f).ToString("F0") + "%";
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

	private IEnumerator Cooldown(float interval)
	{
		float duration = (ability.duration + ability.cooldown);
		float time = 0f;
		cooldownCanvasGroup.alpha = 1f;
		bCooldownFinished = false;
		cooldownBar.GetComponent<Image>().color = Color.grey;

		while (time < duration)
		{
			yield return new WaitForSeconds(interval);
			time = Mathf.Clamp(time + interval, 0f, duration);
			float percentOfDuration = time / duration;
			float barLength = cooldownBarMaxSize * percentOfDuration;
			cooldownBar.sizeDelta = new Vector2(barLength, cooldownBarHeight);
			if ((time == duration) && !bCooldownFinished)
			{
				bCooldownFinished = true;
				cooldownBar.GetComponent<Image>().color = Color.white;
			}
		}

		yield return new WaitForSeconds(interval);
		cooldownBar.sizeDelta = new Vector2(0f, cooldownBarHeight);
		cooldownCanvasGroup.alpha = 0f;
	}
}
