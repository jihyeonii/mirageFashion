using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Threading;

public class FashionRecognition : MonoBehaviour
{
    Camera fashionCamera;
    public static FashionRecognition instance;
    WebCamDevice[] devices;
    WebCamTexture webCam;
    public static GameObject canvasFashion;
    public GameObject capturePlane;
    public GameObject imagePlane;    //갤러리 이미지
    public GameObject categoryObj;   //성인,아동 선택
    byte[] imageByte;

    public string myFilename;
    string myFolderLocation;
    string myScreenshotLocation;
    string myDefaultLocation;

    float screenHeight;
    float screenWidth;


    public GameObject option;



    internal Boolean socket_ready = false;
    internal Boolean mIsRecv = false;

    TcpClient tcp_socket;
    NetworkStream net_stream;

    Thread write;

    public String host;
    public Int32 port;

    string received_data = "";

    int longSize = sizeof(long);

    long lCount;

    long lCloth;
    long lCloth1;
    long lColor;
    long lColor1;

    long lXPos;
    long lYPos;
    long lWidth;
    long lHeight;

    public static byte[] img_bytes;

    public string[] colorArray;
    public string[] clothArray;

    public string[] clothArray_k;
    public string[] sClothArray_k;

    public int[] colorIndex;
    public int[] clothIndex;
    public int[] cloth_kIndex;
    byte[] receivedData = new byte[1024];

    public static long nKey;

    public string category = "kid";

    GameObject cropBox;
    GameObject cropBtn;

    Vector3 orignPos;
    Vector3 touchPos;
    Vector3 distansPos;

    RectTransform cropTrans;
    RectTransform orignCropTrans;


    RectTransform newCropTrans;
    float y;
    float x;

    public RectTransform cropRectPos;

    // Use this for initialization
    void Start()
    {
        fashionCamera = GameObject.Find("FashionCamera").GetComponent<Camera>();
        host = "112.172.129.16";
        port = 8004;

        colorArray = new string[11];
        clothArray = new string[19];
        sClothArray_k = new string[13];
        clothArray_k = new string[13];
        colorIndex = new int[11];
        clothIndex = new int[19];
        cloth_kIndex = new int[13];

        colorArray[0] = "black";
        colorArray[1] = "blue";
        colorArray[2] = "brown";
        colorArray[3] = "gray";
        colorArray[4] = "green";
        colorArray[5] = "orange";
        colorArray[6] = "pink";
        colorArray[7] = "purple";
        colorArray[8] = "red";
        colorArray[9] = "white";
        colorArray[10] = "yellow";

        colorIndex[0] = 1;
        colorIndex[1] = 8192;
        colorIndex[2] = 32;
        colorIndex[3] = 4;
        colorIndex[4] = 2048;
        colorIndex[5] = 64;
        colorIndex[6] = 32768;
        colorIndex[7] = 131072;
        colorIndex[8] = 16;
        colorIndex[9] = 8;
        colorIndex[10] = 256;

        clothArray[0] = "바지";
        clothArray[1] = "스커트";
        clothArray[2] = "카디건";
        clothArray[3] = "재킷";
        clothArray[4] = "원피스";
        clothArray[5] = "코트";
        clothArray[6] = "점퍼";
        clothArray[7] = "후드티";
        clothArray[8] = "티셔츠";
        clothArray[9] = "청바지";
        clothArray[10] = "원피스";
        clothArray[11] = "바지";
        clothArray[12] = "바지";
        clothArray[13] = "니트/스웨커";
        clothArray[14] = "블라우스/셔츠";
        clothArray[15] = "바지";
        clothArray[16] = "원피스";
        clothArray[17] = "탱크탑";
        clothArray[18] = "점퍼";

        clothIndex[0] = 50000810;
        clothIndex[1] = 50000808;
        clothIndex[2] = 50000806;
        clothIndex[3] = 50000815;
        clothIndex[4] = 50000807;
        clothIndex[5] = 50000813;
        clothIndex[6] = 50000814;
        clothIndex[7] = 0;
        clothIndex[8] = 50000803;
        clothIndex[9] = 50000809;
        clothIndex[10] = 50000807;
        clothIndex[11] = 50000810;
        clothIndex[12] = 50000810;
        clothIndex[13] = 50000805;
        clothIndex[14] = 50000804;
        clothIndex[15] = 50000810;
        clothIndex[16] = 50000807;
        clothIndex[17] = 0;
        clothIndex[18] = 50000814;

        clothArray_k[0] = "블라우스";
        clothArray_k[1] = "카디건";
        clothArray_k[2] = "원피스";
        clothArray_k[3] = "원피스";
        clothArray_k[4] = "점퍼";
        clothArray_k[5] = "니트/스웨터";
        clothArray_k[6] = "셔츠/남방";
        clothArray_k[7] = "티셔츠";
        clothArray_k[8] = "스커트";
        clothArray_k[9] = "코트";
        clothArray_k[10] = "폴로티";
        clothArray_k[11] = "청바지";
        clothArray_k[12] = "재킷";

        sClothArray_k[0] = "blouse_top";
        sClothArray_k[1] = "cardigan_top";
        sClothArray_k[2] = "onepiece_onepiece";
        sClothArray_k[3] = "onepieceB_onepiece";
        sClothArray_k[4] = "jumper_top";
        sClothArray_k[5] = "sweater_top";
        sClothArray_k[6] = "shirts_top";
        sClothArray_k[7] = "tshirt_top";
        sClothArray_k[8] = "skirt_bottom";
        sClothArray_k[9] = "coat_top";
        sClothArray_k[10] = "polo_top";
        sClothArray_k[11] = "jeans_bottom";
        sClothArray_k[12] = "jacket_top";

        cloth_kIndex[0] = 50000749;
        cloth_kIndex[1] = 50000751;
        cloth_kIndex[2] = 50000752;
        cloth_kIndex[3] = 50000752;
        cloth_kIndex[4] = 50000759;
        cloth_kIndex[5] = 50000750;
        cloth_kIndex[6] = 50000748;
        cloth_kIndex[7] = 50000747;
        cloth_kIndex[8] = 50000753;
        cloth_kIndex[9] = 50000758;
        cloth_kIndex[10] = 0;
        cloth_kIndex[11] = 50000754;
        cloth_kIndex[12] = 50000760;

        category = "kid";

        GameManager.instance.onClothCount = 0;
        canvasFashion = GameObject.Find("FashionCamera").transform.Find("CanvasFashion").gameObject;
        cropBox = canvasFashion.transform.Find("cropBox").gameObject;
        cropBtn = canvasFashion.transform.Find("cropBtn").gameObject;
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        Screen.SetResolution(Screen.width, Screen.height, true);

        option = canvasFashion.transform.Find("option").gameObject;
        //Debug.Log("w : " + Screen.width + ", h : " + Screen.height);
        //capturePlane.gameObject.transform.localScale = new Vector3(720 / 10, 0.1f, (720 / screenWidth) * screenHeight / 10);


    }

    // Update is called once per frame
    void Update()
    {
        

        if (socket_ready)
            readSocket();

        if (mIsRecv)
        {
            recv_parsing();

            mIsRecv = false;
            Thread.Sleep(500);
            closeSocket();

        }


    }
    public void setupSocket()
    {
        if (!socket_ready)
        {
            if (category != "")
            {
                if (category.Contains("kid"))
                {
                    port = 8005;
                }
                else if (category.Contains("adult"))
                {
                    port = 8004;
                }
                try
                {
                    Debug.Log("connect");
                    tcp_socket = new TcpClient(host, port);

                    Debug.Log("1");
                    net_stream = tcp_socket.GetStream();
                    Debug.Log("2");

                    socket_ready = true;

                    tcp_socket.SendTimeout = 1000 * 60;
                    tcp_socket.SendBufferSize = 1024;
                    Debug.Log("connected");

                    send();
                }
                catch (Exception e)
                {
                    // Something went wrong
                    Debug.Log("Socket error: " + e);
                    //isSend = false;
                }
            }
            else
            {
                //popup.SetActive(true);
                //popup.transform.Find("popup").Find("naver").gameObject.SetActive(false);
                //popup.transform.Find("popup").Find("Button").localPosition = new Vector2(0, popup.transform.Find("popup").Find("naver").localPosition.y);
                //popup.transform.Find("popup").Find("Text").GetComponent<UnityEngine.UI.Text>().text = "성인, 아동 종류를 \n선택해 주세요";
            }
        }

    }
    public void closeSocket()
    {
        if (!socket_ready)
            return;

        //socket_writer.Close();
        //socket_reader.Close();
        net_stream.Close();
        tcp_socket.Close();
        socket_ready = false;
        Debug.Log("disconnected");
    }
    public void readSocket()
    {
        int pos = 0;


        if (net_stream.DataAvailable)
        {
            System.Array.Clear(receivedData, 0, 1024);
            int len = net_stream.Read(receivedData, 0, 1024);

            if (len > 0)
            {
                mIsRecv = true;

            }
            img_bytes = null;
        }

    }

    public void cropImage()
    {
        buttonOnOff(false, false, false, false, false, false);
        cropBox.gameObject.SetActive(false);
        cropBtn.gameObject.SetActive(false);
        StartCoroutine(crop());
    }
    IEnumerator crop()
    {
        yield return new WaitForEndOfFrame();
        nKey = (long)7301;
        cropTrans = canvasFashion.transform.Find("cropBox").GetComponent<RectTransform>();
        Vector3 cropRect = fashionCamera.WorldToScreenPoint(new Vector3(cropTrans.position.x, cropTrans.position.y, cropTrans.position.z));
        Texture2D tex = new Texture2D((int)(cropTrans.rect.width * Screen.width / 720), (int)(cropTrans.rect.height * Screen.height / 1280), TextureFormat.RGB24, false);
        Debug.Log("crop: " + cropTrans.position.x + ", " + cropTrans.position.y + ", " + cropTrans.rect.width + ", " + cropTrans.rect.height);
        tex.ReadPixels(new Rect(cropRect.x, cropRect.y, (int)(cropTrans.rect.width * Screen.width / 720), (int)(cropTrans.rect.height * Screen.height / 1280)), 0, 0);
        tex.Apply();

        capturePlane.gameObject.SetActive(true);
        //Renderer renderer = GameObject.Find("Canvas").transform.Find("Plane (1)").GetComponent<Renderer>();
        //renderer.material.mainTexture = tex;

        img_bytes = tex.EncodeToJPG();


        cropBox.gameObject.SetActive(false);
        cropBtn.gameObject.SetActive(false);
        setupSocket();
    }
    public void cancel()
    {
        cropBox.gameObject.SetActive(false);
        cropBtn.gameObject.SetActive(false);

        capturePlane.SetActive(false);
        imagePlane.SetActive(false);
        category = "kid";
        categoryObj.transform.Find("kid").Find("check").gameObject.SetActive(true);
        categoryObj.gameObject.SetActive(false);
        categoryObj.transform.Find("adult").Find("check").gameObject.SetActive(false);
        if (GameManager.instance.charInfo.top != "0" || GameManager.instance.charInfo.bottom != "0" || GameManager.instance.charInfo.onepiece != "0")
        {
            canvasFashion.transform.Find("top").gameObject.SetActive(true);
            canvasFashion.transform.Find("bottom").gameObject.SetActive(true);
            GameManager.instance.character.SetActive(true);
            buttonOnOff(true, true, true, false, false, true);
        }
        else
        {
            canvasFashion.transform.Find("top").gameObject.SetActive(false);
            canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
            GameManager.instance.character.SetActive(false);
            buttonOnOff(true, true, true, false, false, false);
        }
    }
    public void recv_parsing()
    {
        int pos = 0;


        if (receivedData.Length > 0)
        {

            byte[] count = new byte[longSize];
            byte[] cloth = new byte[longSize];
            byte[] cloth1 = new byte[longSize];
            byte[] color = new byte[longSize];
            byte[] color1 = new byte[longSize];

            Array.Copy(receivedData, 0, count, 0, longSize);
            pos += longSize;
            Array.Copy(receivedData, pos, cloth, 0, longSize);
            pos += longSize;
            Array.Copy(receivedData, pos, cloth1, 0, longSize);
            pos += longSize;
            Array.Copy(receivedData, pos, color, 0, longSize);
            pos += longSize;
            Array.Copy(receivedData, pos, color1, 0, longSize);

            lCount = BitConverter.ToInt64(count, 0);
            Debug.Log("lCount : " + lCount);

            categoryObj.gameObject.SetActive(false);
            categoryObj.transform.Find("kid").Find("check").gameObject.SetActive(true);
            categoryObj.transform.Find("adult").Find("check").gameObject.SetActive(false);

           

            if (lCount == 0)
            {
                GameObject popup = PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("popup").gameObject;
                Debug.Log("인식결과 없음");
                //popup.SetActive(true);
                //popup.transform.Find("popup").Find("naver").gameObject.SetActive(false);
                //popup.transform.Find("popup").Find("Button").localPosition = new Vector2(0, popup.transform.Find("popup").Find("naver").localPosition.y);
                //popup.transform.Find("popup").Find("Text").GetComponent<UnityEngine.UI.Text>().text = "인식결과 없음";

                if (GameManager.instance.charInfo.top != "0" || GameManager.instance.charInfo.bottom != "0" || GameManager.instance.charInfo.onepiece != "0")
                    buttonOnOff(true, true, true, false, false, true);
                else
                    buttonOnOff(true, true, true, false, false, false);
                capturePlane.SetActive(false);
                imagePlane.gameObject.SetActive(false);
                popup.SetActive(true);
                popup.transform.Find("popup").Find("Text").GetComponent<UnityEngine.UI.Text>().text = "인식결과가 없습니다.";
                popup.transform.Find("popup").Find("Button").gameObject.SetActive(false);
                popup.transform.Find("popup").Find("Button2").localPosition = new Vector2(0, popup.transform.Find("popup").Find("Button2").localPosition.y);
            }
            else if (lCount == 1)
            {
                Debug.Log("1개인식");
                lCloth = BitConverter.ToInt64(cloth, 0);
                lCloth1 = BitConverter.ToInt64(cloth1, 0);
                lColor = BitConverter.ToInt64(color, 0);
                lColor1 = BitConverter.ToInt64(color1, 0);

                Debug.Log("count : " + lCount + ", cloth : " + lCloth + ", cloth1 : " + lCloth1 + ", color : " + lColor + ", color1 : " + lColor1);
                GameObject popup = PopupManager.instance.cameraPopup.transform.Find("CanvasPopup").Find("popup").gameObject;

                option.SetActive(true);
                Debug.Log(sClothArray_k[lCloth] + "_" + colorArray[lColor] + "_img");
                option.transform.Find("List").Find("0").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[sClothArray_k[lCloth] + "_" + colorArray[lColor] + "_img"];
                //option.transform.Find("List").Find("0").Find("Text").GetComponent<UnityEngine.UI.Text>().text = clothArray_k[lCloth];
                option.transform.Find("Color").Find("GameObject").Find(colorArray[lColor]).Find("check").gameObject.SetActive(true);
                GameManager.instance.selectCloth = sClothArray_k[lCloth];
                if (GameManager.instance.selectCloth.Contains("top") || GameManager.instance.selectCloth.Contains("onepiece"))
                    GameManager.instance.selectTopColor = colorArray[lColor];
                else if (GameManager.instance.selectCloth.Contains("bottom"))
                    GameManager.instance.selectBottomColor = colorArray[lColor];

                capturePlane.SetActive(false);
                imagePlane.SetActive(false);
                buttonOnOff(true, true, true, false, false, false);

                canvasFashion.transform.Find("top").gameObject.SetActive(true);
                canvasFashion.transform.Find("bottom").gameObject.SetActive(true);
            }
            else if (lCount > 1)
            {
                lXPos = BitConverter.ToInt64(cloth, 0);
                lYPos = BitConverter.ToInt64(cloth1, 0);
                lWidth = BitConverter.ToInt64(color, 0);
                lHeight = BitConverter.ToInt64(color1, 0);
                float r = (float)Screen.height / ((float)Screen.width / (float)720);
                Debug.Log(r);
                float textureH = AndroidFuntionCall.texture.height * 720 / AndroidFuntionCall.texture.width;
                Rect rect;
                Debug.Log("text@!!!!!" + AndroidFuntionCall.texture.height * 720 / AndroidFuntionCall.texture.width);

                rect = new Rect((float)lXPos, (((float)r - (float)textureH) / (float)2) + ((float)textureH - (float)lYPos - (float)lHeight), (float)lWidth, (float)lHeight);
                Debug.Log("count : " + lCount + ", x : " + lXPos + ", y : " + lYPos + ", width : " + lWidth + ", height : " + lHeight);
                Debug.Log("rect : " + rect + ", screen.w : " + Screen.width + ", screenH: " + Screen.height);
                //crop

                RectTransform rectTrans;
                rectTrans = cropBox.GetComponent<RectTransform>();


                GameObject obj = cropBox.gameObject;
                obj.SetActive(true);


                cropRectPos.sizeDelta = new Vector2((float)rect.width, (float)rect.height);


                Vector3 vPos;


                vPos.x = rect.x;
                vPos.y = rect.y;
                vPos.z = rectTrans.position.z;

                rectTrans.anchoredPosition = vPos;
                cropBtn.gameObject.SetActive(true);

                //
                buttonOnOff(false, false, false, false, false, false);

            }
            canvasFashion.transform.Find("loading").gameObject.SetActive(false);
            canvasFashion.transform.Find("reCapture").gameObject.SetActive(false);
        }

    }
    public void reCapture()
    {
        capturePlane.gameObject.SetActive(false);
        imagePlane.gameObject.SetActive(false);
        if (GameManager.instance.charInfo.top != "0" || GameManager.instance.charInfo.bottom != "0" || GameManager.instance.charInfo.onepiece != "0")
        {
            canvasFashion.transform.Find("top").gameObject.SetActive(true);
            canvasFashion.transform.Find("bottom").gameObject.SetActive(true);
            buttonOnOff(true, true, true, false, false, true);
            GameManager.instance.character.SetActive(true);
        }
        else
        {
            canvasFashion.transform.Find("top").gameObject.SetActive(false);
            canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
            buttonOnOff(true, true, true, false, false, false);
        }
        //canvasFashion.transform.Find("selectCategory").gameObject.SetActive(false);
        category = "kid";
        categoryObj.transform.Find("kid").Find("check").gameObject.SetActive(true);
        categoryObj.transform.Find("adult").Find("check").gameObject.SetActive(false);
        
    }
    public void selectCategory(GameObject obj)
    {

        if (!obj.transform.Find("check").gameObject.active)
            obj.transform.Find("check").gameObject.SetActive(true);

        if (obj.name.Contains("kid"))
        {
            category = obj.name;
            obj.transform.parent.Find("adult").Find("check").gameObject.SetActive(false);
        }
        else if (obj.name.Contains("adult"))
        {
            category = obj.name;
            obj.transform.parent.Find("kid").Find("check").gameObject.SetActive(false);
        }
    }
    public void send()
    {
        if (img_bytes != null)
        {
            write = new Thread(writeImageSocket);
            Debug.Log("!!!!!!: " + write.IsAlive);
            if (!write.IsAlive)
            {
                Debug.Log("write");
                try
                {
                    write.Start();
                    canvasFashion.transform.Find("loading").gameObject.SetActive(true);
                }
                catch (Exception e)
                {

                }
            }
        }
        else
        {
            Debug.Log("img is null");
        }


    }
    public void writeImageSocket()
    {
        Debug.Log("image Byte :" + img_bytes.Length);
        string text = "abcd";
        long nImageSize = (long)img_bytes.Length;

        long nTotal = (long)longSize + (long)longSize + (long)longSize + nImageSize;
        Debug.Log("longSize : " + (long)longSize + ", Text :" + text.Length + ", Total :" + nTotal);
        byte[] pSendData = new byte[nTotal];
        Array.Clear(pSendData, 0, pSendData.Length);

        int nPos = 0;
        Debug.Log("nTotal : " + nTotal);
        byte[] bTotal = BitConverter.GetBytes(nTotal);
        Debug.Log("bTotal len : " + bTotal.Length);
        Array.Copy(bTotal, 0, pSendData, nPos, longSize);

        Debug.Log("pos : " + nPos);
        nPos += longSize;


        Debug.Log("nKey : " + nKey);
        byte[] bKey = BitConverter.GetBytes(nKey);
        Debug.Log("bKey len : " + bKey.Length);
        Array.Copy(bKey, 0, pSendData, nPos, longSize);

        Debug.Log("pos : " + nPos);
        nPos += longSize;

        Debug.Log("nImageSize : " + nImageSize);

        byte[] bImageSize = BitConverter.GetBytes(nImageSize);
        Debug.Log("bImageSize len : " + bImageSize.Length);
        Array.Copy(bImageSize, 0, pSendData, nPos, longSize);

        nPos += longSize;

        Array.Copy(img_bytes, 0, pSendData, nPos, nImageSize);

        char[] c = new char[pSendData.Length * 2];
        byte b;
        for (int bx = 0, cx = 0; bx < pSendData.Length; ++bx, ++cx)
        {
            b = ((byte)(pSendData[bx] >> 4));
            c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

            b = ((byte)(pSendData[bx] & 0x0F));
            c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
        }

        string hex = new string(c);
        Debug.Log("pSendData HEX : " + hex);
        net_stream.Write(pSendData, 0, pSendData.Length);
        Debug.Log("complete");

    }

    /// <summary>
    /// 카드인식화면 이동
    /// </summary>
        public void cardRecognition()
    {
        GameManager.instance.isCardRecognition = true;
        if (GameManager.instance.onClothCount != 0)
        {
            PopupManager.instance.showPopup("카드인식화면으로 넘어가시면 기존 착용된 의상이 사라집니다. \n 계속 진행하시겠습니까?");
        }
        else
            GameManager.instance.showRecognitionCanvas(true);

    }
    /// <summary>
    /// 촬영
    /// </summary>
    public void capture()
    {
        buttonOnOff(false, false, false, false, false, false);
        if (GameManager.instance.character.active)
            GameManager.instance.character.SetActive(false);
        canvasFashion.transform.Find("top").gameObject.SetActive(false);
        canvasFashion.transform.Find("bottom").gameObject.SetActive(false);
        StartCoroutine(takePicture());

    }
    public IEnumerator takePicture()
    {
        yield return new WaitForEndOfFrame();
        AndroidFuntionCall.texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        AndroidFuntionCall.texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        AndroidFuntionCall.texture.Apply();


        img_bytes = ScaleTexture(AndroidFuntionCall.texture, 720, (int)((720 * Screen.height) / Screen.width)).EncodeToJPG();
        capturePlane.SetActive(true);
        capturePlane.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(AndroidFuntionCall.texture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));

        nKey = (long)7300;
        img_bytes = ScaleTexture(AndroidFuntionCall.texture, 720, (int)((720 * Screen.height) / Screen.width)).EncodeToJPG();
        buttonOnOff(false, false, false, true, true, false);

        //categoryObj.gameObject.SetActive(true);
        category = "kid";
        categoryObj.transform.Find("kid").Find("check").gameObject.SetActive(true);
        categoryObj.transform.Find("adult").Find("check").gameObject.SetActive(false);
    }
    public void buttonOnOff(bool album, bool capture, bool card, bool reCapture, bool send, bool camera)
    {
        canvasFashion.transform.Find("album").gameObject.SetActive(album);
        canvasFashion.transform.Find("capture").gameObject.SetActive(capture);
        canvasFashion.transform.Find("card").gameObject.SetActive(card);
        canvasFashion.transform.Find("reCapture").gameObject.SetActive(reCapture);
        canvasFashion.transform.Find("send").gameObject.SetActive(send);
        canvasFashion.transform.Find("camera").gameObject.SetActive(camera);
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

    /// <summary>
    /// 의상 선택
    /// </summary>
    /// <param name="obj"></param>
    public void select(GameObject obj)
    {
        if (obj.ToString().Contains("0") || obj.ToString().Contains("1") || obj.ToString().Contains("2"))
        {

            //if(result.ContainsKey("list"))
            //{
            //    for(int i = 0; i < 3; i++)
            //    {
            //        option.transform.Find("List").Find(i.ToString()).Find("check").gameObject.SetActive(false);
            //    }
            //    result["list"] = obj.GetComponent<Image>().sprite.name.ToString().Replace("_img","");
            //}
            //else
            //{
            //    result.Add("list", obj.GetComponent<Image>().sprite.name.ToString().Replace("_img",""));
            //}

        }
        else
        {
            for (int i = 0; i < 11; i++)
            {
                option.transform.Find("Color").Find("GameObject").GetChild(i).Find("check").gameObject.SetActive(false);
            }
            if (GameManager.instance.selectCloth.Contains("top") || GameManager.instance.selectCloth.Contains("onepiece"))
            {
                GameManager.instance.selectTopColor = obj.name;
                option.transform.Find("List").Find("0").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectTopColor + "_img"];
            }
            else if (GameManager.instance.selectCloth.Contains("bottom"))
            {
                GameManager.instance.selectBottomColor = obj.name;
                option.transform.Find("List").Find("0").GetComponent<UnityEngine.UI.Image>().sprite = LoadAsset.instance.fashionClothImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectBottomColor + "_img"];
            }
            obj.transform.Find("check").gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 선택완료
    /// </summary>
    public void complete()
    {
        if ((GameManager.instance.selectTopColor != "" || GameManager.instance.selectBottomColor != "") && GameManager.instance.selectCloth != "")
        {
            canvasFashion.transform.Find("top").gameObject.SetActive(true);
            canvasFashion.transform.Find("bottom").gameObject.SetActive(true);

            GameManager.instance.character.SetActive(true);
            GameManager.instance.fashionCharacter.transform.GetChild(18).gameObject.SetActive(true);
            option.gameObject.SetActive(false);
            //for (int i = 0; i < 3; i++)
            //{
            //    option.transform.Find("List").Find(i.ToString()).Find("check").gameObject.SetActive(false);
            //}
            for (int i = 0; i < 11; i++)
            {
                option.transform.Find("Color").Find("GameObject").GetChild(i).Find("check").gameObject.SetActive(false);
            }

            GameManager.instance.character.SetActive(true);
            if (GameManager.instance.onClothCount == 0)
            {
                canvasFashion.transform.Find("top").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                canvasFashion.transform.Find("bottom").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                GameManager.instance.charInfo.body = "char_ari";
                GameManager.instance.offCharObj("top");
                GameManager.instance.offCharObj("bottom");
                GameManager.instance.offCharObj("onepiece");
                GameManager.instance.offCharObj("shoes");
                GameManager.instance.offCharObj("acc");

            }
            GameManager.instance.fashionCharacter.SetActive(true);



            int index = GameManager.instance.selectCloth.IndexOf("_");
            if (GameManager.instance.selectCloth.Substring(index + 1).Contains("top"))
            {
                if (GameManager.instance.charInfo.top == "0")
                {
                    GameManager.instance.onClothCount++;
                }
                if (GameManager.instance.charInfo.onepiece.Contains("onepiece"))
                {
                    canvasFashion.transform.Find("bottom").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_bottom");
                    GameManager.instance.charInfo.onepiece = "0";
                    GameManager.instance.offCharObj("onepiece");
                    GameManager.instance.onClothCount = 1;
                }
                GameManager.instance.offCharObj("top");
                GameManager.instance.charInfo.top = GameManager.instance.selectCloth;
                canvasFashion.transform.Find("top").GetComponent<Image>().sprite = LoadAsset.instance.fashionClothImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectTopColor + "_img"];
            }
            else if (GameManager.instance.selectCloth.Substring(index + 1).Contains("bottom"))
            {
                if (GameManager.instance.charInfo.bottom == "0")
                {
                    GameManager.instance.onClothCount++;
                }
                if (GameManager.instance.charInfo.onepiece.Contains("onepiece"))
                {
                    canvasFashion.transform.Find("top").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/icon_top");
                    GameManager.instance.charInfo.onepiece = "0";
                    GameManager.instance.offCharObj("onepiece");
                    GameManager.instance.onClothCount = 1;
                }
                GameManager.instance.offCharObj("bottom");
                GameManager.instance.charInfo.bottom = GameManager.instance.selectCloth;
                canvasFashion.transform.Find("bottom").GetComponent<Image>().sprite = LoadAsset.instance.fashionClothImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectBottomColor + "_img"];
            }
            else if (GameManager.instance.selectCloth.Substring(index + 1).Contains("onepiece"))
            {
                GameManager.instance.charInfo.top = "0";
                GameManager.instance.charInfo.bottom = "0";
                GameManager.instance.offCharObj("bottom");
                GameManager.instance.offCharObj("top");
                GameManager.instance.offCharObj("onepiece");
                GameManager.instance.onClothCount = 1;
                GameManager.instance.charInfo.onepiece = GameManager.instance.selectCloth;
                canvasFashion.transform.Find("top").GetComponent<Image>().sprite = LoadAsset.instance.fashionClothImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectTopColor + "_img"];
                canvasFashion.transform.Find("bottom").GetComponent<Image>().sprite = LoadAsset.instance.fashionClothImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectTopColor + "_img"];
            }

            GameManager.instance.onCloth(GameManager.instance.selectCloth.Substring(index + 1), GameManager.instance.selectCloth);
            Debug.Log("cloth : " + GameManager.instance.selectCloth.Substring(index + 1));
            if (GameManager.instance.selectCloth.Contains("top") || GameManager.instance.selectCloth.Contains("onepiece"))
                GameManager.instance.fashionCharacter.transform.Find(GameManager.instance.selectCloth).GetComponent<Renderer>().material.mainTexture = LoadAsset.instance.fashionMaterialsImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectTopColor];
            else if (GameManager.instance.selectCloth.Contains("bottom"))
                GameManager.instance.fashionCharacter.transform.Find(GameManager.instance.selectCloth).GetComponent<Renderer>().material.mainTexture = LoadAsset.instance.fashionMaterialsImg[GameManager.instance.selectCloth + "_" + GameManager.instance.selectBottomColor];
            if (!GameManager.instance.charInfo.top.Contains("top") && !GameManager.instance.charInfo.onepiece.Contains("onepiece"))
                GameManager.instance.onCloth("top", "basic_top");
            if (!GameManager.instance.charInfo.bottom.Contains("bottom") && !GameManager.instance.charInfo.onepiece.Contains("onepiece"))
                GameManager.instance.onCloth("bottom", "basic_bottom");

            capturePlane.SetActive(false);
            canvasFashion.transform.Find("camera").gameObject.SetActive(true);

        }
        else
        {
            //둘중하나 선택X
            //if (result.ContainsKey("list"))
            //{
            //    PopupManager.instance.showPopup("색상을 선택해주세요");
            //}
            //else if (result.ContainsKey("color"))
            //{
            //    PopupManager.instance.showPopup("의상을 선택해주세요");
            //}
        }
    }
    public void scaleTouchDown()
    {
        cropTrans = cropBox.GetComponent<RectTransform>();
        newCropTrans = cropTrans;
        orignPos = fashionCamera.WorldToScreenPoint(new Vector3(cropTrans.position.x, cropTrans.position.y, cropTrans.position.z));
        orignCropTrans = cropBox.GetComponent<RectTransform>();

        x = orignCropTrans.rect.width;
        y = orignCropTrans.rect.height;
        //Debug.Log("pos :  " + orignPos.x + ",  " + orignPos.y + ", touch: " + trans.position.x + ", " + trans.position.y);
    }
    public void scaleDrag_XPYP()
    {
        touchPos = Input.mousePosition;
        Debug.Log("debugP : " + Input.mousePosition);
        cropTrans.sizeDelta = new Vector2(Math.Abs((orignPos.x - touchPos.x) * 720 / Screen.width), Math.Abs((orignPos.y - touchPos.y) * 1280 / Screen.height));
    }
    public void scaleDrag_XPYN()
    {
        touchPos = Input.mousePosition;
        newCropTrans.sizeDelta = new Vector2(Math.Abs((orignPos.x - touchPos.x) * 720 / Screen.width), Math.Abs(y + ((orignPos.y - touchPos.y)) * 1280 / Screen.height));

        Vector3 pos;
        pos.x = orignPos.x;
        pos.y = touchPos.y;
        pos.z = orignPos.z;
        newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
        //Debug.Log("size : " + tran.rect.height);

        cropTrans = newCropTrans;
    }
    public void scaleDrag_XNYN()
    {
        touchPos = Input.mousePosition;
        newCropTrans.sizeDelta = new Vector2(Math.Abs(x + ((orignPos.x - touchPos.x)) * 720 / Screen.width), Math.Abs(y + ((orignPos.y - touchPos.y)) * 1280 / Screen.height));

        Vector3 pos;
        pos.x = touchPos.x;
        pos.y = touchPos.y;
        pos.z = orignPos.z;
        newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
        //Debug.Log("size : " + tran.rect.height);

        cropTrans = newCropTrans;
    }
    public void scaleDrag_XNYP()
    {
        touchPos = Input.mousePosition;
        newCropTrans.sizeDelta = new Vector2(Math.Abs(x + ((orignPos.x - touchPos.x)) * 720 / Screen.width), (touchPos.y - orignPos.y) * 1280 / Screen.height);

        Vector3 pos;
        pos.x = touchPos.x;
        pos.y = orignPos.y;
        pos.z = orignPos.z;
        newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
        //Debug.Log("size : " + tran.rect.height);

        cropTrans = newCropTrans;
    }
    public void scaleDragX_N()
    {
        touchPos = Input.mousePosition;
        if ((orignPos.x - touchPos.x + x) > 0)
        {

            newCropTrans.sizeDelta = new Vector2(Math.Abs(x + ((orignPos.x - touchPos.x)) * 720 / Screen.width), orignCropTrans.rect.height);
            //trans.sizeDelta = new Vector2(trans.rect.width, Math.Abs(orignTrans.rect.width + distansPos.y));


            Vector3 pos;
            pos.x = touchPos.x;
            pos.y = orignPos.y;
            pos.z = orignPos.z;
            newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
            //Debug.Log("size : " + tran.rect.height);

            cropTrans = newCropTrans;
        }
    }
    public void scaleDragX_P()
    {
        touchPos = Input.mousePosition;

        if ((touchPos.x - orignPos.x) > 0)
        {
            newCropTrans.sizeDelta = new Vector2((touchPos.x - orignPos.x) * 720 / Screen.width, orignCropTrans.rect.height);
            Debug.Log("input : " + Input.mousePosition.x + ", orign :  " + touchPos.x + ", " + orignPos.x + ", value : " + (touchPos.x - orignPos.x));
            //trans.sizeDelta = new Vector2(trans.rect.width, Math.Abs(orignTrans.rect.width + distansPos.y));


            Vector3 pos;
            pos.x = orignPos.x;
            pos.y = orignPos.y;
            pos.z = orignPos.z;
            newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
            //Debug.Log("size : " + tran.rect.height);

            cropTrans = newCropTrans;
        }
    }
    public void scaleDragY_N()
    {
        touchPos = Input.mousePosition;
        if ((orignPos.y - touchPos.y + y) > 0)
        {

            newCropTrans.sizeDelta = new Vector2(orignCropTrans.rect.width, Math.Abs(y + ((orignPos.y - touchPos.y)) * 1280 / Screen.height));
            Debug.Log("orign :  " + orignPos.y + ", " + touchPos.y + ", value : " + (orignPos.y - touchPos.y));
            //trans.sizeDelta = new Vector2(trans.rect.width, Math.Abs(orignTrans.rect.width + distansPos.y));


            Vector3 pos;
            pos.x = orignPos.x;
            pos.y = touchPos.y;
            pos.z = orignPos.z;
            newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
            //Debug.Log("size : " + tran.rect.height);

            cropTrans = newCropTrans;
        }
    }
    public void scaleDragY_P()
    {
        touchPos = Input.mousePosition;

        if ((touchPos.y - orignPos.y) > 0)
        {
            newCropTrans.sizeDelta = new Vector2(orignCropTrans.rect.width, (touchPos.y - orignPos.y) * 1280 / Screen.height);
            //trans.sizeDelta = new Vector2(trans.rect.width, Math.Abs(orignTrans.rect.width + distansPos.y));


            Vector3 pos;
            pos.x = orignPos.x;
            pos.y = orignPos.y;
            pos.z = orignPos.z;
            newCropTrans.position = fashionCamera.ScreenToWorldPoint(pos);
            //Debug.Log("size : " + tran.rect.height);

            cropTrans = newCropTrans;
        }
    }

    public void moveTouchDown()
    {
        cropTrans = canvasFashion.transform.Find("cropBox").GetComponent<RectTransform>();
        orignPos = fashionCamera.WorldToScreenPoint(new Vector3(cropTrans.position.x, cropTrans.position.y, cropTrans.position.z));
        distansPos = orignPos - Input.mousePosition;
    }
    public void moveDrag()
    {
        touchPos = Input.mousePosition;
        //Debug.Log("debugP : " + touchPos);
        Vector3 pos = fashionCamera.ScreenToWorldPoint(touchPos + distansPos);
        Debug.Log("pos : " + pos);
        pos.z = cropTrans.position.z;
        cropTrans.position = pos;

    }
}