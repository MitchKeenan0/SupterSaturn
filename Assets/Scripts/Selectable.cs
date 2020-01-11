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

			Renderer r = GetComponentInChildren<Renderer>();
			if (r != null)
				r.material.color = value ? Color.red : Color.blue;
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
