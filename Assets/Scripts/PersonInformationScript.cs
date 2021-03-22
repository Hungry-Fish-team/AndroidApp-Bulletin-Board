using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;
using System.Text;

public class PersonInformationScript : MonoBehaviour
{
    RegisteredPersonScript registeredPersonScript;

    public class PersonInformation
    {
        private string personName;
        private string personPassword;
        private int personAccessLevel;
        private string personMail;

        /**
         * 1 - new person without name
         * 2 - person without password
         * 3 - registered person
        **/

        public PersonInformation()
        {
        }

        public PersonInformation(string personName, string personMail, string personPassword)
        {
            this.personName = personName;
            this.personMail = personMail;
            this.personPassword = personPassword;
            personAccessLevel = 2;
        }

        public string ReturnPersonName()
        {
            return this.personName;
        }

        public void LoadPersonName(string name)
        {
            this.personName = name;
        }

        public void SetNewPersonAccessLevel(int personAccessLevel)
        {
            this.personAccessLevel = personAccessLevel;
        }

        public int ReturnPersonAccessLevel()
        {
            return this.personAccessLevel;
        }

        public void ReloadPersonAccessLevel()
        {
            if (personPassword != null)
            {
                personAccessLevel = 2;
            }
            else
            {
                personAccessLevel = 1;
            }
        }

        public string ReturnPersonMail()
        {
            if (personMail != "")
            {
                return this.personMail;
            }
            else
            {
                return "-";
            }
        }

        public void LoadPersonMail(string personMail)
        {
            this.personMail = personMail;
        }

        public void DeleteProfile()
        {
            personPassword = null;
            personAccessLevel = 1;
        }

        public void PasswordInput(string input)
        {
            string passText = input;
            //Debug.Log(passText);
            string encryptPass = PasswordEncryption(passText);
            //Debug.Log(encryptPass);
            this.personPassword = encryptPass;
        }

        public string PasswordEncryption(string passwordString)
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

        public string ReturnPersonPassword()
        {
            return personPassword;
        }

        public string ReturnPersonEncryptedPassword()
        {
            string personName = this.personName;
            string personPassword = this.personPassword;
            string personPasswordToSave = null;
            int encryptInt = 0;

            if (personPassword != null)
            {
                if (personName != null)
                {
                    for (int i = 0; i < personName.Length; i++)
                    {
                        encryptInt += (int)personName[i];
                    }
                }
                for (int i = 0; i < personPassword.Length; i++)
                {
                    personPasswordToSave += (char)((int)personPassword[i] + encryptInt);
                }
            }
            else
            {
                personPasswordToSave = "-";
            }

            return personPasswordToSave;
        }

        public string ReturnPersonEncryptedPassword(string personPassword)
        {
            string personName = this.personName;
            string personPasswordToSave = null;
            int encryptInt = 0;

            if (personPassword != null)
            {
                for (int i = 0; i < personName.Length; i++)
                {
                    encryptInt += (int)personName[i];
                }

                for (int i = 0; i < personPassword.Length; i++)
                {
                    personPasswordToSave += (char)((int)personPassword[i] + encryptInt);
                }
            }
            else
            {
                personPasswordToSave = "-";
            }

            return personPasswordToSave;
        }

        public void LoadPersonPassword(string personPassword)
        {
            this.personPassword = personPassword;
        }

        public void LoadPersonEncryptedPassword(string password)
        {
            if (password != null)
            {
                string personName = this.personName;
                string personPassword = null;
                string personPasswordFromSave = password;
                int encryptInt = 0;

                for (int i = 0; i < personName.Length; i++)
                {
                    encryptInt += (int)personName[i];
                }

                if (password != "-")
                {
                    for (int i = 0; i < personPasswordFromSave.Length; i++)
                    {
                        personPassword += (char)((int)personPasswordFromSave[i] - encryptInt);
                    }
                }
                else
                {
                    personPassword = null;
                }

                this.personPassword = personPassword;
                //Debug.Log(personPassword);
            }
        }
    }

    public PersonInformation personProfile = new PersonInformation();

    string fileForProfileSave;

    private void LoadFiles()
    {
        if (!File.Exists(Application.persistentDataPath + "/fileForProfileData.json"))
        {
            CreateFilesForSave("/fileForProfileData.json");
        }
        fileForProfileSave = Application.persistentDataPath + "/fileForProfileData.json";
    }

    private void CreateFilesForSave(string nameOfFile)
    {
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
    }

    public void SaveAllDataToFile()
    {
        JSONObject personDATA = new JSONObject();

        personDATA.Add("PersonName", personProfile.ReturnPersonName());
        personDATA.Add("PersonMail", personProfile.ReturnPersonMail());
        personDATA.Add("PersonPassword", personProfile.ReturnPersonEncryptedPassword());

        if (File.Exists(fileForProfileSave))
        {
            File.WriteAllText(fileForProfileSave, personDATA.ToString());
        }

        Debug.Log("Person Save");
    }

    public void LoadAllDataFromFile()
    {
        LoadFiles();

        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave));

            if (personDATA != null)
            {
                personProfile.LoadPersonName(personDATA["PersonName"]);
                personProfile.SetNewPersonAccessLevel(1);
                personProfile.LoadPersonMail(personDATA["PersonMail"]);
                personProfile.LoadPersonEncryptedPassword(personDATA["PersonPassword"]);
            }
            else
            {
                personProfile.SetNewPersonAccessLevel(0);
            }
        }
    }

    private void InitializationAllObjects()
    {
        registeredPersonScript = GameObject.Find("GameManager").GetComponent<RegisteredPersonScript>();

        LoadAllDataFromFile();
    }

    private void OnEnable()
    {
        InitializationAllObjects();
    }
}
