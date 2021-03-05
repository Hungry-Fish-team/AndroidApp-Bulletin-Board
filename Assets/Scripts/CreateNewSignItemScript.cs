using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewSignItemScript : MonoBehaviour
{
    GameManager gameManager;
    PersonInformationScript personInformationScript;

    [SerializeField]
    SignItemScriptableObject signItemScriptableObject;

    [SerializeField]
    InputField nameEventInputField;
    [SerializeField]
    InputField placeNameInputField;
    [SerializeField]
    InputField dateTimeInputField;
    [SerializeField]
    Image spriteImage;
    [SerializeField]
    InputField infoEventInputField;
    [SerializeField]
    Button saveButton;

    [SerializeField]
    string nameEvent;
    [SerializeField]
    string placeName;
    [SerializeField]
    string dateTime;
    [SerializeField]
    Sprite image;
    [SerializeField]
    string infoEvent;

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();

        signItemScriptableObject = new SignItemScriptableObject();
    }

    void Start()
    {
        InitializationAllObjects();
    }

    private void Update()
    {
        CheckInputField();
    }

    void CheckInputField()
    {
        if(nameEventInputField.text != "" && placeNameInputField.text != "" && dateTimeInputField.text != "" && infoEventInputField.text != "" && spriteImage.sprite != null)
        {
            nameEvent = nameEventInputField.text;
            placeName = placeNameInputField.text;
            dateTime = dateTimeInputField.text;
            image = spriteImage.sprite;
            infoEvent = infoEventInputField.text;

            saveButton.interactable = true;
        }
        else
        {
            saveButton.interactable = false;
            Debug.Log("Empty All lines");
        }
    }

    public void SaveInfoAboutEvent()
    {
        if (personInformationScript.personProfile.ReturnPersonName() != "") {
            WriteInfo();

            //gameManager.listSigns.Add(signItemScriptableObject);

            gameManager.TryAddNewSignFunc(signItemScriptableObject);

            gameManager.OpenMenuObject();

            gameManager.ReloadObjestForDashboard();
        }
        else
        {
            string text = "Your name is null";
            StartCoroutine(gameManager.ErrorOrInfoFunc(text));
        }
    }

    private void WriteInfo()
    {
        signItemScriptableObject = new SignItemScriptableObject();

        signItemScriptableObject.name = (gameManager.listSigns.Count + 1).ToString();

        signItemScriptableObject.nameEventText = nameEvent;
        signItemScriptableObject.placeNameText = placeName;
        signItemScriptableObject.dateTimeText = dateTime;
        signItemScriptableObject.icon = image;
        signItemScriptableObject.infoEventText = infoEvent;
        signItemScriptableObject.ownerEvent = personInformationScript.personProfile.ReturnPersonName();

        if (personInformationScript.personProfile.ReturnPersonName() != "")
        {
            signItemScriptableObject.peopleList = new List<string>();
            signItemScriptableObject.peopleList.Add(personInformationScript.personProfile.ReturnPersonName());
        }
        else
        {
            Debug.Log("Login is null");
        }
    }

    private void OnDisable()
    {
        nameEventInputField.text = "";
        placeNameInputField.text = "";
        dateTimeInputField.text = "";
        spriteImage.sprite = null;
        infoEventInputField.text = "";

        nameEvent = "";
        placeName = "";
        dateTime = "";
        image = null;
        infoEvent = "";
    }
}
