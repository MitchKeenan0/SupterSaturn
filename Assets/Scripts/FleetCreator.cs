using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FleetCreator : MonoBehaviour
{
	public GameObject fleetPanel;
	public GameObject fleetPanelSlotPrefab;
	public GameObject selectionPanel;
	public GameObject selectionPanelCardPrefab;
	public GameObject loadingPanel;
	public Text playerNameText;
	public Text placeholderNameText;
	public Text chevronValueText;
	public Text chevronBalanceText;
	public Image chevronDividerImage;
	public Color availableSlotColor;
	public Color lockedSlotColor;
	public Color moneyColor;
	public Color bankruptColor;
	public Button startGameButton;
	public Button emptyButton;
	public Button emptyAllButton;

	private Game game;
	private Player player;
	private SpacecraftViewer spacecraftViewer;
	private FleetPanelSlot selectedPanelSlot; // :'D
	private int maxSlotsAvailable = 8;

	private List<FleetPanelSlot> panelSlots;
	private List<SelectionPanelCard> spacecraftCardList;

	private IEnumerator deselectGraceCoroutine;
	private IEnumerator loadGraceCoroutine;

	public List<FleetPanelSlot> GetPanelSlots() { return panelSlots; }

	void Awake()
	{
		player = FindObjectOfType<Player>();
		game = FindObjectOfType<Game>();
		game.LoadGame();

		emptyButton.interactable = false;
		loadingPanel.SetActive(true);
	}

	void Start()
    {
		panelSlots = new List<FleetPanelSlot>();
		spacecraftCardList = new List<SelectionPanelCard>();
		spacecraftViewer = GetComponent<SpacecraftViewer>();
		
		loadGraceCoroutine = LoadWait(0.3f);
		StartCoroutine(loadGraceCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		InitFleetPanel();
		InitSelectionPanel();
		UpdateChevronBalance();

		string playerName = player.playerName;
		if (playerName == "")
			playerName = "No Name";
		playerNameText.text = player.playerName;
		placeholderNameText.text = player.playerName;
		///Debug.Log("FleetCreator set player name " + player.playerName);

		loadingPanel.SetActive(false);
	}

	void InitFleetPanel()
	{
		int slots = panelSlots.Count;
		if (slots < maxSlotsAvailable)
		{
			for (int i = 0; i < maxSlotsAvailable; i++)
				CreateFleetPanelSlot();
		}

		List<Card> selectedCards = new List<Card>(game.GetSelectedCards());
		if (selectedCards.Count == 0)
			selectedCards = game.initialSpacecraftCards;

		FleetPanelSlot[] fleetPanelSlots = fleetPanel.GetComponentsInChildren<FleetPanelSlot>();
		int numSlots = fleetPanelSlots.Length;
		for (int i = 0; i < numSlots; i++)
		{
			FleetPanelSlot panelSlot = fleetPanelSlots[i];
			Card spacecraftCard = null;
			if ((i < selectedCards.Count) && (i < maxSlotsAvailable))
				spacecraftCard = selectedCards[i];
			panelSlot.SetSlot(spacecraftCard, i);

			Image panelImage = panelSlot.GetComponent<Image>();
			if (i < maxSlotsAvailable)
				panelImage.color = availableSlotColor;
			else
				panelImage.color = lockedSlotColor;

			if ((i == 0) && (spacecraftCard != null))
			{
				spacecraftViewer.DisplaySpacecraft(spacecraftCard.numericID);
			}
		}
	}

	void InitSelectionPanel()
	{
		List<Card> playerSpacecraftCards = new List<Card>(game.cardLibrary);
		int numCards = playerSpacecraftCards.Count;
		for(int i = 0; i < numCards; i++)
		{
			Card playerCard = playerSpacecraftCards[i];
			if (playerCard != null)
			{
				SelectionPanelCard selectionCard = CreateSelectionCard();
				if (selectionCard != null)
					selectionCard.SetCard(playerCard);
			}
		}
	}

	SelectionPanelCard CreateSelectionCard()
	{
		GameObject cardObj = Instantiate(selectionPanelCardPrefab, selectionPanel.transform);
		cardObj.transform.SetParent(selectionPanel.transform);
		SelectionPanelCard card = cardObj.GetComponent<SelectionPanelCard>();
		spacecraftCardList.Add(card);
		return card;
	}

	FleetPanelSlot CreateFleetPanelSlot()
	{
		GameObject slotObj = Instantiate(fleetPanelSlotPrefab, fleetPanel.transform);
		FleetPanelSlot panelSlot = slotObj.GetComponent<FleetPanelSlot>();
		panelSlots.Add(panelSlot);
		return panelSlot;
	}

	void UpdateChevronBalance()
	{
		int total = 0;
		List<Card> selectedCards = new List<Card>(game.GetSelectedCards());
		int numSelected = selectedCards.Count;
		for (int i = 0; i < numSelected; i++)
		{
			if (game.GetSelectedCards()[i] != null)
			{
				int thisCost = game.GetSelectedCards()[i].cost;
				total += thisCost;
			}
		}
		chevronBalanceText.text = total.ToString();

		if (total <= game.GetChevrons())
		{
			startGameButton.interactable = true;
			chevronBalanceText.color = moneyColor;
			chevronDividerImage.color = moneyColor;
		}
		else
		{
			startGameButton.interactable = false;
			chevronBalanceText.color = bankruptColor;
			chevronDividerImage.color = bankruptColor;
		}

		chevronValueText.text = game.GetChevrons().ToString();
		Debug.Log("game chevrons " + game.GetChevrons());
	}

	private IEnumerator DeselectAfterTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		selectedPanelSlot = null;
		emptyButton.interactable = false;
	}

	// UI Button functions
	public void SelectSlot(int buttonIndex)
	{
		selectedPanelSlot = panelSlots[buttonIndex];
		if (selectedPanelSlot.GetCard() != null)
		{
			int displayID = selectedPanelSlot.GetCard().numericID;
			spacecraftViewer.DisplaySpacecraft(displayID);
			emptyButton.interactable = true;
		}
	}

	public void DeselectSlot()
	{
		deselectGraceCoroutine = DeselectAfterTime(0.2f);
		StartCoroutine(deselectGraceCoroutine);
	}

	public void DemoSlotSprite(Sprite demoSprite, int index)
	{
		if (selectedPanelSlot != null)
		{
			if (demoSprite != null)
			{
				SelectionPanelCard relevantCard = spacecraftCardList[index];
				selectedPanelSlot.PreviewSlot(demoSprite, relevantCard.cardName, relevantCard.cardCost);
			}
			else
			{
				selectedPanelSlot.PreviewSlot(null, null, 0);
			}
		}
	}

	public void SelectSpacecraft(int buttonIndex)
	{
		spacecraftViewer.DisplaySpacecraft(buttonIndex);
		if (selectedPanelSlot != null)
		{
			int selectionIndex = selectedPanelSlot.transform.GetSiblingIndex();
			Card spacecraftCard = spacecraftCardList[buttonIndex].GetCard();
			if ((selectedPanelSlot.GetCard() == null) || (spacecraftCard.numericID != selectedPanelSlot.GetCard().numericID))
			{
				selectedPanelSlot.SetSlot(spacecraftCard, selectionIndex);
				UpdateChevronBalance();
				game.SaveGame();
			}
		}
	}

	public void EmptySlot()
	{
		if (selectedPanelSlot != null)
		{
			int slotIndex = selectedPanelSlot.transform.GetSiblingIndex();
			selectedPanelSlot.SetSlot(null, slotIndex);
			UpdateChevronBalance();
			selectedPanelSlot = null;
			game.SaveGame();
			///InitFleetPanel();
		}
	}

	public void EmptyAll()
	{
		int numSlots = game.GetSelectedCards().Count;
		for (int i = 0; i < numSlots; i++)
		{
			FleetPanelSlot ps = panelSlots[i];
			if (ps != null)
			{
				int slotIndex = ps.transform.GetSiblingIndex();
				panelSlots[i].SetSlot(null, i);
			}
		}

		selectedPanelSlot = null;
		UpdateChevronBalance();
		game.SaveGame();
		///InitFleetPanel();
	}

	public void SetName(string value)
	{
		///Debug.Log("Set player name " + value);
		player.SetName(value);
	}

	public void StartGame()
	{
		sceneLoadCoroutine = LoadScene(0.3f, "CampaignScene");
		StartCoroutine(sceneLoadCoroutine);
	}

	public void BackToMenu()
	{
		sceneLoadCoroutine = LoadScene(0.3f, "BaseScene");
		StartCoroutine(sceneLoadCoroutine);
	}

	private IEnumerator sceneLoadCoroutine;
	private IEnumerator LoadScene(float waitTime, string sceneName)
	{
		loadingPanel.SetActive(true);
		SaveGame();
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene(sceneName);
	}

	public void SaveGame()
	{
		game.SaveGame();
	}
}
