using UnityEngine;
using System.Collections;
using UnityEditor;

public class cleanCache{
    [MenuItem("Util/CleanCache")]
    // Use this for initialization
    public static void CleanCache()
    {
        if (Caching.CleanCache())
        {
            Debug.Log("successed");
        }
        else
        {
            Debug.Log("failed");
        }
    }


}
