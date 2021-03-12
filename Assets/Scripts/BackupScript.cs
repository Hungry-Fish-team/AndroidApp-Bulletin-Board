using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BackupScript : MonoBehaviour
{
    GameManager gameManager;
    SignItemScriptableObject signItemScriptableObject;

    [SerializeField]
    bool isBackUpWork = true;

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        InitializationAllObjects();

        if (isBackUpWork == true)
        {
            LoadAllDataFromFile();

            gameManager.LoadObjestForDashboard();
        }
    }

    private void OnApplicationQuit()
    {
        if (isBackUpWork == true)
        {
            SaveAllDataToFile();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        
    }

    private void SaveDashBoardInfo()
    {
        //public string nameEventText;
        //public string placeNameText;
        //public string dateTimeText;
        //public string infoEventText;
        //public List<string> peopleList;
        //public Sprite icon;
        //public string ownerEvent;
    }

    string fileForDashBoardSave;

    private void LoadFiles()
    {
        if (!File.Exists(Application.persistentDataPath + "/fileForDashBoardSave.json"))
        {
            CreateFilesForSave("/fileForDashBoardSave.json");
        }
        fileForDashBoardSave = Application.persistentDataPath + "/fileForDashBoardSave.json";
    }

    private void CreateFilesForSave(string nameOfFile)
    {
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
    }

    public void SaveAllDataToFile()
    {
        JSONObject dashBoardDATA = new JSONObject();

        for (int i = 0; i < gameManager.listSigns.Count; i++) {
            JSONArray lineOfAllSigns = new JSONArray();

            Debug.Log(gameManager.listSigns[i].nameEventText);

            //JSONObject signDATA = new JSONObject();

            lineOfAllSigns.Add("nameEventText", gameManager.listSigns[i].nameEventText);
            lineOfAllSigns.Add("placeNameText", gameManager.listSigns[i].placeNameText);
            lineOfAllSigns.Add("dateTimeText", gameManager.listSigns[i].dateTimeText);
            lineOfAllSigns.Add("infoEventText", gameManager.listSigns[i].infoEventText);

            JSONArray lineOfPersonOfSign = new JSONArray();
            //Debug.Log(gameManager.listSigns[i].peopleList.Count);
            for (int j = 0; j < gameManager.listSigns[i].peopleList.Count; j++)
            {
                lineOfPersonOfSign.Add(gameManager.listSigns[i].peopleList[j]);
            }
            lineOfAllSigns.Add("peopleList", lineOfPersonOfSign);

            byte[] spriteBytes = gameManager.listSigns[i].icon.texture.EncodeToPNG();
            JSONArray lineOfSpriteBytes = new JSONArray();
            for (int j = 0; j < spriteBytes.Length; j++)
            {
                lineOfSpriteBytes.Add(spriteBytes[j]);
            }
            lineOfAllSigns.Add("icon", lineOfSpriteBytes);

            lineOfAllSigns.Add("nameEventText", gameManager.listSigns[i].ownerEvent);

            dashBoardDATA.Add("AllSigns" + i.ToString(), lineOfAllSigns);
        }

        if (File.Exists(fileForDashBoardSave))
        {
            File.WriteAllText(fileForDashBoardSave, dashBoardDATA.ToString());
        }
    }

    public void LoadAllDataFromFile()
    {
        LoadFiles();

        SignItemScriptableObject newSignItemScriptableObject;

        int i = 0;

        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForDashBoardSave)) != null)
        {
            JSONObject dashBoardDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForDashBoardSave));

            if (dashBoardDATA != null)
            {
                while (dashBoardDATA["AllSigns" + i.ToString()] != null) 
                {
                    newSignItemScriptableObject = new SignItemScriptableObject();

                    newSignItemScriptableObject.name = (gameManager.listSigns.Count + 1).ToString();

                    newSignItemScriptableObject.nameEventText = dashBoardDATA["AllSigns" + i.ToString()].AsArray[0];
                    newSignItemScriptableObject.placeNameText = dashBoardDATA["AllSigns" + i.ToString()].AsArray[1];
                    newSignItemScriptableObject.dateTimeText = dashBoardDATA["AllSigns" + i.ToString()].AsArray[2];
                    newSignItemScriptableObject.infoEventText = dashBoardDATA["AllSigns" + i.ToString()].AsArray[3];

                    newSignItemScriptableObject.peopleList = new List<string>();

                    for(int j = 0; j < dashBoardDATA["AllSigns" + i.ToString()].AsArray[4].Count; j++)
                    {
                        newSignItemScriptableObject.peopleList.Add(dashBoardDATA["AllSigns" + i.ToString()].AsArray[4].AsArray[j]);
                    }

                    Texture2D tex = new Texture2D(1, 1);
                    byte[] fileData = new byte[dashBoardDATA["AllSigns" + i.ToString()].AsArray[5].Count];
                    for (int j = 0; j < dashBoardDATA["AllSigns" + i.ToString()].AsArray[5].Count; j++)
                    {
                        fileData[j] = (byte)dashBoardDATA["AllSigns" + i.ToString()].AsArray[5].AsArray[j];
                    }
                    tex.LoadImage(fileData);
                    tex.Apply();
                    newSignItemScriptableObject.icon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

                    newSignItemScriptableObject.ownerEvent = dashBoardDATA["AllSigns" + i.ToString()].AsArray[6];

                    i++;

                    gameManager.listSigns.Add(newSignItemScriptableObject);
                }
            }
        }
    }
}
