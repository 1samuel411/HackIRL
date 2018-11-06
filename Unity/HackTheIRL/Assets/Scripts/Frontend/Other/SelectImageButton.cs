using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ImageAndVideoPicker;

public class SelectImageButton : MonoBehaviour
{

    private void Start()
    {
#if UNITY_ANDROID
        AndroidPicker.CheckPermissions();  
#endif
        PickerEventListener.onImageLoad += OnImageLoad;
    }

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
        AndroidPicker.BrowseImage(true);
#endif
    }


    void OnImageLoad(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation orientation)
    {
        // imgPath : browsed image path // tex : image texture
        Texture2D newTex = duplicateTexture(tex);
        formController.SelectImage(newTex);
    }

    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}