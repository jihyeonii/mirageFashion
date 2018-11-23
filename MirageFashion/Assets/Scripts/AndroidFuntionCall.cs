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

    string strImgPath;

    public static Texture2D texture;
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

      
    }

    // Update is called once per frame
    void Update () {
	
	}
    public void consoleLog(string msg)
    {
        plugins.Call("unityLog", msg);
    }
    public void btnGallery()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
#if UNITY_ANDROID && !UNITY_EDITOR
		plugins.Call("Gallery");
#elif UNITY_IOS
		Debug.Log("iOS Gallery open!!!!");
		imagePicker.OnPressShowPicker();
#elif UNITY_EDITOR
        //Debug.Log(Application.persistentDataPath);
        strImgPath = "C:\\Android\\Camera\\image.jpg";
        imgPath(strImgPath);
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
        if (GameManager.instance.uiState == GameManager.UIState.camera)
        {
            GameManager.instance.character.SetActive(false);
            btnOnOff(false);
            strImgPath = path;
            //string strImgPath = path;
            //pathList = Directory.GetFiles(strImgPath);
            StartCoroutine(LoadImage());
        }
        else
        {
            strImgPath = path;
            GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("album").gameObject.SetActive(false);
            GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("capture").gameObject.SetActive(false);
            GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("card").gameObject.SetActive(false);
            StartCoroutine(LoadImage());
        }
    }
    public IEnumerator LoadImage()
    {
        if (GameManager.instance.uiState == GameManager.UIState.camera)
        {
            yield return new WaitForEndOfFrame();
            if (System.IO.File.Exists(strImgPath))
            {
                byte[] byteTexture = System.IO.File.ReadAllBytes(strImgPath);

                if (byteTexture.Length > 0)
                {
                    Debug.Log(byteTexture.Length);

                    texture = new Texture2D(1, 1);

                    texture.LoadImage(byteTexture);

                    Rect rect = new Rect();
                    rect.x = 0;
                    rect.y = 0;
                    rect.width = texture.width;
                    rect.height = texture.height;


                    if (strImgPath.Contains("Camera"))
                    {
                        Vector2 width = new Vector3(((float)texture.height) * (float)1280 / (float)texture.width, ((float)texture.width) * (float)1280 / (float)texture.width);
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").gameObject.SetActive(true);
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").Find("image").GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, rect, new Vector2(0, 0));
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").Find("image").GetComponent<RectTransform>().sizeDelta = new Vector2(width.y, width.x);
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").Find("image").transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    }
                    else
                    {
                        Vector2 width = new Vector3(((float)texture.width) * (float)720 / (float)texture.width, ((float)texture.height) * (float)720 / (float)texture.width);
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").gameObject.SetActive(true);
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").Find("image").GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, rect, new Vector2(0, 0));
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").Find("image").GetComponent<RectTransform>().sizeDelta = width;
                        GameManager.instance.charCamera.transform.Find("Canvas").Find("background").Find("image").transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }

                }
            }
            
            img.gameObject.SetActive(true);

            btnOnOff(true);
            GameManager.instance.loadGallery = true;
            GameManager.instance.character.SetActive(true);
            GameManager.instance.changeChar(GameManager.instance.charInfo.body);
            GameManager.instance.checkAnimation();
        }
        else
        {
            yield return new WaitForEndOfFrame();
            if (System.IO.File.Exists(strImgPath))
            {
                byte[] byteTexture = System.IO.File.ReadAllBytes(strImgPath);

                if (byteTexture.Length > 0)
                {
                    Debug.Log(byteTexture.Length);

                    texture = new Texture2D(1, 1);

                    texture.LoadImage(byteTexture);

                    Rect rect = new Rect();
                    rect.x = 0;
                    rect.y = 0;
                    rect.width = texture.width;
                    rect.height = texture.height;

                    Vector2 width = new Vector3(((float)texture.width) * (float)720 / (float)texture.width, ((float)texture.height) * (float)720 / (float)texture.width);
                    FashionRecognition.canvasFashion.transform.Find("Image").gameObject.SetActive(true);
                    FashionRecognition.canvasFashion.transform.Find("Image").transform.GetComponent<RectTransform>().sizeDelta = width;
                    FashionRecognition.canvasFashion.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, rect, new Vector2(0, 0));
                    FashionRecognition.canvasFashion.transform.Find("Image").transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

                    FashionRecognition.img_bytes = ScaleTexture(texture, 720, (int)(texture.height * 720 / texture.width)).EncodeToJPG();
                }
            }
            FashionRecognition.nKey = (long)7300;

            FashionRecognition.canvasFashion.transform.Find("capturePlane").gameObject.SetActive(true);
            FashionRecognition.canvasFashion.transform.Find("capturePlane").GetComponent<UnityEngine.UI.Image>().sprite = null;
            FashionRecognition.canvasFashion.transform.Find("album").gameObject.SetActive(false);
            FashionRecognition.canvasFashion.transform.Find("reCapture").gameObject.SetActive(true);
            FashionRecognition.canvasFashion.transform.Find("send").gameObject.SetActive(true);
            FashionRecognition.canvasFashion.transform.Find("selectCategory").transform.Find("kid").Find("check").gameObject.SetActive(true);
            FashionRecognition.canvasFashion.transform.Find("selectCategory").transform.Find("adult").Find("check").gameObject.SetActive(false);

            FashionRecognition.canvasFashion.transform.Find("top").gameObject.SetActive(false);
            FashionRecognition.canvasFashion.transform.Find("bottom").gameObject.SetActive(false);

        }
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
                LoadAsset.instance.showPopup(LoadAsset.Error.permission);
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
   
}

