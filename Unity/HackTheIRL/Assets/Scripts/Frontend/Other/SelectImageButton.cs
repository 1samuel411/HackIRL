using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SelectImageButton : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void ImageUploaderCaptureClick();

    public FormController formController;

    IEnumerator LoadTexture(string url)
    {
        WWW image = new WWW(url);
        yield return image;
        Texture2D texture = new Texture2D(1, 1);
        image.LoadImageIntoTexture(texture);
        Debug.Log("Loaded image size: " + texture.width + "x" + texture.height);
        formController.SelectImage(texture);
    }

    void FileSelected(string url)
    {
        StartCoroutine(LoadTexture(url));
    }

    public void OnButtonPointerDown()
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Open image", "", "jpg,png,bmp");
        if (!System.String.IsNullOrEmpty(path))
            FileSelected("file:///" + path);
#else
        ImageUploaderCaptureClick ();
#endif
    }
}
