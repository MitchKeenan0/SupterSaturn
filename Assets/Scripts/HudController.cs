using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
	private InputController ic;
	private SkillPanel sp;
	private NavigationHud nv;

    void Awake()
    {
		ic = FindObjectOfType<InputController>();
		sp = FindObjectOfType<SkillPanel>();
    }

	public void SetHudsEnabled(bool value)
	{
		float a = value ? 1f : 0f;
		ic.GetComponent<CanvasGroup>().alpha = a;
		sp.GetComponent<CanvasGroup>().alpha = a;

		ic.GetComponent<CanvasGroup>().blocksRaycasts = value;
		sp.GetComponent<CanvasGroup>().blocksRaycasts = value;
	}

	public void SetCanvasGroupEnabled(CanvasGroup cg, bool value)
	{
		cg.alpha = value ? 1f : 0f;
		cg.blocksRaycasts = value;
		cg.interactable = false;
	}
}
