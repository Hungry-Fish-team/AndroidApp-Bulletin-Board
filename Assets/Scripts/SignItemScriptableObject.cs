using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SignItemScriptableObject", menuName = "SignItemScriptableObject", order = 51)]

public class SignItemScriptableObject : ScriptableObject
{
    public string nameEventText;
    public string placeNameText;
    public string dateTimeText;
    public string infoEventText;
    public List<string> peopleList;
    public Sprite icon;
    public string ownerEvent;
}
