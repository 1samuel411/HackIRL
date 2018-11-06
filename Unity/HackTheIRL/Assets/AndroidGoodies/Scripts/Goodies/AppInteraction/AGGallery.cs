﻿#if UNITY_ANDROID
using DeadMosquito.AndroidGoodies.Internal;
using System;
using UnityEngine;

namespace DeadMosquito.AndroidGoodies
{
    /// <summary>
    /// Methods to interact with device gallery.
    /// </summary>
    public static class AGGallery
    {
        /// <summary>
        /// Picks the image from gallery.
        /// </summary>
        /// <param name="onSuccess">On success callback. Image is received as callback parameter</param>
        /// <param name="onCancel">On cancel callback.</param>
        /// <param name="imageFormat">Image format.</param>
        /// <param name="maxSize">Max image size. If provided image will be downscaled.</param>
        public static void PickImageFromGallery(Action<ImagePickResult> onSuccess, Action onCancel,
            ImageFormat imageFormat = ImageFormat.PNG, ImageResultSize maxSize = ImageResultSize.Original)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (onSuccess == null)
            {
                throw new ArgumentNullException("onSuccess", "Success callback cannot be null");
            }

            AGUtils.RunOnUiThread(
                () =>
                    AGActivityUtils.PickPhotoFromGallery(
                        new AGActivityUtils.OnPickPhotoListener(onSuccess, onCancel), imageFormat, maxSize));
        }

        /// <summary>
        /// Saves the image to android gallery.
        /// </summary>
        /// <returns>The image to save to the gallery.</returns>
        /// <param name="texture2D">Texture2D to save.</param>
        /// <param name="title">Title.</param>
        /// <param name="folder">Inner folder in Pictures directory. Must be a valid folder name</param>
        /// <param name="imageFormat">Image format.</param>
        public static void SaveImageToGallery(Texture2D texture2D, string title, string folder = null,
            ImageFormat imageFormat = ImageFormat.PNG)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            AGFileUtils.SaveImageToGallery(texture2D, title, folder, imageFormat);
        }
    }
}

#endif