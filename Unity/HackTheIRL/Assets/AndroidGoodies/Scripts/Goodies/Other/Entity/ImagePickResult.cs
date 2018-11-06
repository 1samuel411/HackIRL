#if UNITY_ANDROID
using UnityEngine;

namespace DeadMosquito.AndroidGoodies
{
    /// <summary>
    /// Image that was picked
    /// </summary>
    public class ImagePickResult
    {
        public string FileName { get; private set; }

        public Texture2D Image { get; private set; }

        public ImagePickResult(string fileName, Texture2D image)
        {
            FileName = fileName;
            Image = image;
        }
    }
}
#endif