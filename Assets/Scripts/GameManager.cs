using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using Photon.Pun;
using UnityEditor;
using Photon.Realtime;

public class GameManager : MonoBehaviour, IPunObservable
{
    PersonInformationScript personInformationScript;
    RegisteredPersonScript registeredPersonScript;

    [SerializeField]
    AnimationScript animationScript;
    [SerializeField]
    LoadInfoScript loadInfoScript;

    public List<SignItemScriptableObject> listSigns;

    public int sendInfoIndex = 0;

    [SerializeField]
    GameObject prefabOfSignItem;

    [SerializeField]
    GameObject dashboard;

    [SerializeField]
    public Text errorOrInfoText;

    bool waitingResultBool = false;

    [SerializeField]
    string[] personsName;

    public string eventName = "Dashboard";

    private void OnDisable()
    {
        if (dashboard != null)
        {
            dashboard.SetActive(false);
        }
    }

    private void OnEnable()
    {
        dashboard.SetActive(true);
    }

    public IEnumerator ErrorOrInfoFunc(string info)
    {
        errorOrInfoText.text = info;
        yield return new WaitForSeconds(2.5f);
        errorOrInfoText.text = "";
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            SendListSigns(stream);
        }
        else
        {
            TakeListSigns(stream);
        }
    }

    private void SendListSigns(PhotonStream stream)
    {
        stream.SendNext(sendInfoIndex);

        stream.SendNext(listSigns.Count);

        for (int i = 0; i < listSigns.Count; i++)
        {
            string nameEventText = listSigns[i].nameEventText;
            string placeNameText = listSigns[i].placeNameText;
            string dateTimeText = listSigns[i].dateTimeText;
            string infoEventText = listSigns[i].infoEventText;

            Sprite icon = listSigns[i].icon;

            string ownerEvent = listSigns[i].ownerEvent;

            stream.SendNext(nameEventText);
            stream.SendNext(placeNameText);
            stream.SendNext(dateTimeText);
            stream.SendNext(infoEventText);

            List<string> peopleList = listSigns[i].peopleList;
            stream.SendNext(peopleList.Count);
            for (int j = 0; j < peopleList.Count; j++)
            {
                stream.SendNext(peopleList[j]);
            }

            byte[] bytes = icon.texture.EncodeToPNG();
            stream.SendNext(bytes);
            stream.SendNext(ownerEvent);
        }
    }

    private void TakeListSigns(PhotonStream stream)
    {
        int newSendInfoIndex = (int)stream.ReceiveNext();
        //Debug.Log(newSendInfoIndex);

        if (sendInfoIndex != newSendInfoIndex)
        {
            sendInfoIndex = newSendInfoIndex;

            listSigns.Clear();

            int countSigns = (int)stream.ReceiveNext();

            for (int i = 0; i < countSigns; i++)
            {
                SignItemScriptableObject newSignItem = new SignItemScriptableObject();

                newSignItem.nameEventText = (string)stream.ReceiveNext();
                newSignItem.placeNameText = (string)stream.ReceiveNext();
                newSignItem.dateTimeText = (string)stream.ReceiveNext();
                newSignItem.infoEventText = (string)stream.ReceiveNext();

                int countPersonOfSign = (int)stream.ReceiveNext();
                List<string> newPeopleList = new List<string>();
                for (int j = 0; j < countPersonOfSign; j++)
                {
                    newPeopleList.Add((string)stream.ReceiveNext());
                }
                newSignItem.peopleList = newPeopleList;

                byte[] bytes = (byte[])stream.ReceiveNext();
                //Texture2D texture = new Texture2D(64, 64, TextureFormat.PVRTC_RGBA4, false);
                //texture.LoadRawTextureData(bytes);
                Texture2D texture = new Texture2D(64, 64);
                texture.LoadImage(bytes);
                texture.Apply();
                newSignItem.icon = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                newSignItem.ownerEvent = (string)stream.ReceiveNext();

                listSigns.Add(newSignItem);
            }

            LoadObjestForDashboard();
        }
    }

    public void TryAddNewSignFunc(SignItemScriptableObject newSignItemScriptableObject)
    {
        //if (!PhotonNetwork.IsMasterClient)
        {
            sendInfoIndex++;

            string newPeopleList = "";

            for (int i = 0; i < newSignItemScriptableObject.peopleList.Count; i++)
            {
                newPeopleList += newSignItemScriptableObject.peopleList[i] + " ";
            }

            byte[] bytesImage = null;//newSignItemScriptableObject.icon.texture.EncodeToPNG();

            GetComponent<PhotonView>().RPC("AddNewSignFunc", RpcTarget.All,
                newSignItemScriptableObject.nameEventText, newSignItemScriptableObject.placeNameText, newSignItemScriptableObject.dateTimeText,
                newSignItemScriptableObject.infoEventText, newSignItemScriptableObject.peopleList.Count, newPeopleList, bytesImage, newSignItemScriptableObject.ownerEvent);
        }
    }

    public void TryRemoveSignFromList(int numberOfSign)
    {
        GetComponent<PhotonView>().RPC("RemoveSignFromList", RpcTarget.All, numberOfSign);
    }

    public void TryRemovePersonFromSign(int numberOfPerson, int numberOfSign)
    {
        GetComponent<PhotonView>().RPC("RemovePersonFromSign", RpcTarget.All, numberOfPerson, numberOfSign);
    }

    public void TryAddPersonToSign(string nameOfPerson, int numberOfSign)
    {
        GetComponent<PhotonView>().RPC("AddNewPersonToSign", RpcTarget.All, nameOfPerson, numberOfSign);
    }

    public void TryOpenMenu()
    {
        GetComponent<PhotonView>().RPC("OpenMenuObject", RpcTarget.All);
    }

    public void TryStartReloadSign()
    {
        Debug.Log("Try reload");
        GetComponent<PhotonView>().RPC("StartReloadSign", RpcTarget.All);
    }

    [PunRPC]
    private void AddNewSignFunc(string nameEventText, string placeNameText, string dateTimeText, string infoEventText, int countNewPeople, string newPeopleList, byte[] bytesImage, string ownerEvent)
    {
        sendInfoIndex++;

        SignItemScriptableObject newSignItem = new SignItemScriptableObject();

        newSignItem.name = listSigns.Count.ToString();

        newSignItem.nameEventText = nameEventText;
        newSignItem.placeNameText = placeNameText;
        newSignItem.dateTimeText = dateTimeText;
        newSignItem.infoEventText = infoEventText;

        string[] newPeopleMassForSing = newPeopleList.Split(' ');
        List<string> newPeopleListForSign = new List<string>();
        for (int i = 0; i < countNewPeople; i++)
        {
            newPeopleListForSign.Add(newPeopleMassForSing[i]);
        }
        newSignItem.peopleList = newPeopleListForSign;

        Texture2D textureImage = new Texture2D(1, 1);
        textureImage.LoadImage(bytesImage);
        textureImage.Apply();
        newSignItem.icon = Sprite.Create(textureImage, new Rect(0.0f, 0.0f, textureImage.width, textureImage.height), new Vector2(0.5f, 0.5f), 100.0f);

        newSignItem.ownerEvent = ownerEvent;

        listSigns.Add(newSignItem);

        Debug.Log(newSignItem.ownerEvent + " " + newSignItem.nameEventText);

        LoadObjestForDashboard();
    }

    [PunRPC]
    private void AddNewInfoIndex()
    {
        if (sendInfoIndex < 1000)
        {
            sendInfoIndex++;
        }
        else
        {
            sendInfoIndex = 0;
            sendInfoIndex++;
        }
    }

    [PunRPC]
    private void RemoveSignFromList(int numberOfSign)
    {
        AddNewInfoIndex();

        if (loadInfoScript.FindNumberOfOpenEvent() == numberOfSign)
        {
            OpenMenuObject();
        }

        listSigns.RemoveAt(numberOfSign);

        LoadObjestForDashboard();
    }

    [PunRPC]
    private void RemovePersonFromSign(int numberOfPerson, int numberOfSign)
    {
        AddNewInfoIndex();

        listSigns[numberOfSign].peopleList.RemoveAt(numberOfPerson);

        loadInfoScript.openEvent = listSigns[numberOfSign];

        //TryStartReloadSign();

        StartReloadSign();
    }

    [PunRPC]
    private void AddNewPersonToSign(string nameOfPerson, int numberOfSign)
    {
        AddNewInfoIndex();

        listSigns[numberOfSign].peopleList.Add(nameOfPerson);

        loadInfoScript.openEvent = listSigns[numberOfSign];

        //TryStartReloadSign();

        StartReloadSign();
    }

    [PunRPC]
    private void StartReloadSign()
    {
        AddNewInfoIndex();

        if (loadInfoScript.isActiveAndEnabled == true)
        {
            Debug.Log("Reload");
            loadInfoScript.LoadInfo();
        }
    }

    void AppSettings()
    {
        //Application.runInBackground = true;

        //PlayerSettings.statusBarHidden = false;

        //Screen.fullScreen = false;
    }

    void InitializationAllObjects()
    {
        //photonView = transform.GetComponent<PhotonView>();

        animationScript = GameObject.Find("GameManager").GetComponent<AnimationScript>();
        loadInfoScript = GameObject.Find("GameManager").GetComponent<LoadInfoScript>();
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();
        registeredPersonScript = GameObject.Find("GameManager").GetComponent<RegisteredPersonScript>();
    }

    void Start()
    {
        AppSettings();

        InitializationAllObjects();

        LoadObjestForDashboard();
    }

    [PunRPC]
    public void OpenMenuObject()
    {
        if (eventName != "Menu" && eventName != "Event" && eventName != "NewItem" && eventName != "Login" && eventName != "JoinedEvent" && eventName != "ImageManager")
        {
            eventName = "Menu";
        }
        else
        {
            GetComponent<LoadInfoScript>().enabled = false;
            eventName = "Dashboard";
        }

        animationScript.OpenOrCloseObjects(eventName);
    }

    public void OpenEventObject(SignItemScriptableObject signItem)
    {
        if (eventName != "Event")
        {
            eventName = "Event";
        }
        else
        {
            eventName = "Dashboard";
        }

        //animationScript.eventObject.GetComponent<LoadInfoScript>().openEvent = signItem;
        GetComponent<LoadInfoScript>().openEvent = signItem;
        GetComponent<LoadInfoScript>().enabled = true;

        animationScript.OpenOrCloseObjects(eventName);
    }

    public void OpenCreateNewItemObject()
    {
        if (eventName != "NewItem")
        {
            eventName = "NewItem";
        }
        else
        {
            eventName = "Dashboard";
        }

        animationScript.OpenOrCloseObjects(eventName);
    }

    public void OpenLoginObject()
    {
        if (eventName != "Login")
        {
            eventName = "Login";
        }
        else
        {
            eventName = "Dashboard";
        }

        animationScript.OpenOrCloseObjects(eventName);
    }

    public void OpenJoinedEventObject()
    {
        if (eventName != "JoinedEvent")
        {
            eventName = "JoinedEvent";
        }
        else
        {
            eventName = "Dashboard";
        }

        LoadObjectForDashboardJoined();

        animationScript.OpenOrCloseObjects("Dashboard");
    }

    public void LoadObjestForDashboard()
    {
        DestroyAllObjectForDashboard();

        GameObject contentForDashboard = dashboard.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < listSigns.Count; i++)
        {
            GameObject newSign = Instantiate(prefabOfSignItem, contentForDashboard.transform);

            newSign.GetComponent<LoadInfoForSignItem>().signItem = listSigns[i];
        }
    }

    private void LoadObjectForDashboardJoined()
    {
        DestroyAllObjectForDashboard();

        GameObject contentForDashboard = dashboard.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < listSigns.Count; i++)
        {
            for (int j = 0; j < listSigns[i].peopleList.Count; j++)
            {
                if (listSigns[i].peopleList[j] == personInformationScript.personProfile.ReturnPersonName())
                {
                    GameObject newSign = Instantiate(prefabOfSignItem, contentForDashboard.transform);

                    newSign.GetComponent<LoadInfoForSignItem>().signItem = listSigns[i];
                }
            }
        }
    }

    private void DestroyAllObjectForDashboard()
    {
        GameObject contentForDashboard = dashboard.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < contentForDashboard.transform.childCount; i++)
        {
            Destroy(contentForDashboard.transform.GetChild(i).gameObject);
        }
    }

    public void ReloadObjestForDashboard()
    {
        GameObject contentForDashboard = dashboard.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        DestroyAllLoadedSignItems(contentForDashboard);

        LoadObjestForDashboard();
    }

    public void DestroyAllLoadedSignItems(GameObject content)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(0).gameObject);
        }
    }
}
