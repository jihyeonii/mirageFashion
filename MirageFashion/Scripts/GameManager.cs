
using UnityEngine;
using Vuforia;
using System;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TouchScript.Gestures;
using UnityEngine.SceneManagement;

public class CardInfo
{
    public string name;         //아이템 이름 ex)char_ari
    public string strName;      //아이템 이름 ex)아리
    public int history;         //카드인식 기록
    public int level;           //0:일반 1:스페셜
    public string type;         //0:캐릭터 1:상의 2:하의 3:원피스 4:신발 5:악세사리
    public string style;        //0:멋져 1:귀여워 2:발랄해 3:아름다워 4:예뻐 5: 아리 6:민 7: 슈엘 8:수하
    public int score;
    public int indexStyle;      

    int[] arrayLevel = new int[2];
    string[] arrayType = new string[6];
    string[] arrayStyle = new string[9];
    public CardInfo(string mName, string mStrName, int mHistory, int mLevel, int mType, int mStyle, int mScore)
    {
        arrayLevel[0] = 0; arrayLevel[1] = 1;
        arrayType[0] = "캐릭터"; arrayType[1] = "상의"; arrayType[2] = "하의"; arrayType[3] = "원피스"; arrayType[4] = "신발"; arrayType[5] = "악세사리";
        arrayStyle[0] = "멋져"; arrayStyle[1] = "귀여워"; arrayStyle[2] = "발랄해"; arrayStyle[3] = "아름다워"; arrayStyle[4] = "예뻐"; arrayStyle[5] = "패션에 관심이 많고, 따뜻한 마음을 지닌 생기발랄한 명랑 소녀"; arrayStyle[6] = "공부보다 운동과 개그를 좋아하는 시원한 성격의 왈가닥 소녀"; arrayStyle[7] = "아름답지만 차가운 성격의 미소녀"; arrayStyle[8] = "수줍음 많고 똑똑한 우등생으로 차분한 성격의 모범생 소녀";

        name = mName;
        strName = mStrName;
        history = mHistory;
        level = arrayLevel[mLevel];
        type = arrayType[mType];
        style = arrayStyle[mStyle];
        indexStyle = mStyle;
        score = mScore;
    }
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject character;

    public enum UIState
    {
        guide, main, paint, camera, save, fashion   //guide: 처음 소개화면, main: 카드인식화면, paint: 꾸미기화면, camera: 카메라화면, save: 저장화면, fashion: 패션인식(촬영)화면
    }

    public GameObject cameraMainUI;                 //인식화면 UI 카메라
    public GameObject cameraPaintUI;                //꾸미기화면, 저장화면 UI 카메라
    public static GameObject canvasBottomButtonUI;  //인식화면 bottom버튼
    public static GameObject canvasTopButtonUI;     //인식화면 top버튼
    public static GameObject charControlBtnUI;      //캐릭터 회전 버튼
    public static GameObject canvasRecObj;          //인식화면 좌측 착용의상 이미지
    public GameObject canvasCaptureUI;              //카메라촬영 canvas
    public static MeshRenderer capturePic;          //카메라촬영 plane
    public static GameObject canvasCameraUI;        //카메라화면 버튼
    //public static GameObject canvasMainUI;
    public GameObject canvasPaintUI;                //꾸미기화면
    public GameObject canvasSaveUI;                 //저장화면
    public GameObject guideButtonUI;                //사용방법 페이지

    public GameObject cameraAR;                     //카드인식 카메라(AR Camera)

    public GameObject GuideLine;                    //카드인식중 나타내는 가이드라인

    //public GameObject dressPopup;
    public GameObject cameraTutorial;               //앱실행 후 사용방법 화면, 한번만 실행


    public CharacterInfo charInfo;

   

    //public static GameObject menuLayout;

    
    byte[] imageByte;

    public string myFilename;
    string myFolderLocation;
    string myScreenshotLocation;
    string myDefaultLocation;

    public static Texture2D texture;
    Material whiteDiffuseMat;

    float screenHeight;
    float screenWidth;

    //public Text DebugTxt;
    //string path = "/storage/emulated/0/DCIM/TEST/";

    public UIState uiState;
    public PaintState paintState;
    public enum PaintState
    {
        none, pen, text
    }


 
    public bool isPopup = false;
    //public bool isTracking = false;
    Texture2D[] texList;
    string[] pathList;

    public static GameObject[] recognizeObj;

    public GameObject effectCamera;
    public GameObject charCamera;
    public ParticleSystem topEffect;
    public ParticleSystem bottmEffect;
    public ParticleSystem onepieceEffect;
    public ParticleSystem shoesEffect;
    public ParticleSystem accEffect;
    public GameObject leaveEffect;
    public GameObject missionEffect;
    public GameObject appearCharEffect;

    public Transform effectTransform;

    public Dictionary<string, CardInfo> dicCard = new Dictionary<string, CardInfo>();

    public Animator characterAni;

    bool cancle = false;
    public Text testT;

    public GameObject arCharacter;
    public GameObject fashionCharacter;
    public Transform lockerCharacter;

    //cam
    bool mAutofocusEnabled = true;

    public bool loadGallery = false;
    //bool checkPermission = false;

    //사운드
    public GameObject guideSound;
    public GameObject clickSound;
    public GameObject loadSound;
    public GameObject captureSound;
    public GameObject saveSound;
    public GameObject missionSound;
    public GameObject lockerSound;

    float backKeyDelayTime = 0;

    public bool dressroomLoad = false;

    public bool availableRecognize = true;
    public string recogObjName = null;
    public string offObjName = null;

    public float waitingTime = 0f;
    public bool isOnCloth;

    public GameObject clickEffect;

    public bool isCardRecognition = true;           //true: 카드인식 false: 패션인식
    public GameObject fashionCamera;

    public string selectCloth = "";
    public string selectColor = "";
    public int onClothCount = 0;
    private void Start()
    {
        DontDestroyOnLoad(this);
        instance = this;

        guideButtonUI.SetActive(true);
        uiState = UIState.main;
        canvasBottomButtonUI = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("panelBottom").gameObject;
        canvasTopButtonUI = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("panelTop").gameObject;
        canvasCameraUI = cameraMainUI.transform.Find("CanvasCameraButtonUI").gameObject;
        canvasRecObj = cameraMainUI.transform.Find("CanvasMainButtonUI").Find("recognizeObj").gameObject;
        //menuLayout = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("panelTop").Find("btnMenu").Find("MenuLayout").gameObject;
        capturePic = canvasCaptureUI.transform.Find("Plane").GetComponent<MeshRenderer>();
        //GuideLine = cameraAR.transform.Find("Camera").transform.Find("Canvas").transform.Find("GuideLine").gameObject;
        charControlBtnUI = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("CharControlBtn").gameObject;

        //resetChar();
        charInfo = new CharacterInfo("0", "0", "0", "0", "0", "0");

        //캡처 plane 
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        capturePic.transform.localScale = new Vector3(720 / 10, 0.1f, (720 / screenWidth) * screenHeight / 10);
        capturePic.transform.LookAt(cameraMainUI.transform);
        capturePic.transform.rotation = Quaternion.Euler(new Vector3(90, -180, 0));

        //WriteData("bbb");
        //ReadData();
        recognizeObj = new GameObject[4];
        recognizeObj[0] = charControlBtnUI.transform.Find("top").gameObject;
        recognizeObj[1] = charControlBtnUI.transform.Find("bottom").gameObject;
        recognizeObj[2] = charControlBtnUI.transform.Find("shoes").gameObject;
        recognizeObj[3] = charControlBtnUI.transform.Find("acc").gameObject;

        gameObject.GetComponent<PopupManager>().enabled = true;
        //사용가이드
        //PlayerPrefs.SetString("guide", "on");
        if (PlayerPrefs.GetString("guide") != "off")
        {
            uiState = UIState.guide;
            cameraTutorial.SetActive(true);
            //GameObject.Find("tutorial").gameObject.SetActive(true);
            //GameObject.Find("PopupCamera").gameObject.SetActive(true);
            GameObject.Find("PopupCamera").transform.Find("CanvasPopup").transform.Find("Guide").gameObject.SetActive(true);
            GameObject.Find("PopupCamera").transform.Find("CanvasPopup").transform.Find("Guide").transform.Find("Text").GetComponent<Text>().text = "캐릭터 카드를 먼저 보여줘!";
            //GameObject.Find("PopupCamera").transform.Find("CanvasPopup").transform.Find("Guide").transform.Find("btn").gameObject.SetActive(false);
        }

        setFont();
        setCard();



        //characterload
        arCharacter = Instantiate(LoadAsset.assetCharacter) as GameObject;
        arCharacter.transform.parent = character.transform;
        arCharacter.name = "character";
        arCharacter.transform.localPosition = new Vector3(0, -0.8f, 15f);
        arCharacter.transform.localScale = new Vector3(200, 200, 200);
        arCharacter.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
        arCharacter.layer = 11;
        for (int j = 0; j < arCharacter.transform.GetChildCount(); j++)
        {
            arCharacter.transform.GetChild(j).gameObject.layer = 11;

            for (int k = 0; k < 4; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    arCharacter.transform.GetChild(k).GetChild(l).gameObject.layer = 11;
                }
            }
        }
        arCharacter.SetActive(false);

        //패션인식 캐릭터
        fashionCharacter = Instantiate(LoadAsset.fashionCharacter) as GameObject;
        fashionCharacter.transform.parent = character.transform;
        fashionCharacter.name = "fashionCharacter";
        fashionCharacter.transform.localPosition = new Vector3(0, -0.8f, 15f);
        fashionCharacter.transform.localScale = new Vector3(200, 200, 200);
        fashionCharacter.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
        fashionCharacter.layer = 11;
        for (int j = 0; j < fashionCharacter.transform.GetChildCount(); j++)
        {
            fashionCharacter.transform.GetChild(j).gameObject.layer = 11;

           
                for (int l = 0; l < 2; l++)
                {
                    fashionCharacter.transform.GetChild(18).GetChild(l).gameObject.layer = 11;
                }
            
        }
        fashionCharacter.SetActive(false);

        //보관함 캐릭터
        lockerCharacter = GameObject.Find("PopupCamera").transform.Find("CanvasPopup").transform.Find("SettingPage").transform.Find("DressRoom").transform.Find("GameObject").transform;

        GameObject[] locker = new GameObject[3];
        for (int j = 0; j < 3; j++)
        {
            locker[j] = Instantiate(LoadAsset.assetCharacter) as GameObject;
            locker[j].transform.parent = lockerCharacter.transform.Find("locker_" + j);
            locker[j].name = "character";
            locker[j].transform.localPosition = new Vector3(0, -240, 0);
            locker[j].transform.localScale = new Vector3(3300, 3300, 3300);
            locker[j].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            locker[j].GetComponent<Animator>().enabled = false;
            locker[j].layer = 15;
            for (int k = 0; k < locker[j].transform.GetChildCount(); k++)
            {
                locker[j].transform.GetChild(k).gameObject.layer = 15;
            }
            for (int k = 0; k < 4; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    locker[j].transform.GetChild(k).GetChild(l).gameObject.layer = 15;
                }
            }
        }
        matchMaterial();
        setGuide();

    }
    public void setFont()
    {
        GameObject obj = GameObject.Find("PopupCamera").transform.Find("CanvasPopup").gameObject;
        obj.transform.Find("SettingPage").Find("1").Find("btnIntroduce").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("btnDressRoom").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("btnCardInfo").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("btnGuide").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("btnShop").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("version").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("version").Find("Text1").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("1").Find("version").Find("versionTxt").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("MirageIntro").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("Guide").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("DressRoom").Find("btnLoad").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("DressRoom").Find("btnRevert").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("charInfo").Find("name").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("charInfo").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("itemImg").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("itemInfo").Find("itemName").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("itemInfo").Find("level").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("itemInfo").Find("type").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("SettingPage").Find("CardInfo").Find("cardInfo").Find("itemInfo").Find("style").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("PraiseMsg").Find("Image").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("Toast").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("Guide").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        obj.transform.Find("Mission").Find("Image").transform.Find("Text1").GetComponent<Text>().font = LoadAsset.instance.font["yanolja"];
        obj.transform.Find("Mission").Find("Image").transform.Find("Text2").GetComponent<Text>().font = LoadAsset.instance.font["yanolja"];
        obj.transform.Find("Mission").Find("msg").GetComponent<Text>().font = LoadAsset.instance.font["yanolja"];
        obj.transform.Find("Mission").Find("Toggle").Find("Label").GetComponent<Text>().font = LoadAsset.instance.font["yanolja"];

        guideButtonUI.transform.Find("obj").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        for (int i = 1; i < 11; i++)
        {
            Transform contentObj = guideButtonUI.transform.Find("Guide").Find("Scroll View").Find("Viewport").Find("Content");
            contentObj.Find("Text_" + i).GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        }
        guideButtonUI.transform.Find("Guide").Find("Scroll View").Find("Viewport").Find("Content").Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
    }
    public void matchMaterial()
    {

        characterAni = arCharacter.GetComponent<Animator>();
        //characterAni.SetBool("char_suha", true);
        for (int i = 0; i < LoadAsset.instance.material.Count; i++)
        {
            if (LoadAsset.instance.material[i].name.Contains("top") || LoadAsset.instance.material[i].name.Contains("bottom") || LoadAsset.instance.material[i].name.Contains("onepiece") || LoadAsset.instance.material[i].name.Contains("shoes") || LoadAsset.instance.material[i].name.Contains("acc") || LoadAsset.instance.material[i].name.Contains("body"))
            {
                if (arCharacter.transform.Find(LoadAsset.instance.material[i].name) != null)
                {
                    arCharacter.transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    arCharacter.transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
                    if (LoadAsset.instance.material[i].name.Contains("basic"))
                    {
                        fashionCharacter.transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                        fashionCharacter.transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
                    }
                }
                if (LoadAsset.instance.material[i].name.Contains("body")&& fashionCharacter.transform.Find(LoadAsset.instance.material[i].name) != null)
                {
                    fashionCharacter.transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    fashionCharacter.transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
                }
                for (int j = 0; j < 3; j++)
                {
                    if (lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name) != null)
                    {
                        lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                        lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                    }
                }
            }
            else
            {
                arCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                arCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
                if(LoadAsset.instance.material[i].name.Contains("char_ari") && fashionCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)) != null)
                {
                    fashionCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    fashionCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
                }
                //DestroyObject(fashionCharacter.transform.Find("char_min").gameObject);
                //DestroyObject(fashionCharacter.transform.Find("char_suha").gameObject);
                //DestroyObject(fashionCharacter.transform.Find("char_shuel").gameObject);
                for (int j = 0; j < 3; j++) 
                {
                    lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                }
            }
        }
        for( int i = 0; i<LoadAsset.instance.fashionMaterial.Count; i++)
        {
            if(fashionCharacter.transform.Find(LoadAsset.instance.fashionMaterial[i].name)  != null)
            {
                fashionCharacter.transform.Find(LoadAsset.instance.fashionMaterial[i].name).GetComponent<Renderer>().material = LoadAsset.instance.fashionMaterial[i];
                fashionCharacter.transform.Find(LoadAsset.instance.fashionMaterial[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
            }
        }
    }
    public void setGuide()
    {
        Transform obj = guideButtonUI.transform.Find("Guide").Find("Scroll View").Find("Viewport").Find("Content");
        //Transform obj2 = PopupManager.guideUI.transform.Find("Scroll View").Find("Viewport").Find("Content");
        for (int i = 1; i < 11; i++)
        {
            obj.Find("use_" + i).GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg["use_" + i];
            //obj2.Find("use_" + i).GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg["use_" + i];
        }

    }
    public void Update()
    {
        if (isCardRecognition)
        {
            charCamera.transform.position = cameraAR.transform.position;
            charCamera.transform.rotation = cameraAR.transform.rotation;
        }
        //testT.text = character.transform.localPosition.ToString();
        //cameraAR.transform.localPosition = new Vector3(0, 0, 0);
        //Debug.Log(character.transform.position);
        if (!charInfo.body.Contains("char") && isCardRecognition)
        {
            canvasBottomButtonUI.transform.Find("btnSaveChar").transform.gameObject.SetActive(false);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {

                if (uiState == UIState.camera && loadGallery)
                {
                    if (backKeyDelayTime == 0)
                    {

                        AndroidFuntionCall.instance.img.gameObject.SetActive(false);
                        AndroidFuntionCall.instance.btnOnOff(true);
                        canvasTopButtonUI.transform.Find("btnCamSwitch").gameObject.SetActive(true);
                        loadGallery = false;
                        StartCoroutine(backKeyDelay());
                    }
                }
                else if (PopupManager.instance.popupState == PopupManager.Popup.settingCardInfo || PopupManager.instance.popupState == PopupManager.Popup.guide || PopupManager.instance.popupState == PopupManager.Popup.settingDressRoom || PopupManager.instance.popupState == PopupManager.Popup.settingIntro)
                {
                    if (backKeyDelayTime == 0)
                    {

                        PopupManager.instance.goSettingHome();
                        PopupManager.instance.popupState = PopupManager.Popup.settingHome;
                        StartCoroutine(backKeyDelay());
                    }
                }
                else if (PopupManager.instance.popupState == PopupManager.Popup.editDressRoom)
                {
                    DressRoomManager.instance.revert();
                    DressRoomManager.instance.edit(PopupManager.lockerUI.transform.Find("btnEdit").gameObject);
                    StartCoroutine(backKeyDelay());
                }

                else if (uiState == UIState.paint)
                {
                    if (backKeyDelayTime == 0)
                    {

                        goCameraScene();
                        StartCoroutine(backKeyDelay());
                    }
                }
                else if (PopupManager.instance.popupState == PopupManager.Popup.guide)
                {
                    StartCoroutine(backKeyDelay());
                    PopupManager.instance.closeGuide();
                }
                else if (isPopup == false)
                {

                    if (cancle == false)
                    {
                        if (backKeyDelayTime == 0)
                        {
                            AndroidFuntionCall.instance.onBackPressed();
                            StartCoroutine(WaitCancle());
                        }
                    }
                    else
                    {
                        if (backKeyDelayTime == 0)
                        {
                            Application.Quit();
                        }
                    }
                }
            }
        }

        //if (uiState == UIState.camera || isPopup)
        //    GuideLine.SetActive(false);

        if ((PopupManager.instance.popupState != PopupManager.Popup.home && PopupManager.instance.popupState != PopupManager.Popup.settingHome) || /*(uiState == UIState.camera && loadGallery) ||*/ uiState == UIState.paint || uiState == UIState.save)
        {
            cameraAR.SetActive(false);
        }
        else
            cameraAR.SetActive(true);
        if (dressroomLoad)
        {
            StartCoroutine(appearChar());
        }
        if (availableRecognize && isPopup == false && isCardRecognition)
        {
            waitingTime += Time.smoothDeltaTime;
            if(waitingTime > 5f)
            {
                waitingTime = 0f;
                if(charInfo.body.Contains("char"))
                {
                    PopupManager.instance.showToast("의상 카드를 비춰주세요");
                }
                else
                {
                    PopupManager.instance.showToast("캐릭터 카드를 비춰주세요");
                }
            }
        }
        else
            waitingTime = 0f;

    }
    IEnumerator backKeyDelay()
    {
        backKeyDelayTime = 1;
        yield return new WaitForSeconds(0.5f);
        backKeyDelayTime = 0;
    }
    IEnumerator WaitCancle()
    {

        yield return new WaitForSeconds(0.5f);
        cancle = true;
        StartCoroutine(this.Cancle());
    }
    IEnumerator Cancle()
    {
        yield return new WaitForSeconds(1.0f);
        //PopupManager.toast.SetActive(false);
        cancle = false;
    }
    public void checkAnimation()
    {

        //세트 애니메이션
        if (charInfo.body.Contains("ari") && charInfo.onepiece.Contains("witch") && charInfo.shoes.Contains("witch"))
            characterAni.SetBool("witch", true);
        else
            characterAni.SetBool("witch", false);
        if (charInfo.body.Contains("ari") && charInfo.onepiece.Contains("princess") && charInfo.acc.Contains("princess"))
            characterAni.SetBool("princess", true);
        else
            characterAni.SetBool("princess", false);
        if (charInfo.body.Contains("min") && charInfo.top.Contains("stewardess") && charInfo.bottom.Contains("stewardess") && (charInfo.shoes.Contains("stewardess") || charInfo.acc.Contains("stewardess")))
            characterAni.SetBool("stewardess", true);
        else
            characterAni.SetBool("stewardess", false);
        if (charInfo.body.Contains("min") && charInfo.top.Contains("zookeeper") && charInfo.bottom.Contains("zookeeper") && charInfo.shoes.Contains("zookeeper"))
            characterAni.SetBool("zookeeper", true);
        else
            characterAni.SetBool("zookeeper", false);
        if (charInfo.body.Contains("min") && charInfo.top.Contains("police") && charInfo.bottom.Contains("police") && charInfo.shoes.Contains("police") && charInfo.acc.Contains("police"))
            characterAni.SetBool("police", true);
        else
            characterAni.SetBool("police", false);
        if (charInfo.body.Contains("shuel") && charInfo.onepiece.Contains("cats") && charInfo.shoes.Contains("cats") && charInfo.acc.Contains("cats"))
            characterAni.SetBool("cats", true);
        else
            characterAni.SetBool("cats", false);
        if (charInfo.body.Contains("shuel") && charInfo.onepiece.Contains("shuelidol") && charInfo.shoes.Contains("shuelidol") && charInfo.acc.Contains("shuelidol"))
            characterAni.SetBool("idol", true);
        else
            characterAni.SetBool("idol", false);
        if (charInfo.body.Contains("shuel") && charInfo.onepiece.Contains("patissier") && charInfo.acc.Contains("patissier"))
            characterAni.SetBool("patissier", true);
        else
            characterAni.SetBool("patissier", false);
        if (charInfo.body.Contains("suha") && charInfo.top == "officeworker_top" && charInfo.bottom == "officeworker_bottom" && charInfo.shoes == "officeworker_shoes")
            characterAni.SetBool("officeworker2", true);
        else
            characterAni.SetBool("officeworker2", false);
        if (charInfo.body.Contains("suha") && charInfo.top == "officeworker2_top" && charInfo.bottom == "officeworker2_bottom" && charInfo.shoes == "officeworker2_shoes")
            characterAni.SetBool("officeworker", true);
        else
            characterAni.SetBool("officeworker", false);
        if (charInfo.body.Contains("suha") && charInfo.top == "snoop2_top" && charInfo.bottom == "snoop2_bottom" && charInfo.shoes == "snoop2_shoes")
            characterAni.SetBool("snoop", true);
        else
            characterAni.SetBool("snoop", false);
        if (characterAni.GetBool("princess") || characterAni.GetBool("witch") || characterAni.GetBool("police") || characterAni.GetBool("stewardess") || characterAni.GetBool("snoop") || characterAni.GetBool("patissier"))
        {
            if (uiState == UIState.main)
            {
                missionEffect.SetActive(true);
                instantiateSound(missionSound);
            }
            else
            {
                missionEffect.SetActive(false);
            }
        }
        else
        {
            missionEffect.SetActive(false);
        }
        if (charInfo.body.Contains("shuel"))
        {
            for (int i = 0; i < LoadAsset.instance.material.Count; i++)
            {
                if (LoadAsset.instance.material[i].name.Contains("witch2_onepiece"))
                {
                    arCharacter.transform.Find("witch_onepiece").GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    arCharacter.transform.Find("witch_onepiece").GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                }
                if (LoadAsset.instance.material[i].name.Contains("witch2_shoes"))
                {
                    arCharacter.transform.Find("witch_shoes").GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    arCharacter.transform.Find("witch_shoes").GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                }
            }
        }
        else
        {
            for (int i = 0; i < LoadAsset.instance.material.Count; i++)
            {
                if (LoadAsset.instance.material[i].name.Contains("witch_onepiece"))
                {
                    arCharacter.transform.Find("witch_onepiece").GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    arCharacter.transform.Find("witch_onepiece").GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                }
                if (LoadAsset.instance.material[i].name.Contains("witch_shoes"))
                {
                    arCharacter.transform.Find("witch_shoes").GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    arCharacter.transform.Find("witch_shoes").GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                }
            }
        }
    }
   
    public void loadChar()
    {
        //StartCoroutine(appearAni());
        arCharacter.transform.gameObject.SetActive(false);
        arCharacter.transform.gameObject.SetActive(true);
        characterAni.SetBool(charInfo.body, true);
        PopupManager.instance.goSettingHome();
        PopupManager.instance.closeSetting();
        isPopup = false;
        if(uiState == UIState.camera)
        {
            GuideLine.SetActive(false);
        }
        else if(uiState == UIState.main)
        {
            GuideLine.SetActive(true);
        }

        appearCharEffect.SetActive(false);
        effectCamera.SetActive(true);
        appearCharEffect.SetActive(true);
    }

    public IEnumerator appearAni()
    {
        arCharacter.transform.gameObject.SetActive(false);
        arCharacter.transform.gameObject.SetActive(true);
        characterAni.SetBool("appear_char", true);
        appearCharEffect.SetActive(false);
        appearCharEffect.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        changeChar(charInfo.body);
        characterAni.SetBool("appear_char", false);
    }

    public void btnGuide()
    {
        isPopup = true;
        PopupManager.instance.popupState = PopupManager.Popup.guide;
        GuideLine.SetActive(false);
        guideButtonUI.transform.Find("obj").gameObject.SetActive(false);
        guideButtonUI.transform.Find("Guide").gameObject.SetActive(true);

    }
    public void closeGuide()
    {
        Transform contentPos = guideButtonUI.transform.Find("Guide").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>();
        isPopup = false;
        PopupManager.instance.popupState = PopupManager.Popup.home;
        guideButtonUI.transform.Find("Guide").gameObject.SetActive(false);
        guideButtonUI.transform.Find("obj").gameObject.SetActive(true);
        contentPos.localPosition = new Vector3(contentPos.localPosition.x, 0, contentPos.localPosition.z);
        GuideLine.SetActive(true);
    }
    public void openURL(string url)
    {
        Application.OpenURL(url);
    }
   
    public void changeChar(string cardName)
    {
        characterAni.SetBool("char_ari", false);
        characterAni.SetBool("char_min", false);
        characterAni.SetBool("char_shuel", false);
        characterAni.SetBool("char_suha", false);

        arCharacter.transform.GetChild(0).gameObject.SetActive(false);
        arCharacter.transform.GetChild(1).gameObject.SetActive(false);
        arCharacter.transform.GetChild(2).gameObject.SetActive(false);
        arCharacter.transform.GetChild(3).gameObject.SetActive(false);
        arCharacter.transform.Find(cardName).gameObject.SetActive(true);
        characterAni.SetBool(cardName, true);

    }
    /// <summary>
    /// 캐릭터 의상제거
    /// </summary>
    /// <param name="obj"></param>
    public void offCharObj(string obj)
    {
        if (uiState == UIState.main || uiState == UIState.camera)
        {
            GameObject[] anotherObj = new GameObject[arCharacter.transform.childCount];
            for (int i = 0; i < anotherObj.Length; i++)
            {
                anotherObj[i] = arCharacter.transform.GetChild(i).gameObject;
                if (anotherObj[i].name.Contains(obj))
                {
                    anotherObj[i].SetActive(false);
                }
            }
        }
        else if (uiState == UIState.fashion)
        {
            GameObject[] anotherObj = new GameObject[fashionCharacter.transform.childCount];
            for (int i = 0; i < anotherObj.Length; i++)
            {
                anotherObj[i] = fashionCharacter.transform.GetChild(i).gameObject;
                if (anotherObj[i].name.Contains(obj))
                {
                    anotherObj[i].SetActive(false);
                }
            }
        }
    }
    public void WriteData(string strData)

    {
        string m_strPath = "Assets/Resources/";

        FileStream f = new FileStream(m_strPath + "Data.txt", FileMode.Append, FileAccess.Write);

        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Unicode);

        writer.WriteLine(strData);
        writer.Close();

    }
    public void ReadData()
    {
        TextAsset data = Resources.Load("Data", typeof(TextAsset)) as TextAsset;
        StringReader sr = new StringReader(data.text);

        string sources = sr.ReadLine();
        string[] values;
        while (sources != null)
        {
            values = sources.Split(',');

            if (values.Length == 0)
            {
                sr.Close();
                return;
            }
            sources = sr.ReadLine();

        }
    }
    public void modeChangeDrag(GameObject obj)
    {
        Vector3 pos;
        pos = obj.transform.position;

        if (uiState == UIState.main)
        {
            pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;
            obj.transform.position = pos;
            if (pos.x < cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x)
            {
                pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
                obj.transform.position = pos;
            }
            if (pos.x > cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width * 3 / 4, 1280, 0)).x)
            {
                Color color = new Color(1, 0.6f, 0.8f, 1);
                canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            }
            else
            {
                Color color = new Color(1, 1, 1);
                canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            }
        }
        else if (uiState == UIState.camera)
        {
            CharacterController.instance.gameObject.GetComponent<TransformGesture>().enabled = false;
            if (isCardRecognition)
            {
                //인식모드
                pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;
                obj.transform.position = pos;
                if(pos.x> cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width/2, 0, 0)).x)
                {
                    pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
                    obj.transform.position = pos;
                }
                if (pos.x < cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 4, 1280, 0)).x)
                {
                    Color color = new Color(1, 0.6f, 0.8f);
                    canvasCameraUI.transform.Find("panelBottom").transform.Find("RecognitionModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
                }
                else
                {
                    Color color = new Color(1, 1, 1);
                    canvasCameraUI.transform.Find("panelBottom").transform.Find("RecognitionModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
                }
            }
        }
        else if (uiState == UIState.save)
        {
            //인식, 카메라모드
            pos.x = cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;
            obj.transform.position = pos;

            if (pos.x < cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 4, 1280, 0)).x)
            {
                Color color = new Color(1, 0.6f, 0.8f);
                canvasSaveUI.transform.Find("RecognitionModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            }
            
            else if (pos.x > cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width * 3 / 4, 1280, 0)).x)
            {
                Color color = new Color(1, 0.6f, 0.8f);
                canvasSaveUI.transform.Find("cameraModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            }
            else
            {
                Color color = new Color(1,1, 1);
                canvasSaveUI.transform.Find("cameraModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
                canvasSaveUI.transform.Find("RecognitionModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            }
        }
    }
   
    public void modeBtnDown()
    {
        Color color = new Color(1, 1, 1,1);
        if (uiState == UIState.main)
        {
            canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(true);
            canvasBottomButtonUI.transform.Find("cameraModeImg").GetComponent<UnityEngine.UI.Image>().color = color;
        }
        else if (uiState == UIState.camera)
        {
            if (isCardRecognition)
            {
                canvasCameraUI.transform.Find("panelBottom").transform.Find("RecognitionModeImg").gameObject.SetActive(true);
                canvasCameraUI.transform.Find("panelBottom").transform.Find("RecognitionModeImg").GetComponent<UnityEngine.UI.Image>().color = color;
            }
        }
        else if (uiState == UIState.save)
        {
            canvasSaveUI.transform.Find("RecognitionModeImg").gameObject.SetActive(true);
            canvasSaveUI.transform.Find("RecognitionModeImg").GetComponent<UnityEngine.UI.Image>().color = color;
            canvasSaveUI.transform.Find("cameraModeImg").gameObject.SetActive(true);
            canvasSaveUI.transform.Find("cameraModeImg").GetComponent<UnityEngine.UI.Image>().color = color;
        }
    }
    public void modeBtnUp(GameObject obj)
    {
        Vector3 pos;
        pos = obj.transform.position;

        if (uiState == UIState.main)
        {
            //카메라모드
            pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 1280, 0)).x;
            obj.transform.position = pos;
            if (Input.mousePosition.x > Screen.width *3/4)
                goCameraScene();
            canvasBottomButtonUI.transform.Find("cameraModeImg").GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else if (uiState == UIState.camera)
        {
            //인식모드
            CharacterController.instance.gameObject.GetComponent<TransformGesture>().enabled = true;
            if (isCardRecognition) { 
}
            pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 1280, 0)).x;
            obj.transform.position = pos;
            if (Input.mousePosition.x < Screen.width / 4)
            {
                if (isCardRecognition)
                {
                    goTrackingScene();
                }
            }
            else if (Input.mousePosition.x > Screen.width / 2 - 50 && Input.mousePosition.x < Screen.width / 2 + 50)
            {
                goPaintScene();
                instantiateSound(captureSound);
            }
            canvasCameraUI.transform.Find("panelBottom").transform.Find("RecognitionModeImg").gameObject.SetActive(false);
        }
        else if (uiState == UIState.save)
        {
            //인식, 카메라모드
            pos.x = cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 1280, 0)).x;
            obj.transform.position = pos;
            if (Input.mousePosition.x < Screen.width / 4)
                goTrackingScene();
            else if (Input.mousePosition.x > Screen.width *3/4)
            {
                goCameraScene();
                checkAnimation();
            }
            canvasSaveUI.transform.Find("RecognitionModeImg").gameObject.SetActive(false);
            canvasSaveUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
        }
    }
    public void instantiateSound(GameObject sound)
    {
        Instantiate(sound);
    }
    ////보관함 팝업 
    //public void btnLocker ()
    //{
    //    isPopup = true;
    //    dressPopup.SetActive(true);
    //    if (0 < dicLockerChar.Count && dicLockerChar.Count < 4)
    //    {
    //        if (dicLockerChar.ContainsKey("char0"))
    //        {
    //            if (dicLockerChar["char0"].body.Contains("char"))
    //            {
    //                GameObject[] anotherObj = new GameObject[dressPopup.transform.Find("GameObject").transform.Find("locker_1").childCount];
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_1").gameObject.SetActive(true);
    //                for (int i = 0; i < dressPopup.transform.Find("GameObject").transform.Find("locker_1").childCount; i++)
    //                {
    //                    anotherObj[i] = dressPopup.transform.Find("GameObject").transform.Find("locker_1").GetChild(i).gameObject;
    //                    anotherObj[i].SetActive(false);
    //                }
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("btnLoad").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("btnRemove").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("body").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find(dicLockerChar["char0"].body).gameObject.SetActive(true);
    //                //top;
    //                if (dicLockerChar["char0"].top.Contains("top"))
    //                {
    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find(dicLockerChar["char0"].top).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    if (dicLockerChar["char0"].onepiece.Contains("onepiece"))
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("basic_top").gameObject.SetActive(false);
    //                    else
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("basic_top").gameObject.SetActive(true);
    //                }
    //                //bottom

    //                if (dicLockerChar["char0"].bottom.Contains("bottom"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find(dicLockerChar["char0"].bottom).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    if(dicLockerChar["char0"].onepiece.Contains("onepiece"))
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("basic_bottom").gameObject.SetActive(false);
    //                    else
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find("basic_bottom").gameObject.SetActive(true);
    //                }
    //                //onepiece
    //                if (dicLockerChar["char0"].onepiece.Contains("onepiece"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find(dicLockerChar["char0"].onepiece).gameObject.SetActive(true);
    //                }

    //                //shoes

    //                if (dicLockerChar["char0"].shoes.Contains("shoes"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find(dicLockerChar["char0"].shoes).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                }
    //                //acc

    //                if (dicLockerChar["char0"].acc.Contains("acc"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_1").transform.Find(dicLockerChar["char0"].acc).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                }
    //            }
    //        }
    //        else
    //        {

    //        }
    //        if (dicLockerChar.ContainsKey("char1"))
    //        {
    //            if (dicLockerChar["char1"].body.Contains("char"))
    //            {
    //                GameObject[] anotherObj = new GameObject[dressPopup.transform.Find("GameObject").transform.Find("locker_2").childCount];
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_2").gameObject.SetActive(true);
    //                for (int i = 0; i < dressPopup.transform.Find("GameObject").transform.Find("locker_2").childCount; i++)
    //                {
    //                    anotherObj[i] = dressPopup.transform.Find("GameObject").transform.Find("locker_2").GetChild(i).gameObject;
    //                    anotherObj[i].SetActive(false);
    //                }
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("btnLoad").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("btnRemove").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("body").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find(dicLockerChar["char1"].body).gameObject.SetActive(true);
    //                //TOP

    //                if (dicLockerChar["char1"].top.Contains("top"))
    //                {
    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find(dicLockerChar["char1"].top).gameObject.SetActive(true);

    //                }
    //                else
    //                {
    //                    if (dicLockerChar["char1"].onepiece.Contains("onepiece"))
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("basic_top").gameObject.SetActive(false);
    //                    else
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("basic_top").gameObject.SetActive(true);
    //                }


    //                //bottom
    //                if (dicLockerChar["char1"].bottom.Contains("bottom"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find(dicLockerChar["char1"].bottom).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    if (dicLockerChar["char1"].onepiece.Contains("onepiece"))
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("basic_bottom").gameObject.SetActive(false);
    //                    else
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find("basic_bottom").gameObject.SetActive(true);
    //                }
    //                //onepiece
    //                if (dicLockerChar["char1"].onepiece.Contains("onepiece"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find(dicLockerChar["char1"].onepiece).gameObject.SetActive(true);
    //                }

    //                //shoes

    //                if (dicLockerChar["char1"].shoes.Contains("shoes"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find(dicLockerChar["char1"].shoes).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                }
    //                //acc

    //                if (dicLockerChar["char1"].acc.Contains("acc"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_2").transform.Find(dicLockerChar["char1"].acc).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                }
    //            }

    //        }
    //        else
    //        {

    //        }
    //        if (dicLockerChar.ContainsKey("char2"))
    //        {
    //            if (dicLockerChar["char2"].body.Contains("char"))
    //            {
    //                GameObject[] anotherObj = new GameObject[dressPopup.transform.Find("GameObject").transform.Find("locker_3").childCount];
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_3").gameObject.SetActive(true);
    //                for (int i = 0; i < dressPopup.transform.Find("GameObject").transform.Find("locker_3").childCount; i++)
    //                {
    //                    anotherObj[i] = dressPopup.transform.Find("GameObject").transform.Find("locker_3").GetChild(i).gameObject;
    //                    anotherObj[i].SetActive(false);
    //                }
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("btnLoad").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("btnRemove").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("body").gameObject.SetActive(true);
    //                dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find(dicLockerChar["char2"].body).gameObject.SetActive(true);
    //                //TOP

    //                if (dicLockerChar["char2"].top.Contains("top"))
    //                {
    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find(dicLockerChar["char2"].top).gameObject.SetActive(true);

    //                }
    //                else
    //                {
    //                    if (dicLockerChar["char2"].onepiece.Contains("onepiece"))
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("basic_top").gameObject.SetActive(false);
    //                    else
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("basic_top").gameObject.SetActive(true);
    //                }


    //                //bottom
    //                if (dicLockerChar["char2"].bottom.Contains("bottom"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find(dicLockerChar["char2"].bottom).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    if (dicLockerChar["char2"].onepiece.Contains("onepiece"))
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("basic_bottom").gameObject.SetActive(false);
    //                    else
    //                        dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find("basic_bottom").gameObject.SetActive(true);
    //                }
    //                //onepiece
    //                if (dicLockerChar["char2"].onepiece.Contains("onepiece"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find(dicLockerChar["char2"].onepiece).gameObject.SetActive(true);
    //                }

    //                //shoes

    //                if (dicLockerChar["char2"].shoes.Contains("shoes"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find(dicLockerChar["char2"].shoes).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                }
    //                //acc

    //                if (dicLockerChar["char2"].acc.Contains("acc"))
    //                {

    //                    dressPopup.transform.Find("GameObject").transform.Find("locker_3").transform.Find(dicLockerChar["char2"].acc).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                }
    //            }

    //        }
    //        else
    //        {

    //        }
    //    }
    //    else if(dicLockerChar.Count == 0)
    //    {
    //        //알림메세지
    //        PopupManager.instance.showToast("아직 저장해 놓은 친구들이 없네~");
    //    }
    //}


    //public void closeDressPopup()
    //{
    //    dressPopup.SetActive(false);
    //    isPopup = false;
    //}
    public void cameraSwitch()
    {
        instantiateSound(clickSound);
        Vector3 charPos = new Vector3(character.transform.localPosition.x, -1 * character.transform.localPosition.y, character.transform.localPosition.z);
        Vector3 charRot = character.transform.localRotation.eulerAngles;
        float z = charRot.y;
        StartCoroutine(waitCloudReco());
        CameraSettings tmp = FindObjectOfType<CameraSettings>();
        if (tmp.IsFrontCameraActive())
        {
            //charPos.z = CharacterController.instance.pos.transform.localPosition.z;
            //character.transform.localPosition = charPos;
            //character.transform.localRotation = Quaternion.Euler(new Vector3(90,  z, 0));
            //character.SetActive(false);
            tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_BACK);
            testT.text = character.transform.localRotation.eulerAngles.ToString();
            //character.SetActive(true);
        }
        else
        {
            //charPos.z = CharacterController.instance.switchPos.transform.localPosition.z;
            //testT.text = charPos.ToString();
            //character.transform.localPosition = charPos;
            //character.transform.localRotation = Quaternion.Euler(new Vector3(90, z, 0));
            tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_FRONT);
            testT.text = character.transform.localRotation.eulerAngles.ToString();
            //character.SetActive(false);
            //character.SetActive(true);
            //changeChar(charInfo.body);
            if (charInfo.body != "0")
                StartCoroutine(appearChar());
        }
    }
    IEnumerator appearChar()
    {
        dressroomLoad = false;
        character.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        character.SetActive(true);
        changeChar(charInfo.body);
        checkAnimation();
    }
    public void album()
    {
        AndroidFuntionCall.instance.btnGallery();
        ////androidfunction
        //{
        //    AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //    AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //    intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_GET_CONTENT"));
        //    AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        //    AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "content://media/internal/images/media");
        //    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        //    intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
        //    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //    currentActivity.Call("startActivity", intentObject);
        //}

        //goPaintScene();
    }


    public void removePaint()
    {
        PaintManager.paintList = new List<GameObject>();
        for (int i = 0; i < PaintManager.instance.parentPaint.transform.childCount; i++)
        {
            PaintManager.paintList.Add(PaintManager.instance.parentPaint.transform.GetChild(i).gameObject);
            Destroy(PaintManager.paintList[i].gameObject);

            PaintManager.objCount = 0;
        }
    }

    public void goTrackingScene()
    {
       
        availableRecognize = true;
        cameraAR.SetActive(true);
        CameraSettings tmp = FindObjectOfType<CameraSettings>();
        if (uiState == UIState.camera)
        {
            if (tmp.IsFrontCameraActive())
                tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_BACK);
            CharacterController.instance.resetTrans();
            canvasCameraUI.SetActive(false);
            effectCamera.SetActive(true);
            //resetChar();
        }
        else if (uiState == UIState.save)
        {
            if (!isCardRecognition)
            {
                FashionRecognition.canvasFashion.transform.Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                FashionRecognition.canvasFashion.transform.Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                FashionRecognition.canvasFashion.transform.Find("top").gameObject.SetActive(false);
                FashionRecognition.canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
            }
            //isTracking = false;
            if (tmp.IsFrontCameraActive())
                tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_BACK);
            character.SetActive(false);
            canvasSaveUI.SetActive(false);
            removePaint();
            PaintManager.instance.paintLayout.SetActive(true);
            cameraPaintUI.SetActive(false);
            canvasPaintUI.SetActive(false);
            CharacterController.instance.resetTrans();
            resetChar();
            guideButtonUI.gameObject.SetActive(true);
            closeGuide();
        }
        else if (uiState == UIState.paint)
        {
            cameraPaintUI.SetActive(false);
            canvasPaintUI.SetActive(false);
            removePaint();
        }
        if (charInfo.body.Contains("char"))
        {
            for (int i = 0; i < 4; i++)
            {
                recognizeObj[i].SetActive(true);
            }
        }
        canvasCaptureUI.SetActive(false);
        canvasBottomButtonUI.SetActive(true);
        canvasTopButtonUI.SetActive(true);
        //charControlBtnUI.SetActive(true);
        GuideLine.SetActive(true);
        uiState = UIState.main;

        if (PlayerPrefs.GetString("mission") != DateTime.Now.ToString("yyyyMMdd") && charInfo.body == "0")
        {
            PopupManager.instance.openToday();
        }
        canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(false);
        AndroidFuntionCall.instance.img.gameObject.SetActive(false);
        canvasTopButtonUI.transform.Find("btnCamSwitch").gameObject.SetActive(true);
        //canvasTopButtonUI.transform.Find("btnGuide").gameObject.SetActive(true);
        PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("SettingPage").Find("btnSetting").gameObject.SetActive(true);
        cameraAR.transform.position = new Vector3(0, 0, 0);
        cameraAR.transform.Find("Camera").transform.position = new Vector3(0, 0, 0);
        cameraAR.transform.Find("Camera").Find("BackgroundPlane").transform.position = new Vector3(0, 0, 400);

        //StartCoroutine(waitCloudReco());
        tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_BACK);
        cameraAR.SetActive(true);
        SwitchAutofocus(true);
         if (charInfo.body.Contains("char"))
        {
            canvasBottomButtonUI.transform.Find("cameraModeImg").GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1,0.5f);
        }
        else
        {
            canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
        }
    }
    public IEnumerator waitCloudReco()
    {
        GuideLine.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        if (uiState == UIState.main && availableRecognize)
            GuideLine.gameObject.SetActive(true);
    }
    public void goCameraScene()
    {
        availableRecognize = false;
        if (uiState != UIState.main)
        {

            cameraAR.SetActive(true);
            SwitchAutofocus(true);
            CameraSettings tmp = FindObjectOfType<CameraSettings>();
            if (tmp.IsFrontCameraActive())
            {
                StartCoroutine(appearChar());
            }
        }
        if (uiState == UIState.main)
        {
            effectCamera.SetActive(false);
            appearCharEffect.SetActive(false);
            GuideLine.SetActive(false);
            canvasBottomButtonUI.SetActive(false);
            canvasCameraUI.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                recognizeObj[i].SetActive(false);
            }
            instance.uiState = UIState.camera;
        }
        else if (uiState == UIState.paint)
        {
            //if(isTracking == false)
            //{
            //    goTrackingScene();
            //}
            //else
            //{
            paintState = PaintState.none;
            //resetPaintLayout();
            removePaint();
            PaintManager.textLayout.SetActive(false);
            PaintManager.penLayout.SetActive(false);
            PaintManager.mainLayout.SetActive(true);

            cameraPaintUI.SetActive(false);
            canvasPaintUI.SetActive(false);
            canvasCaptureUI.SetActive(false);

            charControlBtnUI.SetActive(true);
           
            cameraMainUI.SetActive(true);
            canvasTopButtonUI.SetActive(true);

            

            canvasCameraUI.SetActive(true);
            uiState = UIState.camera;
            instantiateSound(clickSound);

            

            //}
        }
        else if (uiState == UIState.save)
        {
           
            canvasCaptureUI.SetActive(false);
            removePaint();
            cameraMainUI.SetActive(true);
            cameraPaintUI.SetActive(false);
            canvasPaintUI.SetActive(false);
            canvasSaveUI.SetActive(false);
            charControlBtnUI.SetActive(true);
            canvasTopButtonUI.SetActive(true);
            PaintManager.instance.paintLayout.SetActive(true);
            canvasCameraUI.SetActive(true);
            charCamera.transform.Find("Canvas").transform.Find("Plane").gameObject.SetActive(false);
            canvasTopButtonUI.transform.Find("btnCamSwitch").gameObject.SetActive(true);
            uiState = UIState.camera;
        }
        else if(uiState == UIState.fashion)
        {
            fashionCamera.SetActive(false);
            PopupManager.settingPage.SetActive(true);
            cameraMainUI.SetActive(true);
            effectCamera.SetActive(false);
            appearCharEffect.SetActive(false);
            GuideLine.SetActive(false);
            canvasBottomButtonUI.SetActive(false);
            canvasCameraUI.SetActive(true);
            guideButtonUI.SetActive(false);
            charControlBtnUI.SetActive(true);
            charControlBtnUI.transform.Find("top").gameObject.SetActive(false);
            charControlBtnUI.transform.Find("bottom").gameObject.SetActive(false);
            charControlBtnUI.transform.Find("shoes").gameObject.SetActive(false);
            charControlBtnUI.transform.Find("acc").gameObject.SetActive(false);

            uiState = UIState.camera;
        }
        canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(true);
        //canvasTopButtonUI.transform.Find("btnGuide").gameObject.SetActive(false);
        PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("SettingPage").Find("btnSetting").gameObject.SetActive(true);
        loadGallery = false;
        GuideLine.SetActive(false);

    }

    public void goPaintScene()
    {
        if (uiState == UIState.camera)
        {
            canvasCameraUI.SetActive(false);
            canvasTopButtonUI.SetActive(false);
            charControlBtnUI.SetActive(false);
            StartCoroutine(takePicture());
        }
        else if (uiState == UIState.paint)
        {
            if (paintState == PaintState.pen)
            {
                PaintManager.penLayout.SetActive(false);
                PaintManager.colorState = 0;
                PaintManager.penState = 0;
            }
            else if (paintState == PaintState.text)
            {
                PaintManager.textLayout.SetActive(false);
                PaintManager.instance.inputField.text = "";
                PaintManager.colorState = 0;
                PaintManager.fontState = 0;
            }
            uiState = UIState.paint;
            paintState = PaintState.none;
            PaintManager.mainLayout.SetActive(true);
        }
        else if (uiState == UIState.save)
        {
            PaintManager.instance.paintLayout.SetActive(true);
            canvasSaveUI.SetActive(false);
            uiState = UIState.paint;
        }
        //갤러리
        else if (uiState == UIState.main)
        {
            GuideLine.SetActive(false);
            canvasBottomButtonUI.SetActive(false);
            canvasTopButtonUI.SetActive(false);
            charControlBtnUI.SetActive(false);

            //갤러리 사진 
            string path = "/storage/emulated/0/Download/박주현 증명사진.JPG";
            pathList = Directory.GetFiles(path);
            StartCoroutine(LoadImage());
        }
        canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(false);
        PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("SettingPage").Find("btnSetting").gameObject.SetActive(false);
        //cameraAR.SetActive(false);
    }

    public void goSaveScene()
    {
        if (uiState == UIState.paint)
        {
            paintState = PaintState.none;
            PaintManager.instance.paintLayout.SetActive(false);
            StartCoroutine(takePicture());
        }
        cameraAR.SetActive(false);
    }
    public IEnumerator LoadImage()
    {
        texList = new Texture2D[pathList.Length];

        int dummy = 0;
        foreach (string tstring in pathList)
        {
            string pathTemp = @"file://" + tstring;
            WWW www = new WWW(pathTemp);
            yield return www;
            Texture2D texTmp = new Texture2D(Screen.width, Screen.height, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(texTmp);
            texList[dummy] = texTmp;
            capturePic.GetComponent<MeshRenderer>().material.mainTexture = texTmp;
        }
        canvasCaptureUI.SetActive(true);
        cameraPaintUI.SetActive(true);
        canvasPaintUI.SetActive(true);
        uiState = UIState.paint;
        paintState = PaintState.none;
        PaintManager.mainLayout.SetActive(true);
    }

    public IEnumerator takePicture()
    {
        yield return new WaitForEndOfFrame();

        // 화면 캡쳐

        if (uiState == UIState.camera)
        {
            texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
            texture.Apply();
            capturePic.GetComponent<MeshRenderer>().material.mainTexture = texture;
            canvasCaptureUI.SetActive(true);
            //cameraChar.SetActive(false);

            uiState = UIState.paint;
            canvasBottomButtonUI.SetActive(false);
            paintState = PaintState.none;
            cameraPaintUI.SetActive(true);
            canvasPaintUI.SetActive(true);
        }
        else
        {
            texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
            texture.Apply();
            save();
        }
    }
    //<<<<<<< HEAD
    //=======

#if UNITY_IOS
	        private IPicker picker = 
		new PickeriOS();

	public void callShareForiOS()
	{
		try
		{
			picker.SampleMethod(myDefaultLocation,"photo share!!!");
			Debug.Log("iOS Photo Copy");


		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message, this);
		}

	}
#endif


    //>>>>>>> 231c825e4c65ba9c45496cd9d933e29851f1748c
    public void save()
    {
        imageByte = texture.EncodeToPNG();
        DestroyImmediate(texture);

        myFilename = string.Format("Mirage_{0}.png", System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        myFolderLocation = "/storage/emulated/0/DCIM/Mirage/";
        myScreenshotLocation = myFolderLocation + myFilename;
        myDefaultLocation = Application.persistentDataPath + "/" + myFilename;


        // DCIM 폴더에 디렉토리 생성
        if (!Directory.Exists(myFolderLocation))
        {
            Directory.CreateDirectory(myFolderLocation);
        }

        // 임시 디렉토리에 저장

        try
        {
            File.WriteAllBytes(myDefaultLocation, imageByte);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message, this);
        }
        finally
        {
            imageByte = null;
        }

        // 임시 디렉토리에서 DCIM 폴더로 이동
        try
        {
            instantiateSound(saveSound);

            File.Move(myDefaultLocation, myScreenshotLocation);
            canvasSaveUI.SetActive(true);
            PopupManager.instance.showPraiseMsgPopup();
            uiState = UIState.save;
#if UNITY_ANDROID
            AndroidFuntionCall.instance.mediaScan();

#endif
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message, this);
        }
        //<<<<<<< HEAD

        //=======

#if UNITY_IOS
//			callShareForiOS();

		picker.SaveToGalley("gallery", myDefaultLocation);

			instantiateSound(saveSound);
			canvasSaveUI.SetActive(true);
			PopupManager.instance.showPraiseMsgPopup();
			uiState = UIState.save;

#endif
        //>>>>>>> 231c825e4c65ba9c45496cd9d933e29851f1748c
    }


    public void resetChar()
    {
        charInfo = new CharacterInfo("0", "0", "0", "0", "0", "0");
        arCharacter.gameObject.SetActive(false);
    }

    private List<string> GetAllGalleryImagePaths()
    {
        List<string> results = new List<string>();
        HashSet<string> allowedExtesions = new HashSet<string>() { ".png", ".jpg", ".jpeg" };

        try
        {
            AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");

            // Set the tags for the data we want about each image.  This should really be done by calling; 
            //string dataTag = mediaClass.GetStatic<string>("DATA");
            // but I couldn't get that to work...

            const string dataTag = "_data";

            string[] projection = new string[] { dataTag };
            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");

            string[] urisToSearch = new string[] { "EXTERNAL_CONTENT_URI", "INTERNAL_CONTENT_URI" };
            foreach (string uriToSearch in urisToSearch)
            {
                AndroidJavaObject externalUri = mediaClass.GetStatic<AndroidJavaObject>(uriToSearch);
                AndroidJavaObject finder = currentActivity.Call<AndroidJavaObject>("managedQuery", externalUri, projection, null, null, null);
                bool foundOne = finder.Call<bool>("moveToFirst");
                while (foundOne)
                {
                    int dataIndex = finder.Call<int>("getColumnIndex", dataTag);
                    string data = finder.Call<string>("getString", dataIndex);
                    if (allowedExtesions.Contains(Path.GetExtension(data).ToLower()))
                    {
                        string path = @"file:///" + data;
                        results.Add(path);
                    }

                    foundOne = finder.Call<bool>("moveToNext");
                }
            }
        }
        catch (System.Exception e)
        {
            // do something with error...
			Debug.Log(e.ToString());
        }

        return results;
    }
    public void showCardInfo(GameObject obj)
    {

        if (obj.transform.name == "top" && (charInfo.top.Contains("top") || charInfo.onepiece.Contains("onepiece")))
        {
            obj.transform.Find("Image").transform.gameObject.SetActive(true);
        }
        else if (obj.transform.name == "bottom" && (charInfo.bottom.Contains("bottom") || charInfo.onepiece.Contains("onepiece")))
        {
            obj.transform.Find("Image").transform.gameObject.SetActive(true);
        }
        else if (obj.transform.name == "shoes" && charInfo.shoes.Contains("shoes"))
        {
            obj.transform.Find("Image").transform.gameObject.SetActive(true);
        }
        else if (obj.transform.name == "acc" && charInfo.acc.Contains("acc"))
        {
            obj.transform.Find("Image").transform.gameObject.SetActive(true);
        }
    }
    public void btnRecogObj(bool btn)
    {
        instantiateSound(clickSound);
        if(uiState == UIState.main)
        {
            if (btn)
            {
                if (isOnCloth)
                    testArCameraTrackableEventHandler.instance.recObj(recogObjName);
                else
                    offCloth(offObjName);
            }
            else
            {
                if (recogObjName.Contains("char") && !charInfo.body.Contains("char"))
                {
                    guideButtonUI.SetActive(true);
                    closeGuide();
                }
                recogObjName = null;
                offObjName = null;
            }
            canvasRecObj.SetActive(false);
            availableRecognize = true;
            GuideLine.SetActive(true);
        }
        else if(uiState == UIState.fashion)
        {
            if (btn)
            {
                offCloth(offObjName);
                onClothCount--;
            }
            else
            {

            }
            offObjName = null;
            FashionRecognition.canvasFashion.transform.Find("popup").gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 의상 아이콘 팝업
    /// </summary>
    /// <param name="obj"></param>
    public void offClothPopup(string obj)
    {
        if (uiState == UIState.main)
        {
            availableRecognize = false;
            isOnCloth = false;
            canvasRecObj.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];
            if(!obj.Contains("char"))
            {
                canvasRecObj.transform.Find("Text").GetComponent<Text>().text = "해당의상을 지우겠습니까?";
            }
            canvasRecObj.transform.Find("btnConfirm").Find("Text").GetComponent<Text>().text = "지움";
            offObjName = obj;
            canvasRecObj.SetActive(true);
            GuideLine.SetActive(false);
        }
        else if(uiState == UIState.fashion)
        {
            FashionRecognition.canvasFashion.transform.Find("popup").gameObject.SetActive(true);
            Debug.Log("obj : " + obj + "_" + selectColor + "_img");
            FashionRecognition.canvasFashion.transform.Find("popup").Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[obj + "_"+selectColor + "_img"];
            if (!obj.Contains("char"))
            {
                FashionRecognition.canvasFashion.transform.Find("popup").Find("Text").GetComponent<Text>().text = "해당의상을 지우겠습니까?";
            }
            FashionRecognition.canvasFashion.transform.Find("popup").transform.Find("btnConfirm").Find("Text").GetComponent<Text>().text = "지움";
            FashionRecognition.canvasFashion.transform.Find("popup").gameObject.SetActive(true);

            offObjName = obj;

        }
    }
    /// <summary>
    /// 의상아이콘 
    /// </summary>
    /// <param name="cloth"></param>
    public void offCloth(string cloth)
    {
        if (uiState == UIState.main || uiState == UIState.camera)
        {
            if (cloth.Contains("top"))
            {
                offCharObj("top");
                charInfo.top = "0";
                recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                arCharacter.transform.Find("basic_top").transform.gameObject.SetActive(true);
            }
            else if (cloth.Contains("bottom"))
            {
                offCharObj("bottom");
                charInfo.bottom = "0";
                recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                arCharacter.transform.Find("basic_bottom").transform.gameObject.SetActive(true);
            }
            else if (cloth.Contains("onepiece"))
            {
                offCharObj("onepiece");
                charInfo.onepiece = "0";
                arCharacter.transform.Find("basic_top").transform.gameObject.SetActive(true);
                arCharacter.transform.Find("basic_bottom").transform.gameObject.SetActive(true);
                recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
                recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
            }
            else if (cloth.Contains("shoes"))
            {
                offCharObj("shoes");
                charInfo.shoes = "0";
                recognizeObj[2].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_shoes");
            }
            else if (cloth.Contains("acc"))
            {
                offCharObj("acc");
                charInfo.acc = "0";
                recognizeObj[3].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_acc");
            }
            checkAnimation();
            offObjName = null;

        }
        else if(uiState == UIState.fashion)
        {
            if (cloth.Contains("top")||cloth.Contains("onepiece"))
            {
                if(charInfo.top != "0")
                {
                    offCharObj("top");
                    charInfo.top = "0";
                    FashionRecognition.canvasFashion.transform.Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                    onCloth("top", "basic_top");
                }
                else if(charInfo.onepiece != "0")
                {
                    offCharObj("onepiece");
                    charInfo.onepiece = "0";
                    FashionRecognition.canvasFashion.transform.Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                    onCloth("top", "basic_top");
                    FashionRecognition.canvasFashion.transform.Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                    onCloth("bottom", "basic_bottom");
                }
            }
            else if (cloth.Contains("bottom") || cloth.Contains("onepiece"))
            {
                if(charInfo.body != "0")
                {
                    offCharObj("bottom");
                    charInfo.bottom = "0";
                    FashionRecognition.canvasFashion.transform.Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                    onCloth("bottom", "basic_bottom");
                }
                else if (charInfo.onepiece != "0")
                {
                    offCharObj("onepiece");
                    charInfo.onepiece = "0";
                    FashionRecognition.canvasFashion.transform.Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                    onCloth("top", "basic_top");
                    FashionRecognition.canvasFashion.transform.Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                    onCloth("bottom", "basic_bottom");
                }
            }
            else if (cloth.Contains("onepiece"))
            {
                offCharObj("onepiece");
                charInfo.onepiece = "0";
                arCharacter.transform.Find("basic_top").transform.gameObject.SetActive(true);
                arCharacter.transform.Find("basic_bottom").transform.gameObject.SetActive(true);
            }
            else if (cloth.Contains("shoes"))
            {
                offCharObj("shoes");
                charInfo.shoes = "0";
            }
            else if (cloth.Contains("acc"))
            {
                offCharObj("acc");
                charInfo.acc = "0";
            }
            offObjName = null;
        }

    }
    public void offCloth(GameObject obj)
    {
        instantiateSound(clickSound);
        if(uiState == UIState.main)
        {
            if (obj.transform.name == "top" && (charInfo.top.Contains("top") || charInfo.onepiece.Contains("onepiece")))
            {
                if (charInfo.top.Contains("top"))
                {
                    offClothPopup(charInfo.top);
                
                }
                else if (charInfo.onepiece.Contains("onepiece"))
                {
                    offClothPopup(charInfo.onepiece);
                }
            
            }
            else if (obj.transform.name == "bottom" && (charInfo.bottom.Contains("bottom") || charInfo.onepiece.Contains("onepiece")))
            {
                if (charInfo.bottom.Contains("bottom"))
                {
                    offClothPopup(charInfo.bottom);
                }
                else if (charInfo.onepiece.Contains("onepiece"))
                {
                    offClothPopup(charInfo.onepiece);
                }
            
            }
            else if (obj.transform.name == "shoes" && charInfo.shoes.Contains("shoes"))
            {
                offClothPopup(charInfo.shoes);
            }
            else if (obj.transform.name == "acc" && charInfo.acc.Contains("acc"))
            {
                offClothPopup(charInfo.acc);
            }
        }
        else if(uiState == UIState.fashion)
        {
            if (obj.transform.name == "top" && (charInfo.top.Contains("top") || charInfo.onepiece.Contains("onepiece")))
            {
                if (charInfo.top.Contains("top"))
                {
                    offClothPopup(charInfo.top);

                }
                else if (charInfo.onepiece.Contains("onepiece"))
                {
                    offClothPopup(charInfo.onepiece);
                }

            }
            else if (obj.transform.name == "bottom" && (charInfo.bottom.Contains("bottom") || charInfo.onepiece.Contains("onepiece")))
            {
                if (charInfo.bottom.Contains("bottom"))
                {
                    offClothPopup(charInfo.bottom);
                }
                else if (charInfo.onepiece.Contains("onepiece"))
                {
                    offClothPopup(charInfo.onepiece);
                }

            }
        }
    }
    public void onCloth(string obj, string cardName)
    {
        if (cardName.Contains(obj))
        {
            if (uiState == UIState.main || uiState == UIState.camera)
                arCharacter.transform.Find(cardName).gameObject.SetActive(true);
            else if(uiState == UIState.fashion)
                fashionCharacter.transform.Find(cardName).gameObject.SetActive(true);
        }
    }
    public void closeObj(GameObject obj)
    {
        obj.transform.gameObject.SetActive(false);
    }


    public void saveCardInfo(string cardName)
    {
        PlayerPrefs.SetInt(cardName, 1);
    }
    public void SwitchAutofocus(bool ON)
    {
        if (ON)
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
            {
                Debug.Log("Successfully enabled continuous autofocus.");
                mAutofocusEnabled = true;
            }
            else
            {
                // Fallback to normal focus mode
                Debug.Log("Failed to enable continuous autofocus, switching to normal focus mode");
                mAutofocusEnabled = false;
                CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
            }
        }
        else
        {
            Debug.Log("Disabling continuous autofocus (enabling normal focus mode).");
            mAutofocusEnabled = false;
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        }
    }
    public void setCard()
    {
        dicCard.Add("0_0", new CardInfo("char_ari", "진 아리", PlayerPrefs.GetInt("char_ari"), 0, 0, 5, 0));
        dicCard.Add("0_1", new CardInfo("char_min", "선 우민", PlayerPrefs.GetInt("char_min"), 0, 0, 6, 0));
        dicCard.Add("0_2", new CardInfo("char_shuel", "슈엘", PlayerPrefs.GetInt("char_shuel"), 0, 0, 7, 0));
        dicCard.Add("0_3", new CardInfo("char_suha", "우 수하", PlayerPrefs.GetInt("char_suha"), 0, 0, 8, 0));

        dicCard.Add("1_0", new CardInfo("ariidol_top", "가수 블라우스 셔링 베스트", PlayerPrefs.GetInt("ariidol_top"), 0, 1, 2, 180));
        dicCard.Add("1_1", new CardInfo("babysitter_top", "베이비시터 셔츠 레이어 티셔츠", PlayerPrefs.GetInt("babysitter_top"), 0, 1, 2, 190));
        dicCard.Add("1_2", new CardInfo("babysitter2_top", "베이비시터 옐로우 티셔츠", PlayerPrefs.GetInt("babysitter2_top"), 0, 1, 1, 180));
        dicCard.Add("1_3", new CardInfo("fitnesstrainer_top", "헬스트레이너 배꼽티 운동복", PlayerPrefs.GetInt("fitnesstrainer_top"), 0, 1, 4, 190));
        dicCard.Add("1_4", new CardInfo("fitnesstrainer2_top", "헬스트레이너 티 운동복", PlayerPrefs.GetInt("fitnesstrainer2_top"), 0, 1, 1, 180));
        dicCard.Add("1_5", new CardInfo("police_top", "결찰 블라우스", PlayerPrefs.GetInt("police_top"), 1, 1, 0, 430));
        dicCard.Add("1_6", new CardInfo("stewardess_top", "스튜어디스 자켓", PlayerPrefs.GetInt("stewardess_top"), 1, 1, 3, 410));
        dicCard.Add("1_7", new CardInfo("zookeeper_top", "사육사 셔츠자켓", PlayerPrefs.GetInt("zookeeper_top"), 0, 1, 0, 210));
        dicCard.Add("1_8", new CardInfo("codi_top", "패션코디네이터 오프숄더 니트", PlayerPrefs.GetInt("codi_top"), 0, 1, 4, 200));
        dicCard.Add("1_9", new CardInfo("officeworker_top", "회사원 셔츠 블루자켓", PlayerPrefs.GetInt("officeworker_top"), 0, 1, 1, 200));
        dicCard.Add("1_10", new CardInfo("officeworker2_top", "회사원 손뜨개 포인트 블라우스", PlayerPrefs.GetInt("officeworker2_top"), 0, 1, 3, 230));
        dicCard.Add("1_11", new CardInfo("snoop_top", "탐정 리본 포인트 자켓", PlayerPrefs.GetInt("snoop_top"), 0, 1, 1, 210));
        dicCard.Add("1_12", new CardInfo("snoop2_top", "탐정 블라우스 자켓", PlayerPrefs.GetInt("snoop2_top"), 1, 1, 2, 430));
        dicCard.Add("1_13", new CardInfo("teacher_top", "선생님 리본 블라우스", PlayerPrefs.GetInt("teacher_top"), 0, 1, 4, 230));
        dicCard.Add("1_14", new CardInfo("teacher2_top", "선생님 셔츠 가디건", PlayerPrefs.GetInt("teacher2_top"), 0, 1, 1, 190));
        dicCard.Add("1_15", new CardInfo("vet_top", "수의사 티 자켓", PlayerPrefs.GetInt("vet_top"), 0, 1, 0, 220));

        dicCard.Add("2_0", new CardInfo("ariidol_bottom", "가수 플리츠 스커트", PlayerPrefs.GetInt("ariidol_bottom"), 0, 2, 3, 220));
        dicCard.Add("2_1", new CardInfo("babysitter_bottom", "베이비시터 반바지", PlayerPrefs.GetInt("babysitter_bottom"), 0, 2, 2, 190));
        dicCard.Add("2_2", new CardInfo("babysitter2_bottom", "베이비시터 흰색 바지", PlayerPrefs.GetInt("babysitter2_bottom"), 0, 2, 2, 170));
        dicCard.Add("2_3", new CardInfo("fitnesstrainer_bottom", "헬스트레이너 레깅스 반바지", PlayerPrefs.GetInt("fitnesstrainer_bottom"), 0, 2, 2, 180));
        dicCard.Add("2_4", new CardInfo("fitnesstrainer2_bottom", "헬스트레이너 7부 바지", PlayerPrefs.GetInt("fitnesstrainer2_bottom"), 0, 2, 1, 180));
        dicCard.Add("2_5", new CardInfo("police_bottom", "경찰 스커트", PlayerPrefs.GetInt("police_bottom"), 1, 2, 0, 450));
        dicCard.Add("2_6", new CardInfo("stewardess_bottom", "스튜어디스 스커트", PlayerPrefs.GetInt("stewardess_bottom"), 1, 2, 3, 410));
        dicCard.Add("2_7", new CardInfo("zookeeper_bottom", "사육사 반바지", PlayerPrefs.GetInt("zookeeper_bottom"), 0, 2, 0, 210));
        dicCard.Add("2_8", new CardInfo("codi_bottom", "패션코디네이터 청바지", PlayerPrefs.GetInt("codi_bottom"), 0, 2, 4, 210));
        dicCard.Add("2_9", new CardInfo("officeworker_bottom", "회사원 블루 미니스커트", PlayerPrefs.GetInt("officeworker_bottom"), 0, 2, 1, 200));
        dicCard.Add("2_10", new CardInfo("officeworker2_bottom", "회사원 리본 블루 스커트", PlayerPrefs.GetInt("officeworker2_bottom"), 0, 2, 3, 230));
        dicCard.Add("2_11", new CardInfo("snoop_bottom", "탐정 레트 플리츠 스커트", PlayerPrefs.GetInt("snoop_bottom"), 0, 2, 1, 210));
        dicCard.Add("2_12", new CardInfo("snoop2_bottom", "탐정 블루 스커트", PlayerPrefs.GetInt("snoop2_bottom"), 1, 2, 2, 430));
        dicCard.Add("2_13", new CardInfo("teacher_bottom", "선생님 꽃무늬 스커트", PlayerPrefs.GetInt("teacher_bottom"), 0, 2, 3, 230));
        dicCard.Add("2_14", new CardInfo("teacher2_bottom", "선생님 핑크바지", PlayerPrefs.GetInt("teacher2_bottom"), 0, 2, 1, 200));
        dicCard.Add("2_15", new CardInfo("vet_bottom", "수의사 바지", PlayerPrefs.GetInt("vet_bottom"), 0, 2, 0, 220));

        dicCard.Add("3_0", new CardInfo("ariidol2_onepiece", "가수 원피스", PlayerPrefs.GetInt("ariidol2_onepiece"), 0, 3, 0, 220));
        dicCard.Add("3_1", new CardInfo("princess_onepiece", "프린세스 원피스", PlayerPrefs.GetInt("princess_onepiece"), 1, 3, 3, 400));
        dicCard.Add("3_2", new CardInfo("witch_onepiece", "치유마법 원피스", PlayerPrefs.GetInt("witch_onepiece"), 1, 3, 3, 450));
        dicCard.Add("3_3", new CardInfo("cats_onepiece", "캣츠 원피스", PlayerPrefs.GetInt("cats_onepiece"), 0, 3, 4, 240));
        dicCard.Add("3_4", new CardInfo("figure_onepiece", "피겨스케이터 원피스", PlayerPrefs.GetInt("figure_onepiece"), 0, 3, 4, 240));
        dicCard.Add("3_5", new CardInfo("shuelidol_onepiece", "아이돌 샤랄라 원피스", PlayerPrefs.GetInt("shuelidol_onepiece"), 0, 3, 4, 240));
        dicCard.Add("3_6", new CardInfo("patissier_onepiece", "파티시에 미니 셔링 원피스", PlayerPrefs.GetInt("patissier_onepiece"), 1, 3, 3, 450));

        dicCard.Add("4_0", new CardInfo("ariidol_shoes", "가수 롱양말 숏부츠 세트", PlayerPrefs.GetInt("ariidol_shoes"), 0, 4, 2, 200));
        dicCard.Add("4_1", new CardInfo("ariidol2_shoes", "가수 왕리본 핑크 구두", PlayerPrefs.GetInt("ariidol2_shoes"), 0, 4, 0, 210));
        dicCard.Add("4_2", new CardInfo("witch_shoes", "치유마법 타이즈 부츠세트", PlayerPrefs.GetInt("witch_shoes"), 1, 4, 3, 420));
        dicCard.Add("4_3", new CardInfo("babysitter_shoes", "베이비시터 셔링리본 부츠", PlayerPrefs.GetInt("babysitter_shoes"), 0, 4, 0, 170));
        dicCard.Add("4_4", new CardInfo("babysitter2_shoes", "베이비시터 실내 슬리퍼", PlayerPrefs.GetInt("babysitter2_shoes"), 0, 4, 1, 180));
        dicCard.Add("4_5", new CardInfo("fitnesstrainer_shoes", "헬스트레이너 운동화", PlayerPrefs.GetInt("fitnesstrainer_shoes"), 0, 4, 2, 190));
        dicCard.Add("4_6", new CardInfo("fitnesstrainer2_shoes", "헬스트레이너 스니커즈", PlayerPrefs.GetInt("fitnesstrainer2_shoes"), 0, 4, 1, 180));
        dicCard.Add("4_7", new CardInfo("police_shoes", "경찰 블루 구두", PlayerPrefs.GetInt("police_shoes"), 0, 4, 0, 190));
        dicCard.Add("4_8", new CardInfo("stewardess_shoes", "스튜어디스 흰색 구두", PlayerPrefs.GetInt("stewardess_shoes"), 0, 4, 3, 230));
        dicCard.Add("4_9", new CardInfo("zookeeper_shoes", "브라운 워커", PlayerPrefs.GetInt("zookeeper_shoes"), 0, 4, 0, 200));
        dicCard.Add("4_10", new CardInfo("cats_shoes", "캣츠 롱부츠", PlayerPrefs.GetInt("cats_shoes"), 0, 4, 4, 230));
        dicCard.Add("4_11", new CardInfo("codi_shoes", "패션코디네이터 블루핑크 워커", PlayerPrefs.GetInt("codi_shoes"), 0, 4, 4, 180));
        dicCard.Add("4_12", new CardInfo("figure_shoes", "피겨스케이터 신발", PlayerPrefs.GetInt("figure_shoes"), 0, 4, 4, 240));
        dicCard.Add("4_13", new CardInfo("shuelidol_shoes", "아이돌의상 스트랩 샌달", PlayerPrefs.GetInt("shuelidol_shoes"), 0, 4, 4, 240));
        dicCard.Add("4_14", new CardInfo("officeworker_shoes", "브라운 앵글부츠", PlayerPrefs.GetInt("officeworker_shoes"), 0, 4, 1, 190));
        dicCard.Add("4_15", new CardInfo("officeworker2_shoes", "회사원 꽃포인트 스트랩 구두", PlayerPrefs.GetInt("officeworker2_shoes"), 0, 4, 3, 230));
        dicCard.Add("4_16", new CardInfo("snoop2_shoes", "탐정 숏 부츠", PlayerPrefs.GetInt("snoop2_shoes"), 1, 4, 2, 350));
        dicCard.Add("4_17", new CardInfo("teacher_shoes", "선생님 보라구두", PlayerPrefs.GetInt("teacher_shoes"), 0, 4, 3, 210));
        dicCard.Add("4_18", new CardInfo("teacher2_shoes", "선생님 핑크 리본 구두", PlayerPrefs.GetInt("teacher2_shoes"), 0, 4, 1, 190));
        dicCard.Add("4_19", new CardInfo("vet_shoes", "수의사 블루리본 브라운 구두", PlayerPrefs.GetInt("vet_shoes"), 0, 4, 1, 180));

        dicCard.Add("5_0", new CardInfo("ariidol_acc", "리본 헤어밴드", PlayerPrefs.GetInt("ariidol_acc"), 0, 5, 2, 210));
        dicCard.Add("5_1", new CardInfo("princess_acc", "반짝반짝 왕관핀", PlayerPrefs.GetInt("princess_acc"), 0, 5, 3, 230));
        dicCard.Add("5_2", new CardInfo("princess2_acc", "프린세스 왕관", PlayerPrefs.GetInt("princess2_acc"), 1, 5, 3, 350));
        dicCard.Add("5_3", new CardInfo("police_acc", "경찰 모자", PlayerPrefs.GetInt("police_acc"), 1, 5, 0, 380));
        dicCard.Add("5_4", new CardInfo("stewardess_acc", "스튜어디스 모자", PlayerPrefs.GetInt("stewardess_acc"), 1, 5, 3, 410));
        dicCard.Add("5_5", new CardInfo("cats_acc", "캣츠 헤어밴드", PlayerPrefs.GetInt("cats_acc"), 0, 5, 4, 190));
        dicCard.Add("5_6", new CardInfo("shuelidol_acc", "아이돌 헤어리본", PlayerPrefs.GetInt("shuelidol_acc"), 0, 5, 4, 230));
        dicCard.Add("5_7", new CardInfo("patissier_acc", "파티시에 모자", PlayerPrefs.GetInt("patissier_acc"), 1, 5, 3, 400));
    }
    public void fasionRecognition()
    {
        isCardRecognition = false;
        
        if(charInfo.top.Contains("top") || charInfo.bottom.Contains("bottom") || charInfo.onepiece.Contains("onepiece")|| charInfo.shoes.Contains("shoes") || charInfo.acc.Contains("acc") || charInfo.body.Contains("char"))
        {
            PopupManager.instance.showPopup("패션인식화면으로 넘어가시면 기존 착용된 의상이 사라집니다. \n 계속 진행하시겠습니까?");
        }
        else
        {
            showRecognitionCanvas(isCardRecognition);
        }
    }
    public void showRecognitionCanvas(bool cardRec)     
    {
        if (cardRec)
        {
            onClothCount = 0;
            FashionRecognition.canvasFashion.transform.Find("top").gameObject.SetActive(false);
            FashionRecognition.canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
            
            uiState = UIState.main;
        }
        else
            uiState = UIState.fashion;
        PopupManager.settingPage.SetActive(cardRec);
        PopupManager.toast.SetActive(cardRec);
        
        cameraMainUI.SetActive(cardRec);
        GuideLine.SetActive(cardRec);
        offCloth(charInfo.top);
        offCloth(charInfo.bottom);
        offCloth(charInfo.onepiece);
        offCloth(charInfo.shoes);
        offCloth(charInfo.acc);
        character.SetActive(cardRec);
        arCharacter.SetActive(cardRec);
        fashionCharacter.SetActive(!cardRec);
        effectCamera.SetActive(cardRec);
        for (int i = 1; i < effectCamera.transform.GetChildCount(); i++)
        {
            effectCamera.transform.GetChild(i).gameObject.SetActive(false);
        }
        fashionCamera.SetActive(!cardRec);
        CharacterController.instance.resetTrans();
        instance.resetChar();
        instance.guideButtonUI.gameObject.SetActive(cardRec);

    }
}


