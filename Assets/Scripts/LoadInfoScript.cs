using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadInfoScript : MonoBehaviour
{
    public delegate void SomeFuncToCall();
    SomeFuncToCall someFuncToCall;

    GameManager gameManager;
    PersonInformationScript personInformationScript;

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
    Button joinButton;
    [SerializeField]
    Button leaveButton;
    [SerializeField]
    Button deleteButton;

    [SerializeField]
    GameObject personPrefab;

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();
    }

    //void Start()
    //{
    //    InitializationAllObjects();

    //    LoadInfo();

    //    LoadJoinOrLeaveButton();
    //}

    private void OnEnable()
    {
        InitializationAllObjects();

        LoadInfo();
    }

    public void LoadInfo()
    {
        DestroyAllInfo();

        nameEventText.text = openEvent.nameEventText;
        placeNameText.text = openEvent.placeNameText;
        dateTimeText.text = openEvent.dateTimeText;
        infoEventText.text = openEvent.infoEventText;
        if (openEvent.peopleList.Count != 0)
        {
            for (int i = 0; i < openEvent.peopleList.Count; i++)
            {
                GameObject newPerson = Instantiate(personPrefab, peopleListContent.transform);
                newPerson.transform.GetChild(0).GetComponent<Text>().text = openEvent.peopleList[i];
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
        //foreach (SignItemScriptableObject signItem in gameManager.listSigns)
        //{
        //    if(signItem == openEvent)
        //    {
        //        return gameManager.listSigns.IndexOf(openEvent);
        //    }
        //}
        //return -1;

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
        gameManager.TryAddPersonToSign(personInformationScript.personProfile.ReturnPersonName(), FindNumberOfOpenEvent());
        //openEvent = gameManager.listSigns[FindNumberOfOpenEvent()];
    }

    private void RemovePersonFromEvent()
    {
        for (int i = 0; i < openEvent.peopleList.Count; i++)
        {
            if (openEvent.peopleList[i] == personInformationScript.personProfile.ReturnPersonName())
            {
                if (FindNumberOfOpenEvent() != -1) {
                    gameManager.TryRemovePersonFromSign(i, FindNumberOfOpenEvent());
                    //openEvent = gameManager.listSigns[FindNumberOfOpenEvent()];
                }
            }
        }
    }
    
    private void DeleteEvent()
    {
        if (FindNumberOfOpenEvent() != -1) {
            gameManager.TryRemoveSignFromList(FindNumberOfOpenEvent());
        }

        //gameManager.TryOpenMenu();
    }

    private void LoadJoinOrLeaveButton()
    {
        if (FindMyPersonName() == true)
        {
            joinButton.gameObject.SetActive(false);

            if (openEvent.ownerEvent == personInformationScript.personProfile.ReturnPersonName())
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
            if(openEvent.peopleList[i] == personInformationScript.personProfile.ReturnPersonName())
            {
                return true;
            }
        }
        return false;
    }
}
