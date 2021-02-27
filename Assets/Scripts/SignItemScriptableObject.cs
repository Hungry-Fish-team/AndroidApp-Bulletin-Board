using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SignItemScriptableObject", menuName = "SignItemScriptableObject", order = 51)]

public class SignItemScriptableObject : ScriptableObject
{
    [SerializeField]
    public string nameEventText;
    [SerializeField]
    public string placeNameText;
    [SerializeField]
    public string dateTimeText;
    [SerializeField]
    public string infoEventText;
    [SerializeField]
    public List<string> peopleList;
    [SerializeField]
    public Sprite icon;
    [SerializeField]
    public string ownerEvent;
}
