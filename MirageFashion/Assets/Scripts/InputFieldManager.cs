using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour {
    public InputField inputField;
    public string stringField = null;
	// Use this for initialization
	void Start () {
        
	}
   
    // Update is called once per frame
    public void OnEdit()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            stringField = inputField.text;
            PaintManager.instance.test.text = "edit";
        }
    }
    public void OnEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputField.text = stringField;
            inputField.transform.Find("Text").GetComponent<Text>().text = stringField;
            PaintManager.instance.test.text = "end";
        }
    }
}
