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
					if ((cl.GetNeighbors().Count > 0) && (!originLocations.Contains(cl)))
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
							Vector3 locationPosition = furthestLocation.transform.position;
							moi.SetCameraPosition(locationPosition + (locationPosition.normalized * 10));

							Vector3 cameraPosition = cameraMain.transform.position;
							Vector3 relativePos = locationPosition - cameraPosition;
							Quaternion toCenter = Quaternion.LookRotation(relativePos, Vector3.up);
							toCenter.y *= 100;
							Debug.Log("toCenter rotation: " + toCenter);
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
		Debug.Log("saving campaign");
		SaveCampaign save = CreateSaveCampaignObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/campaignsave.save");
		bf.Serialize(file, save);
		file.Close();
	}

	private SaveCampaign CreateSaveCampaignObject()
	{
		SaveCampaign save = new SaveCampaign();
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
