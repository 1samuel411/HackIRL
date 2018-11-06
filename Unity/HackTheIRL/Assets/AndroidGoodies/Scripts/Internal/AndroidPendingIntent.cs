﻿#if UNITY_ANDROID
using UnityEngine;

namespace DeadMosquito.AndroidGoodies.Internal
{
    /// <summary>
    /// Android pending intent.
    /// </summary>
    static class AndroidPendingIntent
    {
        private const string PendingIntentClass = "android.app.PendingIntent";

        private const int FLAG_UPDATE_CURRENT = 0x08000000;

        public static AndroidJavaObject GetActivity(AndroidJavaObject intent)
        {
            using (var pic = new AndroidJavaClass(PendingIntentClass))
            {
                return pic.CallStaticAJO("getActivity", AGUtils.Activity, 0, intent, FLAG_UPDATE_CURRENT);
            }
        }
    }
}
#endif