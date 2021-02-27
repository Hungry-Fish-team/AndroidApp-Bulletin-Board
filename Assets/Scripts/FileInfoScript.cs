using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileInfoScript : MonoBehaviour
{
    [SerializeField]
    GameObject ownObject;

    public Image fileImage;
    public Text fileName;

    public void ChouseThisImage()
    {
        GameObject.Find("LoadImage").GetComponent<Image>().sprite = fileImage.sprite;
        GameObject parent = transform.parent.gameObject;
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        GameObject.Find("BG_FileList").SetActive(false);
    }
}
