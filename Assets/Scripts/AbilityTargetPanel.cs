using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetPanel : MonoBehaviour
{
	private AbilityTargetingHUD abilityTargeting;
	private Transform targetTransform;
	private CanvasGroup canvasGroup;

    void Awake()
    {
		canvasGroup = GetComponent<CanvasGroup>();
		abilityTargeting = FindObjectOfType<AbilityTargetingHUD>();
    }

	public void LoadTarget(Transform value)
	{
		targetTransform = value;
	}

	public void TargetSelected()
	{
		abilityTargeting.SelectTarget(targetTransform);
	}

	public void SetEnabled(bool value)
	{
		canvasGroup.alpha = (value ? 1f : 0f);
		canvasGroup.blocksRaycasts = value;
		canvasGroup.interactable = value;
	}
}
