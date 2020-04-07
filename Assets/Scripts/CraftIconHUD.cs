using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftIconHUD : MonoBehaviour
{
	public GameObject iconParent;
	public GameObject iconPrefab;
	public Vector3 healthBarScale;
	public float updateInterval = 0.2f;

	private Camera cameraMain;
	private ObjectManager objectManager;
	private Game game;
	private List<Spacecraft> spacecraftList;
	private List<Image> iconImageList;
	private List<Image> healthBarList;
	private bool bUpdating = false;

	private IEnumerator loadWaitCoroutine;
	private IEnumerator updateCoroutine;

	void Awake()
	{
		spacecraftList = new List<Spacecraft>();
		iconImageList = new List<Image>();
		game = FindObjectOfType<Game>();
	}

	void Start()
    {
		cameraMain = Camera.main;
		objectManager = FindObjectOfType<ObjectManager>();

		loadWaitCoroutine = LoadWait(0.3f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		InitIconFleet();

		//updateCoroutine = UpdateIconHud(updateInterval);
		//StartCoroutine(updateCoroutine);
		bUpdating = true;
	}

	void InitIconFleet()
	{
		spacecraftList = objectManager.GetSpacecraftList();
		int numCrafts = spacecraftList.Count;
		///Debug.Log("craft icon hud reading list " + numCrafts);

		for (int i = 0; i < numCrafts; i++)
		{
			if (spacecraftList[i] != null)
			{
				Image img = null;
				if (i < iconImageList.Count)
					img = iconImageList[i];
				else
					img = CreateSpacecraftIcon().GetComponent<Image>();

				Spacecraft sp = spacecraftList[i];
				HealthBar healthBar = img.gameObject.GetComponentInChildren<HealthBar>();
				healthBar.transform.localScale = Vector3.one * (1f / healthBar.GetComponent<RectTransform>().sizeDelta.x);
				Health spacecraftHealth = sp.GetComponent<Health>();
				healthBar.InitHeath(spacecraftHealth.maxHealth, spacecraftHealth.GetHealth());

				Color iconColor = img.color;
				if (sp.GetAgent() != null)
				{
					switch (sp.GetAgent().teamID)
					{
						case 0:
							iconColor = Color.blue;
							break;
						case 1:
							iconColor = Color.red;
							break;
						case 2:
							iconColor = Color.black;
							break;
						case 3:
							iconColor = Color.white;
							break;
						case 4:
							iconColor = Color.yellow;
							break;
						default:
							iconColor = img.color;
							break;
					}
				}

				img.color = iconColor;
				sp.SetHUDIcon(img.gameObject);
				img.gameObject.SetActive(true);
				img.enabled = false;
			}
		}
	}

	GameObject CreateSpacecraftIcon()
	{
		GameObject product = null;
		if (iconPrefab != null)
		{
			product = Instantiate(iconPrefab, iconParent.transform);
			iconImageList.Add(product.GetComponent<Image>());
		}

		return product;
	}

	//private IEnumerator UpdateIconHud(float intervalTime)
	//{
	//	while (true)
	//	{
	//		yield return new WaitForSeconds(intervalTime);
	//		UpdateCraftIcons();
	//	}
	//}

	void LateUpdate()
	{
		if (bUpdating)
			UpdateCraftIcons();
		///Debug.Log("delta time " + Time.deltaTime);
	}

	void UpdateCraftIcons()
	{
		spacecraftList = objectManager.GetSpacecraftList();
		if (spacecraftList.Count > 0)
		{
			for (int i = 0; i < spacecraftList.Count; i++)
			{
				if (spacecraftList[i] != null)
				{
					Spacecraft sp = spacecraftList[i];
					if (sp.GetHUDIcon() != null)
					{
						Image img = sp.GetHUDIcon().GetComponent<Image>();
						if (img != null) /// (sp.GetMarks() > 0) || (sp.GetAgent() && (sp.GetAgent().teamID == 0)))
						{
							img.enabled = true;
							if (!img.gameObject.activeInHierarchy)
								img.gameObject.SetActive(true);
							if (img.sprite != sp.craftIcon)
								img.sprite = sp.craftIcon;
							img.rectTransform.sizeDelta = Vector2.one * sp.iconScale;

							Vector3 worldPosition = sp.transform.position;
							Vector3 craftToScreen = (sp.transform.position - cameraMain.transform.position).normalized;
							Vector3 craftScreenPosition = worldPosition;/// + (craftToScreen * 10);
							img.rectTransform.position = craftScreenPosition;
							
							Vector3 toCamera = cameraMain.transform.position - sp.transform.position;
							Color iconColor = img.color;
							iconColor.a = Mathf.Clamp(toCamera.magnitude * 0.01f, 0.01f, 0.55f);
							img.color = iconColor;
						}
						else
						{
							img.enabled = false;
							if (img.gameObject.activeInHierarchy)
								img.gameObject.SetActive(false);
							Debug.Log("disabled icon");
						}
					}
				}
			}
		}
	}

	public void UpdateSpacecraftHealth(Spacecraft sp)
	{
		if (sp.GetHUDIcon() != null)
		{
			HealthBar healthBar = sp.GetHUDIcon().GetComponentInChildren<HealthBar>();
			Health spacecraftHealth = sp.GetComponent<Health>();
			healthBar.InitHeath(spacecraftHealth.maxHealth, spacecraftHealth.GetHealth());
		}
	}

	public void ModifySpacecraftList(Spacecraft sp, bool stays)
	{
		if (stays)
		{
			spacecraftList.Add(sp);
		}
		else
		{
			if (sp.GetHUDIcon() != null)
				sp.GetHUDIcon().SetActive(false);
			sp.SetHUDIcon(null);
			
			if (spacecraftList.Contains(sp))
				spacecraftList.Remove(sp);
		}
	}
}
