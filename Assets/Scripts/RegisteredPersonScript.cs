using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SimpleJSON;
using System.IO;

public class RegisteredPersonScript : MonoBehaviour
{
    [SerializeField]
    bool isBackUpWork = true;

    public class RegisteredPersons : PersonInformationScript.PersonInformation
    {
        private List<PersonInformationScript.PersonInformation> allPersonsInformation = new List<PersonInformationScript.PersonInformation>();

        public void AddNewRegisteredPerson(PersonInformationScript.PersonInformation person)
        {
            PersonInformationScript.PersonInformation newPerson = person;

            allPersonsInformation.Add(newPerson);
        }

        public void AddNewRegisteredPerson(string personName, string personMail, string personPassword)
        {
            PersonInformationScript.PersonInformation newPerson = new PersonInformationScript.PersonInformation(personName, personMail, personPassword);

            allPersonsInformation.Add(newPerson);
        }

        public void DeleteRegisteredPerson(PersonInformationScript.PersonInformation newPerson)
        {
            for (int i = 0; i < allPersonsInformation.Count; i++)
            {
                //Debug.Log(allPersonsInformation[i].ReturnPersonMail());
                if (newPerson.ReturnPersonMail() == allPersonsInformation[i].ReturnPersonMail())
                {
                    allPersonsInformation.RemoveAt(i);
                }
            }
        }

        public void DeleteRegisteredPerson(string personMail)
        {
            for (int i = 0; i < allPersonsInformation.Count; i++)
            {
                //Debug.Log(allPersonsInformation[i].ReturnPersonMail());
                if (personMail == allPersonsInformation[i].ReturnPersonMail())
                {
                    allPersonsInformation.RemoveAt(i);
                }
            }
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

        public bool IsThisPersonRegistered(string mail)
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                //Debug.Log(personInformation.ReturnPersonMail() + " " + personInformation.ReturnPersonEncryptedPassword());
                //Debug.Log(mail + " " + password);

                if (personInformation.ReturnPersonMail() == mail)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsThisPersonRegistered(string mail, string password)
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                //Debug.Log(personInformation.ReturnPersonMail() + " " + personInformation.ReturnPersonEncryptedPassword());

                //Debug.Log(mail + " " + password);

                if (personInformation.ReturnPersonMail() == mail && personInformation.ReturnPersonPassword() == password)
                {
                    return true;
                }
            }
            return false;
        }

        public PersonInformationScript.PersonInformation ReturnRegisteredPersonFromList(string mail, string password)
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                //Debug.Log(personInformation.ReturnPersonMail() + " " + personInformation.ReturnPersonPassword());
                //Debug.Log(mail + " " + password);

                if (personInformation.ReturnPersonMail() == mail && personInformation.ReturnPersonPassword() == password)
                {
                    return personInformation;
                }
            }
            return null;
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

        public void ChangePersonName(string mail, string newName)
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                if (personInformation.ReturnPersonMail() == mail)
                {
                    personInformation.LoadPersonName(newName);
                    break;
                }
            }
        }

        public void ReturnAllRegisteredPersonsToConsole()
        {
            foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
            {
                if (personInformation.ReturnPersonName() != null)
                {
                    //Debug.Log(personInformation.ReturnPersonName());
                    Debug.Log(personInformation.ReturnPersonName() + " " + personInformation.ReturnPersonMail() + " " + personInformation.ReturnPersonEncryptedPassword());
                }
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
            if (PhotonNetwork.IsMasterClient)
            {
                JSONArray allRegisteredPerson = new JSONArray();

                foreach (PersonInformationScript.PersonInformation personInformation in allPersonsInformation)
                {
                    JSONObject personDATA = new JSONObject();

                    personDATA.Add("PersonName", personInformation.ReturnPersonName());
                    personDATA.Add("PersonMail", personInformation.ReturnPersonMail());
                    personDATA.Add("PersonPassword", personInformation.ReturnPersonPassword());

                    allRegisteredPerson.Add(personDATA);
                }

                if (File.Exists(fileForProfileSave))
                {
                    File.WriteAllText(fileForProfileSave, allRegisteredPerson.ToString());
                }

                Debug.Log("Registered Person");
            }
        }

        public void LoadAllDataFromFile()
        {
            //if (PhotonNetwork.IsMasterClient)
            {
                LoadFiles();

                if ((JSONArray)JSON.Parse(File.ReadAllText(fileForProfileSave)) != null)
                {
                    JSONArray personDATA = (JSONArray)JSON.Parse(File.ReadAllText(fileForProfileSave));

                    if (personDATA != null)
                    {
                        //Debug.Log(personDATA.Count);

                        int i = 0;

                        while (i < personDATA.Count)
                        {
                            PersonInformationScript.PersonInformation personInformation = new PersonInformationScript.PersonInformation();

                            string personName = personDATA.AsArray[i]["PersonName"];
                            string personMail = personDATA.AsArray[i]["PersonMail"];
                            string personPassword = personDATA.AsArray[i]["PersonPassword"];

                            //Debug.Log(personName + " " + personMail + " " + personPassword);

                            personInformation.LoadPersonName(personName);
                            personInformation.LoadPersonMail(personMail);
                            personInformation.LoadPersonPassword(personPassword);

                            allPersonsInformation.Add(personInformation);

                            i++;
                        }
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
