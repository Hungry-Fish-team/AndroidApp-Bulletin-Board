using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadInfoScript : MonoBehaviour
{
    GameManager gameManager;
    PersonInformationScript personInformationScript;
    RegisteredPersonScript registeredPersonScript;

    PhotonView photonView;

    public SignItemScriptableObject openEvent;

    [SerializeField]
    Text nameEventText;
    [SerializeField]
    Text placeNameText;
    [SerializeField]
    Text dateTimeText;
    [SerializeField]
    Text infoEventText;
    [SerializeField]
    GameObject peopleListContent;
    [SerializeField]
    Text peopleCountText;
    [SerializeField]
    Button joinButton;
    [SerializeField]
    Button leaveButton;
    [SerializeField]
    Button deleteButton;

    [SerializeField]
    GameObject personPrefab;

    [SerializeField]
    string[] personsName;
    //string personName;

    bool waitingResultBool = false;

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();
        registeredPersonScript = GameObject.Find("GameManager").GetComponent<RegisteredPersonScript>();
        photonView = GetComponent<PhotonView>();
        photonView.ControllerActorNr = GameObject.Find("GameManager").GetComponent<PhotonView>().ControllerActorNr;
    }

    private void OnEnable()
    {
        InitializationAllObjects();

        LoadInfo();
    }

    [PunRPC]
    public void ReturnPersonNameByIDFromServer(string personNameFromServer, int personIndex)
    {
        personsName[personIndex] = personNameFromServer;
        //personName = personNameFromServer;

        waitingResultBool = false;
    }

    [PunRPC]
    public void SendRequesToReturnPersonNameByIDFromServer(string playerName, int personID, int personIndex)
    {
        InitializationAllObjects();

        string personName = registeredPersonScript.registeredPersons.ReturnPersonNameByIDFromServer(personID);

        Debug.Log(personName);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName)
            {
                gameObject.GetComponent<PhotonView>().RPC("ReturnPersonNameByIDFromServer", player, personName, personIndex);
            }
        }
    }

    private IEnumerator PersonNameFromServerByID(int personID, int personIndex)
    {
        Debug.Log(personID + " " + personIndex);

        waitingResultBool = true;
        gameObject.GetComponent<PhotonView>().RPC("SendRequesToReturnPersonNameByIDFromServer", RpcTarget.MasterClient, PhotonNetwork.NickName, personID, personIndex);
        yield return new WaitUntil(() => waitingResultBool == false);

        Debug.Log(personsName[personIndex]);
        GameObject newPerson = Instantiate(personPrefab, peopleListContent.transform);
        newPerson.transform.GetChild(0).GetComponent<Text>().text = personsName[personIndex];
        //newPerson.transform.GetChild(0).GetComponent<Text>().text = personName;
    }

    public void LoadInfo()
    {
        DestroyAllInfo();

        Debug.Log(openEvent.peopleList.Count);

        nameEventText.text = openEvent.nameEventText;
        placeNameText.text = openEvent.placeNameText;
        dateTimeText.text = openEvent.dateTimeText;
        infoEventText.text = openEvent.infoEventText;
        peopleCountText.text = "Count of people:  " + openEvent.peopleList.Count.ToString();

        personsName = new string[openEvent.peopleList.Count];

        if (openEvent.peopleList.Count != 0)
        {
            for (int i = 0; i < openEvent.peopleList.Count; i++)
            {
                StartCoroutine(PersonNameFromServerByID(int.Parse(openEvent.peopleList[i]), i));
            }
        }

        LoadJoinOrLeaveButton();
    }

    private void DestroyAllInfo()
    {
        if (nameEventText.text != "")
        {
            nameEventText.text = "";
            placeNameText.text = "";
            dateTimeText.text = "";
            infoEventText.text = "";
            peopleCountText.text = "";

            //personName = string.Empty;

            for (int i = 0; i < peopleListContent.transform.childCount; i++)
            {
                Destroy(peopleListContent.transform.GetChild(i).gameObject);
            }
        }
    }

    private void OnDisable()
    {
        DestroyAllInfo();
        openEvent = null;
    }

    public void JoinOrLeaveButtonFunc()
    {
        if (joinButton.IsActive() == true)
        {
            AddPersonToEvent();
        }
        else
        {
            if (deleteButton.IsActive() == false)
            {
                RemovePersonFromEvent();
            }
            else
            {
                DeleteEvent();
            }
        }

        //gameManager.TryStartReloadSign();
    }

    public int FindNumberOfOpenEvent()
    {
        for (int i = 0; i < gameManager.listSigns.Count; i++)
        {
            if (openEvent != null && openEvent.name == gameManager.listSigns[i].name)
            {
                return i;
            }
        }
        return -1;

        //return gameManager.listSigns.IndexOf(openEvent);
    }

    private void AddPersonToEvent()
    {
        gameManager.TryAddPersonToSign(personInformationScript.personProfile.ReturnPersonID().ToString(), FindNumberOfOpenEvent());
    }

    private void RemovePersonFromEvent()
    {
        for (int i = 0; i < openEvent.peopleList.Count; i++)
        {
            if (openEvent.peopleList[i] == personInformationScript.personProfile.ReturnPersonID().ToString())
            {
                if (FindNumberOfOpenEvent() != -1) {
                    gameManager.TryRemovePersonFromSign(i, FindNumberOfOpenEvent());
                }
            }
        }
    }
    
    private void DeleteEvent()
    {
        if (FindNumberOfOpenEvent() != -1) {
            gameManager.TryRemoveSignFromList(FindNumberOfOpenEvent());
        }
    }

    private void LoadJoinOrLeaveButton()
    {
        if (FindMyPersonName() == true)
        {
            joinButton.gameObject.SetActive(false);

            if (openEvent.ownerEvent == personInformationScript.personProfile.ReturnPersonID().ToString())
            {
                leaveButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(true);
            }
            else
            {
                leaveButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(false);
            }
        }
        else
        {
            joinButton.gameObject.SetActive(true);
            leaveButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
        }
    }

    private bool FindMyPersonName()
    {
        for(int i = 0; i < openEvent.peopleList.Count; i++)
        {
            if(openEvent.peopleList[i] == personInformationScript.personProfile.ReturnPersonID().ToString())
            {
                return true;
            }
        }
        return false;
    }
}
