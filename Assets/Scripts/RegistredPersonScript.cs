using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SimpleJSON;
using System.IO;

public class RegistredPersonScript : MonoBehaviour
{
    [SerializeField]
    bool isBackUpWork = true;

    public class RegisteredPersons : PersonInformationScript.PersonInformation
    {
        private List<PersonInformationScript.PersonInformation> allPersonsInformation = new List<PersonInformationScript.PersonInformation>();

        public void AddNewRegisteredPerson(PersonInformationScript.PersonInformation newPerson)
        {
            allPersonsInformation.Add(newPerson);
        }

        public void DeleteRegisteredPerson(PersonInformationScript.PersonInformation newPerson)
        {
            allPersonsInformation.Remove(newPerson);
        }

        public bool IsThisPersonRegistered(PersonInformationScript.PersonInformation newPerson)
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                if(personInformation.ReturnPersonMail() == newPerson.ReturnPersonMail() && personInformation.ReturnPersonEncryptedPassword() == newPerson.ReturnPersonEncryptedPassword())
                {
                    return true;
                }
            }
            return false;
        }

        public void ChangePersonName(PersonInformationScript.PersonInformation newPerson, string newName)
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                //Debug.Log(personInformation.ReturnPersonName() + " " + newPerson.ReturnPersonName());
                if (personInformation.ReturnPersonMail() == newPerson.ReturnPersonMail())
                {
                    personInformation.LoadPersonName(newName);
                }
            }
        }

        public void ReturnAllRegisteredPersonsToConsole()
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                Debug.Log(personInformation.ReturnPersonName() + " " + personInformation.ReturnPersonEncryptedPassword() + " " + personInformation.ReturnPersonMail());
            }
        }

        string fileForProfileSave;

        private void LoadFiles()
        {
            if (!File.Exists(Application.persistentDataPath + "/fileForRegisteredProfile.json"))
            {
                CreateFilesForSave("/fileForRegisteredProfile.json");
            }
            fileForProfileSave = Application.persistentDataPath + "/fileForRegisteredProfile.json";
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

            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                personDATA.Add("PersonName", personInformation.ReturnPersonName());
                personDATA.Add("PersonMail", personInformation.ReturnPersonMail());
                personDATA.Add("PersonPassword", personInformation.ReturnPersonEncryptedPassword());
            }

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
                    //Debug.Log(personDATA.Count);

                    int i = 0;

                    while (i < personDATA.Count / 3)
                    {
                        PersonInformationScript.PersonInformation personInformation = new PersonInformationScript.PersonInformation();

                        personInformation.LoadPersonName(personDATA["PersonName"]);
                        personInformation.LoadPersonMail(personDATA["PersonMail"]);
                        personInformation.LoadPersonEncryptedPassword(personDATA["PersonPassword"]);

                        allPersonsInformation.Add(personInformation);

                        i++;
                    }
                }
            }
        }
    }

    public RegisteredPersons registeredPersons = new RegisteredPersons();

    private void Start()
    {
        InitializationAllObjects();
    }

    private void Update()
    {
        //registeredPersons.ReturnAllRegisteredPersonsToConsole();
    }

    private void InitializationAllObjects()
    {
        
    }

    private void OnEnable()
    {
        registeredPersons.LoadAllDataFromFile();
    }

    private void OnApplicationQuit()
    {
        if (isBackUpWork == true)
        {
            registeredPersons.SaveAllDataToFile();
        }
    }
}
