package com.feelingki.fh;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import android.Manifest;
import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.content.res.Configuration;
import android.database.Cursor;
import android.graphics.PixelFormat;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Message;
import android.os.Parcelable;
import android.os.StatFs;
import android.provider.MediaStore;
import android.provider.Settings;
import android.support.v4.app.ActivityCompat;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;
import android.widget.Toast;

import com.feelingki.fh.util.Logger;
import com.unity3d.player.UnityPlayer;

public class UnityPlayerActivity extends Activity
{
	protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code

	private final int SELECT_IMAGE = 1;
	private final int PERMISSION_ACTIVITY_FOR_RESULT_CODE = 0000;
	private final int PERMISSIONS_REQUEST_ACCESS = 0001;

	private AlertDialog mPermissionDeniedNotiAlert;
	private Toast mToast;

	private long pressedTime;

	private boolean isBackPress = false;
	private boolean isPermissionTry = false;

	@SuppressLint("HandlerLeak")
	private Handler mHandler = new Handler() {
		public void handleMessage(Message msg) {

			switch (msg.what) {
			case 0:
				isBackPress = false;
				break;
			case 1:
				isPermissionTry = false;
				break;
			}
		}
	};

	// Setup activity layout
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		super.onCreate(savedInstanceState);
		getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia
														// play happy

		mUnityPlayer = new UnityPlayer(this);
		setContentView(mUnityPlayer);
		mUnityPlayer.requestFocus();
		
//		checkPermission();
		

	}
	public void getAvailableMemory(){
		File path = Environment.getDataDirectory();
		StatFs stat = new StatFs(path.getPath());
		long blockSize = stat.getBlockSize();
		long availableBlocks = stat.getAvailableBlocks();
		
		sendAvailableMemory(String.valueOf((availableBlocks * blockSize)/1024/1024));
	}
	public void sendAvailableMemory(String memory){
		UnityPlayer.UnitySendMessage("Manager", "getAvailableMemory", memory);
	}
	public void Gallery() {
		doSelectImage();
	}

	public void SendPath(String path) {
		UnityPlayer.UnitySendMessage("Manager", "imgPath", path);
	}

	private void doSelectImage() {
		// Intent i = new Intent(Intent.ACTION_GET_CONTENT);
		// i.setType("image/*");
		// i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
		// try {
		// startActivityForResult(i, SELECT_IMAGE);
		// } catch (android.content.ActivityNotFoundException e) {
		// e.printStackTrace();
		// }

		Intent intent = new Intent();

		if (Build.VERSION.SDK_INT < 19) {
			intent.setAction(Intent.ACTION_GET_CONTENT);
		} else {
			intent.setAction(Intent.ACTION_PICK);
			intent.setData(android.provider.MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
		}

		intent.setType("image/*");
		startActivityForResult(intent, SELECT_IMAGE);

	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent intent) {
		super.onActivityResult(requestCode, resultCode, intent);

		if (resultCode == RESULT_OK) {
			if (requestCode == SELECT_IMAGE) {
				Uri uri = intent.getData();
				String path = getPath(uri);
				Log.d("===========", path);
				SendPath(path);
			}
		}

		if (requestCode == PERMISSION_ACTIVITY_FOR_RESULT_CODE) {
			checkPermission();
		}
	}

	// 실제 경로 찾기
	private String getPath(Uri uri) {
		String[] projection = { MediaStore.Images.Media.DATA };
//		 Cursor cursor = managedQuery(uri, projection, null, null, null); // Deprecation
		Cursor cursor = getContentResolver().query(uri, projection, null, null, null);
		int column_index = cursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
		cursor.moveToFirst();
		return cursor.getString(column_index);
	}

	private void sendKakao(String arg) {
		Intent intent = new Intent(Intent.ACTION_SEND);
		intent.setType("image/*");

		File file = new File(Environment.getExternalStorageDirectory() + "/DCIM/Mirage/" + arg);
		Log.d("aaaa", file.toString());
		File[] uriFiles = (new File(Environment.getExternalStorageDirectory() + "/DCIM/Mirage/").listFiles());

		List<ResolveInfo> resInfo = getPackageManager().queryIntentActivities(
				intent, 0);
		if (resInfo.isEmpty()) {
			return;
		}

		List<Intent> shareIntentList = new ArrayList<Intent>();
		Intent shareIntent = (Intent) intent.clone();
		shareIntent.setType("image/*");
		// shareIntent.putExtra(Intent.EXTRA_SUBJECT, "subject");
		// shareIntent.putExtra(Intent.EXTRA_TEXT, "test");
		shareIntent.putExtra(Intent.EXTRA_STREAM, Uri.fromFile(file)/*Uri.fromFile(uriFiles[uriFiles.length - 1])*/);
		shareIntent.setPackage("com.kakao.talk");
		// shareIntent.setPackage("com.sec.android.widgetapp.diotek.smemo");
		// shareIntent.setPackage("com.android.mms");
		shareIntentList.add(shareIntent);

		Intent chooserIntent = Intent.createChooser(shareIntentList.remove(0), "공유하기");
		chooserIntent.putExtra(Intent.EXTRA_INITIAL_INTENTS, shareIntentList.toArray(new Parcelable[] {}));
		try {

			startActivity(shareIntent);
		} catch (Exception e) {
			Toast.makeText(this, "카카오톡이 설치되어 있지 않습니다. ", Toast.LENGTH_SHORT).show();
		}
	}

	public void onBackPressed() {
		Toast.makeText(UnityPlayerActivity.this, "한번 더 누르면 종료됩니다.", Toast.LENGTH_SHORT).show();
	}
	

	@TargetApi(23)
	@SuppressLint("NewApi")
	public void checkPermission() {

		Logger.d("call checkPermission !!");

		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {

			if (checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED
					|| checkSelfPermission(Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {

				isPermissionTry = true;
				mHandler.sendEmptyMessageDelayed(1, 200);
				ActivityCompat.requestPermissions(UnityPlayerActivity.this,
						new String[] { Manifest.permission.CAMERA,
								Manifest.permission.WRITE_EXTERNAL_STORAGE },
						PERMISSIONS_REQUEST_ACCESS);

			}
			else if(checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED
					|| checkSelfPermission(Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED){
				UnityPlayer.UnitySendMessage("Manager", "permissionResult", "true");
				
			}

		} else {
			// M Os 이하
			UnityPlayer.UnitySendMessage("Manager", "permissionResult", "true");
		}

		// int permissionCheck = checkSelfPermission(
		// Manifest.permission.CAMERA);
		// if(permissionCheck == PackageManager.PERMISSION_DENIED ||
		// checkSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
		// PackageManager.PERMISSION_DENIED||
		// checkSelfPermission(Manifest.permission.READ_EXTERNAL_STORAGE) ==
		// PackageManager.PERMISSION_DENIED){
		// //권한없음
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
					&& grantResults[0] == PackageManager.PERMISSION_GRANTED) {
				Logger.d("onRequestPermissionsResult : 권한 허용");
				UnityPlayer.UnitySendMessage("Manager", "permissionResult", "true");
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

	// Quit Unity
	@Override
	protected void onDestroy() {
		mUnityPlayer.quit();
		super.onDestroy();
	}

	// Pause Unity
	@Override
	protected void onPause() {
		super.onPause();
		mUnityPlayer.pause();
	}

	// Resume Unity
	@Override
	protected void onResume() {
		super.onResume();
		mUnityPlayer.resume();
	}

	// This ensures the layout will be correct.
	@Override
	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		mUnityPlayer.configurationChanged(newConfig);
	}

	// Notify Unity of the focus change.
	@Override
	public void onWindowFocusChanged(boolean hasFocus) {
		super.onWindowFocusChanged(hasFocus);
		mUnityPlayer.windowFocusChanged(hasFocus);
	}

	// For some reason the multiple keyevent type is not supported by the ndk.
	// Force event injection by overriding dispatchKeyEvent().
	@Override
	public boolean dispatchKeyEvent(KeyEvent event) {
		if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
			return mUnityPlayer.injectEvent(event);
		return super.dispatchKeyEvent(event);
	}

	// Pass any events not handled by (unfocused) views straight to UnityPlayer
	@Override
	public boolean onKeyUp(int keyCode, KeyEvent event) {
		return mUnityPlayer.injectEvent(event);
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		
		Logger.d("onKeyDown keyCode : " + keyCode );
//        if(keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0){
//        	
//        	
//        	return true;
//        }
		
		return mUnityPlayer.injectEvent(event);
	}

	@Override
	public boolean onTouchEvent(MotionEvent event) {
		return mUnityPlayer.injectEvent(event);
	}

	/* API12 */
	public boolean onGenericMotionEvent(MotionEvent event) {
		return mUnityPlayer.injectEvent(event);
	}
}

