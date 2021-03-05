using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    bool isNetworkAllow;
    [SerializeField]
    string versionNum;
    [SerializeField]
    Image BGLoad_image;

    public IEnumerator checkInternetConnection()
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            isNetworkAllow = false;
            BGLoad_image.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Connected");
            isNetworkAllow = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isNetworkAllow == true) {
            Debug.Log("Connect to Master " + PhotonNetwork.CloudRegion);

            BGLoad_image.gameObject.SetActive(false);

            if (GetComponent<PersonInformationScript>().personProfile.ReturnPersonName() != "") {
                PhotonNetwork.NickName = GetComponent<PersonInformationScript>().personProfile.ReturnPersonName();
            }
            else
            {
                PhotonNetwork.NickName = "player" + Random.Range(0, 1000).ToString();
            }

            PhotonNetwork.JoinRandomRoom();
        }
    }

    private void InitializationAllObjects()
    {
        StartCoroutine(checkInternetConnection());

        versionNum = Application.version;

        string nickName = "Player" + Random.Range(0, 100);

        PhotonNetwork.AutomaticallySyncScene = true;
        //PhotonNetwork.NickName = nickName;
        PhotonNetwork.GameVersion = versionNum;
        //PhotonNetwork.AppVersion = versionNum;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("ru");
    }

    private void Start()
    {
        InitializationAllObjects();

        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Ready");
        }

        //PhotonPeer.RegisterType(Type customType, byte code, SerializeMethod serializeMethod, DeserializeMethod deserializeMethod)
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        CreateRoom();
    }

    public void CreateRoom()
    {
        int rand = Random.Range(0, 1000);
        int RoomSize = 1000;
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)RoomSize,
        };
        PhotonNetwork.CreateRoom("Room" + rand.ToString(), roomOps);

        Debug.Log("Room is create");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    //public static object DeserializeSpriteIcon(byte[] data)
    //{
    //    var result = Sprite.Create(data.);
    //    return result;
    //}

    //public static byte[] SerializeSpriteIcon(object customType)
    //{
    //    var icon = (Sprite)customType;
    //    return icon.texture.GetRawTextureData();
    //}
}
