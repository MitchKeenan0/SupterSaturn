using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseContextHUD : MonoBehaviour
{
	private SpacecraftInformation spacecraftInformation;
	private List<Text> texts;
	private Camera cameraMain;
	private GameObject panel;
	private IEnumerator updateCoroutine;

	public SpacecraftInformation GetSpacecraftInformation() { return spacecraftInformation; }

	void Start()
    {
		cameraMain = Camera.main;
		texts = new List<Text>();
		Text[] ts = GetComponentsInChildren<Text>();
		foreach(Text t in ts)
		{
			texts.Add(t);
			t.gameObject.SetActive(false);
		}

		panel = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
		panel.SetActive(false);
	}

	public void SetSpacecraftInformation(bool value, SpacecraftInformation si)
	{
		spacecraftInformation = si;
		panel.SetActive(value);

		if (value)
		{
			panel.SetActive(true);
			foreach (Text t in texts)
				t.gameObject.SetActive(true);
			ReadMouseContext();
			//updateCoroutine = UpdateMouseContext(0.1f);
			//StartCoroutine(updateCoroutine);
		}
		else
		{
			panel.SetActive(false);
			//StopCoroutine(updateCoroutine);
		}
	}

	private IEnumerator UpdateMouseContext(float waitTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(waitTime);
			ReadMouseContext();
		}
	}

	void ReadMouseContext()
	{
		if (spacecraftInformation != null)
		{
			bool bReadable = (spacecraftInformation.GetTeamID() == 0) || (spacecraftInformation.GetMarks() > 0);
			if (bReadable)
			{
				if (!panel.activeInHierarchy)
					panel.SetActive(true);

				texts[0].text = spacecraftInformation.GetSpacecraftName();
				texts[1].text = spacecraftInformation.GetHeadline();
				//GetDistance(cameraMain.transform.position).ToString("F1") + " km";

				Vector3 spacecraftScreenPosition = cameraMain.WorldToScreenPoint(spacecraftInformation.GetPosition());
				Vector3 offset = new Vector2(50f, -25f);
				panel.transform.position = spacecraftScreenPosition + offset;
			}
			else
			{
				if (panel.activeInHierarchy)
					panel.SetActive(false);
			}
		}
	}
}
