package com.feelingki.fh.util;

import android.app.Activity;
import android.os.Environment;
import android.util.Log;
import android.widget.Toast;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;


public class Logger {
    
    private static final String TAG = "Mirage";
    public static boolean DEBUG_LOG = true;
    public static boolean FILEDEBUG_LOG = false;
    private final static String LOG_FILE_NAME = TAG+ "_Log.txt"; //로그 파일 명

    private static String dateFormat = "yyyy-MM-dd HH:mm:ss:SSS";

    /**
     * Vervose Log
     * @param msg
     */
    public static void v(String msg) {

        if (DEBUG_LOG) {
            String dateFormatMsg = new SimpleDateFormat(dateFormat).format(new Date()) + "\t" + msg + "\r\n";
            Log.v(TAG, dateFormatMsg);
            f(dateFormatMsg, "V");
        }
    }

    /**
     * Debug Log
     * @param msg
     */
    public static void d(String msg) {
        if (DEBUG_LOG) {
            String dateFormatMsg = new SimpleDateFormat(dateFormat).format(new Date()) + "\t" + msg + "\r\n";
            Log.d(TAG, dateFormatMsg);
            f(dateFormatMsg, "D");
        }
    }

    /**
     * Information Log
     * @param msg
     */
    public static void i(String msg) {
        
        if (DEBUG_LOG) {
            String dateFormatMsg = new SimpleDateFormat(dateFormat).format(new Date()) + "\t" + msg + "\r\n";
            Log.i(TAG, dateFormatMsg);
            f(dateFormatMsg, "I");
        }
    }

    /**
     * Warn Log
     * @param msg
     */
    public static void w(String msg) {

        if (DEBUG_LOG) {
            String dateFormatMsg = new SimpleDateFormat(dateFormat).format(new Date()) + "\t" + msg + "\r\n";
            Log.w(TAG, dateFormatMsg);
            f(dateFormatMsg, "W");
        }
    }

    /**
     * Error Log
     * @param msg
     */
    public static void e(String msg) {
        
        if (DEBUG_LOG) {
            String dateFormatMsg = new SimpleDateFormat(dateFormat).format(new Date()) + "\t" + msg + "\r\n";
            Log.e(TAG, dateFormatMsg);
            f(dateFormatMsg, "E");
        }
    }


    /**
     * Exception Log
     * @param e
     */
    public static void e(Exception e ) {
        if(DEBUG_LOG){
            if( e != null && e.toString() != null && e.toString().length() > 0 ) {
                e("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                e(Log.getStackTraceString(e));
                e("-----------------------------------------------------");
                e.printStackTrace();
                e("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            }
        }
    }

    /**
     * Toast Log
     * @param activity
     * @param msg
     */
    public static void t(final Activity activity, final String msg) {
        if (DEBUG_LOG)
            activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(activity, msg, Toast.LENGTH_SHORT).show();
                }
            });
    }

    /**
     * 메소드 시작
     * @param methodName
     */
    public static void start(String methodName) {
        if (DEBUG_LOG) {
            String dateFormatMsg = new SimpleDateFormat(dateFormat).format(new Date()) + "\t********************* " + methodName + "  Method Start *********************\r\n";
            Log.d(TAG, dateFormatMsg);
            f(dateFormatMsg, "D");
        }
    }


    /**
     * File Log
     * @param msg
     * @param level
     */
    private static void f(String msg, String level) {
        if(FILEDEBUG_LOG){
            try {
                if(Environment.MEDIA_MOUNTED.equals(Environment.getExternalStorageState())) {
                    File file = new File(Environment.getExternalStorageDirectory(), LOG_FILE_NAME);
                    FileWriter fw = null;
                    try {
                        if(!file.exists()) {
                            file.createNewFile();
                        }

                        fw = new FileWriter(file, true);
                        fw.append(level + " : \t" + msg);
                    } catch (IOException e) {
                    } finally{
                        try {
                            if(fw != null){
                                fw.flush();
                                fw.close();
                            }
                        } catch (IOException e) {
                        }
                    }
                }
            } catch (Exception e) {
            }
        }
    }
}
