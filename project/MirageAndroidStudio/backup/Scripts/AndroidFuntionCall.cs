using UnityEngine;
using System.Collections;
using System.IO;
//using Kakera;

public class AndroidFuntionCall : MonoBehaviour {
    public static AndroidFuntionCall instance;
    public static AndroidJavaObject plugins;
    Texture2D[] texList;
    string[] pathList;
    public MeshRenderer img;
    float screenWidth;
    float screenHeight;

	#if UNITY_IOS
		private PickerController imagePicker;
	#endif

    //AndroidJavaClass androidPlugin;
    // Use this for initialization
    void Start () {
     
        instance = this;
		#if UNITY_EDITOR

		#elif UNITY_ANDROID
		AndroidJavaClass Ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		plugins = Ajc.GetStatic<AndroidJavaObject>("currentActivity");
		#elif UNITY_IOS
		imagePicker = new PickerController();

		#endif

        screenWidth = Screen.width;
        screenHeight = (float)screenWidth * ((float)Screen.height / (float)Screen.width);
        //img.transform.localScale = new Vector3(720 / 10, 0.1f, (720 / screenWidth) * screenHeight / 10);
        //screenHeight = 2.0f * Mathf.Tan(0.5f * GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad);
        //screenWidth = screenHeight * Screen.width / Screen.height;
        //img.transform.localScale = new Vector3(screenWidth, 1, screenHeight);
        //img.transform.LookAt(GameManager.instance.cameraAR.transform);
        //img.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 180));
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void btnGallery()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
		#if UNITY_ANDROID
		plugins.Call("Gallery");
		#elif UNITY_IOS
		Debug.Log("iOS Gallery open!!!!");
		imagePicker.OnPressShowPicker();
		#endif   
  
    }
    public void btnKakao()
    {
		GameManager.instance.instantiateSound(GameManager.instance.clickSound);
		#if UNITY_ANDROID
		plugins.Call("sendKakao", GameManager.instance.myFilename);
		#elif UNITY_IOS
		GameManager.instance.callShareForiOS();
		#endif
    }
 
    public void imgPath(string path)
    {
        GameManager.instance.character.SetActive(false);
        btnOnOff(false);
        string strImgPath = path;
        pathList = Directory.GetFiles(strImgPath);
        StartCoroutine(LoadImage());
    }
    public IEnumerator LoadImage()
    {
        //texList = new Texture2D[pathList.Length];
        //int dummy = 0;
        foreach (string tstring in pathList)
        {
            WWW www = new WWW("file://" + tstring);
            yield return www;

            Texture2D texTmp = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            www.LoadImageIntoTexture(texTmp);
            if(texTmp.width > texTmp.height)
            {
                //가로사진
                CameraSettings tmp = FindObjectOfType<CameraSettings>();
                if (tmp.IsFrontCameraActive())
                {
                    img.transform.localRotation = Quaternion.Euler(new Vector3(180, 90, 270));
                }
                else
                {
                    img.transform.localRotation = Quaternion.Euler(new Vector3(180, 90, 270));
                }
                
                img.transform.localScale = new Vector3((720 / screenWidth) * screenHeight / 10, 0.1f, 720 / 10);
                texTmp = ScaleTexture(texTmp, Screen.height, Screen.width);
            }
            else
            {
                CameraSettings tmp = FindObjectOfType<CameraSettings>();
                if (tmp.IsFrontCameraActive())
                {
                    img.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 180));
                }
                else
                {
                    img.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 180));
                }
                img.transform.localScale = new Vector3(720 / 10, 0.1f, (720 / screenWidth) * screenHeight / 10);
                
                texTmp = ScaleTexture(texTmp, Screen.width, Screen.height);
            }
           
            img.material.mainTexture = texTmp;
        }
        img.gameObject.SetActive(true);
        
        btnOnOff(true);
        GameManager.instance.loadGallery = true;
        GameManager.instance.character.SetActive(true);
        GameManager.instance.changeChar(GameManager.instance.charInfo.body);
        GameManager.instance.checkAnimation();
    }
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
    public void btnOnOff(bool on)
    {
        GameManager.canvasCameraUI.SetActive(on);
        GameManager.canvasTopButtonUI.transform.Find("btnCamSwitch").gameObject.SetActive(false);
        GameManager.canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(on);
        //애니메이션 버튼
        //GameManager.canvasTopButtonUI.transform.Find("btnPauseAni").gameObject.SetActive(on);
        GameManager.instance.cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("CharControlBtn").gameObject.SetActive(on);
        
    }
    public void onBackPressed()
    {
        plugins.Call("onBackPressed");
    }
    
    public void permissionResult(string result)
    {
        LoadAsset.instance.test.text = "permission :" + result;
        if (result != null && result != "")
        {
            if (result.Contains("true"))
            {
                LoadAsset.instance.permissionCamera = true;
                LoadAsset.instance.permissionStorage = true;
            }
            else
            {
                LoadAsset.instance.permissionCamera = false;
                LoadAsset.instance.permissionStorage = false;
                LoadAsset.instance.showPopup(LoadAsset.statePopup.permission);
            }
            

            LoadAsset.instance.checkPermission = true;
        }
        else
        {

        }
    }
    public void checkAvailableMemory()
    {
        plugins.Call("getAvailableMemory");
    }
    public void getAvailableMemory(string memory)
    {
        if(memory !=null && memory != "")
        {
            LoadAsset.instance.availableMemory = long.Parse(memory);
            LoadAsset.instance.checkavailableMemory();
        }
    }
    public void mediaScan()
    {
#if UNITY_ANDROID
        plugins.Call("mediaScan", GameManager.instance.myFilename);
#endif
    }
   
//    public void check()
//    {

//#if UNITY_EDITOR

//#else
//        plugins.Call("checkPermission");
//#endif
//    }

}

//    private AndroidJavaObject javaObj = null;

//    private AndroidJavaObject GetJavaObject()
//    {
//        if (javaObj == null)
//        {
//            javaObj = new AndroidJavaObject("com.feelingk.test2.MainActivity");
//        }
//        return javaObj;
//    }

//    private void SetActivityInNativePlugin()
//    {
//        // Retrieve current Android Activity from the Unity Player
//        AndroidJavaClass jclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//        AndroidJavaObject activity = jclass.GetStatic<AndroidJavaObject>("currentActivity");

//        // Pass reference to the current Activity into the native plugin,
//        // using the 'setActivity' method that we defined in the ImageTargetLogger Java class
//        GetJavaObject().Call("setActivity", activity);
//    }

//    private void ShowTargetInfo(string eventMsg)
//    {
//        GetJavaObject().Call("showTargetInfo", eventMsg);
//    }
//#else
//    private void ShowTargetInfo(string targetName) {
//        Debug.Log("ShowTargetInfo method placeholder for Play Mode (not running on Android device)");
//    }
//#endif
