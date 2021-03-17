﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAccessScript : MonoBehaviour
{
    PersonInformationScript personInformationScript;

    [SerializeField]
    Button createNewEventButton;
    [SerializeField]
    Button joinedEventButton;
    [SerializeField]
    Button loginButton;
    [SerializeField]
    Text appVersionText;

    private void InitializationAllObjects()
    {
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();

        appVersionText.text = Application.version;
    }

    private void OnEnable()
    {
        InitializationAllObjects();
        AccessInitialization();
    }

    private void FirstLevelAccess()
    {
        createNewEventButton.interactable = false;
        joinedEventButton.interactable = true;
        loginButton.interactable = true;
    }

    private void SecondLevelAccess()
    {
        createNewEventButton.interactable = true;
        joinedEventButton.interactable = true;
        loginButton.interactable = true;
    }

    private void AccessInitialization()
    {
        if(personInformationScript.personProfile.ReturnPersonAccessLevel() >= 2)
        {
            SecondLevelAccess();
        }
        else
        {
            FirstLevelAccess();
        }
    }
}