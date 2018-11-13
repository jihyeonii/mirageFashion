using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class DressRoomManager : MonoBehaviour {
    public static DressRoomManager instance;
    GameObject[] locker;
    GameObject[] lockerPos;
    GameObject[] lockerEdit;
    bool isClick = false;


    Vector3 prePos;
    Vector3 curPos;

    public GameObject lockerObj;
    int lockerNum = 0;

    bool isTurn = false;
    public Dictionary<string, CharacterInfo> dicLockerChar;
    Dictionary<string, CharacterInfo> orignLocker;
    int direction;
    // Use this for initialization
    void Start () {
        instance = this;
        dicLockerChar = new Dictionary<string, CharacterInfo>();
        orignLocker = new Dictionary<string, CharacterInfo>();
        locker = new GameObject[3];
        lockerEdit = new GameObject[3];
        locker[0] = lockerObj.transform.Find("locker_0").gameObject;
        locker[1] = lockerObj.transform.Find("locker_1").gameObject;
        locker[2] = lockerObj.transform.Find("locker_2").gameObject;
        lockerPos = new GameObject[3];
        lockerPos[0] = lockerObj.transform.Find("locker0_pos").gameObject;
        lockerPos[1] = lockerObj.transform.Find("locker1_pos").gameObject;
        lockerPos[2] = lockerObj.transform.Find("locker2_pos").gameObject;
        lockerEdit[0] = locker[0].transform.Find("btnAdd,Remove").gameObject;
        lockerEdit[1] = locker[1].transform.Find("btnAdd,Remove").gameObject;
        lockerEdit[2] = locker[2].transform.Find("btnAdd,Remove").gameObject;

        gameObject.transform.Find("background").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["dressBackground"];
        //if(Screen.height * 720 /1280  < Screen.width)
        //{
        //    gameObject.transform.Find("background").transform.localScale = new Vector3(Screen.width, Screen.width * 1280 / 720);
        //}
        //else
        //{

        //     gameObject.transform.Find("background").transform.localScale = new Vector3(Screen.height * 720 / 1280, Screen.height, 1);
        //}

        //보관함
        if (PlayerPrefs.GetString("locker0_body").Contains("char"))
        {
            dicLockerChar.Add("char0", new CharacterInfo(PlayerPrefs.GetString("locker0_body"), PlayerPrefs.GetString("locker0_top"), PlayerPrefs.GetString("locker0_bottom"), PlayerPrefs.GetString("locker0_onepiece"), PlayerPrefs.GetString("locker0_shoes"), PlayerPrefs.GetString("locker0_acc")));
            
        }
        else
        {

        }
        if (PlayerPrefs.GetString("locker1_body").Contains("char"))
        {
            dicLockerChar.Add("char1", new CharacterInfo(PlayerPrefs.GetString("locker1_body"), PlayerPrefs.GetString("locker1_top"), PlayerPrefs.GetString("locker1_bottom"), PlayerPrefs.GetString("locker1_onepiece"), PlayerPrefs.GetString("locker1_shoes"), PlayerPrefs.GetString("locker1_acc")));
        }
        else
        {

        }
        if (PlayerPrefs.GetString("locker2_body").Contains("char"))
        {
            dicLockerChar.Add("char2", new CharacterInfo(PlayerPrefs.GetString("locker2_body"), PlayerPrefs.GetString("locker2_top"), PlayerPrefs.GetString("locker2_bottom"), PlayerPrefs.GetString("locker2_onepiece"), PlayerPrefs.GetString("locker2_shoes"), PlayerPrefs.GetString("locker2_acc")));
        }
        else
        {

        }

        reloadLocker();
        if (dicLockerChar.ContainsKey("char0"))
        {
            gameObject.transform.Find("btnLoad").gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.Find("btnLoad").gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update() {


#if UNITY_EDITOR
        //input();
#else
        //mobileInput();
#endif
        /*보관함 회전*/
        if (isTurn)
        {
            trun();
        }
        else
        {
            locker[lockerNum].transform.localScale = new Vector3(1, 1, 1);
            if(lockerNum == 2) {
                locker[0].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                locker[1].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else if(lockerNum == 1)
            {
                locker[0].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                locker[2].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else if (lockerNum == 0)
            {
                locker[1].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                locker[2].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }

    }
    public void reloadLocker()
    {
        GameObject[] anotherObj;
        for(int i = 0; i < 3; i++)
        {
            anotherObj = new GameObject[locker[i].transform.Find("character").transform.childCount];
            for (int j = 0; j < locker[i].transform.Find("character").transform.childCount; j++)
            {
                anotherObj[j] = locker[i].transform.Find("character").transform.GetChild(j).gameObject;
                anotherObj[j].SetActive(false);
            }
          
            if (dicLockerChar.ContainsKey("char" + i))
            {
                if (dicLockerChar["char"+i].body.Contains("char"))
                {
                    locker[i].transform.Find("emptySlot").gameObject.SetActive(false);
                    locker[i].transform.Find("character").transform.Find("body").gameObject.SetActive(true);
                    locker[i].transform.Find("character").transform.Find(dicLockerChar["char"+i].body).gameObject.SetActive(true);
                    //top;
                    if (dicLockerChar["char" + i].top.Contains("top"))
                    {
                        locker[i].transform.Find("character").transform.Find(dicLockerChar["char" + i].top).gameObject.SetActive(true);
                    }
                    else
                    {
                        if (dicLockerChar["char" + i].onepiece.Contains("onepiece"))
                            locker[i].transform.Find("character").transform.Find("basic_top").gameObject.SetActive(false);
                        else
                            locker[i].transform.Find("character").transform.Find("basic_top").gameObject.SetActive(true);
                    }
                    //bottom

                    if (dicLockerChar["char" + i].bottom.Contains("bottom"))
                    {

                        locker[i].transform.Find("character").transform.Find(dicLockerChar["char" + i].bottom).gameObject.SetActive(true);
                    }
                    else
                    {
                        if (dicLockerChar["char" + i].onepiece.Contains("onepiece"))
                            locker[i].transform.Find("character").transform.Find("basic_bottom").gameObject.SetActive(false);
                        else
                            locker[i].transform.Find("character").transform.Find("basic_bottom").gameObject.SetActive(true);
                    }
                    //onepiece
                    if (dicLockerChar["char" + i].onepiece.Contains("onepiece"))
                    {

                        locker[i].transform.Find("character").transform.Find(dicLockerChar["char" + i].onepiece).gameObject.SetActive(true);
                    }
                    //shoes
                    if (dicLockerChar["char" + i].shoes.Contains("shoes"))
                    {

                        locker[i].transform.Find("character").transform.Find(dicLockerChar["char" + i].shoes).gameObject.SetActive(true);
                    }
                    else
                    {
                    }
                    //acc
                    if (dicLockerChar["char" + i].acc.Contains("acc"))
                    {

                        locker[i].transform.Find("character").transform.Find(dicLockerChar["char" + i].acc).gameObject.SetActive(true);
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                locker[i].transform.Find("emptySlot").gameObject.SetActive(true);
            }
        }
    }
    public void turnRight()
    {
        GameManager.instance.instantiateSound(GameManager.instance.lockerSound);
        if (lockerNum < 2)
            lockerNum++;
        else
            lockerNum = 0;
        direction = 1;
        isTurn = true;
    }
    public void turnLeft()
    {
        GameManager.instance.instantiateSound(GameManager.instance.lockerSound);
        if (lockerNum > 0)
            lockerNum--;
        else
            lockerNum = 2;
        direction = 0;
        isTurn = true;
    }
    public void trun()// 0:L, 1:R
    {
        float speed = 4f;
        if (lockerNum == 2)
        {
            if(direction == 1)
            {
                locker[0].transform.SetSiblingIndex(0);
                locker[1].transform.SetSiblingIndex(1);
                locker[2].transform.SetSiblingIndex(2);
            }
            else
            {
                locker[1].transform.SetSiblingIndex(0);
                locker[2].transform.SetSiblingIndex(1);
                locker[0].transform.SetSiblingIndex(2);
            }
            locker[0].transform.position = Vector3.MoveTowards(locker[0].transform.position, lockerPos[1].transform.position, speed * Time.smoothDeltaTime);
            locker[1].transform.position = Vector3.MoveTowards(locker[1].transform.position, lockerPos[2].transform.position, speed * Time.smoothDeltaTime);
            locker[2].transform.position = Vector3.MoveTowards(locker[2].transform.position, lockerPos[0].transform.position, speed * Time.smoothDeltaTime);
            locker[0].transform.localScale = Vector3.Lerp(locker[0].transform.localScale, lockerPos[1].transform.localScale, 0.06f);
            locker[1].transform.localScale = Vector3.Lerp(locker[1].transform.localScale, lockerPos[1].transform.localScale, 0.06f);
            locker[2].transform.localScale = Vector3.Lerp(locker[2].transform.localScale, lockerPos[0].transform.localScale, 0.03f);
        }
        else if (lockerNum == 1)
        {
            if (direction == 1)
            {
                locker[2].transform.SetSiblingIndex(0);
                locker[0].transform.SetSiblingIndex(1);
                locker[1].transform.SetSiblingIndex(2);
            }
            else
            {
                locker[0].transform.SetSiblingIndex(0);
                locker[1].transform.SetSiblingIndex(1);
                locker[2].transform.SetSiblingIndex(2);
            }
            locker[0].transform.position = Vector3.MoveTowards(locker[0].transform.position, lockerPos[2].transform.position, speed * Time.smoothDeltaTime);
            locker[1].transform.position = Vector3.MoveTowards(locker[1].transform.position, lockerPos[0].transform.position, speed * Time.smoothDeltaTime);
            locker[2].transform.position = Vector3.MoveTowards(locker[2].transform.position, lockerPos[1].transform.position, speed * Time.smoothDeltaTime);
            locker[2].transform.localScale = Vector3.Lerp(locker[2].transform.localScale, lockerPos[1].transform.localScale, 0.06f);
            locker[1].transform.localScale = Vector3.Lerp(locker[1].transform.localScale, lockerPos[0].transform.localScale, 0.03f);
            locker[0].transform.localScale = Vector3.Lerp(locker[0].transform.localScale, lockerPos[1].transform.localScale, 0.06f);
        }
        else
        {
            if(direction == 1)
            {
                locker[1].transform.SetSiblingIndex(0);
                locker[2].transform.SetSiblingIndex(1);
                locker[0].transform.SetSiblingIndex(2);
            }
            else
            {
                locker[2].transform.SetSiblingIndex(0);
                locker[1].transform.SetSiblingIndex(1);
                locker[0].transform.SetSiblingIndex(2);
            }
            locker[0].transform.position = Vector3.MoveTowards(locker[0].transform.position, lockerPos[0].transform.position, speed * Time.smoothDeltaTime);
            locker[1].transform.position = Vector3.MoveTowards(locker[1].transform.position, lockerPos[1].transform.position, speed * Time.smoothDeltaTime);
            locker[2].transform.position = Vector3.MoveTowards(locker[2].transform.position, lockerPos[2].transform.position, speed * Time.smoothDeltaTime);
            locker[1].transform.localScale = Vector3.Lerp(locker[1].transform.localScale, lockerPos[1].transform.localScale, 0.06f);
            locker[0].transform.localScale = Vector3.Lerp(locker[0].transform.localScale, lockerPos[0].transform.localScale, 0.03f);
            locker[2].transform.localScale = Vector3.Lerp(locker[2].transform.localScale, lockerPos[1].transform.localScale, 0.06f);
        }
        if (locker[lockerNum].transform.position == lockerPos[0].transform.position)
        {
            isTurn = false;
        }
        locker[0].transform.LookAt(GameObject.Find("PopupCamera").transform.Find("LookAt").transform);
        locker[1].transform.LookAt(GameObject.Find("PopupCamera").transform.Find("LookAt").transform);
        locker[2].transform.LookAt(GameObject.Find("PopupCamera").transform.Find("LookAt").transform);

        if (dicLockerChar.ContainsKey("char" + lockerNum))
        {
            gameObject.transform.Find("btnLoad").gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.Find("btnLoad").gameObject.SetActive(false);
        }
    }
    
   
    public void edit(GameObject obj)
    {
        
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        if (obj.transform.Find("Text").GetComponent<Text>().text.Contains("편집"))
        {
            PopupManager.instance.popupState = PopupManager.Popup.editDressRoom;
            orignLocker = new Dictionary<string, CharacterInfo>();
            for(int i = 0; i< 3; i++)
            {
                if (dicLockerChar.ContainsKey("char" + i))
                {
                    if (dicLockerChar["char" + i].body.Contains("char"))
                    {
                        orignLocker.Add("char" + i, dicLockerChar["char" + i]);
                    }
                }
               
            }
            lockerEdit[0].SetActive(true);
            lockerEdit[1].SetActive(true);
            lockerEdit[2].SetActive(true);
            for(int i =0; i<3; i++)
            {
                if (dicLockerChar.ContainsKey("char" + i))
                {
                    lockerEdit[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_locker_remove");
                }
                else
                {
                    if (GameManager.instance.charInfo.body.Contains("char"))
                        lockerEdit[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_locker_add");
                    else
                        lockerEdit[i].SetActive(false);
                }
            }
            obj.transform.Find("Text").GetComponent<Text>().text = "완료";
            gameObject.transform.Find("btnLeft").transform.gameObject.SetActive(false);
            gameObject.transform.Find("btnRight").transform.gameObject.SetActive(false);
            gameObject.transform.Find("btnBack").transform.gameObject.SetActive(false);
            gameObject.transform.Find("btnLoad").transform.gameObject.SetActive(false);

            
        }
        else if (obj.transform.Find("Text").GetComponent<Text>().text.Contains("완료"))
        {
            PopupManager.instance.popupState = PopupManager.Popup.settingDressRoom;
            lockerEdit[0].SetActive(false);
            lockerEdit[1].SetActive(false);
            lockerEdit[2].SetActive(false);
            obj.transform.Find("Text").GetComponent<Text>().text = "편집";
            gameObject.transform.Find("btnLeft").transform.gameObject.SetActive(true);
            gameObject.transform.Find("btnRight").transform.gameObject.SetActive(true);
            gameObject.transform.Find("btnBack").transform.gameObject.SetActive(true);
            if (dicLockerChar.ContainsKey("char" + lockerNum))
            {
                gameObject.transform.Find("btnLoad").transform.gameObject.SetActive(true);
            }
            gameObject.transform.Find("btnRevert").transform.gameObject.SetActive(false);
            savePlayerPrefs();
            reloadLocker();
        }
    }
    public void removeOrAdd(int num)//locker num
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        if (dicLockerChar.ContainsKey("char"+num))
        {
            //삭제
            dicLockerChar.Remove("char" + num);
        }
        else
        {
            //보관함 추가
            if (GameManager.instance.charInfo.body.Contains("char"))
            {
                dicLockerChar.Add("char" + num, new CharacterInfo(GameManager.instance.charInfo.body,
                                                                                        GameManager.instance.charInfo.top,
                                                                                        GameManager.instance.charInfo.bottom,
                                                                                        GameManager.instance.charInfo.onepiece,
                                                                                        GameManager.instance.charInfo.shoes,
                                                                                        GameManager.instance.charInfo.acc));
            }
            else
            {

            }
        }
        gameObject.transform.Find("btnRevert").transform.gameObject.SetActive(false);
        for(int i = 0; i<3; i++)
        {
            if(orignLocker.ContainsKey("char"+i))
            {
                if(dicLockerChar.ContainsKey("char"+i))
                {
                    if (dicLockerChar["char"+i] != orignLocker["char" + i])
                    {
                        gameObject.transform.Find("btnRevert").transform.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (orignLocker.ContainsKey("char" + i))
                    {
                        gameObject.transform.Find("btnRevert").transform.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (dicLockerChar.ContainsKey("char" + i))
                {
                    gameObject.transform.Find("btnRevert").transform.gameObject.SetActive(true);
                }
            }
        }
       
        for(int i = 0; i<3; i++)
        {
            if (dicLockerChar.ContainsKey("char" + i))
            {
                lockerEdit[i].SetActive(true);
                lockerEdit[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_locker_remove");
            }
            else
            {
                if (GameManager.instance.charInfo.body.Contains("char"))
                {
                    lockerEdit[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_locker_add");
                    lockerEdit[i].SetActive(true);
                }
                else
                {
                    lockerEdit[i].SetActive(false);
                }
            }
        }
        reloadLocker();
    }
    public void revert()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        for (int i = 0; i< 3; i++)
        {
            if(orignLocker.ContainsKey("char" + i))
            {
                dicLockerChar["char" + i] = orignLocker["char" + i];
            }
            else
            {
                dicLockerChar.Remove("char" + i);
            }
        }
        reloadLocker();
        for (int i=0; i<3; i++)
        {
            if (dicLockerChar.ContainsKey("char" + i))
            {
                lockerEdit[i].SetActive(true);
                lockerEdit[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_locker_remove");
            }
            else
            {
                if (GameManager.instance.charInfo.body.Contains("char"))
                {

                    lockerEdit[i].SetActive(true);
                    lockerEdit[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_locker_add");
                }
                else
                    lockerEdit[i].SetActive(false);
            }
        }
        
        gameObject.transform.Find("btnRevert").transform.gameObject.SetActive(false);
        
    }
    public void loadLocker()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        if (GameManager.instance.isCardRecognition)
        {

            CameraSettings tmp;
            GameManager.instance.resetChar();
            if (GameManager.instance.uiState == GameManager.UIState.main)
            {
                if (GameManager.instance.charInfo.body.Contains("char") == false)
                {
                    GameManager.charControlBtnUI.SetActive(true);
                    for (int i = 0; i < 4; i++)
                    {
                        GameManager.recognizeObj[i].SetActive(true);

                    }
                    GameManager.instance.character.SetActive(true);
                    GameManager.instance.characterAni.SetBool("appear_char", true);
                    GameManager.instance.arCharacter.gameObject.SetActive(true);
                    GameManager.instance.characterAni.SetBool("appear_char", false);
                    GameManager.canvasBottomButtonUI.transform.Find("btnSaveChar").transform.gameObject.SetActive(true);
                }
                GameManager.instance.GuideLine.SetActive(true);
                GameManager.canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(false);
            }
            GameManager.instance.character.transform.localScale = GameManager.instance.charCamera.transform.Find("objPos").transform.localScale;//CharacterController.instance.pos.transform.localScale;
            GameManager.instance.character.transform.rotation = GameManager.instance.charCamera.transform.Find("objPos").transform.rotation;//CharacterController.instance.pos.transform.rotation;

            PopupManager.instance.popupState = PopupManager.Popup.home;
            GameManager.instance.cameraAR.SetActive(true);
            GameManager.instance.SwitchAutofocus(true);



            GameManager.instance.character.transform.localPosition = GameManager.instance.charCamera.transform.Find("objPos").transform.localPosition;//CharacterController.instance.pos.transform.localPosition;
            GameManager.instance.character.transform.rotation = GameManager.instance.charCamera.transform.Find("objPos").transform.rotation; //CharacterController.instance.pos.transform.rotation;

            GameManager.instance.charInfo.body = dicLockerChar["char" + lockerNum].body;
            GameManager.instance.charInfo.top = dicLockerChar["char" + lockerNum].top;
            GameManager.instance.charInfo.bottom = dicLockerChar["char" + lockerNum].bottom;
            GameManager.instance.charInfo.onepiece = dicLockerChar["char" + lockerNum].onepiece;
            GameManager.instance.charInfo.shoes = dicLockerChar["char" + lockerNum].shoes;
            GameManager.instance.charInfo.acc = dicLockerChar["char" + lockerNum].acc;

            GameManager.instance.offCharObj("char");
            GameManager.instance.offCharObj("top");
            GameManager.instance.offCharObj("bottom");
            GameManager.instance.offCharObj("onepiec");
            GameManager.instance.offCharObj("shoes");
            GameManager.instance.offCharObj("acc");

            GameManager.instance.onCloth("char", GameManager.instance.charInfo.body);
            GameManager.instance.onCloth("top", GameManager.instance.charInfo.top);
            GameManager.instance.onCloth("bottom", GameManager.instance.charInfo.bottom);
            GameManager.instance.onCloth("onepiece", GameManager.instance.charInfo.onepiece);
            GameManager.instance.onCloth("shoes", GameManager.instance.charInfo.shoes);
            GameManager.instance.onCloth("acc", GameManager.instance.charInfo.acc);

            GameManager.recognizeObj[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
            GameManager.recognizeObj[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
            GameManager.recognizeObj[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_shoes");
            GameManager.recognizeObj[3].GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_acc");
            if (GameManager.instance.charInfo.top.Contains("top"))
            {
                GameManager.recognizeObj[0].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.charInfo.top + "_img"];

                if (!GameManager.instance.charInfo.bottom.Contains("bottom"))
                    GameManager.instance.arCharacter.transform.Find("basic_bottom").gameObject.SetActive(true);
            }
            if (GameManager.instance.charInfo.bottom.Contains("bottom"))
            {
                GameManager.recognizeObj[1].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.charInfo.bottom + "_img"];

                if (!GameManager.instance.charInfo.top.Contains("top"))
                    GameManager.instance.arCharacter.transform.Find("basic_top").gameObject.SetActive(true);
            }
            if (GameManager.instance.charInfo.onepiece.Contains("onepiece"))
            {
                GameManager.recognizeObj[0].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.charInfo.onepiece + "_img"];
                GameManager.recognizeObj[1].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.charInfo.onepiece + "_img"];
            }
            if (GameManager.instance.charInfo.shoes.Contains("shoes"))
            {
                GameManager.recognizeObj[2].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.charInfo.shoes + "_img"];
            }
            if (GameManager.instance.charInfo.acc.Contains("acc"))
            {
                GameManager.recognizeObj[3].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.charInfo.acc + "_img"];
            }
            if (!GameManager.instance.charInfo.top.Contains("top") && !GameManager.instance.charInfo.bottom.Contains("bottom") && !GameManager.instance.charInfo.onepiece.Contains("onepiece"))
            {
                GameManager.instance.arCharacter.transform.Find("basic_bottom").gameObject.SetActive(true);
                GameManager.instance.arCharacter.transform.Find("basic_top").gameObject.SetActive(true);
            }
            Debug.Log("char : " + GameManager.instance.charInfo.body);
            lockerObj.transform.rotation = Quaternion.Euler(new Vector3(lockerObj.transform.rotation.x, 0, lockerObj.transform.rotation.z));
            lockerNum = 0;
            locker[0].transform.LookAt(GameObject.Find("PopupCamera").transform.Find("LookAt").transform);
            locker[0].transform.localScale = new Vector3(1, 1, 1);
            locker[1].transform.LookAt(GameObject.Find("PopupCamera").transform.Find("LookAt").transform);
            locker[1].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            locker[2].transform.LookAt(GameObject.Find("PopupCamera").transform.Find("LookAt").transform);
            locker[2].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            GameManager.instance.loadChar();
            if (dicLockerChar.ContainsKey("char0"))
            {
                gameObject.transform.Find("btnLoad").gameObject.SetActive(true);
            }
            else
            {
                gameObject.transform.Find("btnLoad").gameObject.SetActive(false);
            }
            GameManager.instance.checkAnimation();

            if (GameManager.instance.uiState == GameManager.UIState.camera)
            {
                GameManager.instance.GuideLine.SetActive(false);
            }
            PopupManager.settingPage.transform.position = new Vector3(PopupManager.instance.cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 0)).x, PopupManager.settingPage.transform.position.y, PopupManager.settingPage.transform.position.z);
            GameManager.instance.isPopup = false;

            for (int i = 0; i < 3; i++)
                locker[i].transform.position = lockerPos[i].transform.position;
            GameManager.instance.instantiateSound(GameManager.instance.loadSound);

            tmp = FindObjectOfType<CameraSettings>();
            if (tmp.IsFrontCameraActive())
            {
                GameManager.instance.dressroomLoad = true;
            }
        }
        else
        {
            PopupManager.instance.showPopup("보관함 의상을 불러오면 기존 패션인식된 의상이 사라집니다. \n 계속진행하시겠습니까?");
        }
    }
    
    public void savePlayerPrefs()
    {
        for (int i = 0; i < 3; i++)
        {
            if (dicLockerChar.ContainsKey("char"+i))
            {

                PlayerPrefs.SetString("locker"+i+"_body", dicLockerChar["char" + i].body);
                PlayerPrefs.SetString("locker"+i+"_top", dicLockerChar["char" + i].top);
                PlayerPrefs.SetString("locker"+i+"_bottom", dicLockerChar["char" + i].bottom);
                PlayerPrefs.SetString("locker"+i+"_onepiece", dicLockerChar["char" + i].onepiece);
                PlayerPrefs.SetString("locker"+i+"_shoes", dicLockerChar["char" + i].shoes);
                PlayerPrefs.SetString("locker"+i+"_acc", dicLockerChar["char" + i].acc);
            }
            else
            {
                PlayerPrefs.SetString("locker"+i+"_body", "0");
            }
        }
    }
   
   
}
