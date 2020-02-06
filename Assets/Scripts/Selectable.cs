using UnityEngine;

public class Selectable : MonoBehaviour
{
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

			Renderer[] rendererArray = GetComponentsInChildren<Renderer>();
			int numRenders = rendererArray.Length;
			if (rendererArray.Length > 0)
			{
				for (int i = 0; i < numRenders; i++)
				{
					Renderer r = rendererArray[i];
					if ((r != null) && (r.CompareTag("Selection")))
						r.material.color = value ? Color.green : Color.white;
				}
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
