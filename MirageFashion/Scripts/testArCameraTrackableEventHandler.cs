using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Net;

public class testArCameraTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PUBLIC_MEMBER_VARIABLES

    Text test;

    public static testArCameraTrackableEventHandler instance;
    //GameObject characterParent;
    public string cardName;
    //int cardObj; 
    

    #endregion

    #region PRIVATE_MEMBER_VARIABLES

    private TrackableBehaviour mTrackableBehaviour;


    #endregion

    #region UNTIY_MONOBEHAVIOUR_METHODS

    private void Start()
    {
        instance = this;

        //characterParent = GameObject.Find("ARCamera").transform.Find("Camera").transform.Find("Canvas").transform.Find("Character").gameObject;
        //test = GameObject.Find("ARCamera").transform.Find("Camera").transform.Find("Canvas").transform.Find("Text").GetComponent<Text>();
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    private void Update()
    {
        GameManager.instance.cameraAR.transform.position = new Vector3(0, 0, 0);
        GameManager.instance.cameraAR.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    #endregion

    #region PUBLIC_METHODS

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            if (GameManager.instance.uiState == GameManager.UIState.main && GameManager.instance.isPopup == false && GameManager.instance.availableRecognize)
            {
                //StartCoroutine(delayRecognize());
                GameManager.instance.availableRecognize = false;
                OnTrackingFound();
            }
        }
        else
        {
            OnTrackingLost();
        }
    }

    IEnumerator delayRecognize()
    {
        GameManager.instance.availableRecognize = false;
        yield return new WaitForSeconds(2.5f);
        GameManager.instance.availableRecognize = true;
    }
    #endregion

    #region PRIVATE_METHODS
    public void recObj(string obj)
    {
        if (obj.Contains("char"))
        {
            if (GameManager.instance.charInfo.body.Contains("char") == false)
            {
                GameManager.instance.guideButtonUI.SetActive(false);
                GameManager.canvasBottomButtonUI.transform.Find("cameraModeImg").gameObject.SetActive(true);
                CharacterController.instance.resetTrans();
                GameManager.charControlBtnUI.SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    GameManager.recognizeObj[i].SetActive(true);
                    switch (i)
                    {
                        case 0:
                            GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                            break;
                        case 1:
                            GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
                            break;
                        case 2:
                            GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_shoes");
                            break;
                        case 3:
                            GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_acc");
                            break;
                    }

                }
                GameManager.instance.character.SetActive(true);

                //test.text = cardName;

                GameManager.instance.charInfo.body = obj;
                GameManager.instance.changeChar(obj);
                GameManager.instance.offCharObj("top");
                GameManager.instance.offCharObj("bottom");
                GameManager.instance.offCharObj("onepiece");
                GameManager.instance.offCharObj("shoes");
                GameManager.instance.offCharObj("acc");
                GameManager.canvasBottomButtonUI.transform.Find("btnSaveChar").transform.gameObject.SetActive(true);
                GameManager.instance.effectCamera.SetActive(true);



                GameManager.instance.arCharacter.transform.Find("basic_bottom").gameObject.SetActive(true);
                GameManager.instance.arCharacter.transform.Find("basic_top").gameObject.SetActive(true);
                //if (PlayerPrefs.GetString("guide") != "off")
                //{
                //    PopupManager.instance.showGuide();
                //}

                //애니메이션 테스트
                StartCoroutine(GameManager.instance.appearAni());

            
            }
            else
            {

                GameManager.instance.arCharacter.transform.localPosition = new Vector3(0, -0.8f, 15f);
                GameManager.instance.arCharacter.transform.localScale = new Vector3(200, 200, 200);
                GameManager.instance.arCharacter.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
                GameManager.instance.appearCharEffect.SetActive(false);
                //test.text = cardName;
                GameManager.instance.charInfo.body = obj;
                GameManager.instance.changeChar(obj);
            }
        }
        else if (obj.Contains("top"))
        {
            GameManager.instance.charInfo.onepiece = "0";
            GameManager.instance.charInfo.top = obj;
            StartCoroutine(OnCloth("top", obj));

            GameManager.recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];

            if (GameManager.instance.charInfo.bottom.Contains("bottom") == false)
            {
                GameManager.recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
                GameManager.instance.arCharacter.transform.Find("basic_bottom").gameObject.SetActive(true);
            }
            StartCoroutine(onParticle(GameManager.instance.topEffect));
        }
        else if (obj.Contains("bottom"))
        {
            GameManager.instance.charInfo.bottom = obj;
            GameManager.instance.charInfo.onepiece = "0";

            StartCoroutine(OnCloth("bottom", obj));

            GameManager.recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];
            if (GameManager.instance.charInfo.top.Contains("top") == false)
            {
                GameManager.recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                GameManager.instance.arCharacter.transform.Find("basic_top").gameObject.SetActive(true);
            }
            StartCoroutine(onParticle(GameManager.instance.bottmEffect));
        }
        else if (obj.Contains("onepiece"))
        {
            GameManager.instance.charInfo.onepiece = obj;

            GameManager.instance.charInfo.top = "0";
            GameManager.instance.charInfo.bottom = "0";
            StartCoroutine(OnCloth("onepiece", obj));

            GameManager.recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];
            GameManager.recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];

            StartCoroutine(onParticle(GameManager.instance.onepieceEffect));
        }
        else if (obj.Contains("shoes"))
        {
            GameManager.instance.charInfo.shoes = obj;
            StartCoroutine(OnCloth("shoes", obj));

            GameManager.recognizeObj[2].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];

            StartCoroutine(onParticle(GameManager.instance.shoesEffect));
        }
        else if (obj.Contains("acc"))
        {
            GameManager.instance.charInfo.acc = obj;
            StartCoroutine(OnCloth("acc", obj));

            GameManager.recognizeObj[3].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];

            StartCoroutine(onParticle(GameManager.instance.accEffect));
        }

        GameManager.instance.checkAnimation();
        if (PlayerPrefs.GetString("guide") != "off")
        {
            PopupManager.instance.showGuide();
        }
        GameManager.instance.availableRecognize = true;
        GameManager.instance.recogObjName = null;
    }
    public void recObjPopup(string obj)
    {
        GameManager.instance.isOnCloth = true;
        GameManager.canvasRecObj.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[obj + "_img"];
        if (obj.Contains("char"))
        {
            GameManager.canvasRecObj.transform.Find("Text").GetComponent<Text>().text = "해당캐릭터를 소환하시겠습니까?";
        }
        else
        {
            GameManager.canvasRecObj.transform.Find("Text").GetComponent<Text>().text = "해당의상을 착용하시겠습니까?";
        }
        GameManager.canvasRecObj.transform.Find("btnConfirm").Find("Text").GetComponent<Text>().text = "적용";
        GameManager.instance.recogObjName = obj;
        GameManager.canvasRecObj.SetActive(true);

        GameManager.instance.GuideLine.SetActive(false);
        GameManager.instance.guideButtonUI.SetActive(false);
    }

   
    private void OnTrackingFound()
    {
        GameManager.instance.cameraAR.transform.position = new Vector3(0, 0, 0);
        GameManager.instance.cameraAR.transform.eulerAngles = new Vector3(0, 0, 0);
        if (GameManager.instance.charInfo.body.Contains("char"))
        {
            if (cardName.Contains("char") && cardName != GameManager.instance.charInfo.body|| 
                cardName.Contains("top") && cardName != GameManager.instance.charInfo.top|| 
                cardName.Contains("bottom") && cardName != GameManager.instance.charInfo.bottom ||
                cardName.Contains("onepiece") && cardName != GameManager.instance.charInfo.onepiece ||
                cardName.Contains("shoes") && cardName != GameManager.instance.charInfo.shoes ||
                cardName.Contains("acc") && cardName != GameManager.instance.charInfo.acc)
            {
                if (cardName.Contains("top") && !GameManager.instance.charInfo.top.Contains("top") && !GameManager.instance.charInfo.onepiece.Contains("onepiece") ||
                    cardName.Contains("bottom") && !GameManager.instance.charInfo.bottom.Contains("bottom") && !GameManager.instance.charInfo.onepiece.Contains("onepiece") ||
                    cardName.Contains("onepiece") && !GameManager.instance.charInfo.onepiece.Contains("onepiece") && !GameManager.instance.charInfo.bottom.Contains("bottom") && !GameManager.instance.charInfo.top.Contains("top") ||
                    cardName.Contains("shoes") && !GameManager.instance.charInfo.shoes.Contains("shoes") ||
                    cardName.Contains("acc") && !GameManager.instance.charInfo.acc.Contains("acc"))
                    recObj(cardName);
                else
                    recObjPopup(cardName);

                GameManager.instance.saveCardInfo(cardName);
                GameManager.instance.instantiateSound(GameManager.instance.loadSound);
                Handheld.Vibrate();
            }
            
            else
            {
                
                GameManager.instance.availableRecognize = true;
            }
           

        }
        else if (GameManager.instance.charInfo.body.Contains("char") == false)
        {
            if (cardName.Contains("char"))
            {
                //recObjPopup(cardName);
                recObj(cardName);
                GameManager.instance.saveCardInfo(cardName);
                GameManager.instance.instantiateSound(GameManager.instance.loadSound);
                Handheld.Vibrate();
            }
            else
            {
                //test.text = "캐릭터카드 인식 필요";
                PopupManager.instance.showToast("캐릭터 카드를 먼저 보여주세요");
                GameManager.instance.availableRecognize = true;
            }
        }

        
    }

    /* private void OnTrackingFound()
     {

         if (GameManager.instance.charInfo.body.Contains("char"))
         {
             if (cardName.Contains("char") && cardName != GameManager.instance.charInfo.body)
             {
                 //캐릭터
                GameManager.instance.arCharacter.transform.localPosition = new Vector3(0, -0.8f, 15f);
                GameManager.instance.arCharacter.transform.localScale = new Vector3(200, 200, 200);
                GameManager.instance.arCharacter.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
                GameManager.instance.appearCharEffect.SetActive(false);
                //test.text = cardName;
                GameManager.instance.charInfo.body = cardName;
                GameManager.instance.changeChar(cardName);
                GameManager.instance.appearCharEffect.SetActive(true);


             }
             else if (cardName.Contains("top")/* && cardName != GameManager.instance.charInfo.top/)
             {
                 //상의

                 GameManager.instance.charInfo.onepiece = "0";
                 GameManager.instance.charInfo.top = cardName;
                 StartCoroutine(OnCloth("top", cardName));

                 GameManager.recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[cardName+"_img"];

                 if (GameManager.instance.charInfo.bottom.Contains("bottom") == false)
                 {
                     GameManager.recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
                     GameManager.instance.arCharacter.transform.Find("basic_bottom").gameObject.SetActive(true);
                 }
                 StartCoroutine(onParticle(GameManager.instance.topEffect));
             }
             else if (cardName.Contains("bottom")/* && cardName != GameManager.instance.charInfo.bottom)
             {
                 //하의
                 GameManager.instance.charInfo.bottom = cardName;
                 GameManager.instance.charInfo.onepiece = "0";

                 StartCoroutine(OnCloth("bottom", cardName));

                 GameManager.recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[cardName + "_img"];
                 if (GameManager.instance.charInfo.top.Contains("top") == false)
                 {
                     GameManager.recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                     GameManager.instance.arCharacter.transform.Find("basic_top").gameObject.SetActive(true);
                 }
                 StartCoroutine(onParticle(GameManager.instance.bottmEffect));

             }
             else if (cardName.Contains("onepiece")/* && cardName != GameManager.instance.charInfo.onepiece)
             {
                 //원피스
                 GameManager.instance.charInfo.onepiece = cardName;

                 GameManager.instance.charInfo.top = "0";
                 GameManager.instance.charInfo.bottom = "0";
                 StartCoroutine(OnCloth("onepiece", cardName));

                 GameManager.recognizeObj[0].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[cardName + "_img"];
                 GameManager.recognizeObj[1].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[cardName + "_img"];

                 StartCoroutine(onParticle(GameManager.instance.onepieceEffect));
             }
             else if (cardName.Contains("shoes")/* && cardName != GameManager.instance.charInfo.shoes)
             {
                 //신발
                 GameManager.instance.charInfo.shoes = cardName;
                 StartCoroutine(OnCloth("shoes", cardName));

                 GameManager.recognizeObj[2].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[cardName + "_img"];

                 StartCoroutine(onParticle(GameManager.instance.shoesEffect));
             }
             else if (cardName.Contains("acc")/* && cardName != GameManager.instance.charInfo.acc)
             {
                 //모자
                 GameManager.instance.charInfo.acc = cardName;
                 StartCoroutine(OnCloth("acc", cardName));

                 GameManager.recognizeObj[3].GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.dicClothImg[cardName + "_img"];

                 StartCoroutine(onParticle(GameManager.instance.accEffect));
             }
             //test.text = cardName;
             if (PlayerPrefs.GetString("guide") != "off")
             {
                 PopupManager.instance.showGuide();
             }
             GameManager.instance.saveCardInfo(cardName);
             GameManager.instance.instantiateSound(GameManager.instance.loadSound);
             Handheld.Vibrate();

         }
         else if (GameManager.instance.charInfo.body.Contains("char") == false)
         {
             if (cardName.Contains("char"))
             {
                 GameManager.instance.guideButtonUI.SetActive(false);
                 CharacterController.instance.resetTrans();
                 GameManager.charControlBtnUI.SetActive(true);
                 for (int i = 0; i < 4; i++)
                 {
                     GameManager.recognizeObj[i].SetActive(true);
                     switch (i)
                     {
                         case 0:
                             GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                             break;
                         case 1:
                             GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_onepiece");
                             break;
                         case 2:
                             GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_shoes");
                             break;
                         case 3:
                             GameManager.recognizeObj[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Button/icon_acc");
                             break;
                     }

                 }
                 GameManager.instance.character.SetActive(true);

                 //test.text = cardName;

                 GameManager.instance.charInfo.body = cardName;
                 GameManager.instance.changeChar(cardName);
                 GameManager.instance.offCharObj("top");
                 GameManager.instance.offCharObj("bottom");
                 GameManager.instance.offCharObj("onepiece");
                 GameManager.instance.offCharObj("shoes");
                 GameManager.instance.offCharObj("acc");
                 GameManager.canvasBottomButtonUI.transform.Find("btnSaveChar").transform.gameObject.SetActive(true);
                 GameManager.instance.effectCamera.SetActive(true);



                 GameManager.instance.arCharacter.transform.Find("basic_bottom").gameObject.SetActive(true);
                 GameManager.instance.arCharacter.transform.Find("basic_top").gameObject.SetActive(true);
                 if (PlayerPrefs.GetString("guide") != "off")
                 {
                     PopupManager.instance.showGuide();
                 }

                 //애니메이션 테스트
                 StartCoroutine(GameManager.instance.appearAni());

                 GameManager.instance.saveCardInfo(cardName);
                 GameManager.instance.instantiateSound(GameManager.instance.loadSound);
                 Handheld.Vibrate();
             }
             else
             {
                 //test.text = "캐릭터카드 인식 필요";
                 PopupManager.instance.showToast("캐릭터 카드를 먼저 보여줘");
             }
         }

         GameManager.instance.checkAnimation();
     }
     */
   
    IEnumerator OnCloth(string obj, string cardName)
    {
        yield return new WaitForSeconds(0.5f);
        if (obj.Contains("top"))
        {
            GameManager.instance.offCharObj("top");
            GameManager.instance.offCharObj("onepiece");
        }
        else if (obj.Contains("bottom"))
        {
            GameManager.instance.offCharObj("bottom");
            GameManager.instance.offCharObj("onepiece");
        }
        else if (obj.Contains("onepiece"))
        {
            GameManager.instance.offCharObj("onepiece");
            GameManager.instance.offCharObj("top");
            GameManager.instance.offCharObj("bottom");
        }
        else if (obj.Contains("shoes"))
        {
            GameManager.instance.offCharObj("shoes");
        }
        else if (obj.Contains("acc"))
        {
            GameManager.instance.offCharObj("acc");
        }
        GameManager.instance.onCloth(obj, cardName);
    }
    
    private void OnTrackingLost()
    {
        //test.text = "lost";
        //GameManager.instance.isTracking = false;    
        //charObj.SetActive(false);
    }
   
    public IEnumerator onParticle(ParticleSystem particle)
    {

        if (particle.name.Contains("top"))
        {
            GameManager.instance.effectTransform.localRotation = Quaternion.EulerAngles(new Vector3(0, 0, 0));
            GameManager.instance.effectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            if (GameManager.instance.charInfo.body.Contains("ari"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.005f, 0.06f, 0.39f);
            }
            else if (GameManager.instance.charInfo.body.Contains("min"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.005f, 0.06f, 0.39f);
            }
            else if (GameManager.instance.charInfo.body.Contains("shuel"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0.01f, 0.06f, 0.39f);
            }
            else
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0f, 0.06f, 0.39f);
            }
        }
        if (particle.name.Contains("bottom"))
        {
            GameManager.instance.effectTransform.localRotation = Quaternion.EulerAngles(new Vector3(-90, 0, 0));
            GameManager.instance.effectTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            if (GameManager.instance.charInfo.body.Contains("ari"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0, -0.007f, 0.35f);
            }
            else if (GameManager.instance.charInfo.body.Contains("min"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0, -0.007f, 0.35f);
            }
            else if (GameManager.instance.charInfo.body.Contains("shuel"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0.01f, 0f, 0.35f);
            }
            else
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0.01f, 0f, 0.35f);
            }
        }
        if (particle.name.Contains("onepiece"))
        {
            GameManager.instance.effectTransform.localRotation = Quaternion.EulerAngles(new Vector3(0, 0, 0));
            GameManager.instance.effectTransform.localScale = new Vector3(1, 1, 1);
            if (GameManager.instance.charInfo.body.Contains("ari"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.002f, 0.031f, 0.28f);
            }
            else if (GameManager.instance.charInfo.body.Contains("min"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.002f, 0.031f, 0.28f);
            }
            else if (GameManager.instance.charInfo.body.Contains("shuel"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0.01f, 0.035f, 0.28f);
            }
            else
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0f, 0.035f, 0.28f);
            }
        }
        if (particle.name.Contains("shoes"))
        {
            GameManager.instance.effectTransform.localRotation = Quaternion.EulerAngles(new Vector3(0, 0, 0));
            GameManager.instance.effectTransform.localScale = new Vector3(1, 1, 1);
            if (GameManager.instance.charInfo.body.Contains("ari"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.001f, -0.11f, 0.3f);
            }
            else if (GameManager.instance.charInfo.body.Contains("min"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0, -0.09f, 0.24f);
            }
            else if (GameManager.instance.charInfo.body.Contains("shuel"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0.02f, -0.14f, 0.4f);
            }
            else
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.01f, -0.14f, 0.4f);
            }
        }
        if (particle.name.Contains("acc"))
        {
            GameManager.instance.effectTransform.localRotation = Quaternion.EulerAngles(new Vector3(0, 0, 0));
            GameManager.instance.effectTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            if (GameManager.instance.charInfo.body.Contains("ari"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(-0.005f, 0.080f, 0.2f);
            }
            else if (GameManager.instance.charInfo.body.Contains("min"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0, 0.076f, 0.2f);
            }
            else if (GameManager.instance.charInfo.body.Contains("shuel"))
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0.01f, 0.082f, 0.23f);
            }
            else
            {
                GameManager.instance.effectTransform.localPosition = new Vector3(0, 0.078f, 0.2f);
            }
        }
        particle.transform.position = GameManager.instance.effectTransform.position;
        particle.transform.rotation = GameManager.instance.effectTransform.rotation;
        particle.transform.localScale = GameManager.instance.effectTransform.localScale;
        particle.gameObject.SetActive(false);
        particle.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        particle.gameObject.SetActive(false);
    }

    #endregion // PRIVATE_METHODS
}
