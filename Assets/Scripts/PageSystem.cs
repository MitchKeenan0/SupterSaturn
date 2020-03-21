using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageSystem : MonoBehaviour
{
	public GameObject pilotPanel;
	public GameObject gamePanel;
	public GameObject spacecraftPanel;
	public Button pilotButton;
	public Button gameButton;
	public Button spacecraftButton;

	private CanvasGroup pilotCanvasGroup;
	private CanvasGroup gameCanvasGroup;
	private CanvasGroup spacecraftCanvasGroup;

	private List<CanvasGroup> canvasGroups;

    void Start()
    {
		canvasGroups = new List<CanvasGroup>();

		pilotCanvasGroup = pilotPanel.GetComponent<CanvasGroup>();
		canvasGroups.Add(pilotCanvasGroup);
		gameCanvasGroup = gamePanel.GetComponent<CanvasGroup>();
		canvasGroups.Add(gameCanvasGroup);
		spacecraftCanvasGroup = spacecraftPanel.GetComponent<CanvasGroup>();
		canvasGroups.Add(spacecraftCanvasGroup);

		SelectPage(1);
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(gameButton.gameObject);
	}

	public void SelectPage(int pageIndex)
	{
		int numPages = canvasGroups.Count;
		for(int i = 0; i < numPages; i++)
		{
			CanvasGroup cg = canvasGroups[i];
			bool selectedPage = (i == pageIndex);
			cg.alpha = (selectedPage) ? 1f : 0f;
			cg.blocksRaycasts = selectedPage;
		}
	}
}
