using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PaintManager: MonoBehaviour , IPointerDownHandler {
    public static PaintManager instance;
    public GameObject parentPaint;

    public static GameObject penLayout;
    //public static GameObject stickerLayout;
    public static GameObject textLayout;
    public static GameObject mainLayout;

    public static GameObject optionLayout;
    
    public static int colorState = 0;
    public static int fontState = 0; 

    public static int penState = 0;

    public Font font1;
    public InputField inputField;
    string inputText ;
    public Text text;

    //lineRenderer
    public GameObject linePrefab;
    LineRenderer lineRenderer;

    bool isMousePressed;
    public static int objCount = 0;
    GameObject line;
    List<Vector3> drawPoint = new List<Vector3>();
    public static List<GameObject> paintList;

    public GameObject paintLayout;
    GameObject closeBtn;

     int PEN_TOOL = 0;
     int TEXT_TOOL = 1;

    //
    Vector2 preDistance;
    Vector2 curDistance;

    Vector3 distanceMousePos;
    Vector3 clickPos;
    Vector3 textPos;

    float delayTime = 0;
    bool isMove = false;
    GameObject obj;
    string msg = "";
    public enum txtTransformState
    {
        none, move, expand, reduce
    }

    public Text test;
    // Use this for initialization
    void Start () {
        paintList = new List<GameObject>();
        instance = this;
        penLayout = paintLayout.transform.Find("DrawPenLayout").gameObject;
        textLayout = paintLayout.transform.Find("DrawTextLayout").gameObject;
        mainLayout = paintLayout.transform.Find("MainLayout").gameObject;
        optionLayout = textLayout.transform.Find("Option").gameObject;
        isMousePressed = false; 
	}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.paintState == GameManager.PaintState.text || GameManager.instance.paintState == GameManager.PaintState.none)
        {
            Ray ray1, ray2;
            RaycastHit hitinfo;

            if (Input.touchCount > 0)
            {
                if (delayTime < 0.2f)
                {

                    if (Input.touchCount > 1)
                    {
                        //scale
                        ray1 = GameManager.instance.cameraPaintUI.GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
                        ray2 = GameManager.instance.cameraPaintUI.GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(1).position);
                        if (Physics.Raycast(ray1, out hitinfo, Mathf.Infinity))
                        {
                            if (hitinfo.transform.tag == "Text")
                            {
                                preDistance = (Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
                                curDistance = Input.GetTouch(0).position - Input.GetTouch(1).position;
                                float delta = curDistance.magnitude - preDistance.magnitude;

                                if (delta < 0)
                                {
                                    GameManager.instance.testT.text = "축소";
                                    Vector3 scale = hitinfo.transform.parent.transform.localScale;
                                    if (scale.x + (delta * 0.07f) > 0.7)
                                    {
                                        scale.x += (delta * 0.07f);
                                        scale.y += (delta * 0.07f);
                                        //scale.z += (delta * 0.07f);
                                        hitinfo.transform.parent.transform.localScale = scale;
                                    }
                                }
                                else if (delta > 0)
                                {
                                    GameManager.instance.testT.text = "확대";
                                    Vector3 scale = hitinfo.transform.parent.transform.localScale;
                                    if (scale.x + (delta * 0.07f) < 4)
                                    {
                                        scale.x += (delta * 0.07f);
                                        scale.y += (delta * 0.07f);
                                        //scale.z += (delta * 0.07f);
                                        hitinfo.transform.parent.transform.localScale = scale;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        delayTime += Time.deltaTime;
                    }
                }
                else
                {
                    //test.text = delayTime.ToString();
                    switch (Input.GetTouch(0).phase)
                    {
                        case TouchPhase.Began:
                            isMove = true;
                            //test.text = "시작";
                            obj = TouchUICheck(1);
                            textPos = GameManager.instance.cameraPaintUI.GetComponent<Camera>().WorldToScreenPoint(obj.transform.position);
                            textPos.z = 0;
                            distanceMousePos = textPos - Input.mousePosition;
                            //test.text = "textPos : " + textPos.ToString() + ", distance : " + distanceMousePos.ToString();
                            break;
                        case TouchPhase.Moved:
                            if (isMove == false)
                                goto case TouchPhase.Began;
                            else
                            {
                                Vector3 pos = GameManager.instance.cameraPaintUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0) + distanceMousePos);

                                pos.z = 100 - ((float)objCount) / 10;
                                obj.gameObject.transform.position = pos;
                                //test.text = obj.transform.position + ", " + distanceMousePos;
                                Vector3 viewport = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().WorldToScreenPoint(pos);
                                if (viewport.x < 30 || viewport.x > Screen.width - 30 || viewport.y < 30 || viewport.y > Screen.height - 30)
                                {
                                    Destroy(obj.gameObject);
                                    objCount--;
                                }
                            }

                            break;
                        case TouchPhase.Ended:
                            isMove = false;
                            delayTime = 0f;
                            //test.text = "끝";
                            break;
                    }
                }
            }
        }
        if (GameManager.instance.paintState == GameManager.PaintState.text || GameManager.instance.paintState == GameManager.PaintState.pen)
        {
            Color color0 = new Color(1, 1, 1); 
            GameObject.Find("btnColor0").GetComponent<Image>().color = color0;

            Color color1 = new Color(0, 0, 0);
            GameObject.Find("btnColor1").GetComponent<Image>().color = color1;

            Color color2 = new Color(1, 0.5f, 0.9f);
            GameObject.Find("btnColor2").GetComponent<Image>().color = color2;

            Color color3 = new Color(1, 0, 0);
            GameObject.Find("btnColor3").GetComponent<Image>().color = color3;

            Color color4 = new Color(1, 0.5f, 0); 
            GameObject.Find("btnColor4").GetComponent<Image>().color = color4;

            Color color5 = new Color(1, 1, 0);
            GameObject.Find("btnColor5").GetComponent<Image>().color = color5;

            Color color6 = new Color(0, 1, 0);
            GameObject.Find("btnColor6").GetComponent<Image>().color = color6;

            Color color7 = new Color(0, 0, 1);
            GameObject.Find("btnColor7").GetComponent<Image>().color = color7;

            Color color8 = new Color(0.05f, 0.05f, 0.4f);
            GameObject.Find("btnColor8").GetComponent<Image>().color = color8;

            Color color9 = new Color(0.5f, 0, 0.5f);
            GameObject.Find("btnColor9").GetComponent<Image>().color = color9;

           


            if (GameManager.instance.paintState == GameManager.PaintState.text)
            {
                Color colorInputTxt = inputField.transform.Find("Text").GetComponent<Text>().color;
                Color colorText = text.GetComponent<Text>().color;
                switch (colorState)
                {
                    case 0:
                        colorInputTxt = new Color(1, 1, 1);
                        colorText = new Color(1, 1, 1);

                        break;
                    case 1:
                        colorInputTxt = new Color(0, 0, 0);
                        colorText = new Color(0, 0, 0);
                        break;

                    case 2:
                        colorInputTxt = new Color(1, 0.5f, 0.9f);
                        colorText = new Color(1, 0.5f, 0.9f);
                        break;
                    case 3:
                        colorInputTxt = new Color(1, 0, 0);
                        colorText = new Color(1, 0, 0);
                        break;
                    case 4:
                        colorInputTxt = new Color(1, 0.5f, 0);
                        colorText = new Color(1, 0.5f, 0);
                        break;
                    case 5:
                        colorInputTxt = new Color(1, 1, 0);
                        colorText = new Color(1, 1, 0);
                        break;
                    case 6:
                        colorInputTxt = new Color(0, 1, 0);
                        colorText = new Color(0, 1, 0);
                        break;
                    case 7:
                        colorInputTxt = new Color(0, 0, 1);
                        colorText = new Color(0, 0, 1);
                        break;
                    case 8:
                        colorInputTxt = new Color(0.05f, 0.05f, 0.4f);
                        colorText = new Color(0.05f, 0.05f, 0.4f);
                        break;
                    case 9:
                        colorInputTxt = new Color(0.5f, 0, 0.5f);
                        colorText = new Color(0.5f, 0, 0.5f);
                        break;
                }
                inputField.transform.Find("Text").GetComponent<Text>().color = colorInputTxt;
                text.GetComponent<Text>().color = colorText;
                
                if (TouchScreenKeyboard.visible)
                {
					#if UNITY_ANDROID
                    float keyboardHeight = GetKeyboardSize();
                    Vector3 pos = optionLayout.transform.position;
                   
                    pos.y = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(-360, keyboardHeight + 150, 0)).y;
                    //textLayout.transform.Find("Text").GetComponent<Text>().text = keyboardHeight.ToString() ;
                    optionLayout.transform.position = pos;
					#endif
                    optionLayout.transform.parent.Find("btnComplete").gameObject.SetActive(false);
                    //Vector3 pos = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(-360, keyboardHeight + 150,0));
                    //pos.z = 100;
                    //optionLayout.transform.position = pos;
                    msg = inputField.text;
                }
                else if (TouchScreenKeyboard.visible == false)
                {
                    Debug.Log(optionLayout.transform.position + ", " + GameManager.instance.cameraPaintUI.GetComponent<Camera>().WorldToScreenPoint(optionLayout.transform.position));
                    Vector3 pos = optionLayout.transform.position;
                    pos.y = GameManager.instance.cameraPaintUI/*.transform.find("camera")*/.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 90)).y;
                    optionLayout.transform.parent.Find("btnComplete").gameObject.SetActive(true);
                    //pos.z = 90;
                    optionLayout.transform.position = pos;
                    inputField.text = msg;
                }
            }

            if (GameManager.instance.paintState == GameManager.PaintState.pen)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //2D raycast
                    GraphicRaycaster gr = this.GetComponent<GraphicRaycaster>();
                    PointerEventData ped = new PointerEventData(null);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    gr.Raycast(ped, results);
                    if (results.Count > 0)
                    {
                        //UI 
                    }
                    //if (EventSystem.current.IsPointerOverGameObject() == false)
                    if (results.Count == 0 || (results.Count > 0 && results[0].gameObject.name.Contains("text")))
                    {  //UI이 위가 아니면.
                        objCount++;

                        isMousePressed = true;
                        line = Instantiate(linePrefab) as GameObject;
                        line.transform.parent = parentPaint.transform;
                        lineRenderer = line.GetComponent<LineRenderer>();

                        Material whiteDiffuseMat = new Material(Shader.Find("Custom/ColoredLines"));

                        switch (colorState)
                        {
                            case 0:
                                whiteDiffuseMat.color = new Color(1,1,1);
                                break;
                            case 1:
                                whiteDiffuseMat.color = new Color(0,0,0);
                                break;
                            case 2:
                                whiteDiffuseMat.color = new Color(1,0.5f,0.9f);
                                break;
                            case 3:
                                whiteDiffuseMat.color = new Color(1,0,0);
                                break;
                            case 4:
                                whiteDiffuseMat.color = new Color(1, 0.5f,0);
                                break;
                            case 5:
                                whiteDiffuseMat.color = new Color(1,1,0);
                                break;
                            case 6:
                                whiteDiffuseMat.color = new Color(0,1,0);
                                break;
                            case 7:
                                whiteDiffuseMat.color = new Color(0,0,1);
                                break;
                            case 8:
                                whiteDiffuseMat.color = new Color(0.05f,0.05f,0.4f);
                                break;
                            case 9:
                                whiteDiffuseMat.color = new Color(0.5f,0,0.5f);
                                break;

                        }

                        lineRenderer.material = whiteDiffuseMat;

                        switch (penState)
                        {
                            case 0:
                                lineRenderer.SetWidth(0.4f, 0.4f);
                                break;
                            case 1:
                                lineRenderer.SetWidth(0.8f, 0.8f);
                                break;
                            case 2:
                                lineRenderer.SetWidth(1.6f, 1.6f);
                                break;
                            case 3:
                                lineRenderer.SetWidth(3.2f, 3.2f);
                                break;
                        }

                        lineRenderer.SetVertexCount(500);
                    }
                    else
                    {
                        //ui
                    }

                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isMousePressed = false;
                    drawPoint.Clear();
                }
                if (isMousePressed)
                {
                    Vector3 mousePos = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = 100 - ((float)objCount) / 10;
                    if (!drawPoint.Contains(mousePos))
                    {
                        drawPoint.Add(mousePos);
                        lineRenderer.SetVertexCount(drawPoint.Count);
                        lineRenderer.SetPosition(drawPoint.Count - 1, mousePos);
                    }
                }
            }
            
            else
            {
                isMousePressed = false;
                drawPoint.Clear();
            }

        }
    }
    GameObject TouchUICheck(int touchCount)
    {
        for (int i = 0; i < touchCount; i++)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.GetTouch(i).position;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            if (raycastResults.Count > 0)
            {
                for (int j = 0; j < raycastResults.Count; j++)
                {
                    int layer = raycastResults[j].gameObject.layer;
                    if (layer == 16) //unity default UI layer == 5
                    {
                        return raycastResults[j].gameObject;
                    }
                }
            }
        }

        return null;

    }
    public void OnPointerDown(PointerEventData ped)
    {
    }
    public void OnPointerUp(PointerEventData ped)
    {
    }
	#if UNITY_ANDROID
    public int GetKeyboardSize()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);
                textLayout.transform.Find("Text").GetComponent<Text>().text =  Screen.height + ", "+Rct.Call<int>("height").ToString();
                return Screen.height - Rct.Call<int>("height");
            }
        }
    }
	#endif
    public void btnSelectTools(int i)
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        switch (i)
        {
            case 0:
                penLayout.SetActive(true);
                colorState = 2;
                GameManager.instance.paintState = GameManager.PaintState.pen;
                penLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + colorState).Find("Image").gameObject.SetActive(true);
                for (int j = 0; j < 10; j++)
                {
                    penLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + j).Find("Image").gameObject.SetActive(false);
                }
                penLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + colorState).Find("Image").gameObject.SetActive(true);
                penLayout.transform.Find("Option").Find("ThickUI").Find("btnThick" + penState).Find("Image").Find("Image").gameObject.SetActive(true);
                break;
            case 1:
                colorState = 0;
                textLayout.SetActive(true);
                GameManager.instance.paintState = GameManager.PaintState.text;
                for (int j = 0; j < 10; j++)
                {
                    textLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + j).Find("Image").gameObject.SetActive(false);
                }
                textLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + colorState).Find("Image").gameObject.SetActive(true);
                break;
        }
        mainLayout.SetActive(false);
    }
    public void clickText(GameObject obj)
    {
        textPos = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().WorldToScreenPoint(obj.transform.position);
        textPos.z = 0;
        distanceMousePos = textPos - Input.mousePosition;
        Debug.Log(textPos + ", " + Input.mousePosition);
    }
    public void moveText(GameObject obj)
    {
        Vector3 pos = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition + distanceMousePos) ;

        pos.z = 100 - ((float)objCount) / 10;
        obj.transform.position = pos;
        Vector3 viewport = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().WorldToScreenPoint(pos);
        if (viewport.x < 30 || viewport.x > Screen.width - 30 || viewport.y < 30 || viewport.y > Screen.height - 30)
        {
            Destroy(obj);
            objCount--;
        }
    }
   
    public void btnCompleteTxt()
    {
        msg = "";
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        if (inputField.text != "" && inputField.text != null)
        {
            objCount++;
            inputText = inputField.text;
            text.text = inputText;
            Vector3 pos = GameManager.instance.cameraPaintUI/*.transform.Find("Camera")*/.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));
            pos.z = 100 - ((float)objCount) / 10;
            GameObject obj = Instantiate(text.gameObject, pos, text.transform.rotation) as GameObject;
            obj.transform.parent = parentPaint.transform;
            float scale = 1f;
            Vector3 textScale = obj.transform.localScale;

            textScale.x = scale;
            textScale.y = scale;
            textScale.z = scale;

            obj.transform.localScale = textScale;

            GameManager.instance.goPaintScene();
        }
        else
        {
            GameManager.instance.goPaintScene();
        }
    }
    public void btnChangeColor(int i)
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        colorState = i;
        if(GameManager.instance.paintState == GameManager.PaintState.pen)
        {
            for(int j = 0; j<10; j++)
            {
                penLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + j).Find("Image").gameObject.SetActive(false);
            }
            penLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + i).Find("Image").gameObject.SetActive(true);
        }
        else
        {
            for (int j = 0; j < 10; j++)
            {
                textLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + j).Find("Image").gameObject.SetActive(false);
            }
            textLayout.transform.Find("Option").Find("ColorUI").Find("color").Find("btnColor" + i).Find("Image").gameObject.SetActive(true);
        }
    } 
    public void btnChangeFont(int i)
    {
        fontState = i;
        

    }
    public void btnChangeThick(int i)
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        penState = i;
        for (int j = 0; j < 4; j++)
        {
            penLayout.transform.Find("Option").Find("ThickUI").Find("btnThick" + j).Find("Image").Find("Image").gameObject.SetActive(false);
        }
        penLayout.transform.Find("Option").Find("ThickUI").Find("btnThick" + i).Find("Image").Find("Image").gameObject.SetActive(true);
    }
    public void btnEraser()
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        paintList = new List<GameObject>();
        if(parentPaint.transform.childCount > 0)
        {
            for (int i = 0; i < parentPaint.transform.childCount; i++)
            {
                if (parentPaint.transform.GetChild(i).name.Contains("line"))
                    paintList.Add(parentPaint.transform.GetChild(i).gameObject);
                
            }
            if(paintList.Count > 0)
            {
                Destroy(paintList[paintList.Count - 1].gameObject);
                paintList.Remove(paintList[paintList.Count - 1].gameObject);
                objCount--;
            }
        }
        
    }
    public void btnSaveScene()
    {
        GameManager.instance.paintState = GameManager.PaintState.none;
        paintLayout.SetActive(false);
        StartCoroutine(GameManager.instance.takePicture());
        GameManager.instance.uiState = GameManager.UIState.save;
    }
    
   
}