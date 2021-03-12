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

public class LoginAndRegistrationScript : MonoBehaviour
{
    GameManager gameManager;
    PersonInformationScript personInformationScript;
    RegistredPersonScript registredPersonScript;

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
    string personName;
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
        registredPersonScript = GameObject.Find("GameManager").GetComponent<RegistredPersonScript>();

        personName = personInformationScript.personProfile.ReturnPersonName();
        loginAndRegistrationInputField.text = personName;
    }

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        InitializationAllObjects();

        if (personInformationScript.personProfile.ReturnPersonAccessLevel() < 2)
        {
            FirstStateOfMailConf();
        }
        else
        {
            FifthStateOfMailConf();
        }
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
                if (mailInputField.text != null)
                {
                    if(waitingCode == true)
                    {
                        WaitingTrueCode(personMail, newCode);
                    }
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

            registredPersonScript.registeredPersons.ChangePersonName(personInformationScript.personProfile, personName);

            registredPersonScript.registeredPersons.SaveAllDataToFile();

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

    private void FirstStateOfMailConf()
    {
        //Debug.Log("First");
        mailInputField.gameObject.SetActive(true);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(true);
        sendCodeAgainButton.interactable = true;
        passwordInputField.gameObject.SetActive(false);
        registerButton.interactable = false;
    }

    private void SecondStateOfMailConf()
    {
        //Debug.Log("Second");
        mailInputField.gameObject.SetActive(true);
        mailInputField.interactable = false;
        mailCodeInputField.gameObject.SetActive(true);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(false);
        registerButton.interactable = false;
    }

    private void ThirdStateOfMailConf()
    {
        //Debug.Log("Third");
        mailInputField.gameObject.SetActive(true);
        mailInputField.interactable = false;
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(true);
        registerButton.interactable = true;
    }

    private void FourthStateOfMailConf()
    {
        //Debug.Log("Fourt");
        mailInputField.gameObject.SetActive(false);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(false);
        registerButton.interactable = false;
    }

    private void FifthStateOfMailConf()
    {
        //Debug.Log("Fifth");
        mailInputField.gameObject.SetActive(false);
        mailCodeInputField.gameObject.SetActive(false);
        sendCodeAgainButton.gameObject.SetActive(false);
        passwordInputField.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
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
            personInformationScript.personProfile.PasswordInput(passwordInputField.text);
            personInformationScript.personProfile.SetNewPersonAccessLevel(2);
            personInformationScript.SaveAllDataToFile();

            registredPersonScript.registeredPersons.AddNewRegisteredPerson(personInformationScript.personProfile);

            registredPersonScript.registeredPersons.SaveAllDataToFile();

            FourthStateOfMailConf();
        }
    }
}
