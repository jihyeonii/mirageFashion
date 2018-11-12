using UnityEngine;
using System.Collections;

public class IntroScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //LoadSceneManager.loadScene("updateScene");
        //PlayerPrefs.SetString("guide", "on");
        Application.LoadLevel("updateScene");
        //GameManager.instance.uiState = GameManager.UIState.main;
	}
	
	
}
