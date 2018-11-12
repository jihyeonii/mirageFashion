using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GuideManager : MonoBehaviour
{
    public static GuideManager instance;
    Vector2 screenSize;
    float minSwipeDist;
    Vector2 swipeDirection;

    Vector2 touchDownPos;
    bool swiped = false;
    bool isPress = false;

    bool isPrePage = false;
    bool isNextPage = false;
    Vector3 mouseDownPos;

    Vector3 pos;

    public Camera tutorialCamera;
    public GameObject img;


    private bool isInputBlocked = false;

    public static int cardNum = 0;
    float screenWidth;
    Vector3 finishPos;
    void Awake()
    {
        screenWidth = Screen.width *  1280/ Screen.height;
        //screenSize = new Vector2(Screen.width, Screen.height);
        //minSwipeDist = Mathf.Max(screenSize.x, screenSize.y) / 10;
        //img = GameManager.instance.cameraTutorial.transform.Find("Canvas").transform.Find("Image").gameObject;
        img.GetComponent<RectTransform>().sizeDelta = new Vector2(screenWidth * 4, img.GetComponent<RectTransform>().sizeDelta.y);

        gameObject.transform.parent.Find("Canvas").Find("Image").Find("1").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guide_1"];
        gameObject.transform.parent.Find("Canvas").Find("Image").Find("2").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guide_2"];
        gameObject.transform.parent.Find("Canvas").Find("Image").Find("3").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guide_3"];
        gameObject.transform.parent.Find("Canvas").Find("Image").Find("4").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guide_4"];
        //Debug.Log(Screen.width);
        for (int i=1; i<5; i++)
        {
            img.transform.Find(i.ToString()).transform.GetComponent<RectTransform>().sizeDelta = new Vector2(720, 1280);
        }
        img.transform.Find("1").transform.localPosition = new Vector3(screenWidth * 0.5f, 0);
        img.transform.Find("2").transform.localPosition = new Vector3(screenWidth * 1.5f, 0);
        img.transform.Find("3").transform.localPosition = new Vector3(screenWidth * 2.5f, 0);
        img.transform.Find("4").transform.localPosition = new Vector3(screenWidth * 3.5f, 0);                                                                                                                                                                                                                                                                                                                                                             
        tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
        //tutorialCamera = GameManager.instance.cameraTutorial.GetComponent<Camera>();
    }
    //// Use this for initialization
    void Start()
    {
        instance = this;
        if (PlayerPrefs.GetString("guide") != "off")
            gameObject.transform.parent.Find("Canvas").Find("background").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guideBg"];
        else
        {
            gameObject.transform.parent.Find("Canvas").Find("background").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["guideBg2"];
            img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
            //Debug.Log(img.transform.position); 
        }

    }


    void Update()
    {
        processInput();
        if (tutorialCamera.WorldToScreenPoint(img.transform.position).x >= 0)
        {
            img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
        }
        else if (-Screen.width * 3 >= tutorialCamera.WorldToScreenPoint(img.transform.position).x)
        {
            img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 3, 0, 100));
        }
        if (isPress == false)
        {
            if (cardNum == 0)
            {
                img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
            }
            else if (cardNum == 1)
            {
                img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width, 0, 100));
            }
            else if (cardNum == 2)
            {
                img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 2, 0, 100));
            }
            else if (cardNum == 3)
            {
                img.transform.position = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 3, 0, 100));
            }
        }
      
        if (PlayerPrefs.GetString("guide") == "off")
        {
            for (int i = 0; i < 4; i++)
            {
                PopupManager.mirageIntroUI.transform.Find("page").Find("Image" + i).GetComponent<Image>().sprite = Resources.Load<Sprite>("page_nor");
            }
            PopupManager.mirageIntroUI.transform.Find("page").Find("Image" + cardNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("page_sel");
            tutorialCamera.transform.Find("Canvas").Find("page").gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                tutorialCamera.transform.Find("Canvas").Find("page").Find("Image" + i).GetComponent<Image>().sprite = Resources.Load<Sprite>("page_nor");
            }
            tutorialCamera.transform.Find("Canvas").Find("page").Find("Image" + cardNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("page_sel");
        }
        //if (isPress)
        //{
            if (isPrePage)
            {
                switch (cardNum)
                {
                    case 0:
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
                        prePage(finishPos);
                        break;
                    case 1:
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width, 0, 100));
                        prePage(finishPos);
                        break;
                    case 2:
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 2, 0, 100));
                        prePage(finishPos);
                        break;
                    case 3:
                        
                        break;
                }
            }
            if (isNextPage)
            {
                
                switch (cardNum)
                {
                    case 0:
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
                        nextPage(finishPos);
                        break;
                    case 1:
                        
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width, 0, 100));
                        nextPage(finishPos);
                        break;
                    case 2:
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 2, 0, 100));
                        nextPage(finishPos);
                        break;
                    case 3:
                        finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 3, 0, 100));
                        nextPage(finishPos);
                        break;
                }
            }
        //}
    }
    public void nextPage(Vector3 pos)
    {
        if (img.transform.position.x<= pos.x)
        {
            //isPress = false;
            isNextPage = false;
            Debug.Log("finish");
        }
        else
        {
            img.transform.position = Vector3.MoveTowards(img.transform.position, pos, 6f * Time.smoothDeltaTime);
        }
    }
    public void prePage(Vector3 pos)
    {
        if(img.transform.position.x>= pos.x)
        {
            isPrePage = false;
            //isNextPage = false;
        }
        else
        {
            img.transform.position = Vector3.MoveTowards(img.transform.position, pos, 6f * Time.smoothDeltaTime);
        }
    }
    
    void processInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //GameManager.instance.instantiateSound(GameManager.instance.guideSound);
            isPress = false;
            pos = tutorialCamera.WorldToScreenPoint(img.transform.position);
            if (-Screen.width * 0.5f < pos.x )
            {
                cardNum = 0;
            }
            else if (Screen.width * -1.5f < pos.x && pos.x <= -Screen.width * 0.5f)
            {
                cardNum = 1;
            }
            else if (Screen.width * -2.5f < pos.x && pos.x <= Screen.width * -1.5f)
            {
                cardNum = 2;
            }
            else if (Screen.width * -3.5f < pos.x && pos.x <= Screen.width * -2.5f)
            {
                cardNum = 3;
            }
            //Debug.Log("pos :"+pos.x+", screen : " + Screen.width + ", cardNum : " + cardNum);
            mouseDownPos = Input.mousePosition;
            swiped = false;
        }
        else if (Input.GetMouseButton(0))
        {
            isPress = true;
            swipeDirection = (Input.mousePosition - mouseDownPos).normalized;

            //Debug.Log(pos);
            //0Debug.Log((Input.mousePosition - mouseDownPos).x);
            Debug.Log("cardNum :" + cardNum);
            switch (cardNum)
            {
                case 0:
                    pos = tutorialCamera.ScreenToWorldPoint(new Vector2((Input.mousePosition - mouseDownPos).x,/* Screen.height / 2*/0));
                    break;
                case 1:
                    pos = tutorialCamera.ScreenToWorldPoint(new Vector2(-Screen.width + (Input.mousePosition - mouseDownPos).x, 0));
                    break;
                case 2:
                    pos = tutorialCamera.ScreenToWorldPoint(new Vector2(-Screen.width * 2 + (Input.mousePosition - mouseDownPos).x, 0));
                    break;
                case 3:
                    pos = tutorialCamera.ScreenToWorldPoint(new Vector2(-Screen.width * 3 + (Input.mousePosition - mouseDownPos).x, 0));
                    break;
            }

            pos.z = 100;
            img.transform.position = pos;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            swiped = true;

            switch (cardNum)
            {
                case 0:
                    if ((Input.mousePosition - mouseDownPos).x < -Screen.width/5)
                    {
                        //finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-screenWidth, 0, 100));
                        //Debug.Log(finishPos);
                        //StartCoroutine(moveNextPage(finishPos));
                        cardNum = 1;
                        isNextPage = true;
                    }
                    else
                    {
                        cardNum = 0;
                    }
                    break;
                case 1:
                    if ((Input.mousePosition - mouseDownPos).x < -Screen.width / 5)
                    {
                        //finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-screenWidth*2, 0, 100));
                        //StartCoroutine(moveNextPage(finishPos));
                        cardNum = 2;
                        isNextPage = true;
                    }

                    else if ((Input.mousePosition - mouseDownPos).x > Screen.width / 5)
                    {
                        //finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(0, 0, 100));
                        //StartCoroutine(movePrePage(finishPos));
                        cardNum = 0;
                        isPrePage = true;
                    }
                    else
                    {

                        cardNum = 1;
                    }
                    break;
                case 2:
                    if ((Input.mousePosition - mouseDownPos).x < -Screen.width / 5)
                    {
                        //finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-screenWidth * 3, 0, 100));
                        //StartCoroutine(moveNextPage(finishPos));
                        cardNum = 3;
                        isNextPage = true;
                    }
                    else if ((Input.mousePosition - mouseDownPos).x > Screen.width / 5)
                    {
                        //finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-screenWidth, 0, 100));
                        //StartCoroutine(movePrePage(finishPos));
                        cardNum = 1;
                        isPrePage = true;
                    }
                    else
                    {

                        cardNum = 2;
                    }
                    break;
                case 3:
                    if ((Input.mousePosition - mouseDownPos).x > Screen.width / 5)
                    {
                        //finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-screenWidth * 2, 0, 100));
                        //StartCoroutine(movePrePage(finishPos));
                        cardNum = 2;
                        isPrePage = true;
                    }
                    else if ((Input.mousePosition - mouseDownPos).x < -Screen.width / 5 && GameManager.instance.uiState == GameManager.UIState.guide)
                    {
                        tutorialCamera.gameObject.SetActive(false);
                        GameManager.instance.uiState = GameManager.UIState.main;
                        //cardNum = 2;
                    }
                    else if ((Input.mousePosition - mouseDownPos).x < -Screen.width / 5 && GameManager.instance.uiState != GameManager.UIState.guide)
                    {
                        cardNum = 3;
                    }
                    //else
                    //{
                    //    finishPos = tutorialCamera.ScreenToWorldPoint(new Vector3(-Screen.width * 3, 0, 100));
                    //    StartCoroutine(movePrePage(finishPos));
                    //    cardNum = 2;
                    //}
                    break;
            }
            if (cardNum < 3)
            {

                StartCoroutine(count());
            }

        }
    }
    IEnumerator count()
    {
        yield return new WaitForSeconds(0.8f);
        isPress = false;
    }
    

}

