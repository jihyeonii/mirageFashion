using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoadAsset : MonoBehaviour {

    public static LoadAsset instance;
    public string bundleURL;

    public GameObject background;

    public int serverAssetVersion;
    int clientAssetVersion;

    public string clientVersion;
    public string storeVersion;

    bool isCheckAssetVer = false;
    bool isCheckStoreVer = false;
    bool checkAsset = false;
    bool checkStore = false;

    WWW www;
    public Text test;
    public string msg;


    string[] assetName;
    public static GameObject assetCharacter;
    public static GameObject fashionCharacter;

    public Dictionary<string, Sprite> dicClothImg;
    public Dictionary<string, Font> font;
    public List<Material> material;
    public List<Material> fashionMaterial;
    public Dictionary<string, Texture> fashionMaterialsImg;
    public Dictionary<string, Sprite> fashionClothImg;

    float loadingPercent;
    public Slider sliderProgressBar;
    Text progressText;

    string[] assetBundleNames;
    int count = 0;

    bool downloading = false;
    bool assetDownloading = false;
    public string isNet = null;
    bool down3G = false;
    bool isFinished = false;
    //
    public GameObject popup;

    public enum statePopup
    {
        basic, netNull, down3G, Update, permission, memory, loadError
    }
    public statePopup popupState = statePopup.basic;

    public bool checkPermission = false;
    public bool permissionCamera = false;
    public bool permissionStorage = false;

    string msg1, msg2 = "";

    public long availableMemory; //MB
    public bool checkMemory = false;
    public bool checkAvailableMemory = false;

    bool loadingFail = false;

    public GameObject introSound;
    void Start() {
        instance = this;

        background.transform.localScale = new Vector3(720, 1280, 1);
        //권한 체크
        //AndroidFuntionCall.instance.permissionResult("true,true");

        dicClothImg = new Dictionary<string, Sprite>();
        fashionClothImg = new Dictionary<string, Sprite>();
        font = new Dictionary<string, Font>();
        fashionMaterialsImg = new Dictionary<string, Texture>();
        progressText = sliderProgressBar.transform.Find("Text").GetComponent<Text>();


        //local 
        //bundleURL = "http://112.172.131.183/";
        bundleURL = "http://112.172.129.142/";


        //checkPermission = true;

        clientVersion = Application.version;
       
        clientAssetVersion = PlayerPrefs.GetInt("assetVersion");
        //DontDestroyOnLoad(this);
    }
    private void Update()
    {
        
        test.text = "isnet : " + isNet;
        if (Application.loadedLevelName == "introScene")
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                isNet = "3G";
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                isNet = "WIFI";
            else
                isNet = null;

            if (isNet == null)
            {
                popupState = statePopup.netNull;
                showPopup(popupState, "네트워크 연결을 확인해 주세요.");
            }
            /*3G팝업 테스트*/
            //else if (isNet == "WIFI" && down3G == false && isCheckAssetVer && isCheckStoreVer && (clientAssetVersion != serverAssetVersion))
            else if (isNet == "3G" && down3G == false && isCheckAssetVer && isCheckStoreVer && (clientAssetVersion != serverAssetVersion))
            {
                test.text = "3g";
                popupState = statePopup.down3G;
                showPopup(popupState, "3G/4G 환경에서는 데이터 요금이 발생할 수 있습니다.");

            }
            //else if (isCheckStoreVer && clientVersion != storeVersion)
            //{
            //    test.text = "update";
            //    showPopup(statePopup.Update);
            //}
            else if(isNet != null && down3G && permissionCamera && permissionStorage && checkAvailableMemory && loadingFail == false)
            {
                popup.SetActive(false);
            }
#if UNITY_EDITOR
                checkMemory = true;
                checkAvailableMemory = true;
#elif    UNITY_ANDROID
                if (isCheckAssetVer && (clientAssetVersion != serverAssetVersion) && checkMemory == false && checkAvailableMemory == false)
                {
              //  test.text = "callCheckAvailableMemory";
                    checkMemory = true;
                    AndroidFuntionCall.instance.checkAvailableMemory();
                }
#endif           
            if(isCheckAssetVer && (clientAssetVersion == serverAssetVersion))
            {
                checkAvailableMemory = true;
            }
            /* 3G팝업 테스트*/
            //if (clientAssetVersion == serverAssetVersion && isCheckAssetVer)
            if (clientAssetVersion == serverAssetVersion || isNet == "WIFI" && isCheckAssetVer)
                down3G = true;
            if((checkPermission && permissionCamera && permissionStorage) && isCheckAssetVer == false && checkAsset == false && isNet != null)
            {
                checkAsset = true;
                StartCoroutine(getAssetVersion());
            }
            if (isCheckStoreVer == false && checkStore == false && isNet != null)
            {
                checkStore = true;
                StartCoroutine(getStoreVersion());
            }
            if (downloading && (isNet == "3G" || isNet == "WIFI") && assetDownloading)
            {
                sliderProgressBar.gameObject.SetActive(true);
                loadingPercent = (www.progress * 100) / assetBundleNames.Length + (100 / assetBundleNames.Length * count);
                if (loadingPercent > 99)
                {
                    loadingPercent = 100;
                }
                sliderProgressBar.value = loadingPercent / 100;
                progressText.text = string.Format("Loading...{0}%", (int)loadingPercent);
            }

            /* 3G팝업 테스트*/
            //if (checkAvailableMemory && (checkPermission && permissionCamera && permissionStorage) && downloading == false && assetDownloading == false && ((isNet == "3G" && down3G) || (isNet == "WIFI" && down3G)) && isFinished == false && isCheckStoreVer && isCheckAssetVer && clientVersion == storeVersion)
            if (checkAvailableMemory && (checkPermission && permissionCamera && permissionStorage) && downloading == false && assetDownloading == false && ((isNet == "3G" && down3G) || (isNet == "WIFI")) && isFinished == false && isCheckStoreVer && isCheckAssetVer/* && clientVersion == storeVersion*/)
            {
                test.text = "down";
                StartCoroutine(download());
            }
            if ((permissionCamera == false || permissionStorage == false) && checkPermission)
            {
                permissionCamera = true;
                permissionStorage = true;
#if UNITY_EDITOR
#elif UNITY_ANDROID
                //showPopup(statePopup.permission);
#endif
            }

           
            Debug.Log("downloading : " + downloading + ", isNet :" + isNet + ", isFinish : " + isFinished + ", ischecked Asset, store :" + isCheckAssetVer + ", " + isCheckStoreVer + ", " + clientAssetVersion + serverAssetVersion);
            //test.text = "downloading : " + downloading + ", isNet :" + isNet + ", isFinish : " + isFinished + ", ischecked Asset, store :" + isCheckAssetVer + ", " + isCheckStoreVer + ", " + clientAssetVersion + serverAssetVersion;
        }
    }
    public void checkavailableMemory()
    {
        if (availableMemory >= 0 && availableMemory < 20)
        {
            popupState = statePopup.memory;
            showPopup(popupState, "사용공간이 부족합니다. ");
        }
        else
        {
            checkAvailableMemory = true;
        }
        
    }
    
    public void showPopup(statePopup popupstate, string msg)
    {
        popup.SetActive(true);

        if (popupstate == statePopup.permission)
        {

            if (permissionCamera == false)
            {
                msg1 = "카메라 권한 X";
            }
            if (permissionStorage == false)
            {
                msg2 = "읽기권한 X";
            }
            popupState = statePopup.permission;
            popup.transform.Find("Text").GetComponent<Text>().text = msg1 + "\n" + msg2;
            popup.transform.Find("Button").gameObject.SetActive(true);
            popup.transform.Find("Button").transform.localPosition = new Vector3(0, popup.transform.Find("Button").transform.localPosition.y, popup.transform.Find("Button").transform.localPosition.z);
            popup.transform.Find("Button2").gameObject.SetActive(false);
        }
        //if(popupstate == statePopup.Update)
        //{
        //    popupState = statePopup.Update;
        //    popup.transform.Find("Text").GetComponent<Text>().text = "업데이트 해주세요.";
        //    popup.transform.Find("Button").gameObject.SetActive(true);
        //    popup.transform.Find("Button").transform.localPosition = new Vector3(0, popup.transform.Find("Button").transform.localPosition.y, popup.transform.Find("Button").transform.localPosition.z);
        //    popup.transform.Find("Button2").gameObject.SetActive(false);
        //}
        if (popupstate == statePopup.memory)
        {
            popup.transform.Find("Text").GetComponent<Text>().text = msg;
            popup.transform.Find("Button").gameObject.SetActive(true);
            popup.transform.Find("Button").transform.localPosition = new Vector3(0, popup.transform.Find("Button").transform.localPosition.y, popup.transform.Find("Button").transform.localPosition.z);
            popup.transform.Find("Button2").gameObject.SetActive(false);
        }
        if(popupstate == statePopup.loadError)
        {
            popup.transform.Find("Text").GetComponent<Text>().text = msg;
            popup.transform.Find("Button").gameObject.SetActive(true);
            popup.transform.Find("Button").transform.localPosition = new Vector3(0, popup.transform.Find("Button").transform.localPosition.y, popup.transform.Find("Button").transform.localPosition.z);
            popup.transform.Find("Button2").gameObject.SetActive(false);
        }
        if(popupstate == statePopup.down3G)
        {
            popup.transform.Find("Text").GetComponent<Text>().text = msg;
            popup.transform.Find("Button").gameObject.SetActive(true);
            popup.transform.Find("Button").transform.localPosition = new Vector3(-130, popup.transform.Find("Button").transform.localPosition.y, popup.transform.Find("Button").transform.localPosition.z);
            popup.transform.Find("Button2").gameObject.SetActive(true);
            popup.transform.Find("Button2").transform.localPosition = new Vector3(130, popup.transform.Find("Button2").transform.localPosition.y, popup.transform.Find("Button2").transform.localPosition.z);

        }
        if(popupstate == statePopup.netNull)
        {
            popup.transform.Find("Text").GetComponent<Text>().text = msg;
            popup.transform.Find("Button").gameObject.SetActive(false);
            //popup.transform.Find("Button").transform.localPosition = new Vector3(0, popup.transform.Find("Button").transform.localPosition.y, popup.transform.Find("Button").transform.localPosition.z);
            popup.transform.Find("Button2").gameObject.SetActive(true);
            popup.transform.Find("Button2").transform.localPosition = new Vector3(0, popup.transform.Find("Button2").transform.localPosition.y, popup.transform.Find("Button2").transform.localPosition.z);
        }
    }
    public void popupBtn(bool btn)
    {
        if (popupState == statePopup.down3G)
        {
            down3G = btn;
            popupState = statePopup.basic;
        }
        else if (popupState == statePopup.Update)
        {
            if (btn)
            {
                Application.OpenURL("market://details?id=com.feelingki.fh");
                //Application.Quit();
            }
        }
        else if (popupState == statePopup.permission || popupState == statePopup.memory || popupState == statePopup.loadError || popupState == statePopup.netNull)
        {
            if (btn)
            {
                Application.Quit();
            }
        }

    }
    IEnumerator getAssetVersion()
    {
#if  UNITY_EDITOR || UNITY_ANDROID
        //WWW versionWWW = new WWW(bundleURL + "Android/" + clientVersion + "/assetVersion.txt");
        WWW versionWWW = new WWW(bundleURL + "test/" + clientVersion + "/assetVersion.txt");
#elif UNITY_IOS
        WWW versionWWW = new WWW(bundleURL + "iOS/" + clientVersion + "/assetVersion.txt");
#endif
        yield return versionWWW;
        if (versionWWW.error != null)
        {
            Debug.Log("error");
            showPopup(statePopup.loadError, "리소스 다운로드를 실패했습니다. \n 잠시후 시도해 주세요.");
        }
        serverAssetVersion = int.Parse(versionWWW.text);
        versionWWW.Dispose();
        isCheckAssetVer = true;
        checkAsset = false;
    }
    IEnumerator getStoreVersion()
    {
#if UNITY_EDITOR || UNITY_ANDROID
        //WWW versionWWW = new WWW(bundleURL + "Android/storeVersion.txt");
        WWW versionWWW = new WWW(bundleURL + "test/storeVersion.txt");
#endif
#if UNITY_IOS
        WWW versionWWW = new WWW(bundleURL + "iOS/storeVersion.txt");
#endif
        yield return versionWWW;
        storeVersion = versionWWW.text;
        versionWWW.Dispose();
        isCheckStoreVer = true;
        checkStore = false;
        //if (isCheckStoreVer && clientVersion != storeVersion)
        //{
        //    showPopup(statePopup.Update);
        //}
    }
    IEnumerator download()
    {
        introSound.SetActive(true);
        downloading = true;
        float startTime = Time.realtimeSinceStartup;
        while (!Caching.ready)
            yield return null;
#if UNITY_EDITOR || UNITY_ANDROID
        //www = WWW.LoadFromCacheOrDownload(bundleURL + "Android/"+clientVersion+"/Android", serverAssetVersion);
        www = WWW.LoadFromCacheOrDownload(bundleURL + "test/" + clientVersion + "/Android", serverAssetVersion);

#endif
#if UNITY_IOS
		Debug.Log("ios");
        www = WWW.LoadFromCacheOrDownload(bundleURL + "iOS/"+clientVersion+"/iOS", serverAssetVersion);
#endif
        yield return www;
        if (www.error != null)
        {
            Debug.Log("www Error : " + www.error);
            test.text = "www Error : " + www.error;
            downloading = false;
            loadingFail = true;
            popupState = statePopup.loadError;
            showPopup(popupState, "리소스 다운로드를 실패했습니다. \n 잠시후 시도해 주세요.");
        }
        
        AssetBundle bundleManifest = www.assetBundle;
        www.Dispose();
        AssetBundleManifest assetBundleManifest = bundleManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        
        assetBundleNames = assetBundleManifest.GetAllAssetBundles();
        downloading = true;

		bool caching = false;

        for (int i = 0; i<assetBundleNames.Length; i++)
        {
            string assetBundleName = assetBundleNames[i];


#if UNITY_EDITOR || UNITY_ANDROID
            //caching = Caching.IsVersionCached(bundleURL + "Android/" +clientVersion +"/"+ assetBundleName, assetBundleManifest.GetAssetBundleHash(assetBundleName));
            caching = Caching.IsVersionCached(bundleURL + "test/" + clientVersion + "/" + assetBundleName, assetBundleManifest.GetAssetBundleHash(assetBundleName));
#endif
#if UNITY_IOS
        
            caching = Caching.IsVersionCached(bundleURL + "iOS/"+clientVersion +"/" + assetBundleName, assetBundleManifest.GetAssetBundleHash(assetBundleName));
#endif
            Debug.Log("isCaching : " + caching);

#if UNITY_EDITOR || UNITY_ANDROID
            if (caching == false)
            {
                
            }
            //www = WWW.LoadFromCacheOrDownload(bundleURL + "Android/" + clientVersion + "/" + assetBundleName, assetBundleManifest.GetAssetBundleHash(assetBundleName));
            www = WWW.LoadFromCacheOrDownload(bundleURL + "test/" + clientVersion + "/" + assetBundleName, assetBundleManifest.GetAssetBundleHash(assetBundleName));
#endif
#if UNITY_IOS
            www = WWW.LoadFromCacheOrDownload(bundleURL + "iOS/" + clientVersion + "/" + assetBundleName, assetBundleManifest.GetAssetBundleHash(assetBundleName));
#endif
            yield return www;
            assetDownloading = true;
            AssetBundle bundle = www.assetBundle;
            
            assetName = bundle.GetAllAssetNames();

            if (assetBundleName == "character")
            {
                assetCharacter = bundle.LoadAsset<GameObject>("character");

            }
            if(assetBundleName == "fashioncharacter")
            {
                fashionCharacter = bundle.LoadAsset<GameObject>("fashioncharacter");
            }
            if(assetBundleName == "fashionimage")
            {
                for(int j =0; j< assetName.Length; j++)
                {
                    fashionClothImg.Add(bundle.LoadAsset<Sprite>(assetName[j]).name, bundle.LoadAsset<Sprite>(assetName[j]));
                }
            }
            if(assetBundleName == "fashionmaterials")
            {
                for (int j = 0; j < assetName.Length; j++)
                {
                    if (bundle.LoadAsset<Material>(assetName[j]) != null)
                    {
                        fashionMaterial.Add(bundle.LoadAsset<Material>(assetName[j]));
                    }
                }
            }
            if(assetBundleName == "fashionmaterialsimage")
            {
                for (int j = 0; j < assetName.Length; j++)
                {
                    fashionMaterialsImg.Add(bundle.LoadAsset<Texture>(assetName[j]).name, bundle.LoadAsset<Texture>(assetName[j]));
                }
            }
            if (assetBundleName == "material")
            {
                for (int j = 0; j < assetName.Length; j++)
                {
                    if (bundle.LoadAsset<Material>(assetName[j]) != null)
                    {
                        material.Add(bundle.LoadAsset<Material>(assetName[j]));
                    }
                }

            }
            if (assetBundleName == "image")
            {
                Debug.Log(assetName.Length);
                for (int j = 0; j < assetName.Length; j++)
                {
                    dicClothImg.Add(bundle.LoadAsset<Sprite>(assetName[j]).name, bundle.LoadAsset<Sprite>(assetName[j]));
                }
            }
            if(assetBundleName == "font")
            {
                for(int j = 0; j < assetName.Length; j++)
                {
                    //Debug.Log(bundle.LoadAsset<Font>(assetName[j]).name);
                    font.Add(bundle.LoadAsset<Font>(assetName[j]).name, bundle.LoadAsset<Font>(assetName[j]));
                }
            }
            float endTime = Time.realtimeSinceStartup - startTime;
            
            Debug.Log("time : " + endTime);
            bundle.Unload(false);
            www.Dispose();
            count++;
        }
        isFinished = true;
        downloading = false;
        assetDownloading = false;
        PlayerPrefs.SetInt("assetVersion", serverAssetVersion);
        if(loadingFail == false)
        {
            GameObject.Find("Canvas").transform.Find("character").GetComponent<Animator>().enabled = true;
            GameObject.Find("Canvas").transform.Find("Effect").gameObject.SetActive(true);
        }
    }

}
