using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
	public GameObject startEffectPrefab;
	public Button loopToggleButton;
	public float warmup = 0.1f;
	public float duration = 0.5f;
	public float cooldown = 1f;
	public Sprite abilitySprite;
	public string abilityName = "";
	public bool bTargeting = false;

	private IEnumerator warmupCoroutine;
	private IEnumerator durationCoroutine;
	private IEnumerator cooldownCoroutine;
	private float activeTime = 0f;
	private bool bActive = false;
	private bool bCooling = false;
	private bool bLooping = false;
	private Button myButton;

	public bool IsActive() { return bActive; }
	public bool IsCooling() { return bCooling; }
	public GameObject GetUIObject() { return myButton.gameObject; }

	public void BondToButton(Button button)
	{
		myButton = button;
	}

	public virtual void StartAbility()
	{
		if (!bCooling && !bActive)
		{
			activeTime = 0f;
			warmupCoroutine = AbilityWarmup(warmup);
			StartCoroutine(warmupCoroutine);
			if (myButton != null)
				myButton.interactable = false;
		}
	}

	public virtual void UpdateAbility()
	{

	}

	public virtual void EndAbility()
	{

	}

	private IEnumerator AbilityWarmup(float warmTime)
	{
		yield return new WaitForSeconds(warmTime);
		FireAbility();
	}

	public virtual void FireAbility()
	{
		bActive = true;
		durationCoroutine = AbilityDuration(Time.deltaTime);
		StartCoroutine(durationCoroutine);

		SkillPanelAbilityPanel spap = GetUIObject().GetComponent<SkillPanelAbilityPanel>();
		if (spap != null)
		{
			spap.BeginCooldown();
		}
	}

	private IEnumerator AbilityDuration(float updateInterval)
	{
		while(activeTime <= duration)
		{
			activeTime += updateInterval;
			UpdateAbility();
			yield return new WaitForSeconds(updateInterval);
		}

		EndAbility();
		bActive = false;
		cooldownCoroutine = AbilityCooldown(cooldown);
		StartCoroutine(cooldownCoroutine);
	}

	private IEnumerator AbilityCooldown(float coolTime)
	{
		bCooling = true;
		yield return new WaitForSeconds(coolTime);
		bCooling = false;
		if (myButton != null)
			myButton.interactable = true;
		if (bLooping)
			StartAbility();
	}

	public void ToggleLooping()
	{
		bLooping = !bLooping;
	}
}
