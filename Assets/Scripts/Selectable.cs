using UnityEngine;

public class Selectable : MonoBehaviour
{
	private CircleRenderer circle;

	void Awake()
	{
		circle = gameObject.GetComponentInChildren<CircleRenderer>();
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
