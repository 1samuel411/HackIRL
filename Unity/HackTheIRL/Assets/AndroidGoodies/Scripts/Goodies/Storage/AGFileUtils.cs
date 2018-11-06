#if UNITY_ANDROID
using UnityEngine;
using System;
using System.IO;
using DeadMosquito.AndroidGoodies.Internal;

namespace DeadMosquito.AndroidGoodies
{
    public static class AGFileUtils
    {
        /// <summary>
        /// Saves the image to android gallery.
        /// </summary>
        /// <returns>The image to save to the gallery.</returns>
        /// <param name="texture2D">Texture2D to save.</param>
        /// <param name="title">Title.</param>
        /// <param name="folder">Inner folder in Pictures directory. Must be a valid folder name.
        /// If not specified the image will be save to default pictures folder</param>
        /// <param name="imageFormat">Image format.</param>
        public static void SaveImageToGallery(Texture2D texture2D, string title, string folder = null,
            ImageFormat imageFormat = ImageFormat.PNG)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (texture2D == null)
            {
                throw new ArgumentNullException("texture2D", "Image to save cannot be null");
            }

            AndroidPersistanceUtilsInternal.SaveImageToPictures(texture2D, title, folder, imageFormat);
        }
    }
}

#endif