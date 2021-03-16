using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using Photon.Pun;
using UnityEditor;

public class GameManager : MonoBehaviour, IPunObservable
{
    PersonInformationScript personInformationScript;

    AnimationScript animationScript;
    [SerializeField]
    LoadInfoScript loadInfoScript;

    [SerializeField]
    public List<SignItemScriptableObject> listSigns;

    [SerializeField]
    GameObject prefabOfSignItem;

    [SerializeField]
    GameObject contentForDashboard;

    [SerializeField]
    public Text errorOrInfoText;

    public string eventName = "Dashboard";

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

    public void TryAddNewSignFunc(SignItemScriptableObject newSignItemScriptableObject)
    {
        //if (!PhotonNetwork.IsMasterClient)
        {
            string newPeopleList = "";

            for (int i = 0; i < newSignItemScriptableObject.peopleList.Count; i++)
            {
                newPeopleList += newSignItemScriptableObject.peopleList[i] + " ";
            }

            byte[] bytesImage = newSignItemScriptableObject.icon.texture.EncodeToPNG();

            transform.GetComponent<PhotonView>().RPC("AddNewSignFunc", RpcTarget.AllBuffered,
                newSignItemScriptableObject.nameEventText, newSignItemScriptableObject.placeNameText, newSignItemScriptableObject.dateTimeText,
                newSignItemScriptableObject.infoEventText, newSignItemScriptableObject.peopleList.Count, newPeopleList, bytesImage, newSignItemScriptableObject.ownerEvent);
        }
    }

    public void TryRemoveSignFromList(int numberOfSign)
    {
        transform.GetComponent<PhotonView>().RPC("RemoveSignFromList", RpcTarget.All, numberOfSign);
    }

    public void TryRemovePersonFromSign(int numberOfPerson, int numberOfSign)
    {
        transform.GetComponent<PhotonView>().RPC("RemovePersonFromSign", RpcTarget.All, numberOfPerson, numberOfSign);
    }

    public void TryAddPersonToSign(string nameOfPerson, int numberOfSign)
    {
        transform.GetComponent<PhotonView>().RPC("AddNewPersonToSign", RpcTarget.All, nameOfPerson, numberOfSign);
    }

    public void TryOpenMenu()
    {
        transform.GetComponent<PhotonView>().RPC("OpenMenuObject", RpcTarget.All);
    }

    public void TryStartReloadSign()
    {
        transform.GetComponent<PhotonView>().RPC("StartReloadSign", RpcTarget.All);
    }

    [PunRPC]
    private void AddNewSignFunc(string nameEventText, string placeNameText, string dateTimeText, string infoEventText, int countNewPeople, string newPeopleList, byte[] bytesImage, string ownerEvent)
    {
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

        LoadObjestForDashboard();
    }

    [PunRPC]
    private void RemoveSignFromList(int numberOfSign)
    {
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
        listSigns[numberOfSign].peopleList.RemoveAt(numberOfPerson);

        loadInfoScript.openEvent = listSigns[numberOfSign];

        TryStartReloadSign();
    }

    [PunRPC]
    private void AddNewPersonToSign(string nameOfPerson, int numberOfSign)
    {
        listSigns[numberOfSign].peopleList.Add(nameOfPerson);

        loadInfoScript.openEvent = listSigns[numberOfSign];

        TryStartReloadSign();
    }

    [PunRPC]
    private void StartReloadSign()
    {
        if (loadInfoScript.isActiveAndEnabled == true)
        {
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
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();
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
            LoadObjestForDashboard();
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

        animationScript.eventObject.GetComponent<LoadInfoScript>().openEvent = signItem;

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

        for (int i = 0; i < listSigns.Count; i++)
        {
            GameObject newSign = Instantiate(prefabOfSignItem, contentForDashboard.transform);

            newSign.GetComponent<LoadInfoForSignItem>().signItem = listSigns[i];
        }
    }

    private void LoadObjectForDashboardJoined()
    {
        DestroyAllObjectForDashboard();

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
        //Debug.Log("Clear");

        for (int i = 0; i < contentForDashboard.transform.childCount; i++)
        {
            Destroy(contentForDashboard.transform.GetChild(i).gameObject);
        }
    }

    public void ReloadObjestForDashboard()
    {
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
