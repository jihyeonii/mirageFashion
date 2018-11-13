using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class cameraControll : MonoBehaviour
{
    WebCamDevice[] devices;
    WebCamTexture webCam;
    public GameObject plane;
    Texture2D texture;
    byte[] imageByte;

    public string myFilename;
    string myFolderLocation;
    string myScreenshotLocation;
    string myDefaultLocation;

    float screenHeight;
    float screenWidth;

    float resizeW;
    float resizeH;

    // Use this for initialization
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = (float)screenWidth * ((float)Screen.height / (float)Screen.width);
        resizeW = 720;
        resizeH = 720 * Screen.height / Screen.width;
        Screen.SetResolution((int)resizeW, (int)resizeH, true);

        plane.gameObject.transform.localScale = new Vector3(720 / 10, 0.1f, (720 / screenWidth) * screenHeight / 10);
        //plane.gameObject.transform.LookAt(Camera.main.transform);
        //plane.gameObject.transform.rotation = Quaternion.Euler(new Vector3(90, -180, 0));
        //plane.gameObject.SetActive(true);
        webCam = new WebCamTexture();
        Renderer renderer = plane.GetComponent<Renderer>();
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
    public void btnResize()
    {
        StartCoroutine(resize());

    }
    public IEnumerator resize()
    {
        yield return new WaitForEndOfFrame();
        texture = new Texture2D((int)resizeW, (int)resizeH, TextureFormat.RGB24, true);
        texture.ReadPixels(new Rect(0, 0, (int)resizeW, (int)resizeH), 0, 0, true);
        //texture = new Texture2D((int)resizeW, (int)resizeH, TextureFormat.RGB24, true);
        //texture.ReadPixels(new Rect(0, 0, (int)resizeW, (int)resizeH), 0, 0, true);
        texture.Apply();

        //Color[] sourceColor = texture.GetPixels(0);
        //Vector2 sourceSize = new Vector2(Screen.width, Screen.height);

        //float width = Screen.width;
        //float height = Screen.height;

        save();
    }
    public void save()
    {
        imageByte = texture.EncodeToPNG();
        DestroyImmediate(texture);

        myFilename = string.Format("jh{0}.png", System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        myFolderLocation = "/storage/emulated/0/DCIM/Test/";
        myScreenshotLocation = myFolderLocation + myFilename;
        myDefaultLocation = Application.persistentDataPath + "/" + myFilename;


        // DCIM 폴더에 디렉토리 생성
        if (!Directory.Exists(myFolderLocation))
        {
            Directory.CreateDirectory(myFolderLocation);
        }

        // 임시 디렉토리에 저장

        try
        {
            File.WriteAllBytes(myDefaultLocation, imageByte);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message, this);
        }
        finally
        {
            imageByte = null;
        }

        // 임시 디렉토리에서 DCIM 폴더로 이동
#if UNITY_EDITOR || UNITY_ANDROID
        try
        {

            File.Move(myDefaultLocation, myScreenshotLocation);

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message, this);
        }
#endif
        //imageByte = texture.EncodeToPNG();
        //DestroyImmediate(texture);

        //myFilename = string.Format("jh_{0}.png", System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        //myFolderLocation = "/storage/emulated/0/DCIM/Test/";
        //myScreenshotLocation = myFolderLocation + myFilename;
        //myDefaultLocation = Application.persistentDataPath + "/" + myFilename;

        //// DCIM 폴더에 디렉토리 생성
        //if (!Directory.Exists(myFolderLocation))
        //{
        //    Directory.CreateDirectory(myFolderLocation);
        //}
        //// 임시 디렉토리에 저장

        //try
        //{
        //    File.WriteAllBytes(myDefaultLocation, imageByte);
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.Message, this);
        //}
        //finally
        //{
        //    imageByte = null;
        //}
        //try
        //{

        //    File.Move(myDefaultLocation, myScreenshotLocation);
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.Message, this);
        //}
    }
}