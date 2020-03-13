using UnityEngine;

public class Selectable : MonoBehaviour
{
	private CircleRenderer circle;
	private SpacecraftController spacecraftController;
	private OrbitController orbitController;

	void Awake()
	{
		circle = gameObject.GetComponentInChildren<CircleRenderer>();
		orbitController = FindObjectOfType<OrbitController>();
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
			if (sp != null)
			{
				sp.SelectionHighlight(isSelected);
			}

			if (_isSelected && (gameObject.GetComponentInChildren<SpacecraftController>() != null))
				orbitController.SetAutopilot(gameObject.GetComponent<Autopilot>());

			if (!circle)
				circle = gameObject.GetComponent<CircleRenderer>();
			if (circle != null)
			{
				spacecraftController = gameObject.GetComponentInChildren<SpacecraftController>();
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
