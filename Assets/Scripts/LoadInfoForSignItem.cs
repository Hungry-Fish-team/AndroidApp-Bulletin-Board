using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadInfoForSignItem : MonoBehaviour
{
    GameManager gameManager;

    public SignItemScriptableObject signItem;

    [SerializeField]
    Text nameEventText;
    [SerializeField]
    Text placeNameText;
    [SerializeField]
    Text dateTimeText;
    [SerializeField]
    Text infoEventText;
    [SerializeField]
    Image icon;

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        InitializationAllObjects();
        LoadInfoFromSignItem();
    }

    void LoadInfoFromSignItem()
    {
        nameEventText.text = signItem.nameEventText;
        infoEventText.text = signItem.infoEventText;
        dateTimeText.text = signItem.dateTimeText;
        if (signItem.icon != null)
        {
            icon.sprite = signItem.icon;
        }
    }

    public void LoadAllInfoAboutSignItem()
    {
        gameManager.OpenEventObject(signItem);
    }
}
