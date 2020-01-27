using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public GameObject healthBar;
	public GameObject healthUnitPrefab;
	public Text maxHealthText;
	public float unitPadding = 2f;
	public int maxUnitSize = 35;

	private RectTransform rt;
	private List<GameObject> healthUnitList;
	private int barSize;
	private int maxHealth;
	private int unitSize;
	private int currentHealth;

    void Awake()
    {
		rt = GetComponent<RectTransform>();
		barSize = Mathf.FloorToInt(rt.sizeDelta.x);
		healthUnitList = new List<GameObject>();
    }

	public void InitHeath(int maxHp, int currentHp)
	{
		Clear();
		maxHealth = maxHp;
		currentHealth = currentHp;
		if (currentHealth == 0)
		{
			currentHealth = maxHealth;
		}

		if (maxHealth != -1)
		{
			healthBar.SetActive(true);
			maxHealthText.text = maxHp.ToString();

			int spacedBarSize = barSize - (2 * maxHp) + 4; /// the 4 is unit spacing x2 for the sides
			unitSize = Mathf.Clamp(spacedBarSize / maxHp, 1, maxUnitSize);

			for (int i = 0; i < currentHp; i++)
			{
				if (healthUnitList.Count < maxHealth)
					CreateHealthUnit(unitSize);
			}
		}
		else
		{
			healthBar.SetActive(false);
			maxHealthText.text = "";
		}
	}

	void Clear()
	{
		int numUnits = healthUnitList.Count;
		for(int i = 0; i < numUnits; i++)
		{
			healthUnitList[i].SetActive(false);
		}
		healthUnitList.Clear();
	}

	void CreateHealthUnit(float width)
	{
		GameObject unit = Instantiate(healthUnitPrefab, healthBar.transform);
		Vector2 unitScale = unit.GetComponent<RectTransform>().sizeDelta;
		unitScale.x = width;
		unit.GetComponent<RectTransform>().sizeDelta = unitScale;
		healthUnitList.Add(unit);
	}
}
