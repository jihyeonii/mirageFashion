using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

using TouchScript.Gestures;
public class CharacterInfo
{
    public string body;
    public string top;
    public string bottom;
    public string onepiece;
    public string shoes;
    public string acc;

    public CharacterInfo(string mBody, string mTop, string mBottom, string mOnepiece, string mShoes, string mAcc)
    {
        body = mBody;
        top = mTop;
        bottom = mBottom;
        onepiece = mOnepiece;
        shoes = mShoes;
        acc = mAcc;
    }

}
public class CharacterController : MonoBehaviour {
    public static CharacterController instance;
    CharState charState;

	public GameObject pos;
    public GameObject switchPos;

    public GameObject minXPos;
    public GameObject maxXPos;

    public GameObject minYPos;
    public GameObject maxYPos;
    public enum CharState
    {
        Basic,
        Turn,   //0 : 왼쪽, 1:basic 2: 오른쪽
        Scale,
        
    }
    int turnDirection;
    CameraSettings tmp;


    Vector3 testRotPos;
    int rotDirection;
    int clickCount = 0;
    float delayClickTime = 0f;
    bool doubleclick = true;

    public Transform trans;
    public void click(GameObject obj)
    {
        doubleclick = !doubleclick;
        if (doubleclick)
        {
            obj.transform.Find("Text").GetComponent<Text>().text = "doubleClick";
        }
        else
            obj.transform.Find("Text").GetComponent<Text>().text = "1click";
    }
    // Use this for initialization
    void Start () {
        instance = this;
        charState = CharState.Basic;
        //minXPos.transform.localPosition = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector3(50, Screen.height/2, pos.transform.position.z));
        //maxXPos.transform.localPosition = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width - 50, Screen.height/2, pos.transform.position.z));
        //minYPos.transform.localPosition = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, 100, pos.transform.position.z));
        //maxYPos.transform.localPosition = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height - 200, pos.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.uiState == GameManager.UIState.main || GameManager.instance.uiState == GameManager.UIState.paint || GameManager.instance.uiState == GameManager.UIState.save)
        {
            transform.GetComponent<TransformGesture>().enabled = false;
        }
        else if (GameManager.instance.uiState == GameManager.UIState.camera/* && GameManager.isPlayingAnimation*/)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray raycast = GameManager.instance.cameraMainUI.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                Ray raycast2 = GameManager.instance.charCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                RaycastHit hit2;
                //Debug.Log(raycast2.ToString());
                if (Physics.Raycast(raycast, out hit, Mathf.Infinity))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.gameObject.layer == 10)
                        transform.GetComponent<TransformGesture>().enabled = false;
                }
                else if (Physics.Raycast(raycast2, out hit2, Mathf.Infinity))
                {
                    if (hit2.transform.gameObject.layer == 11)
                        transform.GetComponent<TransformGesture>().enabled = true;
                }
            }

        }
        //회전
        if (charState == CharState.Turn)
        {
            switch (turnDirection)
            {
                case 0:
                    //if (tmp.IsFrontCameraActive())
                    GameManager.instance.character.transform.Rotate(new Vector3(0, 0, -5));
                    //else
                    //    GameManager.instance.character.transform.Rotate(new Vector3(0, 0, -5));
                    break;
                case 1:
                    //resetTrans();
                    GameManager.instance.character.transform.rotation = Quaternion.Euler(new Vector3(-90, 180, 0));
                    charState = CharState.Basic;
                    break;
                case 2:
                    //if (tmp.IsFrontCameraActive())
                    GameManager.instance.character.transform.Rotate(new Vector3(0, 0, 5));
                    //else
                    //    GameManager.instance.character.transform.Rotate(new Vector3(0, 0, 5));
                    break;
            }
        }

        //      
        //카메라viewport
        if (this.gameObject.transform.localScale.x > 2.5f)
        {
            gameObject.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
        else if (gameObject.transform.localScale.x < 0.6f)
        {
            gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
        Vector3 characterPos = GameManager.instance.charCamera.GetComponent<Camera>().WorldToScreenPoint(gameObject.transform.position);
        tmp = FindObjectOfType<CameraSettings>();
        Vector2 charSize = new Vector2(gameObject.transform.Find("character").GetComponent<Transform>().localScale.x, gameObject.transform.Find("character").GetComponent<Transform>().localScale.y);

        if (GameManager.instance.charInfo.body != "0")
        {

            if (characterPos.x <= charSize.x / 3)
            {
                //if (tmp.IsFrontCameraActive())
                //{
                //    //        Vector3 pos;
                //    //        pos = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, characterPos.y, 80));
                //    //        pos.z = 80;
                //    //        gameObject.transform.localPosition = pos;
                //    //        //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * (gameObject.GetComponent<RectTransform>().sizeDelta.x * gameObject.transform.localScale.x - (720 * 0.5f)), gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                //}
                //else
                //{

                Vector3 charScreenPos = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(characterPos);
                charScreenPos.x = minXPos.transform.localPosition.x;
                charScreenPos.y = gameObject.transform.localPosition.y;
                charScreenPos.z = pos.transform.localPosition.z;
                //float testX = charScreenPos
                //charScreenPos.z = 80 + GameManager.instance.cameraAR.transform.position.z;
                gameObject.transform.localPosition = charScreenPos;
                //}

            }
            else if (characterPos.x >= Screen.width - (charSize.x / 3))
            {
                //if (tmp.IsFrontCameraActive())
                //{
                //    //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x * gameObject.transform.localScale.x - (720 * 0.5f), gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                //}
                //else
                //{
                Vector3 charScreenPos = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(characterPos);
                charScreenPos.x = maxXPos.transform.localPosition.x;
                charScreenPos.y = gameObject.transform.localPosition.y;
                charScreenPos.z = pos.transform.localPosition.z;
                gameObject.transform.localPosition = charScreenPos;
                //}
            }
            if (characterPos.y <= -charSize.y * 4)
            {
                //if (tmp.IsFrontCameraActive())
                //{
                //    Vector3 charScreenPos = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(characterPos);
                //    charScreenPos.x = gameObject.transform.localPosition.x;
                //    charScreenPos.y = -minYPos.transform.localPosition.y;
                //    charScreenPos.z = pos.transform.localPosition.z;
                //    gameObject.transform.localPosition = charScreenPos;
                //}
                //else
                //{
                Vector3 charScreenPos = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(characterPos);
                charScreenPos.x = gameObject.transform.localPosition.x;
                charScreenPos.y = minYPos.transform.localPosition.y;
                charScreenPos.z = pos.transform.localPosition.z;
                gameObject.transform.localPosition = charScreenPos;
                //}
            }
            else if (characterPos.y >= Screen.height - charSize.y / 2)
            {
                //if (tmp.IsFrontCameraActive())
                //{
                //    Vector3 charScreenPos = GameManager.instance.cameraAR.transform.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(characterPos);
                //    charScreenPos.x = gameObject.transform.localPosition.x;
                //    charScreenPos.y = -maxYPos.transform.localPosition.y;
                //    charScreenPos.z = pos.transform.localPosition.z;
                //    gameObject.transform.localPosition = charScreenPos;
                //}
                //else
                //{
                Vector3 charScreenPos = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(characterPos);
                charScreenPos.x = gameObject.transform.localPosition.x;
                charScreenPos.y = maxYPos.transform.localPosition.y;
                charScreenPos.z = pos.transform.localPosition.z;
                gameObject.transform.localPosition = charScreenPos;

            }
        }
        minXPos.transform.position = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(charSize.x / 3, Screen.height / 2, pos.transform.localPosition.z));
        maxXPos.transform.position = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width - (charSize.x / 3), Screen.height / 2, pos.transform.localPosition.z));
        minYPos.transform.position = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, -charSize.y * 4, pos.transform.localPosition.z));
        maxYPos.transform.position = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height - charSize.y / 2, pos.transform.localPosition.z));

        if (((GameManager.instance.uiState == GameManager.UIState.main && GameManager.instance.availableRecognize && GameManager.instance.isPopup == false) || GameManager.instance.uiState == GameManager.UIState.camera) && GameManager.instance.isPopup == false)
        {

            if (Input.GetMouseButtonDown(0))
            {
                Ray raycast = GameManager.instance.charCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(raycast, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("top") || hit.transform.name.Contains("bottom") || hit.transform.name.Contains("shoes") || hit.transform.name.Contains("acc"))
                    {
                        if (doubleclick)
                        {
                            if (clickCount == 0)
                            {
                                clickCount = 1;
                            }
                            else
                            {
                                Vector3 pos;
                                trans.position = GameManager.instance.charCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,1));
                                //pos.z = 1;
                                //Debug.Log(trans.position);
                                //trans.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                                onEffect(trans);
                                //StartCoroutine(onEffect(GameManager.instance.charInfo));
                                
                                //if (hit.transform.name.Contains("top"))
                                //{
                                //    if (GameManager.instance.charInfo.top.Contains("top"))
                                //    {
                                //        StartCoroutine(onParticle(gameObject.transform.Find("top").GetChild(0).GetComponent<ParticleSystem>()));
                                //    }
                                //    else if (GameManager.instance.charInfo.onepiece.Contains("onepiece"))
                                //    {
                                //        StartCoroutine(onParticle(gameObject.transform.Find("onepiece").GetChild(0).GetComponent<ParticleSystem>()));
                                //    }
                                //}
                                //else if (hit.transform.name.Contains("bottom"))
                                //{
                                //    if (GameManager.instance.charInfo.bottom.Contains("bottom"))
                                //    {
                                //        StartCoroutine(onParticle(gameObject.transform.Find("bottom").GetChild(0).GetComponent<ParticleSystem>()));
                                //    }
                                //    else if (GameManager.instance.charInfo.onepiece.Contains("onepiece"))
                                //    {
                                //        StartCoroutine(onParticle(gameObject.transform.Find("onepiece").GetChild(0).GetComponent<ParticleSystem>()));
                                //    }
                                //}
                                //else if (hit.transform.name.Contains("shoes") && GameManager.instance.charInfo.shoes.Contains("shoes"))
                                //{
                                //    StartCoroutine(onParticle(gameObject.transform.Find("shoes").GetChild(0).GetComponent<ParticleSystem>()));
                                //}
                                //else if (hit.transform.name.Contains("acc") && GameManager.instance.charInfo.acc.Contains("acc"))
                                //{
                                //    StartCoroutine(onParticle(gameObject.transform.Find("acc").GetChild(0).GetComponent<ParticleSystem>()));
                                //}
                            }
                        }

                        else
                        {
                            StartCoroutine(onEffect(GameManager.instance.charInfo));
                        }
                    }
                }
            }
        }
        if (doubleclick)
        {
            if (clickCount == 1)
            {
                delayClickTime += Time.smoothDeltaTime;
            }
            if (delayClickTime > 0.3f)
            {
                clickCount = 0;
                delayClickTime = 0f;
            }
        }

    }
    public void onEffect(Transform pos)
    {
        GameManager.instance.instantiateSound(GameManager.instance.loadSound);
        GameObject effect;
        effect = Instantiate(GameManager.instance.clickEffect, pos);
        effect.layer = 11;
        effect.transform.parent = GameManager.instance.charCamera.transform;
    }
    IEnumerator onEffect(CharacterInfo info)
    {
        GameManager.instance.instantiateSound(GameManager.instance.loadSound);
        Shader shader = Shader.Find("Mobile/Diffuse");
        
        Color color = Color.white;
        if (info.top.Contains("top"))
        {
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.top).GetComponent<Renderer>().material.shader = shader;
            //GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.top).GetComponent<Renderer>().material.color = color;
        }
        if (info.bottom.Contains("bottom"))
        {
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.bottom).GetComponent<Renderer>().material.shader = shader;
            //GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.bottom).GetComponent<Renderer>().material.color = color;
        }
        if (info.onepiece.Contains("onepiece"))
        {
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.onepiece).GetComponent<Renderer>().material.shader = shader;
           // GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.onepiece).GetComponent<Renderer>().material.color = color;
        }
        if (info.shoes.Contains("shoes"))
        {
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.shoes).GetComponent<Renderer>().material.shader = shader;
           // GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.shoes).GetComponent<Renderer>().material.color = color;
        }
        if (info.acc.Contains("acc"))
        {
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.acc).GetComponent<Renderer>().material.shader = shader;
            //GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.acc).GetComponent<Renderer>().material.color = color;
        }
        yield return new WaitForSeconds(0.3f);
        if (info.top.Contains("top"))
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.top).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
        if (info.bottom.Contains("bottom"))
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.bottom).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
        if (info.onepiece.Contains("onepiece"))
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.onepiece).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
        if (info.shoes.Contains("shoes"))
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.shoes).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
        if (info.acc.Contains("acc"))
            GameManager.instance.arCharacter.transform.Find(GameManager.instance.charInfo.acc).GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
    }
    IEnumerator onParticle(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        particle.gameObject.SetActive(false);
    }
    //public void OnPointerDown(PointerEventData ped)
    //{
    //}
    //public void OnPointerUp(PointerEventData ped)
    //{
    //}
    //public float getDis(float size)
    //{
    //    float dis;
    //    size %
    //    return dis;
    //}
    public void btnTurn(int dir)
    {
        charState = CharState.Turn;
        turnDirection = dir;
    }
    public void btnTurnUp()
    {
        charState = CharState.Basic;
    }
    public void btnTestRot()
    {
        Debug.Log("down");
        testRotPos = GameManager.instance.cameraMainUI.GetComponent<Camera>().WorldToScreenPoint(Input.mousePosition);
        
    }
    public void resetTrans()
    {

        if (GameManager.instance.uiState == GameManager.UIState.main)
        {
            GameManager.charControlBtnUI.SetActive(false);
        }
        gameObject.transform.localScale = pos.transform.localScale;
        //this.gameObject.transform.rotation = pos.transform.rotation;
        
            gameObject.transform.localPosition = pos.transform.localPosition;
            gameObject.transform.rotation = pos.transform.rotation;

    }
   
    
}
