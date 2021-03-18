using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mime;
using Photon.Realtime;

public class LoginAndRegistrationScript : MonoBehaviour
{
    GameManager gameManager;
    PersonInformationScript personInformationScript;
    RegisteredPersonScript registeredPersonScript;

    PhotonView photonView;

    [SerializeField]
    InputField loginAndRegistrationInputField;
    [SerializeField]
    Button saveButton;
    [SerializeField]
    InputField mailInputField;
    [SerializeField]
    InputField mailCodeInputField;
    [SerializeField]
    Button sendCodeAgainButton;
    [SerializeField]
    InputField passwordInputField;
    [SerializeField]
    Button registerButton;
    [SerializeField]
    Button leaveFormProfileButton;
    [SerializeField]
    Button deleteProfileButton;
    [SerializeField]
    Button joinProfileButton;
    [SerializeField]
    Text outputText;

    [SerializeField]
    public string personName;
    [SerializeField]
    string personPassword;
    [SerializeField]
    string personMail;
    [SerializeField]
    int newCode;

    private void OnEnable()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        personInformationScript = GameObject.Find("GameManager").GetComponent<PersonInformationScript>();
        registeredPersonScript = GameObject.Find("GameManager").GetComponent<RegisteredPersonScript>();
        photonView = GetComponent<PhotonView>();

        photonView.ControllerActorNr = GameObject.Find("GameManager").GetComponent<PhotonView>().ControllerActorNr;

        personName = personInformationScript.personProfile.ReturnPersonName();
        loginAndRegistrationInputField.text = personName; 
        mailInputField.text = personInformationScript.personProfile.ReturnPersonMail();

        ReloadedInformationForInputField();
    }

    void ReloadedInformationForInputField()
    {
        if (personInformationScript.personProfile.ReturnPersonAccessLevel() < 2)
        {
            loginAndRegistrationInputField.text = personName;
            if (personInformationScript.personProfile.ReturnPersonMail() == null)
            {
                mailInputField.text = "";

                FirstStateOfMailConf();
            }
            else
            {
                mailInputField.text = personInformationScript.personProfile.ReturnPersonMail();
                passwordInputField.text = "";

                if(registeredPersonScript.registeredPersons.IsThisPersonRegistered(mailInputField.text)){

                    ThirdStateOfMailConf();
                }
                else
                {
                    FirstStateOfMailConf();
                }
            }
        }
        else
        {
            FifthStateOfMailConf();
        }
    }

    void InitializationAllObjects()
    {
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
        if (loginAndRegistrationInputField.text != null || personName != null)
        {
            personName = loginAndRegistrationInputField.text;

            if (personInformationScript.personProfile.ReturnPersonAccessLevel() < 2)
            {
                if (mailInputField.text != "")
                {
                    photonView.RPC("SendRequesToReturnIsPersonRegisteredWithoutPasswordFromServer", RpcTarget.MasterClient, PhotonNetwork.NickName, mailInputField.text);
                    
                    if (isThisRegisteredPersonWithoutPassword == true)
                    {
                        isThisRegisteredPersonWithoutPassword = false;

                        SixthStateOfMailConf();
                    } 
                    else if(waitingCode == true)
                    {
                        WaitingTrueCode(personMail, newCode);
                    }
                }
                else
                {
                    FirstStateOfMailConf();
                }
            }
            else
            {
                FifthStateOfMailConf();
            }

            saveButton.interactable = true;
        }
        else
        {
            personName = null;
            personPassword = null;

            saveButton.interactable = false;
            Debug.Log("Login is null");
        }
    }

    public void SaveLogin()
    {
        //if (!PhotonNetwork.NickName.Contains(namePerson))
        {
            PhotonNetwork.NickName = personName;

            registeredPersonScript.registeredPersons.ChangePersonName(personInformationScript.personProfile, personName);

            registeredPersonScript.registeredPersons.SaveAllDataToFile();

            personInformationScript.personProfile.LoadPersonName(personName);

            personInformationScript.SaveAllDataToFile();

            gameManager.OpenMenuObject();

        }
        //else
        //{
        //    Debug.Log(PhotonNetwork.NickName + " " + namePerson);
        //    StartCoroutine(gameManager.ErrorOrInfoFunc("Your name is already in use"));
        //}
    }

    [SerializeField]
    private string appsMail;
    [SerializeField]
    private string appsMailPassword;

    bool waitingCode = false;

    private IEnumerator OutputInfo(string text)
    {
        outputText.gameObject.SetActive(true);
        outputText.text = text;

        yield return new WaitForSeconds(1.5f);

        outputText.text = "";
        outputText.gameObject.SetActive(true);
    }

    private void FirstStateOfMailConf()
    {
        //Debug.Log("First");
        mailInputField.gameObject.SetActive(true);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(true);
        sendCodeAgainButton.interactable = true;
        passwordInputField.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        registerButton.interactable = false;
        leaveFormProfileButton.gameObject.SetActive(false);
        deleteProfileButton.gameObject.SetActive(false);
        joinProfileButton.gameObject.SetActive(false);
    }

    private void SecondStateOfMailConf()
    {
        //Debug.Log("Second");
        mailInputField.gameObject.SetActive(true);
        //mailInputField.interactable = false;
        mailCodeInputField.gameObject.SetActive(true);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        registerButton.interactable = false;
        leaveFormProfileButton.gameObject.SetActive(false);
        deleteProfileButton.gameObject.SetActive(false);
        joinProfileButton.gameObject.SetActive(false);
    }

    private void ThirdStateOfMailConf()
    {
        //Debug.Log("Third");
        mailInputField.gameObject.SetActive(true);
        //mailInputField.interactable = false;
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(true);
        registerButton.gameObject.SetActive(true);
        registerButton.interactable = true;
        leaveFormProfileButton.gameObject.SetActive(false);
        deleteProfileButton.gameObject.SetActive(false);
        joinProfileButton.gameObject.SetActive(false);
    }

    private void FourthStateOfMailConf()
    {
        //Debug.Log("Fourt");
        mailInputField.gameObject.SetActive(false);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(false);
        registerButton.interactable = false;
        leaveFormProfileButton.gameObject.SetActive(false);
        deleteProfileButton.gameObject.SetActive(false);
        joinProfileButton.gameObject.SetActive(false);
    }

    private void FifthStateOfMailConf()
    {
        //Debug.Log("Fifth");
        mailInputField.gameObject.SetActive(false);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        leaveFormProfileButton.gameObject.SetActive(true);
        deleteProfileButton.gameObject.SetActive(true);
        joinProfileButton.gameObject.SetActive(false);
    }

    private void SixthStateOfMailConf()
    {
        //Debug.Log("Fifth");
        mailInputField.gameObject.SetActive(true);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(true);
        registerButton.gameObject.SetActive(false);
        leaveFormProfileButton.gameObject.SetActive(false);
        deleteProfileButton.gameObject.SetActive(false);
        joinProfileButton.gameObject.SetActive(true);
    }

    private string FindTypeOfProfileMail(string mail)
    {
        if (mail != "")
        {
            string typeOfProfileMail = mail.Remove(0, (mail.IndexOf("@") + 1));
            return typeOfProfileMail;
        }
        return null;
    }

    public void SendMessageToProfileMail(string profileMail, string code)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.Body = CreateMailConfirmMessage(code);

        mailMessage.Subject = "Подтверждение почты. Приложение ***";
        mailMessage.From = new MailAddress(appsMail);
        mailMessage.To.Add(profileMail);
        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

        SmtpClient client = new SmtpClient();
        client.Host = "smtp." + FindTypeOfProfileMail(appsMail);
        client.Port = 587;
        client.Credentials = new NetworkCredential(mailMessage.From.Address, appsMailPassword);
        client.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

        client.Send(mailMessage);
    }

    public string CreateMailConfirmMessage(string code)
    {
        string message = "Код для подтверждения почты: " + code + "\r\n" + "C уважением, часть кода отправки кодов на почту";
        return message;
    }

    private void MailConfirmationFunc()
    {
        personMail = mailInputField.text;

        if (personMail != "")
        {
            newCode = GenerateNewCode();

            SecondStateOfMailConf();

            SendMessageToProfileMail(personMail, newCode.ToString());

            mailCodeInputField.text = "";

            waitingCode = true;
        }
    }

    private void WaitingTrueCode(string personMail, int codeToConfirm)
    {
        Debug.Log("Waiting Code");
        if (mailCodeInputField.text != codeToConfirm.ToString())
        {
            mailCodeInputField.transform.GetChild(1).GetComponent<Text>().color = Color.red;
        }
        if (mailCodeInputField.text == codeToConfirm.ToString())
        {
            personInformationScript.personProfile.LoadPersonMail(personMail);

            ThirdStateOfMailConf();
            waitingCode = false;
        }
    }

    private int GenerateNewCode()
    {
        return Random.Range(100000, 999999);
    }

    public void SendNewCode()
    {
        MailConfirmationFunc();
    }

    public void RegisterFunc()
    {
        if (personInformationScript.personProfile.ReturnPersonMail() != "" && passwordInputField.text != null)
        {
            personInformationScript.personProfile.LoadPersonName("Person№" + Random.Range(0, 1000).ToString());
            
            personInformationScript.personProfile.PasswordInput(passwordInputField.text);

            personInformationScript.personProfile.SetNewPersonAccessLevel(2);

            personInformationScript.SaveAllDataToFile();

            photonView.RPC("RegisterFuncToServer", RpcTarget.MasterClient, 
                personInformationScript.personProfile.ReturnPersonName(),
                personInformationScript.personProfile.ReturnPersonMail(),
                personInformationScript.personProfile.ReturnPersonPassword());

            //registeredPersonScript.registeredPersons.AddNewRegisteredPerson(personInformationScript.personProfile);

            //registeredPersonScript.registeredPersons.SaveAllDataToFile();

            loginAndRegistrationInputField.text = personInformationScript.personProfile.ReturnPersonName();

            FourthStateOfMailConf();
        }
    }

    [PunRPC]
    public void RegisterFuncToServer(string personName, string personMail, string personPassword)
    {
        Debug.Log("Server");

        registeredPersonScript.registeredPersons.AddNewRegisteredPerson(personName, personMail, personPassword);

        registeredPersonScript.registeredPersons.SaveAllDataToFile();
    }

    public void LeaveFromProfileFunc()
    {
        personInformationScript.personProfile.DeleteProfile();

        personInformationScript.SaveAllDataToFile();

        ReloadedInformationForInputField();
    }

    public void DeleteProfileFunc()
    {
        photonView.RPC("DeleteProfileFuncToServer", RpcTarget.MasterClient,
            personInformationScript.personProfile.ReturnPersonName(),
            personInformationScript.personProfile.ReturnPersonMail(),
            personInformationScript.personProfile.ReturnPersonPassword());

        //registeredPersonScript.registeredPersons.DeleteRegisteredPerson(personMail);

        //registeredPersonScript.registeredPersons.ReturnAllRegisteredPersonsToConsole();

        //registeredPersonScript.registeredPersons.SaveAllDataToFile();

        personInformationScript.personProfile.DeleteProfile();

        personInformationScript.SaveAllDataToFile();

        FirstStateOfMailConf();
    }

    [PunRPC]
    public void DeleteProfileFuncToServer(string personName, string personMail, string personPassword)
    {
        Debug.Log("Server");

        registeredPersonScript.registeredPersons.DeleteRegisteredPerson(personMail);

        //registeredPersonScript.registeredPersons.ReturnAllRegisteredPersonsToConsole();

        registeredPersonScript.registeredPersons.SaveAllDataToFile();
    }

    public void JoinProfile()
    {
        if(personInformationScript.personProfile.ReturnPersonMail() != "" && passwordInputField.text != null)
        {
            personInformationScript.personProfile.LoadPersonMail(mailInputField.text);

            //Debug.Log(registeredPersonScript.registeredPersons.IsThisPersonRegistered(personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text)));

            //Debug.Log(personInformationScript.personProfile.PasswordEncryption(passwordInputField.text));

            photonView.RPC("SendRequesToReturnIsPersonRegisteredFromServer", RpcTarget.MasterClient, PhotonNetwork.NickName, personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text));

            if (isThisRegisteredPerson == true)
            {
                isThisRegisteredPerson = false;

                //string personName = registeredPersonScript.registeredPersons.ReturnRegisteredPersonFromList(personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text)).ReturnPersonName();

                photonView.RPC("SendRequesToReturnPersonNameFromServer", RpcTarget.MasterClient, PhotonNetwork.NickName, personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text));

                personInformationScript.personProfile.LoadPersonName(personName);

                loginAndRegistrationInputField.text = personInformationScript.personProfile.ReturnPersonName();
                
                personInformationScript.personProfile.PasswordInput(passwordInputField.text);

                personInformationScript.personProfile.SetNewPersonAccessLevel(2);

                personInformationScript.SaveAllDataToFile();

                FourthStateOfMailConf();
            }
            else
            {
                StartCoroutine(OutputInfo("Incorect password"));
            }
        }
    }

    [PunRPC]
    public void SendRequesToReturnPersonNameFromServer(string playerName, string personMail, string personPassword)
    {
        Debug.Log("Server SendRequesToReturnPersonNameFromServer");

        string personName = registeredPersonScript.registeredPersons.ReturnRegisteredPersonFromList(personMail, personPassword).ReturnPersonName();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if(player.NickName == playerName)
            {
                photonView.RPC("ReturnPersonNameFromServer", player, personName);
            }
        }
    }

    [PunRPC]
    public void ReturnPersonNameFromServer(string personNameFromServer)
    {
        personName = personNameFromServer;
    }

    public bool isThisRegisteredPerson = false;
    public bool isThisRegisteredPersonWithoutPassword = false;

    [PunRPC]
    public void SendRequesToReturnIsPersonRegisteredFromServer(string playerName, string personMail, string personPassword)
    {
        //registeredPersonScript.registeredPersons.IsThisPersonRegistered(personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text))

        Debug.Log("Server SendRequesToReturnIsPersonRegisteredFromServer");

        bool isThisPersonRegisteredOnServer = registeredPersonScript.registeredPersons.IsThisPersonRegistered(personMail, personPassword);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName)
            {
                photonView.RPC("ReturnIsPersonRegisteredFromServer", player, isThisPersonRegisteredOnServer);
            }
        }
    }

    [PunRPC]
    public void SendRequesToReturnIsPersonRegisteredWithoutPasswordFromServer(string playerName, string personMail)
    {
        //registeredPersonScript.registeredPersons.IsThisPersonRegistered(personInformationScript.personProfile.ReturnPersonMail(), personInformationScript.personProfile.PasswordEncryption(passwordInputField.text))

        Debug.Log("Server SendRequesToReturnIsPersonRegisteredWithoutPasswordFromServer");

        bool isThisPersonRegisteredOnServer = registeredPersonScript.registeredPersons.IsThisPersonRegistered(personMail);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName)
            {
                photonView.RPC("ReturnIsPersonRegisteredWithoutPasswordFromServer", player, isThisPersonRegisteredOnServer);
            }
        }
    }

    [PunRPC]
    public void ReturnIsPersonRegisteredFromServer(bool isThisPersonRegisteredOnServer)
    {
        isThisRegisteredPerson = isThisPersonRegisteredOnServer;
    }

    [PunRPC]
    public void ReturnIsPersonRegisteredWithoutPasswordFromServer(bool isThisPersonRegisteredOnServer)
    {
        isThisRegisteredPersonWithoutPassword = isThisPersonRegisteredOnServer;
    }
}
