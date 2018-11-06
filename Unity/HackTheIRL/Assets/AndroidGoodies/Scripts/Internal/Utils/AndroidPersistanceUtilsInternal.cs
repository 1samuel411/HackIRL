#if UNITY_ANDROID
using UnityEngine;
using System;
using System.IO;

namespace DeadMosquito.AndroidGoodies.Internal
{
    public static class AndroidPersistanceUtilsInternal
    {
        public const string MediaScannerConnectionClass = "android.media.MediaScannerConnection";

        private const string GoodiesFileFolder = "android-goodies";
        private const string GoodiesShareImageFileName = "android-goodies-share-image.png";

        public static AndroidJavaObject SaveShareImageToExternalStorage(Texture2D tex2D)
        {
            byte[] encoded = tex2D.Encode(ImageFormat.PNG);
            string saveFilePath = SaveFileToExternalStorage(encoded, GoodiesShareImageFileName, GoodiesFileFolder);
            return AndroidUri.FromFile(saveFilePath);
        }

        public static void SaveImageToPictures(Texture2D tex2D, string fileName, string directory = null,
            ImageFormat format = ImageFormat.PNG)
        {
            byte[] encoded = tex2D.Encode(format);

            // add extension
            var ext = format == ImageFormat.PNG ? ".png" : ".jpeg";
            fileName += ext;

            var picsDirectory = string.IsNullOrEmpty(directory)
                ? AGEnvironment.DirectoryPictures
                : Path.Combine(AGEnvironment.DirectoryPictures, directory);

            var savedFilePath = SaveFileToExternalStorage(encoded, fileName, picsDirectory);
            RefreshGallery(savedFilePath);
        }

        public static string SaveFileToExternalStorage(byte[] buffer, string fileName, string directory = null)
        {
            var pathToSave = AGEnvironment.ExternalStorageDirectoryPath;
            if (!string.IsNullOrEmpty(directory))
            {
                pathToSave = Path.Combine(pathToSave, directory);
                Directory.CreateDirectory(pathToSave);
            }

            var filePath = Path.Combine(pathToSave, fileName);

            try
            {
                var file = File.Open(filePath, FileMode.OpenOrCreate);
                var binary = new BinaryWriter(file);
                binary.Write(buffer);
                file.Close();
            }
            catch (Exception e)
            {
                Debug.LogError("Android Goodies failed to save file " + fileName + " to external storage");
                Debug.LogException(e);
            }

            return filePath;
        }

        public static void RefreshGallery(string filePath)
        {
            if (AGDeviceInfo.SDK_INT >= AGDeviceInfo.VersionCodes.KITKAT)
            {
                using (var c = new AndroidJavaClass(MediaScannerConnectionClass))
                {
                    c.CallStatic("scanFile", AGUtils.Activity, new[] {filePath}, null, null);
                }
            }
            else
            {
                var uri = AndroidUri.FromFile(filePath);
                var intent = new AndroidIntent(AndroidIntent.ACTION_MEDIA_MOUNTED, uri);
                AGUtils.SendBroadcast(intent.AJO);
            }
        }

        private const string MediaStoreImagesMediaClass = "android.provider.MediaStore$Images$Media";

        public static string InsertImage(Texture2D texture2D, string title, string description)
        {
            using (var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass))
            {
                using (var cr = AGUtils.Activity.CallAJO("getContentResolver"))
                {
                    var image = AGUtils.Texture2DToAndroidBitmap(texture2D);
                    var imageUrl = mediaClass.CallStaticStr("insertImage", cr, image, title, description);
                    return imageUrl;
                }
            }
        }
    }
}

#endif