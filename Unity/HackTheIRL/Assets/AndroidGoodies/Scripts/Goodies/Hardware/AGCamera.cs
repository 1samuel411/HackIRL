#if UNITY_ANDROID
using DeadMosquito.AndroidGoodies.Internal;
using System;
using UnityEngine;

namespace DeadMosquito.AndroidGoodies
{
    /// <summary>
    /// Methods to interact with device camera.
    /// </summary>
    public static class AGCamera
    {
        const string DefaultAlbumName = "Pictures";

        /// <summary>
        /// Check if device has camera
        /// </summary>
        /// <returns><c>true</c>, if device has camera, <c>false</c> otherwise.</returns>
        public static bool DeviceHasCamera()
        {
            return AGDeviceInfo.SystemFeatures.HasSystemFeature(AGDeviceInfo.SystemFeatures.FEATURE_CAMERA);
        }

        /// <summary>
        /// Check if device has frontal camera
        /// </summary>
        /// <returns><c>true</c>, if device has frontal camera, <c>false</c> otherwise.</returns>
        public static bool DeviceHasFrontalCamera()
        {
            return AGDeviceInfo.SystemFeatures.HasSystemFeature(AGDeviceInfo.SystemFeatures.FEATURE_CAMERA_FRONT);
        }

        /// <summary>
        /// Check if device has camera with autofocus
        /// </summary>
        /// <returns><c>true</c>, if device has camera with autofocus, <c>false</c> otherwise.</returns>
        public static bool DeviceHasCameraWithAutoFocus()
        {
            return DeviceHasCamera() && AGDeviceInfo.SystemFeatures.HasSystemFeature(AGDeviceInfo.SystemFeatures.FEATURE_CAMERA_AUTOFOCUS);
        }

        /// <summary>
        /// Check if device has camera with flashlight
        /// </summary>
        /// <returns><c>true</c>, if device has camera with flashlight, <c>false</c> otherwise.</returns>
        public static bool DeviceHasCameraWithFlashlight()
        {
            return DeviceHasCamera() && AGDeviceInfo.SystemFeatures.HasSystemFeature(AGDeviceInfo.SystemFeatures.FEATURE_CAMERA_FLASH);
        }

        /// <summary>
        /// Required permissions:
        ///     <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
        /// 
        /// Launches the camera app to take a photo and returns resulting Texture2D in callback. The photo is also saved to the device gallery.
        /// 
        /// IMPORTANT! : You don't need any permissions to use this method. It works using Android intent.
        /// </summary>
        /// <param name="onSuccess">On success callback. Image is received as callback parameter</param>
        /// <param name="onCancel">On cancel callback.</param>
        /// <param name="maxSize">Max image size. If provided image will be downscaled.</param>
        /// <param name="albumName">Album where photo will be stored.</param>
        public static void TakePhoto(Action<ImagePickResult> onSuccess, Action onCancel, 
            ImageResultSize maxSize = ImageResultSize.Original,
            string albumName = DefaultAlbumName)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (onSuccess == null)
            {
                throw new ArgumentNullException("onSuccess", "Success callback cannot be null");
            }

            var listener = new AGActivityUtils.OnPickPhotoListener(onSuccess, onCancel);
            AGUtils.RunOnUiThread(() => AGActivityUtils.TakePhotoBig(listener, maxSize, albumName));
        }

        /// <summary>
        /// Takes the small photo.  This thumbnail image might be good for an icon, but not a lot more. 
        /// </summary>
        /// <param name="onSuccess">On success callback. Image is received as callback parameter</param>
        /// <param name="onCancel">On cancel callback.</param>
        public static void TakeSmallPhoto(Action<ImagePickResult> onSuccess, Action onCancel)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (onSuccess == null)
            {
                throw new ArgumentNullException("onSuccess", "Success callback cannot be null");
            }

            var listener = new AGActivityUtils.OnPickPhotoListener(onSuccess, onCancel);
            AGUtils.RunOnUiThread(() => AGActivityUtils.TakePhotoSmall(listener));
        }
    }
}
#endif
