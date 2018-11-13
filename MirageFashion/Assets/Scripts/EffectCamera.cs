using UnityEngine;
using System.Collections;

public class EffectCamera : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        this.gameObject.GetComponent<Camera>().fieldOfView = GameManager.instance.charCamera.GetComponent<Camera>().fieldOfView;
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = GameManager.instance.charCamera.transform.position;
        this.transform.rotation = GameManager.instance.charCamera.transform.rotation;
    }
}
