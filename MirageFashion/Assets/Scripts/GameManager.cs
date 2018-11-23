
using UnityEngine;
using Vuforia;
using System;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TouchScript.Gestures;

public class CardInfo
{
    public string name;        //아이템 이름 ex)char_ari 
    public string strName;     //아이템 이름 ex)아리
    public int history;        //카드인식 기록
    public int level;          //0:일반 1:스페셜
    public string type;        //0:캐릭터 1:상의 2:하의 3:원피스 4:신발 5:악세사리
    public string style;       //0:멋져 1:귀여워 2:발랄해 3:아름다워 4:예뻐 5: 아리 6:민 7: 슈엘 8:수하
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

    public UIState uiState;
    public enum UIState
    {
        guide, main, paint, camera, save, fashion       //guide: 처음 소개화면, main: 카드인식화면, paint: 꾸미기화면, camera: 카메라화면, save: 저장화면, fashion: 패션인식(촬영)화면
    }

    public PaintState paintState;
    public enum PaintState
    {
        none, pen, text
    }

    public Error errorState;
    public enum Error
    {
        none, save, move, gallery                      //save: 사진저장에러, move: 경로에러, gallery
    }

    public GameObject cameraMainUI;                     //인식화면 UI 카메라
    public GameObject cameraPaintUI;                    //꾸미기화면, 저장화면 UI 카메라
    public static GameObject canvasBottomButtonUI;      //인식화면 bottom버튼
    public static GameObject canvasTopButtonUI;         //인식화면 top버튼
    public static GameObject charControlBtnUI;          //캐릭터 회전 버튼
    public static GameObject canvasRecObj;              //인식화면 좌측 착용의상 이미지
    public GameObject canvasCaptureUI;                  //카메라촬영 canvas
    public MeshRenderer capturePic;                     //카메라촬영 plane
    public static GameObject canvasCameraUI;            //카메라화면 버튼
    public static GameObject canvasMainUI;

    public GameObject canvasPaintUI;                    //꾸미기화면
    public GameObject canvasSaveUI;                     //저장화면
    public GameObject guideButtonUI;                    //사용방법 페이지

    public GameObject cameraAR;                         //카드인식 카메라(AR Camera)

    public GameObject GuideLine;                        //카드인식중 나타내는 가이드라인

    public GameObject cameraTutorial;                   //앱실행 후 사용방법 화면, 한번만 실행


    public CharacterInfo charInfo;                      //착용한 캐릭터, 의상 정보



    //갤러리
    byte[] imageByte;

    public string myFilename;
    string myFolderLocation;
    string myScreenshotLocation;
    string myDefaultLocation;

    public static Texture2D texture;

    float screenHeight;
    float screenWidth;
   
    public bool isPopup = false;

    public static GameObject[] recognizeObj;            //카드인식화면 의상 아이콘(상의, 하의, 신발, 악세사리)

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

    bool close = false;
    public Text testT;

    public GameObject arCharacter;                      //카드인식 캐릭터
    public GameObject fashionCharacter;                 //패션인식 캐릭터
    public Transform lockerCharacter;                   //보관함 캐릭터

    bool mAutofocusEnabled = true;

    public bool loadGallery = false;                    //카메라모드-> 갤러리 사진 부름
    //bool checkPermission = false;

    public GameObject guideSound;
    public GameObject clickSound;
    public GameObject loadSound;
    public GameObject captureSound;
    public GameObject saveSound;
    public GameObject missionSound;
    public GameObject lockerSound;

    float backKeyDelayTime = 0;

    public bool dressroomLoad = false;

    public bool availableRecognize = true;              //카드인식 가능 여부 (카메라모드에서 인식 불가능)
    public string recogObjName = null;                  //착용한 의상이 있을 경우 인식한 카드 이름 저장
    public string offObjName = null;                    //의상제거 팝업띄울 때 제거할 카드 이름 저장

    public float waitingTime = 0f;                      //카드 인식
    public bool isOnCloth;

    public GameObject clickEffect;

    //패션인식
    public bool isCardRecognition = true;               //true: 카드인식 false: 패션인식
    public GameObject fashionCamera;

    public string selectCloth = "";
    public string selectTopColor = "";
    public string selectBottomColor = "";
    public int onClothCount = 0;                        

    private void Start()
    {
        instance = this;
        errorState = Error.none;
        guideButtonUI.SetActive(true);
        uiState = UIState.main;
        canvasBottomButtonUI = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("panelBottom").gameObject;
        canvasTopButtonUI = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("panelTop").gameObject;
        canvasCameraUI = cameraMainUI.transform.Find("CanvasCameraButtonUI").gameObject;
        canvasRecObj = cameraMainUI.transform.Find("CanvasMainButtonUI").Find("recognizeObj").gameObject;
        //menuLayout = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("panelTop").Find("btnMenu").Find("MenuLayout").gameObject;
        capturePic = canvasCaptureUI.transform.Find("Plane").GetComponent<MeshRenderer>();
        charControlBtnUI = cameraMainUI.transform.Find("CanvasMainButtonUI").transform.Find("CharControlBtn").gameObject;

        charInfo = new CharacterInfo("0", "0", "0", "0", "0", "0");

        //캡처 plane 
        screenWidth = Screen.width;
        screenHeight = (float)Screen.height;
        capturePic.transform.localScale = new Vector3(720 / 10, 0.1f, (720 / screenWidth) * screenHeight / 10);
        capturePic.transform.LookAt(cameraMainUI.transform);
        capturePic.transform.rotation = Quaternion.Euler(new Vector3(90, -180, 0));

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
            GameObject.Find("PopupCamera").transform.Find("CanvasPopup").transform.Find("Guide").gameObject.SetActive(true);
            GameObject.Find("PopupCamera").transform.Find("CanvasPopup").transform.Find("Guide").transform.Find("Text").GetComponent<Text>().text = "캐릭터 카드를 먼저 보여줘!";
        }

        setFont();
        setCard();

        //카드인식 캐릭터
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
                if (LoadAsset.instance.material[i].name.Contains("body") && fashionCharacter.transform.Find(LoadAsset.instance.material[i].name) != null)
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
                if (LoadAsset.instance.material[i].name.Contains("char_ari") && fashionCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)) != null)
                {
                    Debug.Log("name : " + LoadAsset.instance.material[i].name);
                    fashionCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    fashionCharacter.transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
                }
                
                for (int j = 0; j < 3; j++)
                {
                    lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material = LoadAsset.instance.material[i];
                    lockerCharacter.transform.Find("locker_" + j).transform.Find("character").transform.Find(LoadAsset.instance.material[i].name.Remove(0, 5)).transform.Find(LoadAsset.instance.material[i].name).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                }
            }
        }
        for (int i = 0; i < LoadAsset.instance.fashionMaterial.Count; i++)
        {
            if (fashionCharacter.transform.Find(LoadAsset.instance.fashionMaterial[i].name) != null)
            {
                fashionCharacter.transform.Find(LoadAsset.instance.fashionMaterial[i].name).GetComponent<Renderer>().material = LoadAsset.instance.fashionMaterial[i];
                fashionCharacter.transform.Find(LoadAsset.instance.fashionMaterial[i].name).GetComponent<Renderer>().material.shader = Shader.Find(/*"Mobile/Unlit (Supports Lightmap)"*/"Unlit/Texture");
            }
        }
    }
    public void setGuide()
    {
        Transform obj = guideButtonUI.transform.Find("Guide").Find("Scroll View").Find("Viewport").Find("Content");
        for (int i = 1; i < 11; i++)
        {
            obj.Find("use_" + i).GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg["use_" + i];
        }

    }
    public void Update()
    {
        charCamera.transform.position = cameraAR.transform.position;
        charCamera.transform.rotation = cameraAR.transform.rotation;
        if (!charInfo.body.Contains("char"))
        {
            canvasBottomButtonUI.transform.Find("btnSaveChar").transform.gameObject.SetActive(false);
        }

        //안드로이드 뒤로가기
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (uiState == UIState.camera && loadGallery)
                {
                    if (backKeyDelayTime == 0)
                    {
                        AndroidFuntionCall.instance.img.gameObject.SetActive(false);
                        charCamera.transform.Find("Canvas").Find("background").gameObject.SetActive(false);
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
                //안드로이드 뒤로가기 두번눌러 앱종료
                else if (isPopup == false)
                {
                    if (close == false)
                    {
                        if (backKeyDelayTime == 0)
                        {
                            AndroidFuntionCall.instance.onBackPressed();
                            StartCoroutine(WaitClose());
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

        if ((PopupManager.instance.popupState != PopupManager.Popup.none && PopupManager.instance.popupState != PopupManager.Popup.settingHome) || /*(uiState == UIState.camera && loadGallery) ||*/ uiState == UIState.paint || uiState == UIState.save)
        {
            cameraAR.SetActive(false);
        }
        else
            cameraAR.SetActive(true);

        if (dressroomLoad)
        {
            StartCoroutine(appearChar());
        }
        if (availableRecognize && isPopup == false && isCardRecognition && uiState == UIState.main)
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
    IEnumerator WaitClose()
    {
        yield return new WaitForSeconds(0.5f);
        close = true;
        StartCoroutine(this.Close());
    }
    IEnumerator Close()
    {
        yield return new WaitForSeconds(1.0f);
        //PopupManager.toast.SetActive(false);
        close = false;
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
        //캐릭터가 슈엘일 때 치유마법 의상 색 바뀜.
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

    //카드인식화면 초기화면 가이드 버튼
    public void btnGuide()
    {
        isPopup = true;
        GuideLine.SetActive(false);
        guideButtonUI.transform.Find("obj").gameObject.SetActive(false);
        guideButtonUI.transform.Find("Guide").gameObject.SetActive(true);

    }
    public void closeGuide()
    {
        Transform contentPos = guideButtonUI.transform.Find("Guide").Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>();
        isPopup = false;
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
            if (charInfo.top == "0" && charInfo.bottom == "0" && charInfo.onepiece == "0")
                GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("camera").gameObject.SetActive(false);
            else
                GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("camera").gameObject.SetActive(true);
        }
    }
    public void modeBtnDown()
    {
        Color color = new Color(1, 1, 1, 1);
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
    public void modeChangeDrag(GameObject obj)
    {
        Vector3 pos;
        pos = obj.transform.position;

        if (uiState == UIState.main)
        {
            //카메라모드로
            pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;
            obj.transform.position = pos;
            if (pos.x < cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x)
            {
                pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
                obj.transform.position = pos;
            }
            if (pos.x > cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width * 3 / 4, 0, 0)).x)
            {
                Color color = new Color(1, 0.6f, 0.8f,1);
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
            //인식모드로
            CharacterController.instance.gameObject.GetComponent<TransformGesture>().enabled = false;
            if (isCardRecognition)
            {
                //인식모드
                pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;
                obj.transform.position = pos;
                if (pos.x > cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x)
                {
                    pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
                    obj.transform.position = pos;
                }
                if (pos.x < cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 4, 0, 0)).x)
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
            //인식, 카메라모드로
            pos.x = cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;
            obj.transform.position = pos;

            if (pos.x < cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 4, 0, 0)).x)
            {
                Color color = new Color(1, 0.6f, 0.8f);
                canvasSaveUI.transform.Find("RecognitionModeImg").gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            }
            
            else if (pos.x > cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width * 3 / 4, 0, 0)).x)
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
   
   
    public void modeBtnUp(GameObject obj)
    {
        Vector3 pos;
        pos = obj.transform.position;

        if (uiState == UIState.main)
        {
            pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 1280, 0)).x;
            obj.transform.position = pos;
            if (Input.mousePosition.x > Screen.width *3/4)
                goCameraScene();
            canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
        }
        else if (uiState == UIState.camera)
        {
            CharacterController.instance.gameObject.GetComponent<TransformGesture>().enabled = true;
            pos.x = cameraMainUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 1280, 0)).x;
            obj.transform.position = pos;
            if (Input.mousePosition.x < Screen.width / 4)
            {
                if(isCardRecognition)
                    goTrackingScene();
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
            tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_BACK);
            testT.text = character.transform.localRotation.eulerAngles.ToString();
        }
        else
        {
            tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_FRONT);
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
            canvasBottomButtonUI.transform.Find("btnSaveChar").gameObject.SetActive(true);
            canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
            canvasBottomButtonUI.transform.Find("fashion").gameObject.SetActive(true);
            charControlBtnUI.SetActive(true);
            loadGallery = false;
        }
        else if (uiState == UIState.save)
        {
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
            if (!isCardRecognition)
            {
                //패션인식일 경우
                showRecognitionCanvas(isCardRecognition);
                FashionRecognition.canvasFashion.transform.Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                FashionRecognition.canvasFashion.transform.Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                FashionRecognition.canvasFashion.transform.Find("top").gameObject.SetActive(false);
                FashionRecognition.canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
                FashionRecognition.canvasFashion.transform.Find("camera").gameObject.SetActive(false);
            }
            canvasBottomButtonUI.transform.Find("btnSaveChar").gameObject.SetActive(false);
            canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
            canvasBottomButtonUI.transform.Find("fashion").gameObject.SetActive(true);
            charControlBtnUI.SetActive(false);
            canvasTopButtonUI.SetActive(true);
            loadGallery = false;
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
        onClothCount = 0;
        if (isCardRecognition)
        {
            //카드인식
            GuideLine.SetActive(true);
            uiState = UIState.main;
        }
        else
        {
            //패션인식
            GuideLine.SetActive(false);
            uiState = UIState.fashion;
        }


        if (PlayerPrefs.GetString("mission") != DateTime.Now.ToString("yyyyMMdd") && charInfo.body == "0" && isCardRecognition)
        {
            PopupManager.instance.openToday();
        }
        canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(false);
        AndroidFuntionCall.instance.img.gameObject.SetActive(false);
        canvasTopButtonUI.transform.Find("btnCamSwitch").gameObject.SetActive(true);
        PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("SettingPage").Find("btnSetting").gameObject.SetActive(true);

        cameraAR.transform.position = new Vector3(0, 0, 0);
        cameraAR.transform.Find("Camera").transform.position = new Vector3(0, 0, 0);
        cameraAR.transform.Find("Camera").Find("BackgroundPlane").transform.position = new Vector3(0, 0, 400);

        tmp.SelectCamera(CameraDevice.CameraDirection.CAMERA_BACK);
        cameraAR.SetActive(true);
        SwitchAutofocus(true);
        
        canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
        charCamera.transform.Find("Canvas").Find("background").gameObject.SetActive(false);
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
            loadGallery = false;
        }
        else if (uiState == UIState.paint)
        {
            
            paintState = PaintState.none;
            removePaint();
            cameraMainUI.SetActive(true);
            PaintManager.textLayout.SetActive(false);
            PaintManager.penLayout.SetActive(false);
            PaintManager.mainLayout.SetActive(true);
            cameraPaintUI.SetActive(false);
            canvasPaintUI.SetActive(false);
            canvasCaptureUI.SetActive(false);
            charControlBtnUI.SetActive(true);
            canvasTopButtonUI.SetActive(true);

            canvasCameraUI.SetActive(true);
            uiState = UIState.camera;
            instantiateSound(clickSound);

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
        else if (uiState == UIState.fashion)
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
        PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("SettingPage").Find("btnSetting").gameObject.SetActive(true);
        
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
     
        canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(false);
        PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("SettingPage").Find("btnSetting").gameObject.SetActive(false);
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

            uiState = UIState.paint;
            canvasBottomButtonUI.SetActive(false);
            paintState = PaintState.none;
            cameraPaintUI.SetActive(true);
            canvasPaintUI.SetActive(true);
        }
        //사진저장
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
            errorState = Error.none;
            File.WriteAllBytes(myDefaultLocation, imageByte);
        }
        catch (Exception ex)
        {
            errorState = Error.save;
            Debug.LogError(ex.Message, this);
            PopupManager.instance.showPopup("사진을 저장 할 수 없습니다. \n 잠시 후 시도해 주세요.");
            paintState = PaintState.none;
            PaintManager.instance.paintLayout.SetActive(true);
        }
        finally
        {
            imageByte = null;
        }

        // 임시 디렉토리에서 DCIM 폴더로 이동
        try
        {
            errorState = Error.none;
            
            instantiateSound(saveSound);

            File.Move(myDefaultLocation, myScreenshotLocation);
            uiState = UIState.save;
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidFuntionCall.instance.mediaScan();
#endif
            PopupManager.instance.showPraiseMsgToast();
            canvasSaveUI.SetActive(true);
        }
        catch (Exception ex)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            errorState = Error.move;
            PopupManager.instance.showPopup("사진을 저장 할 수 없습니다. \n 잠시 후 시도해 주세요.");
            paintState = PaintState.none;
            PaintManager.instance.paintLayout.SetActive(true);
#endif
            Debug.LogError(ex.Message, this);
        }
        //<<<<<<< HEAD

        //=======

#if UNITY_IOS
//			callShareForiOS();

		picker.SaveToGalley("gallery", myDefaultLocation);

			instantiateSound(saveSound);
			canvasSaveUI.SetActive(true);
			PopupManager.instance.showPraiseMsgToast();
			uiState = UIState.save;

#endif
        //>>>>>>> 231c825e4c65ba9c45496cd9d933e29851f1748c
    }


    public void resetChar()
    {
        charInfo = new CharacterInfo("0", "0", "0", "0", "0", "0");
        arCharacter.gameObject.SetActive(false);
    }
    /// <summary>
    /// 의상 착용했을 때 왼쪽 의상 아이콘 버튼
    /// 원피스는 상의, 하의 두개 표시
    /// </summary>
    /// <param name="obj"></param>
    public void offCloth(GameObject obj)
    {

        instantiateSound(clickSound);
        if (uiState == UIState.main)
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
        else if (uiState == UIState.fashion)
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
   
    public void offClothPopup(string obj)
    {
        if (uiState == UIState.main)
        {
            availableRecognize = false;
            isOnCloth = false;
            canvasRecObj.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];
            if (!obj.Contains("char"))
            {
                canvasRecObj.transform.Find("Text").GetComponent<Text>().text = "해당의상을 지우겠습니까?";
            }
            canvasRecObj.transform.Find("btnConfirm").Find("Text").GetComponent<Text>().text = "지움";
            offObjName = obj;
            canvasRecObj.SetActive(true);
            GuideLine.SetActive(false);
        }
        else if (uiState == UIState.fashion)
        {
            FashionRecognition.canvasFashion.transform.Find("popup").gameObject.SetActive(true);
            if (obj.Contains("top"))
                FashionRecognition.canvasFashion.transform.Find("popup").Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[obj + "_" + selectTopColor + "_img"];
            else if (obj.Contains("bottom"))
                FashionRecognition.canvasFashion.transform.Find("popup").Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[obj + "_" + selectBottomColor + "_img"];
            else if (obj.Contains("onepiece"))
                FashionRecognition.canvasFashion.transform.Find("popup").Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[obj + "_" + selectTopColor + "_img"];
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
    /// 의상 착용, 해제 팝업 버튼
    /// </summary>
    /// <param name="btn"></param>
    public void btnRecogObj(bool btn)
    {
        instantiateSound(clickSound);
        if (uiState == UIState.main)
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
                Debug.Log("body : " + charInfo.body);
                if (recogObjName.Contains("char") && !charInfo.body.Contains("char"))
                {
                    guideButtonUI.SetActive(true);
                    closeGuide();
                }
                recogObjName = "";
                offObjName = "";
            }
            canvasRecObj.SetActive(false);
            availableRecognize = true;
            GuideLine.SetActive(true);
        }
        else if (uiState == UIState.fashion)
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
    /// 캐릭터의상 해제
    /// </summary>
    /// <param name="cloth"></param>
    public void offCloth(string cloth)
    {
        if (uiState == UIState.main || uiState == UIState.camera)
        {
            if (cloth.Contains("top"))
            {
                charInfo.top = "0";
                offCharObj("top");
                recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                arCharacter.transform.Find("basic_top").transform.gameObject.SetActive(true);
            }
            else if (cloth.Contains("bottom"))
            {
                charInfo.bottom = "0";
                offCharObj("bottom");
                recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                arCharacter.transform.Find("basic_bottom").transform.gameObject.SetActive(true);
            }
            else if (cloth.Contains("onepiece"))
            {
                charInfo.onepiece = "0";
                offCharObj("onepiece");
                arCharacter.transform.Find("basic_top").transform.gameObject.SetActive(true);
                arCharacter.transform.Find("basic_bottom").transform.gameObject.SetActive(true);
                recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
                recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
            }
            else if (cloth.Contains("shoes"))
            {
                charInfo.shoes = "0";
                offCharObj("shoes");
                recognizeObj[2].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_shoes");
            }
            else if (cloth.Contains("acc"))
            {
                charInfo.acc = "0";
                offCharObj("acc");
                recognizeObj[3].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_acc");
            }
            checkAnimation();
            offObjName = null;

        }
        else if (uiState == UIState.fashion)
        {
            if (cloth.Contains("top"))
            {
                if (charInfo.top != "0")
                {
                    charInfo.top = "0";
                    offCharObj("top");
                    GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                    onCloth("top", "basic_top");
                }

            }
            else if (cloth.Contains("bottom"))
            {
                if (charInfo.bottom != "0")
                {
                    charInfo.bottom = "0";
                    offCharObj("bottom");
                    GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                    onCloth("bottom", "basic_bottom");
                }

            }
            else if (cloth.Contains("onepiece"))
            {
                charInfo.onepiece = "0";
                offCharObj("onepiece");
                GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                onCloth("top", "basic_top");
                GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                onCloth("bottom", "basic_bottom");
            }

            offObjName = null;
        }

    }
    /// <summary>
    /// 캐릭터 의상 착용
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="cardName"></param>
    public void onCloth(string obj, string cardName)
    {
        if (cardName.Contains(obj))
        {
            if (uiState == UIState.main || uiState == UIState.camera)
                arCharacter.transform.Find(cardName).gameObject.SetActive(true);
            else if (uiState == UIState.fashion)
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
    /// <summary>
    /// 패션인식 버튼
    /// </summary>
    public void fasionRecognition()
    {
        isCardRecognition = false;

        if (charInfo.top.Contains("top") || charInfo.bottom.Contains("bottom") || charInfo.onepiece.Contains("onepiece") || charInfo.shoes.Contains("shoes") || charInfo.acc.Contains("acc") || charInfo.body.Contains("char"))
        {
            PopupManager.instance.showPopup("패션인식화면으로 넘어가시면 기존 착용된 의상이 사라집니다. \n 계속 진행하시겠습니까?");
        }
        else
        {
            showRecognitionCanvas(isCardRecognition);
        }
    }
    /// <summary>
    /// 패션인식, 카드인식 화면으로 이동
    /// true: 카드인식, false: 패션인식
    /// </summary>
    /// <param name="cardRec"></param>
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
        {
            uiState = UIState.fashion;
            PopupManager.toast.SetActive(false);
        }
        PopupManager.settingPage.SetActive(cardRec);
        fashionCamera.SetActive(!cardRec);

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
        CharacterController.instance.resetTrans();
        resetChar();
        guideButtonUI.gameObject.SetActive(cardRec);
        //SwitchAutofocus(true);
        canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
        canvasBottomButtonUI.transform.Find("btnSaveChar").gameObject.SetActive(!cardRec);
    }
}


