package com.feelingki.fh;

import java.util.Map;

import android.annotation.TargetApi;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Intent;
import android.os.Build;
import android.support.annotation.WorkerThread;

import com.google.firebase.messaging.RemoteMessage;

public class FCMService extends com.google.firebase.messaging.FirebaseMessagingService {
	@TargetApi(26)
	@Override
	 @WorkerThread
	 public void onMessageReceived( RemoteMessage remoteMessage )
	 {
	  super.onMessageReceived( remoteMessage );
	  WakeLocker.acquire(this);
	  String from=remoteMessage.getFrom();
      Map<String, String> data=remoteMessage.getData();
      String msg=data.get("msg");
      
      NotificationManager manager=(NotificationManager)getSystemService(NOTIFICATION_SERVICE);
      
      Intent intent = new Intent(getApplicationContext(), checkPermission.class);
      intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
      
      
      PendingIntent contentIntent = PendingIntent.getActivity(this, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
      android.support.v4.app.NotificationCompat.Builder builder=null;
      if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
          String channelId = "one-channel";
          String channelName = "My Channel One";
          String channelDescription = "My Channel One Description";
          NotificationChannel channel = new NotificationChannel(channelId, channelName, NotificationManager.IMPORTANCE_DEFAULT);
          channel.setDescription(channelDescription);

          manager.createNotificationChannel(channel);
          builder = new android.support.v4.app.NotificationCompat.Builder(this);

      } else {
          builder = new android.support.v4.app.NotificationCompat.Builder(this);
      }

      builder.setSmallIcon(R.drawable.app_icon);
      builder.setContentTitle("플라워링하트AR");
      builder.setWhen(System.currentTimeMillis());
      builder.setContentText(msg);
      builder.setAutoCancel(true);
      
      builder.setDefaults(Notification.DEFAULT_VIBRATE);
      builder.setContentIntent(contentIntent);

      manager.notify(222, builder.build());
	 }
}
