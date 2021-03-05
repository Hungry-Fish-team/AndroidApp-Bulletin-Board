using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;
using System.Text;

public class PersonInformationScript : MonoBehaviour
{
    public class personInformation
    {
        private string personName;
        private string personPassword;
        private int personAccessLevel;

        //public personInformation()
        //{
        //    personName = null;
        //    personPassword = null;
        //    personAccessLevel = 0;
        //}

        public string ReturnPersonName()
        {
            return this.personName;
        }

        public void LoadPersonName(string name)
        {
            this.personName = name;
        }

        public bool CheckPersonAccessLevel(int inputLevel)
        {
            if (this.personAccessLevel >= inputLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void PasswordInput(string input)
        {
            string passText = input;
            Debug.Log(passText);
            string encryptPass = passwordEncryption(passText);
            Debug.Log(encryptPass);
            this.personPassword = encryptPass;
        }

        string passwordEncryption(string passwordString)
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
            string personNameToSave = this.personName;
            string personPassword = this.personPassword;
            string personPasswordToSave = null;
            int encryptInt = 0;

            for (int i = 0; i < personNameToSave.Length; i++)
            {
                encryptInt += (int)personNameToSave[i];
            }

            for (int i = 0; i < personPassword.Length; i++)
            {
                personPasswordToSave += (char)((int)personPassword[i] + encryptInt);
            }

            return personPasswordToSave;
        }

        public void LoadPersonPassword(string password)
        {
            if (password != null)
            {
                string personNameToSave = this.personName;
                string personPassword = null;
                string personPasswordFromSave = password;
                int encryptInt = 0;

                for (int i = 0; i < personNameToSave.Length; i++)
                {
                    encryptInt += (int)personNameToSave[i];
                }

                for (int i = 0; i < personPasswordFromSave.Length; i++)
                {
                    personPassword += (char)((int)personPasswordFromSave[i] - encryptInt);
                }

                this.personPassword = personPassword;
                Debug.Log(personPassword);
            }
        }
    }

    public personInformation personProfile = new personInformation();

    public string fileForProfileSave;

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

        personDATA.Add("personName", personProfile.ReturnPersonName());
        personDATA.Add("personPassword", personProfile.ReturnPersonPassword());

        if (File.Exists(fileForProfileSave))
        {
            File.WriteAllText(fileForProfileSave, personDATA.ToString());
        }
    }

    public void LoadAllDataFromFile()
    {
        LoadFiles();

        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave));

            if (personDATA != null)
            {
                personProfile.LoadPersonName(personDATA["personName"]);
                personProfile.LoadPersonPassword(personDATA["personPassword"]);
            }
        }
    }

    private void InitializationAllObjects()
    {
        LoadAllDataFromFile();
    }

    private void Start()
    {
        InitializationAllObjects();

        personProfile.PasswordInput("Vovan");
    }
}
