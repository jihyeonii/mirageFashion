package com.feelingki.fh;

import android.Manifest;
import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.provider.Settings;
import android.support.v4.app.ActivityCompat;
import android.util.Log;
import android.widget.Toast;

import com.feelingki.fh.util.Logger;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.messaging.FirebaseMessaging;
import com.unity3d.player.UnityPlayer;

public class checkPermission extends Activity{
	private final int PERMISSION_ACTIVITY_FOR_RESULT_CODE = 0000;
	private final int PERMISSIONS_REQUEST_ACCESS = 0001;

	private AlertDialog mPermissionDeniedNotiAlert;
	private Toast mToast;


	private boolean isPermissionTry = false;
	
	

	@SuppressLint("HandlerLeak")
	private Handler mHandler = new Handler() {
		public void handleMessage(Message msg) {

			switch (msg.what) {
			
			case 1:
				isPermissionTry = false;
				break;
			}
		}
	};
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		// TODO Auto-generated method stub
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mainactivity);
		/*FirebaseMessaging clsMessaging = FirebaseMessaging.getInstance();
		clsMessaging.subscribeToTopic("news");
		  
		String token = FirebaseInstanceId.getInstance().getToken();
		Log.d("=====", "token : " + token);*/
		checkPermission();
		
	}

	@TargetApi(23)
	public void checkPermission() {

		Logger.d("call checkPermission !!");

		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {

			if (checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED
					|| checkSelfPermission(Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {

				isPermissionTry = true;
				mHandler.sendEmptyMessageDelayed(1, 200);
				ActivityCompat.requestPermissions(checkPermission.this,
						new String[] { Manifest.permission.CAMERA,
								Manifest.permission.WRITE_EXTERNAL_STORAGE },
						PERMISSIONS_REQUEST_ACCESS);
			}
			else if(checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED
					|| checkSelfPermission(Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED){
				Intent intent = new Intent(checkPermission.this, UnityPlayerNativeActivity.class);
				startActivity(intent);
				UnityPlayer.UnitySendMessage("Manager", "permissionResult", "true");
				finish();
			}

		} else {
			// M Os �씠�븯
			Intent intent = new Intent(checkPermission.this, UnityPlayerNativeActivity.class);
			startActivity(intent);
			UnityPlayer.UnitySendMessage("Manager", "permissionResult", "true");
			finish();
		}

		// int permissionCheck = checkSelfPermission(
		// Manifest.permission.CAMERA);
		// if(permissionCheck == PackageManager.PERMISSION_DENIED ||
		// checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
		// PackageManager.PERMISSION_DENIED||
		// checkSelfPermission(Manifest.permission.READ_EXTERNAL_STORAGE) ==
		// PackageManager.PERMISSION_DENIED){
		// //沅뚰븳�뾾�쓬
		// finish();
		// }
		// else{
		// }
	}

	@TargetApi(23)
	@Override
	public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
		Logger.d("onRequestPermissionsResult : " + requestCode + " / " + permissions.toString());

		switch (requestCode) {
		case PERMISSIONS_REQUEST_ACCESS: {
			if (grantResults.length > 0
					&& grantResults[0] == PackageManager.PERMISSION_GRANTED && grantResults[1] == PackageManager.PERMISSION_GRANTED) {
				Logger.d("onRequestPermissionsResult : 권한 허용");
				
				Intent intent = new Intent(checkPermission.this, UnityPlayerNativeActivity.class);
				startActivity(intent);
				UnityPlayer.UnitySendMessage("Manager", "permissionResult", "true");
				finish();
			} else {

				/**
				 * 다시 묻지 않기를 통해 들어온경우
				 */
				// if(isPermissionTry){
				if (mPermissionDeniedNotiAlert == null
						|| !mPermissionDeniedNotiAlert.isShowing()) {
					mPermissionDeniedNotiAlert = createDialog(this);
					mPermissionDeniedNotiAlert.show();
				}
				// }

			}
		}
			break;
		}

		super.onRequestPermissionsResult(requestCode, permissions, grantResults);
	}

	private AlertDialog createDialog(final Context context) {

		AlertDialog.Builder ab = new AlertDialog.Builder(context);
		ab.setMessage("앱을 실행하는데 필요한 기기의 권한 허용을 거절하였습니다. \n앱을 정상적으로 동작하기 위한 권한 설정 화면으로 이동하시겠습니까?");
		ab.setCancelable(false);
		ab.setPositiveButton("이동", new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface arg0, int arg1) {
				Intent i = new Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
				i.setData(Uri.parse("package:" + getPackageName()));
				startActivityForResult(i, PERMISSION_ACTIVITY_FOR_RESULT_CODE);
				finish();
			}
		});

		ab.setNegativeButton("닫기", new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface arg0, int arg1) {
				showToast("앱을 종료합니다.");
				finish();
			}
		});

		return ab.create();
	}

	private void showToast(String strMsg) {
		if (mToast == null) {
			mToast = Toast.makeText(this, strMsg, Toast.LENGTH_SHORT);
		} else {
			mToast.setText(strMsg);
		}
		mToast.show();
	}

}
