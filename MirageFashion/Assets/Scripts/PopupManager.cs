using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    public GameObject cameraPopup;
    //public static GameObject dialogPopup;
    public static GameObject settingPage;
    public static GameObject praiseMsgPopup;
    public static GameObject toast;
    public static GameObject missionPage;
    public static GameObject userGuide;
    public static int guideNum = 0;

    public Animator closeSettingAnimator;

    public static GameObject settingUI;
    public static GameObject mirageIntroUI;
    public static GameObject cardInfoUI;
    public static GameObject guideUI;
    public static GameObject lockerUI;

    public GameObject tutorialCamra;

    public Text popupTxt;
    public Text praisePopupTxt;
    public Popup popupState;
    Text toastTxt;

    bool isPraisePopup = false;

    int lockerNum = 0;

    float time = 0f;

    public bool closeSettingPage = false;
    public bool openSettingPage = false;

    public Transform openSettingPos;
    public Transform closeSettingPos;
    public enum Popup
    {
        home, settingHome, settingIntro, settingDressRoom, editDressRoom, settingCardInfo, guide
    }
    public enum Msg
    {
        val1, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19, val20
    }

    Dictionary<Msg, string> praiseMsg = new Dictionary<Msg, string>()
    {
        {Msg.val1, "패션 좀 아는 구나~♡" },
        {Msg.val2, "패션센스가 정말 남달라! 최고다!" },
        {Msg.val3, "여신이네! 여신~앞으로도 기대할 게" },
        {Msg.val4, "멋쟁이! 멋쟁이!" },
        {Msg.val5, "패션센스 점수 100점! 아니 만점!" },
        {Msg.val6, "짜잔~아이돌 패션 따라잡기! 성공~!" },
        {Msg.val7, "두근두근~♡ 이렇게 예뻐도 되는 거야? " },
        {Msg.val8, "너무 잘 어울려~아이돌 같아!" },
        {Msg.val9, "친구들에게 보여줘~♡ 부러워할 걸~!" },
        {Msg.val10, "친구들의 반응이 궁금하지 않아? 친구에게 보여주자!" },
        {Msg.val11, "오늘의 패션 주인공은 바로 나야 나! " },
        {Msg.val12, "보면 볼수록 끌리는 스타일인 것 같아!" },
        {Msg.val13, "오~~ 괜찮다! 괜찮다!" },
        {Msg.val14, "예뻐~예뻐~너무 예뻐!!" },
        {Msg.val15, "오늘 옷이 어떤지 친구에게 물어보자!" },
        {Msg.val16, "아~ 정말 배우고 싶다. 패션센스!" },
        {Msg.val17, "막 입어도 예뻐! 이건 반칙이야!" },
        {Msg.val18, "대박~! 너무 너무 잘 어울려~" },
        {Msg.val19, "짝짝~오늘 예쁘다는 말 100번 들을 것 같아!" },
        {Msg.val20, "반짝반짝 눈이 부실 정도로 예뻐!" },

    };
    Dictionary<Msg, string> missionMsg = new Dictionary<Msg, string>()
    {
        {Msg.val1, "감동적인 노래를 부르는 가수로 메이크업 체인지! 무대에서 더 돋보이는 스타일링 기대할게!"},
        {Msg.val2, "정의롭고 용감한 경찰이 되고 싶어! 나도 멋진 제복이 잘 어울리게 스타일링 부탁해!" },
        {Msg.val3, "오늘은 아기를 돌보는 일이야. 따뜻하고 포근한 아이템의 베이비시터로 메이크업 체인지!" },
        {Msg.val4, "선생님으로 메이크업 체인지! 학생들이 존경하는 선생님에게 어울리는 멋진 스타일링 부탁해! " },
        {Msg.val5, "동물을 돌보는 따뜻한 마음의 사육사가 되고 싶어! 어떤 옷을 입으면 좋을까?" },
        {Msg.val6, "하늘에서의 안전과 서비스는 내게 맡겨! 당당하고 멋진 스튜어디스로 스타일링을 부탁해~" },
        {Msg.val7, "아이돌 그룹으로 메이크업 체인지! 너만의 감각으로 우리들의 첫 무대의상을 스타일링 해줘!" },
        {Msg.val8, "뮤지컬배우로 메이크업 체인지! ‘캣츠’의 귀여운 고양이 의상으로 스타일링 부탁해 야옹~" },
        {Msg.val9, "나는 어떤 미스터리도 풀어내는 명탐정! 명석하고 활동적인 탐정 의상 으로 스타일링 부탁해" },
        {Msg.val10,"맛있는 빵을 만들어 줄게! 파티시엘 의상으로 스타일링을 부탁해" },
        {Msg.val11, "거울아 거울아 세상에서 가장 아름다운 프린세스 의상으로 스타일링 부탁해" },
        {Msg.val12, "나랑 같이 운동하러 나가자~ 운동할 때는 어떤 옷을 입어야 되지? 멋진 스포티룩으로 코디 부탁해~" },
        {Msg.val13, "두근두근 첫 출근날! 주목받는 신입사원이 될 수 있는 의상으로 스타일링 부탁해" },
        {Msg.val14, "어둠을 물리치기 위해선 ‘빛의 마법’이 필요해! 우리 같이 힘을 모아 빛의 마법 드레스로 변신하자!" },
        {Msg.val15, "바쁘다! 바빠! 오늘은 회사에서 중요한 발표가 있어! 단정한 의상으로 스타일링을 부탁해! " }
    };

    // Use this for initialization
    void Start()
    {

        instance = this;
        popupState = Popup.home;
        settingPage = cameraPopup.transform.Find("CanvasPopup").transform.Find("SettingPage").transform.gameObject;
        praiseMsgPopup = cameraPopup.transform.Find("CanvasPopup").transform.Find("PraiseMsg").transform.gameObject;
        toast = cameraPopup.transform.Find("CanvasPopup").transform.Find("Toast").transform.gameObject;
        toastTxt = toast.transform.Find("Text").transform.GetComponent<Text>();
        missionPage = cameraPopup.transform.Find("CanvasPopup").transform.Find("Mission").transform.gameObject;
        userGuide = cameraPopup.transform.Find("CanvasPopup").transform.Find("Guide").transform.gameObject;

        settingUI = settingPage.transform.Find("1").transform.gameObject;
        mirageIntroUI = settingPage.transform.Find("MirageIntro").transform.gameObject;
        cardInfoUI = settingPage.transform.Find("CardInfo").transform.gameObject;
        guideUI = settingPage.transform.Find("Guide").transform.gameObject;
        lockerUI = settingPage.transform.Find("DressRoom").transform.gameObject;
        //미션
        if (PlayerPrefs.GetString("mission") != System.DateTime.Now.ToString("yyyyMMdd"))
        {
            openToday();
        }
        else
        {
        }

        settingUI.transform.Find("version").transform.Find("versionTxt").GetComponent<Text>().text = LoadAsset.instance.clientVersion;
        Transform obj = cameraPopup.transform.Find("CanvasPopup").transform.Find("SettingPage").Find("1").Find("version");
#if UNITY_ANDROID || UNITY_EDITOR
        if (LoadAsset.instance.clientVersion == LoadAsset.instance.storeVersion)
        {
            obj.Find("Text1").GetComponent<Text>().text = "/ 최신버전입니다.";
            obj.GetComponent<Button>().enabled = false;
            obj.Find("versionTxt").transform.localPosition = new Vector3(72, obj.Find("versionTxt").transform.localPosition.y, obj.Find("versionTxt").transform.localPosition.z);
        }
        else
        {

            obj.Find("Text1").GetComponent<Text>().text = "/최신버전으로 업데이트 해주세요.";
            obj.GetComponent<Button>().enabled = true;
            obj.Find("versionTxt").transform.localPosition = new Vector3(-24, obj.Find("versionTxt").transform.localPosition.y, obj.Find("versionTxt").transform.localPosition.z);
        }
#endif
        if (closeSettingPage)
        {
            settingPage.transform.position = Vector3.MoveTowards(settingPage.transform.position, cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(-(Screen.width), 1280)), 2f);
            GameManager.instance.isPopup = false;

        }
        //DressRoomManager.instance.dicLockerChar.Remove("char1");
        //GameManager.instance.saveLocker();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPraisePopup)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray;
                RaycastHit hitinfo;

                ray = cameraPopup.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hitinfo) == false)
                {
                    isPraisePopup = false;
                    praiseMsgPopup.SetActive(false);
                    //cameraPopup.SetActive(false);
                }
            }
        }

        //if (GameManager.instance.uiState == GameManager.UIState.main && GameManager.instance.isPopup == false)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Ray ray;
        //        RaycastHit hitinfo;

        //        ray = cameraPopup.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        //        if (Physics.Raycast(ray, out hitinfo) == false)
        //        {
        //            //toast.SetActive(false);
        //            closeGuide();
        //        }
        //    }
        //}
        if (closeSettingPage)
        {
            float speed = 8f * Time.smoothDeltaTime;
            settingPage.transform.position = Vector3.MoveTowards(settingPage.transform.position, closeSettingPos.position, speed);

            if (settingPage.transform.position == closeSettingPos.transform.position)
            {
                closeSettingPage = false;
                GameManager.instance.isPopup = false;

                settingPage.transform.Find("btnSetting").transform.position = new Vector3(cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 0)).x, settingPage.transform.Find("btnSetting").transform.position.y, settingPage.transform.Find("btnSetting").transform.position.z);
                settingPage.transform.Find("btnSetting").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_setting");
                settingPage.transform.Find("1").Find("Button").gameObject.SetActive(false);
                popupState = Popup.home;
                if (GameManager.instance.uiState == GameManager.UIState.main && popupState == Popup.home)
                    GameManager.instance.GuideLine.SetActive(true);
                else
                    GameManager.instance.GuideLine.SetActive(false);

                GameManager.instance.SwitchAutofocus(true);
                if (GameManager.instance.uiState == GameManager.UIState.main)
                    GameManager.instance.availableRecognize = true;
            }
            if (!GameManager.instance.charInfo.body.Contains("char"))
            {
                GameManager.instance.guideButtonUI.SetActive(true);
            }
        }
        else if (openSettingPage)
        {
            float speed = 8f * Time.smoothDeltaTime;
            settingPage.transform.position = Vector3.MoveTowards(settingPage.transform.position, openSettingPos.position, speed);

            if (settingPage.transform.position == openSettingPos.position)
            {
                settingPage.transform.Find("btnSetting").transform.position = new Vector3(cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(520 * Screen.width / 720, 0, 0)).x, settingPage.transform.Find("btnSetting").transform.position.y, settingPage.transform.Find("btnSetting").transform.position.z);
                settingPage.transform.Find("btnSetting").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_setting_close");
                openSettingPage = false;
                GameManager.instance.isPopup = true;
                popupState = Popup.settingHome;
                settingPage.transform.Find("1").Find("Button").gameObject.SetActive(true);
                GameManager.instance.GuideLine.SetActive(false);
                if (GameManager.instance.uiState == GameManager.UIState.main)
                    GameManager.instance.availableRecognize = false;
            }
            GameManager.instance.guideButtonUI.SetActive(false);
        }
        if (missionPage.transform.Find("Toggle").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetString("mission", System.DateTime.Now.ToString("yyyyMMdd"));
            missionPage.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
            missionPage.SetActive(false);
            GameManager.instance.isPopup = false;
            GameManager.instance.GuideLine.SetActive(true);
        }
    }
    
    public void showToast(string msg)
    {
        ////cameraPopup.SetActive(true);
        //dialogPopup.SetActive(false);
        //settingPage.SetActive(false);
        toast.SetActive(true);
        toast.transform.Find("Image").gameObject.SetActive(true);
        toastTxt.text = msg;
        StartCoroutine(this.closeToast());
    }
    IEnumerator closeToast()
    {
        yield return new WaitForSeconds(2.0f);
        //cameraPopup.SetActive(false);
        toast.SetActive(false);
    }


    public void openToday()
    {
        //cameraPopup.SetActive(true);
        GameManager.instance.isPopup = true;
        missionPage.SetActive(true);
        missionPage.transform.Find("Image").transform.Find("Text2").GetComponent<Text>().text = missionMsg[(Msg)Random.Range(0, 15)];
    }



    public void showPraiseMsgPopup()
    {
        isPraisePopup = true;
        //dialogPopup.SetActive(false);
        toast.SetActive(false);
        //cameraPopup.SetActive(true);
        praiseMsgPopup.SetActive(true);
        praisePopupTxt.text = praiseMsg[(Msg)Random.Range(0, 20)];
        //StartCoroutine(this.closePraiseMsgPopup());
    }

    //사용가이드(첫실행)
    public void showGuide()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        if (PlayerPrefs.GetString("guide") != "off")
        {
            userGuide.SetActive(true);
            if (guideNum < 3)
            {
                guideNum++;
            }

            if (guideNum == 1)
            {
                userGuide.transform.Find("Text").GetComponent<Text>().text = "다음은 의상 카드! 한장씩! 한장씩!";
                userGuide.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/guide_img2");
                userGuide.transform.Find("btn").gameObject.SetActive(true);
            }
            else if (guideNum == 2)
            {
                userGuide.transform.Find("btn").transform.Find("Button").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/guide_close");
                userGuide.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/guide_img3");
                userGuide.transform.Find("Text").GetComponent<Text>().text = "스타일링이 끝나면 함께 사진도 찍을 수 있어~♡";
            }
            else if (guideNum == 3)
            {
                PlayerPrefs.SetString("guide", "off");
                userGuide.SetActive(false);
            }

        }
    }
    /// <summary>
    /// 설정페이지 누름효과
    /// </summary>
    /// <param name="obj"></param>
    public void pressedBtn(GameObject obj)
    {
        //obj.transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Bold;
        obj.transform.Find("Text").GetComponent<Text>().fontSize = 27;
        StartCoroutine(orignBtn(obj));
    }
    IEnumerator orignBtn(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        obj.transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        obj.transform.Find("Text").GetComponent<Text>().fontSize = 25;

    }
    public void closeGuide()
    {
        userGuide.SetActive(false);
    }
    //setting
    public void settingPageDown()
    {
        if (!GameManager.canvasRecObj.active)
            GameManager.instance.instantiateSound(GameManager.instance.clickSound);
    }
    public void settingPageDrag(GameObject obj)
    {
        if (!GameManager.canvasRecObj.active)
        {
            Vector3 pos;
            pos = obj.transform.position;
            pos.x = cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x;

            Vector3 pagePos;
            pagePos = settingPage.transform.position;
            pagePos.x = cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)).x;
            if (Input.mousePosition.x < Screen.width / 18 * 12 + obj.GetComponent<RectTransform>().sizeDelta.x / 2)
            {
                settingPage.transform.position = pagePos;
                obj.transform.position = pos;
            }
            if (GameManager.instance.isPopup == false)
            {
                settingUI.SetActive(true);
                mirageIntroUI.SetActive(false);
                cardInfoUI.SetActive(false);
                lockerUI.SetActive(false);
            }
        }
    }
    public void settingPageDrop(GameObject obj)
    {
        if (!GameManager.canvasRecObj.active)
        {
            if (GameManager.instance.isPopup)
            {
                closeSettingPage = true;
            }
            else
            {
                openSettingPage = true;
            }
        }



    }
    public void closeSetting()
    {
        closeSettingPage = true;
        //settingPage.GetComponent<Animator>().enabled = true;
        //settingPage.GetComponent<Animator>().SetBool("closeSetting", true);
    }
    public void goSettingHome()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        settingUI.SetActive(true);
        mirageIntroUI.SetActive(false);
        tutorialCamra.SetActive(false);
        cardInfoUI.SetActive(false);
        guideUI.SetActive(false);
        lockerUI.SetActive(false);
        if (popupState == Popup.settingCardInfo)
        {
            CardInfoManager.instance.showObj(0);
            CardInfoManager.instance.cardInfoPage();
        }

        else if (popupState == Popup.guide)
        {
            Transform contentPos = guideUI.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>();
            contentPos.localPosition = new Vector3(contentPos.localPosition.x, 0, contentPos.localPosition.z);
        }
        popupState = Popup.settingHome;
        GameManager.instance.SwitchAutofocus(true);
        settingPage.transform.position = openSettingPos.position;

        settingUI.transform.Find("btnIntroduce").Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        settingUI.transform.Find("btnIntroduce").Find("Text").GetComponent<Text>().fontSize = 25;
        settingUI.transform.Find("btnDressRoom").Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        settingUI.transform.Find("btnDressRoom").Find("Text").GetComponent<Text>().fontSize = 25;
        settingUI.transform.Find("btnCardInfo").Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        settingUI.transform.Find("btnCardInfo").Find("Text").GetComponent<Text>().fontSize = 25;
        settingUI.transform.Find("btnGuide").Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        settingUI.transform.Find("btnGuide").Find("Text").GetComponent<Text>().fontSize = 25;

        settingUI.transform.Find("btnShop").Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        settingUI.transform.Find("btnShop").Find("Text").GetComponent<Text>().fontSize = 25;


    }
    public void mirageIntro()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        settingPage.transform.position = new Vector3(cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, settingPage.transform.position.y, settingPage.transform.position.z);
        Vector3 pagePos;
        pagePos = settingPage.transform.position;
        pagePos.x = cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(0, 1280)).x;
        showIntro();
        settingUI.SetActive(false);
        mirageIntroUI.SetActive(true);
        popupState = Popup.settingIntro;
    }
    public void showIntro()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        tutorialCamra.SetActive(true);
        tutorialCamra.GetComponent<Camera>().rect = new Rect(0, 0.1f, 1, 0.8f);
        tutorialCamra.transform.Find("Canvas").transform.Find("Image").transform.position = new Vector3(0, -1, 0);
        Debug.Log(tutorialCamra.transform.Find("Canvas").transform.Find("Image").transform.position);
        tutorialCamra.transform.Find("Canvas").transform.Find("background").gameObject.SetActive(false);
        mirageIntroUI.transform.Find("background").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guideBg2"];
        GuideManager.cardNum = 0;
    }
    public void locker()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        settingPage.transform.position = new Vector3(cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, settingPage.transform.position.y, settingPage.transform.position.z);
        settingUI.SetActive(false);
        lockerUI.SetActive(true);
        if (GameManager.instance.isCardRecognition)
            lockerUI.transform.Find("btnEdit").gameObject.SetActive(true);
        else
            lockerUI.transform.Find("btnEdit").gameObject.SetActive(false);

        popupState = Popup.settingDressRoom;
    }
    public void cardInfo()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        settingPage.transform.position = new Vector3(cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, settingPage.transform.position.y, settingPage.transform.position.z);
        settingUI.SetActive(false);
        cardInfoUI.SetActive(true);
        setCard();
        CardInfoManager.instance.showObj(0);
        CardInfoManager.instance.cardInfoPage();
        popupState = Popup.settingCardInfo;
    }
    public void guide()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        settingPage.transform.position = new Vector3(cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, settingPage.transform.position.y, settingPage.transform.position.z);
        settingUI.SetActive(false);
        guideUI.SetActive(true);
        popupState = Popup.guide;

        Transform obj2 = guideUI.transform.Find("Scroll View").Find("Viewport").Find("Content");
        for (int i = 1; i < 11; i++)
        {
            obj2.Find("use_" + i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["use_" + i];
        }
        for (int i = 1; i < 11; i++)
        {
            obj2.Find("Text_" + i).GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
        }
        obj2.Find("Text").GetComponent<Text>().font = LoadAsset.instance.font["nanum"];
    }
    public void shopPopup()
    {
        string url = "http://www.pororomall.com/product/detail.html?product_no=2249&cate_no=213&display_group=1";
        Application.OpenURL(url);
    }
    public void showPopup(string text)
    {
        cameraPopup.transform.Find("CanvasPopup").Find("popup").gameObject.SetActive(true);
        cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Text").GetComponent<Text>().text = text;
        if (GameManager.instance.isCardRecognition == false && GameManager.instance.uiState == GameManager.UIState.fashion)
        {
            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button").gameObject.SetActive(true);
            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button").localPosition = new Vector3(0, cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button2").localPosition.y);
            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button2").gameObject.SetActive(false);
        }
        else
        {

            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button").gameObject.SetActive(true);
            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button").localPosition = new Vector3(-130, cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button2").localPosition.y);
            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button2").gameObject.SetActive(true);
            cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button2").localPosition = new Vector3(130, cameraPopup.transform.Find("CanvasPopup").Find("popup").Find("popup").Find("Button2").localPosition.y);
        }
    }
    public void popupBtn(bool btn)
    {
        string url = "http://www.pororomall.com/product/detail.html?product_no=2249&cate_no=213&display_group=1";
        if (btn)
        {
            if (GameManager.instance.uiState == GameManager.UIState.main && GameManager.instance.isCardRecognition == false && popupState == Popup.home)
            {
                //카드인식에서 패션인식
                GameManager.instance.showRecognitionCanvas(GameManager.instance.isCardRecognition);
            }
            else if (GameManager.instance.uiState == GameManager.UIState.fashion && GameManager.instance.isCardRecognition)
            {
                //패션인식에서 카드인식
                GameObject.Find("FashionCamera").transform.Find("CanvasFashion").Find("camera").gameObject.SetActive(false);
                GameManager.instance.showRecognitionCanvas(GameManager.instance.isCardRecognition);
            }
            else if (GameManager.instance.isCardRecognition == false && popupState != Popup.home)
            {
                //패션인식 중 보관함 불러오기
                GameManager.instance.isCardRecognition = true;
                GameManager.instance.arCharacter.SetActive(true);
                DressRoomManager.instance.loadLocker();
                FashionRecognition.canvasFashion.transform.Find("top").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                FashionRecognition.canvasFashion.transform.Find("bottom").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                FashionRecognition.canvasFashion.transform.Find("top").gameObject.SetActive(false);
                FashionRecognition.canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
                FashionRecognition.canvasFashion.transform.Find("camera").gameObject.SetActive(false);
                GameManager.instance.fashionCharacter.SetActive(false);
                GameManager.instance.goTrackingScene();
                GameManager.instance.uiState = GameManager.UIState.main;
                GameManager.instance.onClothCount = 0;
                GameManager.canvasBottomButtonUI.transform.Find("btnSaveChar").transform.gameObject.SetActive(true);
                GameManager.canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
                GameManager.canvasTopButtonUI.transform.Find("btnCamSwitch").gameObject.SetActive(true);
                GameManager.canvasTopButtonUI.transform.Find("btnAlbum").gameObject.SetActive(false);

            }
            else if (GameManager.instance.isCardRecognition == false && GameManager.instance.uiState == GameManager.UIState.fashion)
            {

            }
            else if (popupState == Popup.settingDressRoom || popupState == Popup.editDressRoom)
            {

                Debug.Log("iscardRec : " + GameManager.instance.isCardRecognition + ", popup : " + popupState);
            }
            else if (popupState == Popup.guide)
            {
                Application.OpenURL(url);
            }
            else
            {
            }
        }
        else
        {
            if ((GameManager.instance.uiState == GameManager.UIState.main && GameManager.instance.isCardRecognition == false) || (GameManager.instance.uiState == GameManager.UIState.fashion && GameManager.instance.isCardRecognition))
            {
                GameManager.instance.isCardRecognition = !GameManager.instance.isCardRecognition;
            }

        }
        cameraPopup.transform.Find("CanvasPopup").Find("popup").gameObject.SetActive(false);
    }
    public void btnUpdate()
    {
        Application.OpenURL("market://details?id=com.feelingki.fh");
    }
    public void closeObj(GameObject obj)
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        obj.SetActive(false);
        GameManager.instance.isPopup = false;
    }
    public void setCard()
    {
        GameManager.instance.dicCard["0_0"].history = PlayerPrefs.GetInt("char_ari");
        GameManager.instance.dicCard["0_1"].history = PlayerPrefs.GetInt("char_min");
        GameManager.instance.dicCard["0_2"].history = PlayerPrefs.GetInt("char_shuel");
        GameManager.instance.dicCard["0_3"].history = PlayerPrefs.GetInt("char_suha");

        GameManager.instance.dicCard["1_0"].history = PlayerPrefs.GetInt("ariidol_top");
        GameManager.instance.dicCard["1_1"].history = PlayerPrefs.GetInt("babysitter_top");
        GameManager.instance.dicCard["1_2"].history = PlayerPrefs.GetInt("babysitter2_top");
        GameManager.instance.dicCard["1_3"].history = PlayerPrefs.GetInt("fitnesstrainer_top");
        GameManager.instance.dicCard["1_4"].history = PlayerPrefs.GetInt("fitnesstrainer2_top");
        GameManager.instance.dicCard["1_5"].history = PlayerPrefs.GetInt("police_top");
        GameManager.instance.dicCard["1_6"].history = PlayerPrefs.GetInt("stewardess_top");
        GameManager.instance.dicCard["1_7"].history = PlayerPrefs.GetInt("zookeeper_top");
        GameManager.instance.dicCard["1_8"].history = PlayerPrefs.GetInt("codi_top");
        GameManager.instance.dicCard["1_9"].history = PlayerPrefs.GetInt("officeworker_top");
        GameManager.instance.dicCard["1_10"].history = PlayerPrefs.GetInt("officeworker2_top");
        GameManager.instance.dicCard["1_11"].history = PlayerPrefs.GetInt("snoop_top");
        GameManager.instance.dicCard["1_12"].history = PlayerPrefs.GetInt("snoop2_top");
        GameManager.instance.dicCard["1_13"].history = PlayerPrefs.GetInt("teacher_top");
        GameManager.instance.dicCard["1_14"].history = PlayerPrefs.GetInt("teacher2_top");
        GameManager.instance.dicCard["1_15"].history = PlayerPrefs.GetInt("vet_top");

        GameManager.instance.dicCard["2_0"].history = PlayerPrefs.GetInt("ariidol_bottom");
        GameManager.instance.dicCard["2_1"].history = PlayerPrefs.GetInt("babysitter_bottom");
        GameManager.instance.dicCard["2_2"].history = PlayerPrefs.GetInt("babysitter2_bottom");
        GameManager.instance.dicCard["2_3"].history = PlayerPrefs.GetInt("fitnesstrainer_bottom");
        GameManager.instance.dicCard["2_4"].history = PlayerPrefs.GetInt("fitnesstrainer2_bottom");
        GameManager.instance.dicCard["2_5"].history = PlayerPrefs.GetInt("police_bottom");
        GameManager.instance.dicCard["2_6"].history = PlayerPrefs.GetInt("stewardess_bottom");
        GameManager.instance.dicCard["2_7"].history = PlayerPrefs.GetInt("zookeeper_bottom");
        GameManager.instance.dicCard["2_8"].history = PlayerPrefs.GetInt("codi_bottom");
        GameManager.instance.dicCard["2_9"].history = PlayerPrefs.GetInt("officeworker_bottom");
        GameManager.instance.dicCard["2_10"].history = PlayerPrefs.GetInt("officeworker2_bottom");
        GameManager.instance.dicCard["2_11"].history = PlayerPrefs.GetInt("snoop_bottom");
        GameManager.instance.dicCard["2_12"].history = PlayerPrefs.GetInt("snoop2_bottom");
        GameManager.instance.dicCard["2_13"].history = PlayerPrefs.GetInt("teacher_bottom");
        GameManager.instance.dicCard["2_14"].history = PlayerPrefs.GetInt("teacher2_bottom");
        GameManager.instance.dicCard["2_15"].history = PlayerPrefs.GetInt("vet_bottom");

        GameManager.instance.dicCard["3_0"].history = PlayerPrefs.GetInt("ariidol2_onepiece");
        GameManager.instance.dicCard["3_1"].history = PlayerPrefs.GetInt("princess_onepiece");
        GameManager.instance.dicCard["3_2"].history = PlayerPrefs.GetInt("witch_onepiece");
        GameManager.instance.dicCard["3_3"].history = PlayerPrefs.GetInt("cats_onepiece");
        GameManager.instance.dicCard["3_4"].history = PlayerPrefs.GetInt("figure_onepiece");
        GameManager.instance.dicCard["3_5"].history = PlayerPrefs.GetInt("shuelidol_onepiece");
        GameManager.instance.dicCard["3_6"].history = PlayerPrefs.GetInt("patissier_onepiece");

        GameManager.instance.dicCard["4_0"].history = PlayerPrefs.GetInt("ariidol_shoes");
        GameManager.instance.dicCard["4_1"].history = PlayerPrefs.GetInt("ariidol2_shoes");
        GameManager.instance.dicCard["4_2"].history = PlayerPrefs.GetInt("witch_shoes");
        GameManager.instance.dicCard["4_3"].history = PlayerPrefs.GetInt("babysitter_shoes");
        GameManager.instance.dicCard["4_4"].history = PlayerPrefs.GetInt("babysitter2_shoes");
        GameManager.instance.dicCard["4_5"].history = PlayerPrefs.GetInt("fitnesstrainer_shoes");
        GameManager.instance.dicCard["4_6"].history = PlayerPrefs.GetInt("fitnesstrainer2_shoes");
        GameManager.instance.dicCard["4_7"].history = PlayerPrefs.GetInt("police_shoes");
        GameManager.instance.dicCard["4_8"].history = PlayerPrefs.GetInt("stewardess_shoes");
        GameManager.instance.dicCard["4_9"].history = PlayerPrefs.GetInt("zookeeper_shoes");
        GameManager.instance.dicCard["4_10"].history = PlayerPrefs.GetInt("cats_shoes");
        GameManager.instance.dicCard["4_11"].history = PlayerPrefs.GetInt("codi_shoes");
        GameManager.instance.dicCard["4_12"].history = PlayerPrefs.GetInt("figure_shoes");
        GameManager.instance.dicCard["4_13"].history = PlayerPrefs.GetInt("shuelidol_shoes");
        GameManager.instance.dicCard["4_14"].history = PlayerPrefs.GetInt("officeworker_shoes");
        GameManager.instance.dicCard["4_15"].history = PlayerPrefs.GetInt("officeworker2_shoes");
        GameManager.instance.dicCard["4_16"].history = PlayerPrefs.GetInt("snoop2_shoes");
        GameManager.instance.dicCard["4_17"].history = PlayerPrefs.GetInt("teacher_shoes");
        GameManager.instance.dicCard["4_18"].history = PlayerPrefs.GetInt("teacher2_shoes");
        GameManager.instance.dicCard["4_19"].history = PlayerPrefs.GetInt("vet_shoes");

        GameManager.instance.dicCard["5_0"].history = PlayerPrefs.GetInt("ariidol_acc");
        GameManager.instance.dicCard["5_1"].history = PlayerPrefs.GetInt("princess_acc");
        GameManager.instance.dicCard["5_2"].history = PlayerPrefs.GetInt("princess2_acc");
        GameManager.instance.dicCard["5_3"].history = PlayerPrefs.GetInt("police_acc");
        GameManager.instance.dicCard["5_4"].history = PlayerPrefs.GetInt("stewardess_acc");
        GameManager.instance.dicCard["5_5"].history = PlayerPrefs.GetInt("cats_acc");
        GameManager.instance.dicCard["5_6"].history = PlayerPrefs.GetInt("shuelidol_acc");
        GameManager.instance.dicCard["5_7"].history = PlayerPrefs.GetInt("patissier_acc");
    }
}