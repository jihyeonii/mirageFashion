using UnityEngine;
using System.Collections;

public class CameraLoader : MonoBehaviour
{
    WebCamDevice[] devices;
    WebCamTexture webCam;

    public static CameraLoader instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
        webCam = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        //devices = WebCamTexture.devices;
        //webCam.deviceName = devices[0].name;
        // set the size of the plane
        //float height = Camera.main.orthographicSize;
        //int width = (int)height * (Screen.width / Screen.height);
        //Debug.Log(width + ", " + height + ", " + Screen.width + ", " + Screen.height);
        //transform.localScale = new Vector3(width, height, 1);

        renderer.material.mainTexture = webCam;
        webCam.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void stopCam()
    {
        webCam.Stop();
    }
    public void playCam()
    {
        webCam.Play();
    }
    public void switchCamera()
    {
        webCam.Stop();
        webCam.deviceName = (webCam.deviceName == devices[0].name ? devices[1].name : devices[0].name);
        webCam.Play();
    }

}
/*
 * using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraLoader : MonoBehaviour
{
    public RawImage rawimage;
    // Use this for initialization
    void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        rawimage.texture = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {


    }

}

    */
