using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    GameManager gameManager;
    PersonInformationScript personInformationScript;

    [SerializeField]
    InputField namePersonInputField;
    [SerializeField]
    Button saveButton;

    [SerializeField]
    string namePerson;

    private void OnEnable()
    {
        Debug.Log("Enable");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();

        namePerson = personInformationScript.personName;
        namePersonInputField.text = namePerson;
    }

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        InitializationAllObjects();
    }

    private void Update()
    {
        ReadInputField();
    }

    private void ReadInputField()
    {
        if (namePersonInputField.text != "" || namePerson != "")
        {
            namePerson = namePersonInputField.text;

            saveButton.interactable = true;
        }
        else
        {
            namePerson = "";

            saveButton.interactable = false;
            Debug.Log("Login is null");
        }
    }

    public void SaveLogin()
    {
        if (!PhotonNetwork.NickName.Contains(namePerson))
        {
            PhotonNetwork.NickName = namePerson;

            personInformationScript.personName = namePerson;

            personInformationScript.SaveAllDataToFile();

            gameManager.OpenMenuObject();

        }
        else
        {
            Debug.Log(PhotonNetwork.NickName + " " + namePerson);
            StartCoroutine(gameManager.ErrorOrInfoFunc("Your name is already in use"));
        }
    }
}
