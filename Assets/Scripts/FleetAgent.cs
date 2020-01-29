using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetAgent : MonoBehaviour
{
	public int maxCapacity = 100;

	private Game game;
	private Campaign campaign;
	private ObjectManager objectManager;
	private Fleet fleet;
	private FleetController myFleetController;
	private FleetController playerFleetController;
	private LocationManager locationManager;
	private TurnManager turnManager;
	private NameLibrary nameLibrary;
	private List<Spacecraft> spacecraftList;

	public List<Spacecraft> GetAgentSpacecraftList() { return spacecraftList; }

	private IEnumerator loadWaitCoroutine;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		game = FindObjectOfType<Game>();
		campaign = FindObjectOfType<Campaign>();
		objectManager = FindObjectOfType<ObjectManager>();
		fleet = GetComponentInParent<Fleet>();
		locationManager = FindObjectOfType<LocationManager>();
		turnManager = FindObjectOfType<TurnManager>();
		myFleetController = GetComponent<FleetController>();
		playerFleetController = GetComponent<FleetController>();
		nameLibrary = FindObjectOfType<NameLibrary>();

		loadWaitCoroutine = LoadWait(0.2f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitAgent();
		TakeTurnActions();
	}

	public void TakeTurnActions()
	{
		if (playerFleetController != null)
		{
			myFleetController.StandbyMove(playerFleetController.GetLocation());
			if (myFleetController.GetRoute() == null)
			{
				int neighborIndex = Random.Range(0, fleet.GetLocation().GetNeighbors().Count);
				CampaignLocation neighbor = fleet.GetLocation().GetNeighbors()[neighborIndex];
				myFleetController.StandbyMove(neighbor);
				Debug.Log("agent set route to neighbor");
			}
		}
	}

	void InitAgent()
	{
		fleet.fleetName = nameLibrary.GetFleetName();

		int fleetSpacecraftCount = Random.Range(1, maxCapacity);
		for (int i = 0; i < fleetSpacecraftCount; i++)
		{
			int numCards = game.cardLibrary.Length;
			int rando = Random.Range(0, numCards);
			int index = Mathf.FloorToInt(Mathf.Sqrt(rando));
			///Debug.Log("FleetAgent index " + index);
			if (index < numCards)
			{
				GameObject spacecraftObj = Instantiate(game.cardLibrary[index].cardObjectPrefab, transform);
				spacecraftObj.SetActive(false);
				Spacecraft sp = spacecraftObj.GetComponent<Spacecraft>();
				spacecraftList.Add(sp);
				game.SetEnemyCard(i, game.cardLibrary[index]);
				game.AddEnemy(sp);
			}
		}
	}
}
