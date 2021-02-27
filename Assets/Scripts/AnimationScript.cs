using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    GameManager gameManager;

    public GameObject menuObject;
    public GameObject eventObject;
    public GameObject createNewEventObject;
    public GameObject loginObject;

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        InitializationAllObjects();
    }

    public void OpenOrCloseObjects(string eventName)
    {
        if (eventName == "Menu")
        {
            menuObject.SetActive(true);
            menuObject.GetComponent<Animator>().SetBool("isClosed", false);
        }
        else
        if (eventName == "Event")
        {
            eventObject.SetActive(true);
            eventObject.GetComponent<Animator>().SetBool("isClosed", false);
        }
        else
        if (eventName == "NewItem")
        {
            createNewEventObject.SetActive(true);
            createNewEventObject.GetComponent<Animator>().SetBool("isClosed", false);
        }
        else
        if (eventName == "Login")
        {
            loginObject.SetActive(true);
            loginObject.GetComponent<Animator>().SetBool("isClosed", false);
        }
        else
        {
            eventObject.GetComponent<Animator>().SetBool("isClosed", true);
            menuObject.GetComponent<Animator>().SetBool("isClosed", true);
            createNewEventObject.GetComponent<Animator>().SetBool("isClosed", true);
            CloseImageManager();
            loginObject.GetComponent<Animator>().SetBool("isClosed", true);
            Invoke("CloseObjects", 0.5f);
        }
    }

    void CloseObjects()
    {
        menuObject.SetActive(false);
        eventObject.SetActive(false);
        createNewEventObject.SetActive(false);
        loginObject.SetActive(false);
    }

    void CloseImageManager()
    {
        if (GameObject.Find("BG_FileList") != null) {
            GameObject parent = GameObject.Find("BG_FileList").transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
            GameObject.Find("BG_FileList").SetActive(false);
        }
    }
}
