using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginAndRegistrationAddonsScript : MonoBehaviour
{
    //[PunRPC]
    //public void SendRequesToReturnPersonNameFromServer(string playerName, string personMail, string personPassword)
    //{
    //    if (gameManager == null)
    //    {
    //        InitializationAllObjects();
    //    }

    //    Debug.Log("Server SendRequesToReturnPersonNameFromServer");

    //    string personName = registeredPersonScript.registeredPersons.ReturnRegisteredPersonFromList(personMail, personPassword).ReturnPersonName();

    //    foreach (Player player in PhotonNetwork.PlayerList)
    //    {
    //        if (player.NickName == playerName)
    //        {
    //            photonView.RPC("ReturnPersonNameFromServer", player, personName);
    //        }
    //    }
    //}

    //[PunRPC]
    //public void ReturnPersonNameFromServer(string personNameFromServer)
    //{
    //    personName = personNameFromServer;
    //}

    //public bool isThisRegisteredPerson = false;
    //public bool isThisRegisteredPersonWithoutPassword = false;

    //[PunRPC]
    //public void SendRequesToReturnIsPersonRegisteredFromServer(string playerName, string personMail, string personPassword)
    //{
    //    if (gameManager == null)
    //    {
    //        InitializationAllObjects();
    //    }

    //    //registeredPersonScript.registeredPersons.IsThisPersonRegistered(personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text))

    //    Debug.Log("Server SendRequesToReturnIsPersonRegisteredFromServer");

    //    bool isThisPersonRegisteredOnServer = registeredPersonScript.registeredPersons.IsThisPersonRegistered(personMail, personPassword);

    //    foreach (Player player in PhotonNetwork.PlayerList)
    //    {
    //        if (player.NickName == playerName)
    //        {
    //            photonView.RPC("ReturnIsPersonRegisteredFromServer", player, isThisPersonRegisteredOnServer);
    //        }
    //    }
    //}

    //[PunRPC]
    //public void SendRequesToReturnIsPersonRegisteredWithoutPasswordFromServer(string playerName, string personMail)
    //{
    //    if (gameManager == null)
    //    {
    //        InitializationAllObjects();
    //    }

    //    //registeredPersonScript.registeredPersons.IsThisPersonRegistered(personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text))

    //    Debug.Log("Server SendRequesToReturnIsPersonRegisteredWithoutPasswordFromServer");

    //    bool isThisPersonRegisteredOnServer = registeredPersonScript.registeredPersons.IsThisPersonRegistered(personMail);

    //    Debug.Log(personMail + " " + registeredPersonScript.registeredPersons.IsThisPersonRegistered(personMail));

    //    foreach (Player player in PhotonNetwork.PlayerList)
    //    {
    //        if (player.NickName == playerName)
    //        {
    //            photonView.RPC("ReturnIsPersonRegisteredWithoutPasswordFromServer", player, isThisPersonRegisteredOnServer);
    //        }
    //    }
    //}

    //[PunRPC]
    //public void ReturnIsPersonRegisteredFromServer(bool isThisPersonRegisteredOnServer)
    //{
    //    isThisRegisteredPerson = isThisPersonRegisteredOnServer;
    //}

    //[PunRPC]
    //public void ReturnIsPersonRegisteredWithoutPasswordFromServer(bool isThisPersonRegisteredOnServer)
    //{
    //    isThisRegisteredPersonWithoutPassword = isThisPersonRegisteredOnServer;
    //}
}
