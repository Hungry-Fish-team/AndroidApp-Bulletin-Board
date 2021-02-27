using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ImageManager : MonoBehaviour
{
    [SerializeField]
    GameObject content;
    [SerializeField]
    GameObject fileImagePrefab;
    [SerializeField]
    Text infoText;
    [SerializeField]
    private bool isOpenToLoad = false;

#if UNITY_EDITOR
    private DirectoryInfo dirInfo = new DirectoryInfo(@"C:\Program Files");
#elif UNITY_ANDROID
    private DirectoryInfo dirInfo = new DirectoryInfo("/mnt/sdcard");
#endif
    [SerializeField]
    public FileInfo[] files;

    private void Start()
    {

    }

    private void OnDisable()
    {
        isOpenToLoad = false;
    }

    public void LoadImageList()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().eventName = "ImageManager";

        infoText.text = "";

        content.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

        transform.gameObject.SetActive(true);
        files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => dirInfo.GetFiles(ext, SearchOption.AllDirectories)).ToArray();
        //files = 

        //IEnumerable<FileInfo> files = dirInfo.EnumerateFiles("*jpg", SearchOption.AllDirectories);

        //Debug.Log(files1[0]);
        //infoText.text = files1[0];

        for (int i = 0; i < 6; i++)
        {
            LoadPrefabsOfFiles();
        }

        isOpenToLoad = true;
    }

    private void Update()
    {
        if (isOpenToLoad == true) {
            CheckContentPosition();
        }
    }

    private void LoadPrefabsOfFiles()
    {
        if (content.transform.childCount != files.Length) {
            FileInfoScript newFile = Instantiate(fileImagePrefab, content.transform).GetComponent<FileInfoScript>();

            newFile.fileName.text = files[content.transform.childCount].Name;

            StartCoroutine(LoadImagesEnumerator(newFile, files[content.transform.childCount]));
        }
    }

    private void CheckContentPosition()
    {
        if(content.GetComponent<RectTransform>().localPosition.y > ((content.transform.childCount - 6) / 2 * 1250))
        {
            Debug.Log(((content.transform.childCount - 6) / 2 * 1250));
            Debug.Log(content.GetComponent<RectTransform>().localPosition.y);
            //1240
            LoadPrefabsOfFiles();
        }
    }

    IEnumerator LoadImagesEnumerator(FileInfoScript newFile, FileInfo file)
    {
        Texture2D tex = new Texture2D(1, 1);
        byte[] fileData = File.ReadAllBytes(file.FullName);
        tex.LoadImage(fileData);
        tex.Apply();
        newFile.fileImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        yield return new WaitForChangedResult();
    }
}
