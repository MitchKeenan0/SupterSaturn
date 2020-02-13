using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class Campaign : MonoBehaviour
{
	private Game game;
	private Camera cameraMain;
	private FleetHUD fleetHud;
	private CampaignBattle campaignBattle;
	private LocationManager locationManager;
	private List<CampaignLocation> locationList;
	private List<Fleet> fleetList;
	private List<CampaignLocation> originLocations = new List<CampaignLocation>();
	private bool bLocationsInit = false;

	void Awake()
    {
		locationList = new List<CampaignLocation>();
		originLocations = new List<CampaignLocation>();
		fleetList = new List<Fleet>();
		cameraMain = Camera.main;
		fleetHud = FindObjectOfType<FleetHUD>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
		game = FindObjectOfType<Game>();
		game.LoadGame();
    }

	public void InitFleetLocations(Fleet fleet)
	{
		if (!bLocationsInit)
		{
			locationList = locationManager.GetAllLocations();
			fleetList = fleetHud.GetFleetList();
			int numFleets = fleetList.Count;

			for (int i = 0; i < numFleets; i++)
			{
				CampaignLocation furthestLocation = null;
				float furthestDistance = 0f;
				foreach (CampaignLocation cl in locationList)
				{
					if ((cl.GetNeighbors().Count > 1) && (!originLocations.Contains(cl)))
					{
						float distanceToLocation = Vector3.Distance(cl.transform.position, Vector3.zero);
						if (distanceToLocation > furthestDistance)
						{
							furthestLocation = cl;
							furthestDistance = distanceToLocation;
						}
					}
				}

				if (furthestLocation != null)
				{
					fleetList[i].SetLocation(furthestLocation, true);
					originLocations.Add(furthestLocation);

					if (fleetList[i].teamID == 0)
					{
						MouseOrbitImproved moi = FindObjectOfType<MouseOrbitImproved>();
						if (moi != null)
						{
							///moi.SetOrbitTarget(furthestLocation.transform);
							Vector3 locationPosition = furthestLocation.transform.position;
							moi.SetCameraPosition(locationPosition + (locationPosition.normalized * 10));

							Quaternion toCenter = Quaternion.LookRotation(locationPosition - moi.transform.position, Vector3.up);
							moi.SetCameraRotation(toCenter);
						}
					}
				}

				bLocationsInit = true;
			}
		}
	}

	public void SaveCampaign()
	{
		SaveCampaign save = CreateSaveCampaignObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/campaignsave.save");
		bf.Serialize(file, save);
		file.Close();
	}

	private SaveCampaign CreateSaveCampaignObject()
	{
		SaveCampaign save = new SaveCampaign();
		if (locationList == null)
			locationList = new List<CampaignLocation>();
		int numLocations = locationList.Count;
		if (numLocations > 0)
		{
			for (int i = 0; i < numLocations; i++)
			{
				CampaignLocation cl = locationList[i];
				save.locationList.Add(cl.locationID);
				save.locationPositionsX.Add(cl.transform.position.x);
				save.locationPositionsY.Add(cl.transform.position.y);
				save.locationPositionsZ.Add(cl.transform.position.z);
				save.locationNameList.Add(cl.locationName);
			}
		}

		Identity[] ids = FindObjectsOfType<Identity>();
		List<Identity> idList = new List<Identity>();
		int numIDs = ids.Length;
		if (numIDs > 0)
		{
			for(int i = 0; i < numIDs; i++)
			{
				Identity id = ids[i];
				save.identityNames.Add(id.identityName);
				save.identityColorsR.Add(id.identityColor.r);
				save.identityColorsG.Add(id.identityColor.g);
				save.identityColorsB.Add(id.identityColor.b);

				if (id.GetFleetList().Count > 0)
				{
					CampaignLocation idLocation = id.GetFleetList()[0].GetLocation();
					if (locationList.Contains(idLocation))
						save.identityLocationList.Add(locationList.IndexOf(idLocation));
				}
			}
		}

		return save;
	}

	public void LoadCampaign()
	{
		if (File.Exists(Application.persistentDataPath + "/campaignsave.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/campaignsave.save", FileMode.Open);
			SaveCampaign save = (SaveCampaign)bf.Deserialize(file);
			file.Close();

			int numSavedLocations = save.locationList.Count;
			if (numSavedLocations > 0)
			{
				List<Vector3> positionList = new List<Vector3>();
				for(int i = 0; i < numSavedLocations; i++)
				{
					Vector3 locationPosition = new Vector3(save.locationPositionsX[i], save.locationPositionsY[i], save.locationPositionsZ[i]);
					positionList.Add(locationPosition);
				}
				locationManager.LoadLocations(save.locationList, positionList, save.locationNameList);
			}

			if (save.identityNames != null)
			{
				int numSavedIdentities = save.identityNames.Count;
				if (numSavedIdentities > 0)
				{
					Identity[] allIDs = FindObjectsOfType<Identity>();
					for (int i = 0; i < numSavedIdentities; i++)
					{
						Identity id = allIDs[i];
						Color idColor = new Color(save.identityColorsR[i], save.identityColorsG[i], save.identityColorsB[i]);
						id.LoadIdentity(save.identityNames[i], idColor);
					}
				}
			}
		}
	}

	public void DeleteSave()
	{
		try
		{
			File.Delete(GetSaveFilePath);
			///Debug.Log("save deleted");
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private string GetSaveFilePath
	{
		get { return Application.persistentDataPath + "/campaignsave.save"; }
	}
}
