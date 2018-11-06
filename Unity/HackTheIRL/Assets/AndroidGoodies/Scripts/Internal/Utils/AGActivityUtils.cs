#if UNITY_ANDROID
using UnityEngine;
using System;

namespace DeadMosquito.AndroidGoodies.Internal
{
    public static class AGActivityUtils
    {
        public const string JarPackageName = "com.deadmosquitogames.";
        public const string AndroidGoodiesActivityClassSignature = JarPackageName + "AndroidGoodiesActivity";
        public const string PhotoPickerUtilsClassSignature = JarPackageName + "PhotoPickerUtils";

        public static AndroidJavaClass AndroidGoodiesActivityClass
        {
            get
            {
                if (_androidGoodiesActivityClass == null)
                {
                    _androidGoodiesActivityClass = new AndroidJavaClass(AndroidGoodiesActivityClassSignature);
                }

                return _androidGoodiesActivityClass;
            }
        }

        static AndroidJavaClass _androidGoodiesActivityClass;

        public static void PickPhotoFromGallery(OnPickPhotoListener listener, ImageFormat imageFormat, ImageResultSize maxSize)
        {
            AndroidGoodiesActivityClass.CallStatic("prepareToPickPhoto", listener, (int)imageFormat, (int)maxSize);
            StartAndroidGoodiesActivity();
        }

        public static void TakePhotoBig(OnPickPhotoListener listener, 
            ImageResultSize maxSize,
            string albumName)
        {
            AndroidGoodiesActivityClass.CallStatic("prepareToTakeBigPhoto", listener, (int)maxSize, albumName);
            StartAndroidGoodiesActivity();
        }

        public static void TakePhotoSmall(OnPickPhotoListener listener)
        {
            AndroidGoodiesActivityClass.CallStatic("prepareToTakeSmallPhoto", listener);
            StartAndroidGoodiesActivity();
        }

        public static void StartAndroidGoodiesActivity()
        {
            using (var clazz = AGUtils.ClassForName(AndroidGoodiesActivityClassSignature))
            {
                using (var intent = new AndroidIntent(AGUtils.Activity, clazz))
                {
                    AGUtils.StartActivity(intent.AJO);
                }
            }
        }

        public class OnPickPhotoListener : AndroidJavaProxy
        {
            readonly Action<ImagePickResult> _onPhotoReceivedAction;
            readonly Action _onCancelAction;

            public OnPickPhotoListener(Action<ImagePickResult> onPhotoReceived, Action onCancel)
                : base(JarPackageName + "OnTextureReceivedListener")
            {
                _onPhotoReceivedAction = onPhotoReceived;
                _onCancelAction = onCancel;
            }

            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedMember.Local
            void onTextureDataReceived(AndroidJavaObject jo, string fileName)
            {
                AndroidJavaObject bufferObject = jo.Get<AndroidJavaObject>("Buffer");
                byte[] buffer = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(bufferObject.GetRawObject());

                GoodiesSceneHelper.Queue(() =>
                    {
                        var tex = new Texture2D(2, 2);
                        tex.LoadImage(buffer);

                        var result = new ImagePickResult(fileName, tex);
                        _onPhotoReceivedAction(result);
                    });
            }

            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedMember.Local
            void onCancel()
            {
                if (_onCancelAction != null)
                {
                    _onCancelAction();
                }
            }
        }
    }
}
#endif