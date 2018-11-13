package com.feelingki.fh;

import com.unity3d.player.UnityPlayer;

import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.util.Log;

/**
 * @deprecated It's recommended that you base your code directly on UnityPlayerActivity or make your own NativeActitivty implementation.
 **/
public class UnityPlayerNativeActivity extends UnityPlayerActivity
{
	@Override protected void onCreate (Bundle savedInstanceState)
	{
		Log.w("Unity", "UnityPlayerNativeActivity has been deprecated, please update your AndroidManifest to use UnityPlayerActivity instead");
		super.onCreate(savedInstanceState);
	}
	
}
