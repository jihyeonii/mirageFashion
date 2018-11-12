using UnityEngine;
using System.Collections;

public class LoadSceneManager {

    public static void loadScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}
