using UnityEngine;
using System.Collections;

public class soundManager : MonoBehaviour {
    float time;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.smoothDeltaTime;
        if (this.gameObject.name.Contains("mission") )
        {
            if (time >7 || (GameManager.instance.uiState != GameManager.UIState.main && GameManager.instance.uiState != GameManager.UIState.camera) || (!GameManager.instance.characterAni.GetBool("princess") && !GameManager.instance.characterAni.GetBool("witch") && !GameManager.instance.characterAni.GetBool("police") && !GameManager.instance.characterAni.GetBool("stewardess") && !GameManager.instance.characterAni.GetBool("snoop") && !GameManager.instance.characterAni.GetBool("patissier")))
            {
                Destroy(this.gameObject);
                time = 0;
            }
        }
        else if (this.gameObject.name.Contains("save"))
        {
            if (time > 2)
            {
                Destroy(this.gameObject);
                time = 0;
            }
        }
        else if (this.gameObject.name.Contains("capture"))
        {
            if (time > 1)
            {
                Destroy(this.gameObject);
                time = 0;
            }
        }
        else
        {
            if(time > 2)
            {
                Destroy(this.gameObject);
                time = 0;
            }
        }
        if (this.gameObject.name.Contains("fx"))
        {
            if(time > 1f)
            {
                Destroy(this.gameObject);
                time = 0;
            }
        }
        
	}
}
