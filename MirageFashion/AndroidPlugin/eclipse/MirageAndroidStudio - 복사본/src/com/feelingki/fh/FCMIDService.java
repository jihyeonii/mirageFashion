package com.feelingki.fh;

import android.support.annotation.WorkerThread;

import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.FirebaseInstanceIdService;

public class FCMIDService extends FirebaseInstanceIdService{
	@Override
	 @WorkerThread
	 public void onTokenRefresh()
	 {
		super.onTokenRefresh( );
	  
		String strToken = FirebaseInstanceId.getInstance().getToken();
	 }
}
