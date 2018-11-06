#if UNITY_ANDROID
using System;
using DeadMosquito.AndroidGoodies.Internal;
using UnityEngine;

namespace DeadMosquito.AndroidGoodies
{
    /// <summary>
    /// Class to open other apps on device.
    /// </summary>
    public static class AGApps
    {
        /// <summary>
        /// Watch YouTube video. Opens video in YouTube app if its installed, falls back to browser.
        /// </summary>
        /// <param name="id">YouTube video id</param>
        public static void WatchYoutubeVideo(string id)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            var intent = new AndroidIntent(AndroidIntent.ACTION_VIEW, AndroidUri.Parse("vnd.youtube:" + id));

            AGUtils.StartActivity(intent.AJO, () =>
                {
                    var fallbackIntent = new AndroidIntent(AndroidIntent.ACTION_VIEW,
                                         AndroidUri.Parse("http://www.youtube.com/watch?v=" + id));
                    AGUtils.StartActivity(fallbackIntent.AJO);
                });
        }

        /// <summary>
        /// Opens the other app on device.
        /// </summary>
        /// <param name="package">Package of the app to open.</param>
        /// <param name="onAppNotInstalled">Invoked when the app with package is not installed</param>
        public static void OpenOtherAppOnDevice(string package, Action onAppNotInstalled = null)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            using (var pm = AGUtils.PackageManager)
            {
                try
                {
                    var launchIntent = pm.CallAJO("getLaunchIntentForPackage", package);
                    launchIntent.CallAJO("addCategory", AndroidIntent.CATEGORY_LAUNCHER);
                    AGUtils.StartActivity(launchIntent);
                }
                catch (Exception ex)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("Could not find launch intent for package:" + package + ", Error: " + ex.StackTrace);
                    }
                    if (onAppNotInstalled != null)
                    {
                        onAppNotInstalled();
                    }
                }
            }
        }
    }
}

#endif