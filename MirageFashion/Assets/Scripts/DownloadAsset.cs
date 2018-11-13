using UnityEngine;
using System;
using System.Collections;

public class DownloadAsset : MonoBehaviour
{
    public string bundleURL = "file:///D:/test/";
    public int version;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(DownloadAndCache());
    }
    IEnumerator DownloadAndCache()
    {
        Debug.Log("start");
        while (!Caching.ready)
            yield return null;
        if (version == PlayerPrefs.GetInt("version"))
        {
            using (WWW www = WWW.LoadFromCacheOrDownload(bundleURL, version))
            {
                yield return null;
                if (www.error != null)
                    throw new Exception("error : " + www.error);
                PlayerPrefs.SetInt("version", version);
            }
            Debug.Log("finish");
        }
        //if (PlayerPrefs.GetString("guide") == "off")
            Application.LoadLevel("gameScene");
        //else
        //{
        //    Application.LoadLevel("tutorialScene");
        //}
    }
    

}
