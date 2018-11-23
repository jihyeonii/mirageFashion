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
        LoadAsset.instance.test.text = "finish";
            SceneManager.LoadScene(1);
    }
    public void effect()
    {
        GameObject.Find("Canvas").transform.Find("effectImg").gameObject.SetActive(true);
    }

    
}
