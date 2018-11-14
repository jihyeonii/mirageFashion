using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class AnimationManager : MonoBehaviour {
    public void endAnimation()
    {
        GameManager.instance.appearCharEffect.SetActive(false);
    }
    
    public void endCharacterAnimation()
    {
        //GameObject.Find("Canvas").transform.Find("character").GetComponent<Animator>().enabled = false;

        //GameObject.Find("Canvas").transform.Find("character").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("img_ari");
        //GameObject.Find("Canvas").transform.Find("Effect").gameObject.SetActive(false);
        LoadAsset.instance.test.text = "finish";
            SceneManager.LoadScene(1);
    }
    public void effect()
    {
        GameObject.Find("Canvas").transform.Find("effectImg").gameObject.SetActive(true);
    }

    
}
