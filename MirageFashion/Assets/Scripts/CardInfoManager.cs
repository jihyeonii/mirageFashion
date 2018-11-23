using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardInfoManager : MonoBehaviour {
    public static CardInfoManager instance;
    float screenWidth;
    float screenHeight;

    public int cardInfoType = 0; // 0 : 캐릭터, 1:상의 2: 하의, 3:원피스 4:신발 5:액세사리

    int objCount = 0;
    int charCount = 0;
    int topCount = 0;
    int bottomCount = 0;
    int onepieceCount = 0;
    int shoesCount = 0;
    int accCount = 0;

    int selectNum = 0;
    // Use this for initialization
    void Start() {

        instance = this;
        screenWidth = Screen.width;
        screenHeight = (float)screenWidth * ((float)Screen.height / (float)Screen.width);
        
        for (int i = 0; i < LoadAsset.assetCharacter.transform.GetChildCount(); i++)
        {
            if (LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("char"))
            {
                charCount++;
            }
            else if (LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("top") && !LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("basic"))
            {
                topCount++;
            }
            else if (LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("bottom") && !LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("basic"))
            {
                bottomCount++;
            }
            else if (LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("onepiece"))
            {
                onepieceCount++;
            }
            else if (LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("shoes"))
            {
                shoesCount++;
            }
            else if (LoadAsset.assetCharacter.transform.GetChild(i).name.Contains("acc"))
            {
                accCount++;
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
    public void cardInfoPage()
    {
        float height = (720 / screenWidth) * screenHeight;
       
            //의상
            Vector3 size = new Vector3(720, height / 128*41, 1);
            gameObject.transform.Find("CardInfo").transform.Find("clothList").GetComponent<RectTransform>().sizeDelta = size;

            //의상종류
            gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.position = PopupManager.instance.cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, height / 128 * 41 * Screen.width / 720, 100));
            size = new Vector3(720, height / 10, 1);
            gameObject.transform.Find("CardInfo").transform.Find("clothType").GetComponent<RectTransform>().sizeDelta = size;

            gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.Find("ScrollRectType").GetComponent<RectTransform>().sizeDelta = size;
            gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.Find("ScrollRectType").transform.Find("type").GetComponent<RectTransform>().sizeDelta = new Vector3((height / 10) * 6, height / 10, 1);

            for (int i = 0; i < gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.Find("ScrollRectType").transform.Find("type").childCount - 2; i++)
            {
                gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.Find("ScrollRectType").transform.Find("type").GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector3(height / 10, height / 10, 1);
                gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.Find("ScrollRectType").transform.Find("type").GetChild(i).transform.position = PopupManager.instance.cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3((i * height / 10) / 720 * Screen.width, height / 128 * 41 * Screen.width / 720, 100));
            }

            //카드세부정보
            gameObject.transform.Find("CardInfo").transform.Find("cardInfo").transform.position = PopupManager.instance.cameraPopup.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, height / 1280 * 538 * Screen.width / 720, 100));
            size = new Vector3(720, height / 1280 *742, 1);
            gameObject.transform.Find("CardInfo").transform.Find("cardInfo").GetComponent<RectTransform>().sizeDelta = size;


            //로고
            size = new Vector3(246, height / 9, 1);
            gameObject.transform.Find("CardInfo").Find("cardInfo").transform.Find("logo").GetComponent<RectTransform>().sizeDelta = size;

            //뒤로가기
            size = new Vector3(54, 48, 1);
            gameObject.transform.Find("CardInfo").transform.Find("btnBack").GetComponent<RectTransform>().sizeDelta = size;




    }
    public void showObj(int type)
    {
        string preObj;
        string curObj;
        if (cardInfoType == 0)
            preObj = "char";
        else if (cardInfoType == 1)
            preObj = "top";
        else if (cardInfoType == 2)
            preObj = "bottom";
        else if (cardInfoType == 3)
            preObj = "onepiece";
        else if (cardInfoType == 4)
            preObj = "shoes";
        else
            preObj = "acc";

        if (type == 0)
            curObj = "char";
        else if (type == 1)
            curObj = "top";
        else if (type == 2)
            curObj = "bottom";
        else if (type == 3)
            curObj = "onepiece";
        else if (type == 4)
            curObj = "shoes";
        else
            curObj = "acc";
        GameObject obj;
        obj = gameObject.transform.Find("CardInfo").transform.Find("clothType").transform.Find("ScrollRectType").transform.Find("type").gameObject;
        obj.transform.Find(preObj).transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_category_" + preObj);
        obj.transform.Find(curObj).transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/btn_category_" + curObj + "_clicked");

        cardInfoType = type;
        Vector3 size;
        int count;
        Vector3 pos;
        count = getCount(type);

       
            size = new Vector3(720, Mathf.CeilToInt((float)count / 3) * 205, 1);

        GameObject[] typeObj = new GameObject[20];
        gameObject.transform.Find("CardInfo").transform.Find("clothList").transform.Find("ScrollRectType").transform.Find("Obj").localPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < 20; i++)
        {
            typeObj[i] = gameObject.transform.Find("CardInfo").transform.Find("clothList").transform.Find("ScrollRectType").transform.Find("Obj").transform.Find("obj" + i).gameObject;
            typeObj[i].SetActive(false);
        }
        gameObject.transform.Find("CardInfo").transform.Find("clothList").transform.Find("ScrollRectType").transform.Find("Obj").GetComponent<RectTransform>().sizeDelta = size;
        gameObject.transform.Find("CardInfo").transform.Find("clothList").transform.Find("ScrollRectType").GetComponent<RectTransform>().sizeDelta = size;
        for (int i = 0; i < count; i++)
        {
            typeObj[i].GetComponent<Image>().sprite = null;
            typeObj[i] = gameObject.transform.Find("CardInfo").transform.Find("clothList").transform.Find("ScrollRectType").transform.Find("Obj").transform.Find("obj" + i).gameObject;
            typeObj[i].SetActive(true);

            typeObj[i].transform.localScale = new Vector3(1, 1, 1);
            
            pos = new Vector3(i % 3 * 240, -(((int)i / 3) * 205), 0);
            

            typeObj[i].transform.localPosition = pos;
            typeObj[i].GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.dicCard[type + "_" + i].name + "_img"]; //Resources.Load<Sprite>("RecognizeImg/" + GameManager.instance.dicCard[type + "_" + i].name + "_img");
            if (GameManager.instance.dicCard[type + "_" + i].history == 1)
            {
                typeObj[i].GetComponent<Image>().color = new Color(1,1,1);
            }
            else
                typeObj[i].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
        }
        for (int i = 0; i < gameObject.transform.Find("CardInfo").transform.Find("cardInfo").childCount; i++)
        {
            gameObject.transform.Find("CardInfo").transform.Find("cardInfo").transform.GetChild(i).gameObject.SetActive(false);
            gameObject.transform.Find("CardInfo").transform.Find("cardInfo").Find("Image").gameObject.SetActive(true);
            gameObject.transform.Find("CardInfo").transform.Find("cardInfo").Find("background").gameObject.SetActive(true);
            gameObject.transform.Find("CardInfo").transform.Find("cardInfo").Find("logo").gameObject.SetActive(true);
        }
    }
    public int getCount(int type)
    {
       
        int count;
        if (type == 0)
        {
            count = charCount;
        }
        else if (type == 1)
        {
            count = topCount;
        }
        else if (type == 2)
        {
            count = bottomCount;
        }
        else if (type == 3)
        {
            count = onepieceCount;
        }
        else if (type == 4)
        {
            count = shoesCount;
        }
        else
        {
            count = accCount;
        }
        return count;
    }
    public void showCardInfo(int num)
    {
        GameManager.instance.instantiateSound(GameManager.instance.clickSound);
        selectNum = num;
        GameObject obj;
        obj = gameObject.transform.Find("CardInfo").transform.Find("cardInfo").gameObject;
        GameObject btn;
        btn = obj.transform.Find("btn").gameObject;
      
        btn.SetActive(true);
        btn.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/cardInfo_open");
        if (cardInfoType != 0)
        {
            obj.transform.Find("charImg").gameObject.SetActive(false);
            obj.transform.Find("charInfo").gameObject.SetActive(false);
            obj.transform.Find("itemImg").gameObject.SetActive(true);
            obj.transform.Find("itemInfo").gameObject.SetActive(false);

            obj.transform.Find("itemImg").transform.Find("Image").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.dicCard[cardInfoType + "_" + selectNum].name + "_img"];
            obj.transform.Find("itemImg").transform.Find("Text").GetComponent<Text>().text = GameManager.instance.dicCard[cardInfoType + "_" + selectNum].strName;
        }
        else
        {
            //character
            obj.transform.Find("charImg").gameObject.SetActive(true);
            obj.transform.Find("charInfo").gameObject.SetActive(false);
            obj.transform.Find("itemImg").gameObject.SetActive(false);
            obj.transform.Find("itemInfo").gameObject.SetActive(false);

            obj.transform.Find("charImg").transform.Find("char").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.dicCard[cardInfoType + "_" + num].name + "_infoImg"];
            obj.transform.Find("charImg").transform.Find("name").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["name_"+ GameManager.instance.dicCard[cardInfoType + "_" + num].name];
        }
    }
    public void cardInfoBtn(GameObject btn)
    {
        GameObject obj;
        obj = gameObject.transform.Find("CardInfo").transform.Find("cardInfo").gameObject;

        if (obj.transform.Find("charImg").gameObject.active)
        {
            obj.transform.Find("charImg").gameObject.SetActive(false);
            obj.transform.Find("charInfo").gameObject.SetActive(true);
            btn.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/cardInfo_close");
            showChar();
        }
        else if (obj.transform.Find("charInfo").gameObject.active)
        {
            obj.transform.Find("charImg").gameObject.SetActive(true);
            obj.transform.Find("charInfo").gameObject.SetActive(false);
            btn.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/cardInfo_open");
        }
        else if (obj.transform.Find("itemImg").gameObject.active)
        {
            obj.transform.Find("itemImg").gameObject.SetActive(false);
            obj.transform.Find("itemInfo").gameObject.SetActive(true);
            btn.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/cardInfo_close");
            showItem();
        }
        else if (obj.transform.Find("itemInfo").gameObject.active)
        {
            obj.transform.Find("itemImg").gameObject.SetActive(true);
            obj.transform.Find("itemInfo").gameObject.SetActive(false);
            btn.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Button/cardInfo_open");
        }
    }
    public void showChar()
    {
        string character;
        if (selectNum == 0)
            character = "char_ari";
        else if (selectNum == 1)
            character = "char_min";
        else if (selectNum == 2)
            character = "char_shuel";
        else
            character = "char_suha";
        GameObject obj;
        obj = gameObject.transform.Find("CardInfo").transform.Find("cardInfo").transform.Find("charInfo").gameObject;
        obj.transform.Find("adaultChar").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["adault_" + character];//Resources.Load<Sprite>("Button/adault_" + character);
        obj.transform.Find("kidChar").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["kid_" + character];
        obj.transform.Find("name").GetComponent<Text>().text = GameManager.instance.dicCard[cardInfoType + "_" + selectNum].strName;
        obj.transform.Find("Text").GetComponent<Text>().text = GameManager.instance.dicCard[cardInfoType + "_" + selectNum].style;
    }
    public void showItem()
    {
        //캐릭터 1:상의 2:하의 3:원피스 4:신발 5:악세사리
        string type;
        if (cardInfoType == 1)
            type = "top";
        else if (cardInfoType == 2)
            type = "bottom";
        else if (cardInfoType == 3)
            type = "onepiece";
        else if (cardInfoType == 4)
            type = "shoes";
        else if (cardInfoType == 5)
            type = "acc";
        else
            type = "char";
        GameObject obj;
        obj = gameObject.transform.Find("CardInfo").transform.Find("cardInfo").transform.Find("itemInfo").gameObject;
        obj.transform.Find("Image").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg[GameManager.instance.dicCard[cardInfoType + "_" + selectNum].name + "_img"];
        obj.transform.Find("itemName").GetComponent<Text>().text = GameManager.instance.dicCard[cardInfoType + "_" + selectNum].strName;
        obj.transform.Find("type").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_cloth_" + type];// Resources.Load<Sprite>("Button/icon_cloth_" + type);
        obj.transform.Find("level").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_level_" + GameManager.instance.dicCard[cardInfoType + "_" + selectNum].level];//
        obj.transform.Find("style").GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_style_" + GameManager.instance.dicCard[cardInfoType + "_" + selectNum].indexStyle];
        int score = GameManager.instance.dicCard[cardInfoType + "_" + selectNum].score;

        //아이템 별☆    0~190 : ☆, 190~230 : ☆☆, 230~340 : ☆☆☆, 340~400 : ☆☆☆☆,  400~ : ☆☆☆☆☆
        if (score <= 190)
        {
            for(int i = 0; i<5; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_empty"];// Resources.Load<Sprite>("Button/icon_score_empty");
            }
            obj.transform.Find("score").GetChild(0).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_fill"];
        }
        else if(190 < score && score <= 230)
        {
            for (int i = 0; i < 5; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_empty"];
            }
            for(int i =0; i < 2; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_fill"];
            }
        }
        else if(230 < score && score <= 340)
        {
            for (int i = 0; i < 5; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_empty"];
            }
            for (int i = 0; i < 3; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_fill"];
            }
        }
        else if(340 < score && score <= 400)
        {
            for (int i = 0; i < 5; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_empty"];
            }
            for (int i = 0; i < 4; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_fill"];
            }
        }
        else if(400< score)
        {
            for (int i = 0; i < 5; i++)
            {
                obj.transform.Find("score").GetChild(i).GetComponent<Image>().sprite = LoadAsset.instance.dicClothImg["icon_score_fill"];
            }
        }
    }
    public void changeType(int type)
    {
        if (cardInfoType != type)
        {

            showObj(type);
            cardInfoType = type;
        }

    }
   
    
}
