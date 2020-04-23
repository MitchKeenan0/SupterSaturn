using UnityEngine;
using System.Collections.Generic;

public class Selectable : MonoBehaviour
{
	private CircleRenderer circle;
	private SpacecraftController spacecraftController;
	private OrbitController orbitController;
	private SkillPanel skillPanel;

	void Awake()
	{
		circle = gameObject.GetComponentInChildren<CircleRenderer>();
		orbitController = FindObjectOfType<OrbitController>();
		skillPanel = FindObjectOfType<SkillPanel>();
	}

	internal bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;

			Spacecraft sp = GetComponent<Spacecraft>();
			if (sp != null && (!GetComponent<NPCSpacecraft>()))
			{
				sp.SelectionHighlight(isSelected);
				if (_isSelected)
				{
					List<Spacecraft> spList = new List<Spacecraft>();
					spList.Add(sp);
					if (!skillPanel)
						skillPanel = FindObjectOfType<SkillPanel>();
					skillPanel.InitAbilities(spList);
				}
			}

			spacecraftController = gameObject.GetComponentInChildren<SpacecraftController>();
			if (_isSelected && (spacecraftController != null))
			{
				if (spacecraftController.GetAutopilot() != null)
					orbitController.SetAutopilot(spacecraftController.GetAutopilot());
				else
					Debug.Log("no autopilot");
			}
			else
			{
				Debug.Log("no controller");
			}

			if (!circle)
				circle = gameObject.GetComponent<CircleRenderer>();
			if (circle != null)
			{
				if (value)
				{
					circle.Open();
					circle.SetPosition(transform.position);
					circle.StartAutoUpdate();
				}
				else
				{
					circle.Close();
					if (spacecraftController != null)
						spacecraftController.SetActive(false);
				}
			}
		}
	}

	public void SetColor(Color value)
	{
		Renderer[] rendererArray = GetComponentsInChildren<Renderer>();
		int numRenders = rendererArray.Length;
		if (rendererArray.Length > 0)
		{
			for (int i = 0; i < numRenders; i++)
			{
				Renderer r = rendererArray[i];
				if ((r != null) && (r.CompareTag("Selection")))
					r.material.color = value;
			}
		}
	}

	private bool _isSelected;

	void OnEnable()
	{
		MouseSelection.selectables.Add(this);
	}

	void OnDisable()
	{
		MouseSelection.selectables.Remove(this);
	}

}
