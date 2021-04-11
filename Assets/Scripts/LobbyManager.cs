using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject gameManager;
    [SerializeField]
    GameObject AdminEnter;

    [SerializeField]
    public bool isNetworkAllow;
    [SerializeField]
    public bool isConnectedToMaster;
    [SerializeField]
    public bool isServerAndRoomWork = false;
    [SerializeField]
    public bool isServerClose = false;
    [SerializeField]
    string versionNum;
    [SerializeField]
    Image BGLoad_image;

    [SerializeField]
    public Button closeServerButton;

    [SerializeField]
    InputField adminMailInputField;
    [SerializeField]
    InputField adminPasswordInputField;
    [SerializeField]
    Button startServerButton;
    [SerializeField]
    Text countDownOfReJoinText;

    private class AdministratorClass : MonoBehaviourPunCallbacks
    {
        private readonly string adminMail = "1307b01a22409f072759939ee98a65c8d7b64f858d047d71e55e6f1f93108daf";
        private readonly string adminPassword = "0fdce5bdbdc7ed5c8c7bb3a14a9c77aa7e062bc9798ee5d30ae216223cfc1e58";

        private string inputAdminMail;
        private string inputAdminPassword;

        private void ReadInputFields(string adminMail, string adminPassword)
        {
            if (adminMail != "" && adminPassword != "")
            {
                inputAdminMail = adminMail;
                inputAdminPassword = adminPassword;
            }
        }

        public bool CheckInputField(string adminMail, string adminPassword)
        {
            ReadInputFields(adminMail, adminPassword);

            if (PasswordEncryption(inputAdminMail) == this.adminMail)
            {
                if (PasswordEncryption(inputAdminPassword) == this.adminPassword)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator CheckingInutField(string adminMail, string adminPassword, Button closeServerButton)
        {
            yield return new WaitUntil(() => CheckInputField(adminMail, adminPassword) == true);

            CreateRoom();

            yield return new WaitUntil(() => PhotonNetwork.InRoom);

            PhotonNetwork.CurrentRoom.SetMasterClient(PhotonNetwork.PlayerList[0]);

            closeServerButton.gameObject.SetActive(true);
        }

        private string PasswordEncryption(string passwordString)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(passwordString);

            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] hashBytes = sha256.ComputeHash(bytes);

            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        private void CreateRoom()
        {
            int RoomSize = 1000;
            RoomOptions roomOps = new RoomOptions()
            {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = (byte)RoomSize,
            };
            PhotonNetwork.CreateRoom("MainRoom", roomOps, new TypedLobby("Default", LobbyType.Default));

            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);

            Debug.Log("Room is create");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            CreateRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            CreateRoom();
        }
    }

    public IEnumerator CheckInternetConnection()
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
        if (isNetworkAllow == true)
        {
            Debug.Log("Connect to Master " + PhotonNetwork.CloudRegion);

            isConnectedToMaster = true;
        }

        StartCoroutine(OpenAdminPanel());
    }

    IEnumerator OpenAdminPanel()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        yield return new WaitUntil(() => PhotonNetwork.InLobby);

        if (PhotonNetwork.InLobby && PhotonNetwork.CountOfRooms != 1)
        {
            AdminEnter.SetActive(true);

            //isServerAndRoomWork = true;
            gameManager.SetActive(false);
        }
        BGLoad_image.gameObject.SetActive(false);
    }

    IEnumerator WaitingResult()
    {
        yield return new WaitUntil(() => isConnectedToMaster == true);

        if (isConnectedToMaster == true)
        {
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

            if (!PhotonNetwork.InRoom)
            {
                yield return new WaitUntil(() => PhotonNetwork.InLobby);
                if (PhotonNetwork.InLobby)
                {
                    yield return new WaitUntil(() => PhotonNetwork.CountOfRooms > 0);
                    if (PhotonNetwork.CountOfRooms > 0)
                    {
                        PhotonNetwork.JoinRoom("MainRoom");

                        isServerAndRoomWork = true;
                        gameManager.SetActive(true);
                        AdminEnter.SetActive(false);
                    }
                }
            }
            else
            {
                isServerAndRoomWork = true;
                gameManager.SetActive(true);
                AdminEnter.SetActive(false);
            }
        }
    }

    public void Update()
    {
        //Debug.Log(PhotonNetwork.InLobby + " in Lobby");
        //Debug.Log(PhotonNetwork.InRoom + " in Room");
        //Debug.Log(PhotonNetwork.CountOfRooms + " in Room");

        //if (PhotonNetwork.CurrentRoom != null)
        //{
        //    Debug.Log(PhotonNetwork.MasterClient);
        //    Debug.Log(PhotonNetwork.LocalPlayer);
        //}

        //Debug.Log(PhotonNetwork.MasterClient);

        //foreach (Player player in PhotonNetwork.PlayerList)
        //{
        //    Debug.Log(player.NickName);
        //}
    }

    private IEnumerator ReJoinToRoom()
    {
        StopCoroutine("ReJoinToRoom");

        yield return new WaitForSeconds(10f);

        if (PhotonNetwork.InRoom != true)
        {

            if (isServerAndRoomWork != true)
            {
                Debug.Log("ReJoin");

                Debug.Log("ReJoinSecondState");
                if (PhotonNetwork.CountOfRooms > 0)
                {
                    StartCoroutine("WaitingResult");
                }

                StartCoroutine(ReJoinToRoom());
            }
        }
    }

    //private IEnumerator Countdown(float duration)
    //{
    //    countDownOfReJoinText.text = "try rejoin after: " + (int)duration;
    //    while (duration > 0)
    //    {
    //        duration = -Time.deltaTime;
    //        yield return null;
    //    }
    //}

    public void StartServer()
    {
        AdministratorClass administrator = new AdministratorClass();

        StopCoroutine("CheckingInutField");
        StopCoroutine("WaitingResult");

        StartCoroutine(administrator.CheckingInutField(adminMailInputField.text, adminPasswordInputField.text, closeServerButton));
        StartCoroutine("WaitingResult");
    }

    private void InitializationAllObjects()
    {
        StartCoroutine(CheckInternetConnection());

        versionNum = Application.version;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Person" + Random.Range(0, 100).ToString();
        PhotonNetwork.GameVersion = versionNum;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("ru");
    }

    private void Start()
    {
        InitializationAllObjects();

        StartCoroutine("WaitingResult");
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnApplicationQuit()
    {
        isServerClose = true;
        CloseSeverFunc();
    }

    public override void OnDisable()
    {
        isServerClose = true;
        CloseSeverFunc();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void CloseSeverFunc()
    {
        if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer)
        {
            Debug.Log("ServClose");

            transform.GetComponent<PhotonView>().RPC("CloseServer", RpcTarget.All);

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PhotonNetwork.CloseConnection(player);
            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.RemovedFromList = true;
        }
    }

    [PunRPC]
    void CloseServer()
    {
        if (gameManager != null)
        {
            if (gameManager.activeSelf == true)
            {
                if (gameManager.GetComponent<GameManager>().eventName == "Menu")
                {
                    gameManager.GetComponent<GameManager>().OpenMenuObject();
                }
                gameManager.SetActive(false);
            }
        }

        isConnectedToMaster = false;
        isServerAndRoomWork = false;

        AdminEnter.SetActive(true);

        adminMailInputField.text = "";
        adminPasswordInputField.text = "";

        if (isServerClose != true)
        {
            StartCoroutine(ReJoinToRoom());
        }
    }
}
