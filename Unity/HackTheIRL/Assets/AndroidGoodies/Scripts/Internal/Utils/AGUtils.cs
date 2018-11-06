#if UNITY_ANDROID
using UnityEngine;
using System;

namespace DeadMosquito.AndroidGoodies.Internal
{
    public static class AGUtils
    {
        private const string JavaLangSystemClass = "java.lang.System";

        private static AndroidJavaObject _activity;

        public static AndroidJavaObject Activity
        {
            get
            {
                if (_activity == null)
                {
                    var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _activity;
            }
        }

        public static AndroidJavaObject ActivityDecorView
        {
            get
            {
                return Activity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
            }
        }

        public static AndroidJavaObject PackageManager
        {
            get { return Activity.CallAJO("getPackageManager"); }
        }

        public static bool HasSystemFeature(string feature)
        {
            using (var pm = PackageManager)
            {
                return pm.CallBool("hasSystemFeature", feature);
            }
        }

        public static long CurrentTimeMillis
        {
            get
            {
                using (var system = new AndroidJavaClass(JavaLangSystemClass))
                {
                    return system.CallStaticLong("currentTimeMillis");
                }
            }
        }

        #region reflection

        public static AndroidJavaObject ClassForName(string className)
        {
            using (var clazz = new AndroidJavaClass("java.lang.Class"))
            {
                return clazz.CallStaticAJO("forName", className);
            }
        }

        public static AndroidJavaObject Cast(this AndroidJavaObject source, string destClass)
        {
            using (var destClassAJC = ClassForName(destClass))
            {
                return destClassAJC.Call<AndroidJavaObject>("cast", source);
            }
        }

        #endregion

        public static bool IsNotAndroidCheck()
        {
            bool isAndroid = Application.platform == RuntimePlatform.Android;

            if (isAndroid)
            {
                GoodiesSceneHelper.Init();
            }

            return !isAndroid;
        }

        public static void RunOnUiThread(Action action)
        {
            Activity.Call("runOnUiThread", new AndroidJavaRunnable(action));
        }

        public static void StartActivity(AndroidJavaObject intent, Action fallback = null)
        {
            try
            {
                Activity.Call("startActivity", intent);
            }
            catch (AndroidJavaException exception)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("Could not start the activity with " + intent.JavaToString() + ": " + exception.Message);
                }
                if (fallback != null)
                    fallback();
            }
            finally
            {
                intent.Dispose();
            }
        }

        public static void StartActivityWithChooser(AndroidJavaObject intent, string chooserTitle)
        {
            try
            {
                AndroidJavaObject jChooser = intent.CallStaticAJO("createChooser", intent, chooserTitle);
                Activity.Call("startActivity", jChooser);
            }
            catch (AndroidJavaException exception)
            {
                Debug.LogWarning("Could not start the activity with " + intent.JavaToString() + ": " + exception.Message);
            }
            finally
            {
                intent.Dispose();
            }
        }

        public static void SendBroadcast(AndroidJavaObject intent)
        {
            Activity.Call("sendBroadcast", intent);
        }

        public static AndroidJavaObject GetMainActivityClass()
        {
            var packageName = AGDeviceInfo.GetApplicationPackage();
            using (var pm = PackageManager)
            {
                var launchIntent = PackageManager.CallAJO("getLaunchIntentForPackage", packageName);
                try
                {
                    var className = launchIntent.CallAJO("getComponent").CallStr("getClassName");
                    return ClassForName(className);
                }
                catch (Exception e)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("Unable to find Main Activity Class: " + e.Message);
                    }
                    return null;
                }
            }
        }

        #region images

        public static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D tex2D, ImageFormat format = ImageFormat.PNG)
        {
            byte[] encoded = tex2D.Encode(format);
            using (var bf = new AndroidJavaClass("android.graphics.BitmapFactory"))
            {
                return bf.CallStaticAJO("decodeByteArray", encoded, 0, encoded.Length);
            }
        }

        #endregion
    }
}
#endif
